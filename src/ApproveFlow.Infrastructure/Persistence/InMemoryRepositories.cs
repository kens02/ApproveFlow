using ApproveFlow.Application.Abstractions;
using ApproveFlow.Domain.Entities;

namespace ApproveFlow.Infrastructure.Persistence;

public sealed class InMemoryLeaveRequestRepository : ILeaveRequestRepository
{
    private readonly InMemoryStores _stores;

    public InMemoryLeaveRequestRepository(InMemoryStores stores)
    {
        _stores = stores;
    }

    public Task AddAsync(LeaveRequest request, CancellationToken cancellationToken)
    {
        _stores.LeaveRequests[request.Id] = request;
        return Task.CompletedTask;
    }

    public Task<LeaveRequest?> GetByIdAsync(Guid requestId, CancellationToken cancellationToken)
    {
        _stores.LeaveRequests.TryGetValue(requestId, out var request);
        return Task.FromResult(request);
    }

    public Task<IReadOnlyList<LeaveRequest>> ListByApplicantAsync(string applicantId, CancellationToken cancellationToken)
    {
        var result = _stores.LeaveRequests.Values.Where(x => x.ApplicantId == applicantId).OrderByDescending(x => x.StartDateTime).ToList();
        return Task.FromResult<IReadOnlyList<LeaveRequest>>(result);
    }

    public Task<IReadOnlyList<LeaveRequest>> ListByApproverAsync(string approverId, CancellationToken cancellationToken)
    {
        var result = _stores.LeaveRequests.Values
            .Where(x => x.WorkflowSteps.Any(step => step.ApproverId == approverId))
            .OrderByDescending(x => x.StartDateTime)
            .ToList();
        return Task.FromResult<IReadOnlyList<LeaveRequest>>(result);
    }

    public Task<IReadOnlyList<LeaveRequest>> ListAllAsync(CancellationToken cancellationToken)
    {
        var result = _stores.LeaveRequests.Values.OrderByDescending(x => x.StartDateTime).ToList();
        return Task.FromResult<IReadOnlyList<LeaveRequest>>(result);
    }
}

public sealed class InMemoryLeaveBalanceRepository : ILeaveBalanceRepository
{
    private readonly InMemoryStores _stores;

    public InMemoryLeaveBalanceRepository(InMemoryStores stores)
    {
        _stores = stores;
    }

    public Task<LeaveBalance> GetOrCreateAsync(string userId, CancellationToken cancellationToken)
    {
        var balance = _stores.LeaveBalances.GetOrAdd(userId, id => new LeaveBalance
        {
            UserId = id,
            GrantedDays = 0,
            UsedDays = 0,
            CarryOverDays = 0
        });

        return Task.FromResult(balance);
    }

    public Task SaveAsync(LeaveBalance balance, CancellationToken cancellationToken)
    {
        _stores.LeaveBalances[balance.UserId] = balance;
        return Task.CompletedTask;
    }
}

public sealed class InMemoryApprovalRouteRepository : IApprovalRouteRepository
{
    private readonly InMemoryStores _stores;

    public InMemoryApprovalRouteRepository(InMemoryStores stores)
    {
        _stores = stores;
    }

    public Task<ApprovalRoute?> GetByApplicantIdAsync(string applicantId, CancellationToken cancellationToken)
    {
        _stores.ApprovalRoutes.TryGetValue(applicantId, out var route);
        return Task.FromResult(route);
    }

    public Task UpsertAsync(ApprovalRoute route, CancellationToken cancellationToken)
    {
        _stores.ApprovalRoutes[route.ApplicantId] = route;
        return Task.CompletedTask;
    }
}

public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
