using ApproveFlow.Domain.Enums;

namespace ApproveFlow.Application.Models;

public sealed record CreateLeaveRequestInput(
    string ApplicantId,
    string ApplicantName,
    string LeaveType,
    UnitType UnitType,
    DateTimeOffset StartDateTime,
    DateTimeOffset EndDateTime,
    string Reason,
    string? AttachmentFileName,
    bool SubmitImmediately);

public sealed record RouteStepInput(
    int StepOrder,
    string ApproverId,
    string ApproverName,
    string NotificationEmail,
    string MailTemplate);

public sealed record SetRouteInput(string ApplicantId, IReadOnlyList<RouteStepInput> Steps);

public sealed record ActionInput(Guid RequestId, string UserId, string? Comment);
