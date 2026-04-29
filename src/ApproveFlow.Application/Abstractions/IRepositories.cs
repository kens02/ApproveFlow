using ApproveFlow.Domain.Entities;

namespace ApproveFlow.Application.Abstractions;

public interface ILeaveRequestRepository
{
    Task AddAsync(LeaveRequest request, CancellationToken cancellationToken);
    Task<LeaveRequest?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveRequest>> ListByApplicantAsync(string applicantId, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveRequest>> ListByApproverAsync(string approverId, CancellationToken cancellationToken);
    Task<IReadOnlyList<LeaveRequest>> ListAllAsync(CancellationToken cancellationToken);
}

public interface ILeaveBalanceRepository
{
    Task<LeaveBalance> GetOrCreateAsync(string userId, CancellationToken cancellationToken);
    Task SaveAsync(LeaveBalance balance, CancellationToken cancellationToken);
}

public interface IApprovalRouteRepository
{
    Task<ApprovalRoute?> GetByApplicantIdAsync(string applicantId, CancellationToken cancellationToken);
    Task UpsertAsync(ApprovalRoute route, CancellationToken cancellationToken);
}

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
