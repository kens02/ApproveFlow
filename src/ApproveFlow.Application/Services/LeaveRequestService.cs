using ApproveFlow.Application.Abstractions;
using ApproveFlow.Application.Models;
using ApproveFlow.Domain.Constants;
using ApproveFlow.Domain.Entities;
using ApproveFlow.Domain.Enums;

namespace ApproveFlow.Application.Services;

public sealed class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _requestRepository;
    private readonly IApprovalRouteRepository _routeRepository;
    private readonly ILeaveBalanceRepository _balanceRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public LeaveRequestService(
        ILeaveRequestRepository requestRepository,
        IApprovalRouteRepository routeRepository,
        ILeaveBalanceRepository balanceRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _requestRepository = requestRepository;
        _routeRepository = routeRepository;
        _balanceRepository = balanceRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<LeaveRequestSummary> CreateAsync(CreateLeaveRequestInput input, CancellationToken cancellationToken)
    {
        ValidateInput(input);

        var route = await _routeRepository.GetByApplicantIdAsync(input.ApplicantId, cancellationToken)
            ?? throw new InvalidOperationException("Approval route is not configured for applicant.");
        route.Validate();

        var workflowSteps = route.Steps
            .OrderBy(x => x.StepOrder)
            .Select(x => new WorkflowStep
            {
                StepOrder = x.StepOrder,
                ApproverId = x.ApproverId,
                Status = WorkflowStepStatus.Pending
            })
            .ToList();

        var request = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            ApplicantId = input.ApplicantId,
            ApplicantName = input.ApplicantName,
            LeaveType = input.LeaveType,
            UnitType = input.UnitType,
            StartDateTime = input.StartDateTime,
            EndDateTime = input.EndDateTime,
            Reason = input.Reason,
            AttachmentFileName = input.AttachmentFileName,
            Status = input.SubmitImmediately ? RequestStatus.Submitted : RequestStatus.Draft,
            WorkflowSteps = workflowSteps,
            ActionHistories = new List<ActionHistory>()
        };

        request.ActionHistories.Add(new ActionHistory
        {
            Id = Guid.NewGuid(),
            RequestId = request.Id,
            ActionType = ActionType.CreateDraft,
            UserId = request.ApplicantId,
            Timestamp = _clock.UtcNow,
            Comment = "Request created"
        });

        if (input.SubmitImmediately)
        {
            request.Status = RequestStatus.InReview;
            request.WorkflowSteps[0].Status = WorkflowStepStatus.Waiting;
            request.ActionHistories.Add(new ActionHistory
            {
                Id = Guid.NewGuid(),
                RequestId = request.Id,
                ActionType = ActionType.Submit,
                UserId = request.ApplicantId,
                Timestamp = _clock.UtcNow,
                Comment = "Submitted"
            });
        }

        await _requestRepository.AddAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (input.SubmitImmediately)
        {
            var first = route.Steps.OrderBy(s => s.StepOrder).First();
            await _notificationService.NotifyAsync(first.NotificationEmail, "Leave approval request", BuildNotificationBody(request), cancellationToken);
        }

        return ToSummary(request);
    }

    public async Task<LeaveRequestSummary> ApproveAsync(ActionInput input, CancellationToken cancellationToken)
    {
        var request = await LoadRequest(input.RequestId, cancellationToken);
        if (request.Status is RequestStatus.Rejected or RequestStatus.Cancelled or RequestStatus.Approved)
        {
            throw new InvalidOperationException("Request is already finalized.");
        }

        var current = request.WorkflowSteps.SingleOrDefault(s => s.Status == WorkflowStepStatus.Waiting)
            ?? throw new InvalidOperationException("No pending approval step.");

        if (!string.Equals(current.ApproverId, input.UserId, StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("Only current approver can approve this step.");
        }

        current.Status = WorkflowStepStatus.Approved;
        current.ActionedAt = _clock.UtcNow;
        current.Comment = input.Comment;

        var next = request.WorkflowSteps.FirstOrDefault(s => s.StepOrder == current.StepOrder + 1);
        if (next is null)
        {
            request.Status = RequestStatus.Approved;
            await ConsumeLeaveBalanceAsync(request, cancellationToken);
        }
        else
        {
            request.Status = RequestStatus.InReview;
            next.Status = WorkflowStepStatus.Waiting;
        }

        request.ActionHistories.Add(new ActionHistory
        {
            Id = Guid.NewGuid(),
            RequestId = request.Id,
            ActionType = ActionType.Approve,
            UserId = input.UserId,
            Timestamp = _clock.UtcNow,
            Comment = input.Comment ?? string.Empty
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToSummary(request);
    }

    public async Task<LeaveRequestSummary> RejectAsync(ActionInput input, CancellationToken cancellationToken)
    {
        var request = await LoadRequest(input.RequestId, cancellationToken);
        if (request.Status is RequestStatus.Rejected or RequestStatus.Cancelled or RequestStatus.Approved)
        {
            throw new InvalidOperationException("Request is already finalized.");
        }

        var current = request.WorkflowSteps.SingleOrDefault(s => s.Status == WorkflowStepStatus.Waiting)
            ?? throw new InvalidOperationException("No pending approval step.");

        if (!string.Equals(current.ApproverId, input.UserId, StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("Only current approver can reject this step.");
        }

        current.Status = WorkflowStepStatus.Rejected;
        current.ActionedAt = _clock.UtcNow;
        current.Comment = input.Comment;
        request.Status = RequestStatus.Rejected;

        request.ActionHistories.Add(new ActionHistory
        {
            Id = Guid.NewGuid(),
            RequestId = request.Id,
            ActionType = ActionType.Reject,
            UserId = input.UserId,
            Timestamp = _clock.UtcNow,
            Comment = input.Comment ?? string.Empty
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToSummary(request);
    }

    public async Task<LeaveRequestSummary> CancelAsync(ActionInput input, CancellationToken cancellationToken)
    {
        var request = await LoadRequest(input.RequestId, cancellationToken);
        if (!string.Equals(request.ApplicantId, input.UserId, StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("Only applicant can cancel request.");
        }

        if (request.Status is RequestStatus.Approved or RequestStatus.Cancelled)
        {
            throw new InvalidOperationException("Request cannot be cancelled.");
        }

        request.Status = RequestStatus.Cancelled;
        foreach (var step in request.WorkflowSteps.Where(x => x.Status == WorkflowStepStatus.Waiting || x.Status == WorkflowStepStatus.Pending))
        {
            step.Status = WorkflowStepStatus.Skipped;
        }

        request.ActionHistories.Add(new ActionHistory
        {
            Id = Guid.NewGuid(),
            RequestId = request.Id,
            ActionType = ActionType.Cancel,
            UserId = input.UserId,
            Timestamp = _clock.UtcNow,
            Comment = input.Comment ?? string.Empty
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToSummary(request);
    }

    public async Task<LeaveRequestSummary?> GetAsync(Guid requestId, CancellationToken cancellationToken)
    {
        var entity = await _requestRepository.GetByIdAsync(requestId, cancellationToken);
        return entity is null ? null : ToSummary(entity);
    }

    public async Task<IReadOnlyList<LeaveRequestSummary>> GetForApplicantAsync(string applicantId, CancellationToken cancellationToken)
        => (await _requestRepository.ListByApplicantAsync(applicantId, cancellationToken)).Select(ToSummary).ToList();

    public async Task<IReadOnlyList<LeaveRequestSummary>> GetForApproverAsync(string approverId, CancellationToken cancellationToken)
        => (await _requestRepository.ListByApproverAsync(approverId, cancellationToken)).Select(ToSummary).ToList();

    public async Task<IReadOnlyList<LeaveRequestSummary>> GetAllAsync(CancellationToken cancellationToken)
        => (await _requestRepository.ListAllAsync(cancellationToken)).Select(ToSummary).ToList();

    private async Task<LeaveRequest> LoadRequest(Guid requestId, CancellationToken cancellationToken)
        => await _requestRepository.GetByIdAsync(requestId, cancellationToken)
           ?? throw new KeyNotFoundException("Request not found.");

    private static void ValidateInput(CreateLeaveRequestInput input)
    {
        if (input.StartDateTime >= input.EndDateTime)
        {
            throw new ArgumentException("Start must be before end.");
        }

        var durationMinutes = (int)(input.EndDateTime - input.StartDateTime).TotalMinutes;
        if (input.UnitType == UnitType.Hour && durationMinutes % LeavePolicy.MinutesStep != 0)
        {
            throw new ArgumentException("Hour unit must be in 15-minute increments.");
        }

        if (input.UnitType == UnitType.Day && durationMinutes % LeavePolicy.MinutesPerDay != 0)
        {
            throw new ArgumentException("Day unit must be in full-day increments (465 minutes).");
        }
    }

    private async Task ConsumeLeaveBalanceAsync(LeaveRequest request, CancellationToken cancellationToken)
    {
        var balance = await _balanceRepository.GetOrCreateAsync(request.ApplicantId, cancellationToken);
        balance.UsedDays += ComputeRequestedDays(request);
        await _balanceRepository.SaveAsync(balance, cancellationToken);
    }

    private static decimal ComputeRequestedDays(LeaveRequest request)
    {
        var totalMinutes = (decimal)(request.EndDateTime - request.StartDateTime).TotalMinutes;
        return Math.Round(totalMinutes / LeavePolicy.MinutesPerDay, 2, MidpointRounding.AwayFromZero);
    }

    private static LeaveRequestSummary ToSummary(LeaveRequest request)
        => new(
            request.Id,
            request.ApplicantId,
            request.ApplicantName,
            request.LeaveType,
            request.UnitType,
            request.StartDateTime,
            request.EndDateTime,
            request.Status,
            request.CurrentStepOrder,
            ComputeRequestedDays(request));

    private static string BuildNotificationBody(LeaveRequest request)
        => $"Applicant: {request.ApplicantName}\nPeriod: {request.StartDateTime:u} - {request.EndDateTime:u}\nReason: {request.Reason}";
}
