using ApproveFlow.Domain.Constants;

namespace ApproveFlow.Domain.Entities;

public sealed class ApprovalRoute
{
    public required string ApplicantId { get; init; }
    public required IReadOnlyList<ApprovalRouteStep> Steps { get; init; }

    public void Validate()
    {
        if (Steps.Count < LeavePolicy.MinApprovers)
        {
            throw new InvalidOperationException($"Approval route requires at least {LeavePolicy.MinApprovers} approvers.");
        }

        if (Steps.Count > LeavePolicy.MaxApprovers)
        {
            throw new InvalidOperationException($"Approval route supports up to {LeavePolicy.MaxApprovers} approvers for PoC.");
        }

        var expected = 1;
        foreach (var step in Steps.OrderBy(s => s.StepOrder))
        {
            if (step.StepOrder != expected)
            {
                throw new InvalidOperationException("Approval steps must be sequential starting from 1.");
            }

            expected++;
        }
    }
}
