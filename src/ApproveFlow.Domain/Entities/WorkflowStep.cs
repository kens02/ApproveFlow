using ApproveFlow.Domain.Enums;

namespace ApproveFlow.Domain.Entities;

public sealed class WorkflowStep
{
    public required int StepOrder { get; init; }
    public required string ApproverId { get; init; }
    public WorkflowStepStatus Status { get; set; }
    public DateTimeOffset? ActionedAt { get; set; }
    public string? Comment { get; set; }
}
