using ApproveFlow.Domain.Enums;

namespace ApproveFlow.Domain.Entities;

public sealed class ActionHistory
{
    public required Guid Id { get; init; }
    public required Guid RequestId { get; init; }
    public required ActionType ActionType { get; init; }
    public required string UserId { get; init; }
    public string Comment { get; init; } = string.Empty;
    public required DateTimeOffset Timestamp { get; init; }
}
