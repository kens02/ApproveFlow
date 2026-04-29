using ApproveFlow.Domain.Enums;

namespace ApproveFlow.Domain.Entities;

public sealed class ApprovalRouteStep
{
    public required int StepOrder { get; init; }
    public required string ApproverId { get; init; }
    public required string ApproverName { get; init; }
    public required string NotificationEmail { get; init; }
    public string MailTemplate { get; init; } = "{ApplicantName} has requested leave.";
    public UserRole Role { get; init; } = UserRole.Approver;
}
