using System.Collections.Concurrent;
using ApproveFlow.Domain.Entities;

namespace ApproveFlow.Infrastructure.Persistence;

public sealed class InMemoryStores
{
    public ConcurrentDictionary<Guid, LeaveRequest> LeaveRequests { get; } = new();
    public ConcurrentDictionary<string, ApprovalRoute> ApprovalRoutes { get; } = new(StringComparer.Ordinal);
    public ConcurrentDictionary<string, LeaveBalance> LeaveBalances { get; } = new(StringComparer.Ordinal);
}
