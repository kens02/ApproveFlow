namespace ApproveFlow.Domain.Entities;

public sealed class LeaveBalance
{
    public required string UserId { get; init; }
    public decimal GrantedDays { get; set; }
    public decimal UsedDays { get; set; }
    public decimal CarryOverDays { get; set; }

    public decimal RemainingDays => GrantedDays + CarryOverDays - UsedDays;
}
