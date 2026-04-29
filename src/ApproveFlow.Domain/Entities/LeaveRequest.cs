using ApproveFlow.Domain.Enums;

namespace ApproveFlow.Domain.Entities;

public sealed class LeaveRequest
{
    public required Guid Id { get; init; }
    public required string ApplicantId { get; init; }
    public required string ApplicantName { get; init; }
    public required string LeaveType { get; init; }
    public required UnitType UnitType { get; init; }
    public required DateTimeOffset StartDateTime { get; init; }
    public required DateTimeOffset EndDateTime { get; init; }
    public required string Reason { get; init; }
    public string? AttachmentFileName { get; init; }
    public RequestStatus Status { get; set; }
    public required List<WorkflowStep> WorkflowSteps { get; init; }
    public required List<ActionHistory> ActionHistories { get; init; }

    public int CurrentStepOrder
    {
        get
        {
            var waiting = WorkflowSteps.FirstOrDefault(x => x.Status == WorkflowStepStatus.Waiting);
            return waiting?.StepOrder ?? 0;
        }
    }
}
