using ApproveFlow.Domain.Enums;

namespace ApproveFlow.Application.Models;

public sealed record LeaveRequestSummary(
    Guid Id,
    string ApplicantId,
    string ApplicantName,
    string LeaveType,
    UnitType UnitType,
    DateTimeOffset StartDateTime,
    DateTimeOffset EndDateTime,
    RequestStatus Status,
    int CurrentStepOrder,
    decimal RequestedDays);

public sealed record LeaveBalanceSummary(string UserId, decimal GrantedDays, decimal UsedDays, decimal CarryOverDays, decimal RemainingDays);

public sealed record DashboardSummary(string UserId, decimal RemainingDays, IReadOnlyList<MonthlyStat> MonthlyStats);

public sealed record MonthlyStat(int Month, decimal UsedDays);
