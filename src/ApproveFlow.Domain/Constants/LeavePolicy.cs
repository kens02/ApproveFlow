namespace ApproveFlow.Domain.Constants;

public static class LeavePolicy
{
    public const int MinutesPerDay = 465;
    public const int MinutesStep = 15;
    public const decimal MonthlyGrantedDays = 2.0m;
    public const decimal MaxCarryOverDays = 30.0m;
    public const int MinApprovers = 4;
    public const int MaxApprovers = 20;
}
