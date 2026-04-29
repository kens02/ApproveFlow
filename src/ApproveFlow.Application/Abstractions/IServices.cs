using ApproveFlow.Application.Models;

namespace ApproveFlow.Application.Abstractions;

public interface INotificationService
{
    Task NotifyAsync(string to, string subject, string body, CancellationToken cancellationToken);
}

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

public interface ILeaveRequestService
{
    Task<LeaveRequestSummary> CreateAsync(CreateLeaveRequestInput input, CancellationToken cancellationToken);
    Task<LeaveRequestSummary> ApproveAsync(ActionInput input, CancellationToken cancellationToken);
    Task<LeaveRequestSummary> RejectAsync(ActionInput input, CancellationToken cancellationToken);
    Task<LeaveRequestSummary> CancelAsync(ActionInput input, CancellationToken cancellationToken);
    Task<LeaveRequestSummary?> GetAsync(Guid requestId, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveRequestSummary>> GetForApplicantAsync(string applicantId, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveRequestSummary>> GetForApproverAsync(string approverId, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveRequestSummary>> GetAllAsync(CancellationToken cancellationToken);
}

public interface IApprovalRouteService
{
    Task SetAsync(SetRouteInput input, CancellationToken cancellationToken);
}

public interface ILeaveBalanceService
{
    Task<LeaveBalanceSummary> GetAsync(string userId, CancellationToken cancellationToken);
}

public interface IDashboardService
{
    Task<DashboardSummary> BuildAsync(string userId, int year, bool fiscalYear, CancellationToken cancellationToken);
}
