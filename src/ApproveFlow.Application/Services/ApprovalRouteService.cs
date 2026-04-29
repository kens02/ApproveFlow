using ApproveFlow.Application.Abstractions;
using ApproveFlow.Application.Models;
using ApproveFlow.Domain.Entities;

namespace ApproveFlow.Application.Services;

public sealed class ApprovalRouteService : IApprovalRouteService
{
    private readonly IApprovalRouteRepository _routeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApprovalRouteService(IApprovalRouteRepository routeRepository, IUnitOfWork unitOfWork)
    {
        _routeRepository = routeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task SetAsync(SetRouteInput input, CancellationToken cancellationToken)
    {
        var route = new ApprovalRoute
        {
            ApplicantId = input.ApplicantId,
            Steps = input.Steps
                .OrderBy(x => x.StepOrder)
                .Select(x => new ApprovalRouteStep
                {
                    StepOrder = x.StepOrder,
                    ApproverId = x.ApproverId,
                    ApproverName = x.ApproverName,
                    NotificationEmail = x.NotificationEmail,
                    MailTemplate = x.MailTemplate
                })
                .ToList()
        };

        route.Validate();
        await _routeRepository.UpsertAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
