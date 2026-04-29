using ApproveFlow.Application.Abstractions;
using ApproveFlow.Application.Models;

namespace ApproveFlow.Application.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly ILeaveRequestRepository _requestRepository;
    private readonly ILeaveBalanceService _balanceService;

    public DashboardService(ILeaveRequestRepository requestRepository, ILeaveBalanceService balanceService)
    {
        _requestRepository = requestRepository;
        _balanceService = balanceService;
    }

    public async Task<DashboardSummary> BuildAsync(string userId, int year, bool fiscalYear, CancellationToken cancellationToken)
    {
        var requests = await _requestRepository.ListByApplicantAsync(userId, cancellationToken);
        var filtered = requests.Where(x => InPeriod(x.StartDateTime, year, fiscalYear)).ToList();
        var monthly = filtered
            .GroupBy(x => x.StartDateTime.Month)
            .Select(g => new MonthlyStat(g.Key, g.Sum(r => (decimal)(r.EndDateTime - r.StartDateTime).TotalMinutes / 465m)))
            .OrderBy(x => x.Month)
            .ToList();

        var balance = await _balanceService.GetAsync(userId, cancellationToken);
        return new DashboardSummary(userId, balance.RemainingDays, monthly);
    }

    private static bool InPeriod(DateTimeOffset date, int year, bool fiscalYear)
    {
        if (!fiscalYear)
        {
            return date.Year == year;
        }

        var start = new DateTimeOffset(year, 4, 1, 0, 0, 0, TimeSpan.Zero);
        var end = start.AddYears(1);
        return date >= start && date < end;
    }
}
