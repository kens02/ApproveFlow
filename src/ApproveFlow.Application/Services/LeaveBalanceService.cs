using ApproveFlow.Application.Abstractions;
using ApproveFlow.Application.Models;
using ApproveFlow.Domain.Constants;

namespace ApproveFlow.Application.Services;

public sealed class LeaveBalanceService : ILeaveBalanceService
{
    private readonly ILeaveBalanceRepository _balanceRepository;

    public LeaveBalanceService(ILeaveBalanceRepository balanceRepository)
    {
        _balanceRepository = balanceRepository;
    }

    public async Task<LeaveBalanceSummary> GetAsync(string userId, CancellationToken cancellationToken)
    {
        var entity = await _balanceRepository.GetOrCreateAsync(userId, cancellationToken);
        if (entity.GrantedDays <= 0)
        {
            entity.GrantedDays = LeavePolicy.MonthlyGrantedDays * 12;
            entity.CarryOverDays = Math.Min(entity.CarryOverDays, LeavePolicy.MaxCarryOverDays);
            await _balanceRepository.SaveAsync(entity, cancellationToken);
        }

        return new LeaveBalanceSummary(entity.UserId, entity.GrantedDays, entity.UsedDays, entity.CarryOverDays, entity.RemainingDays);
    }
}
