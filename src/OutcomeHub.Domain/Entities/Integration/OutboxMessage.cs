namespace OutcomeHub.Domain.Entities.Integration;

public sealed class OutboxMessage
{
    private OutboxMessage() { }

    public Guid Id { get; private set; }
    public string AggregateType { get; private set; } = null!;
    public Guid AggregateId { get; private set; }
    public long AggregateVersion { get; private set; }
    public string EventType { get; private set; } = null!;
    public int EventSchemaVersion { get; private set; }
    public string Payload { get; private set; } = null!;
    public string? Headers { get; private set; }
    public string Classification { get; private set; } = null!;
    public Guid CorrelationId { get; private set; }
    public Guid? CausationId { get; private set; }
    public string? TraceId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public DateTimeOffset AvailableAt { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }
    public int AttemptCount { get; private set; }
    public Guid? LockedBy { get; private set; }
    public DateTimeOffset? LockedUntil { get; private set; }
    public string Status { get; private set; } = null!;
    public string? LastErrorCode { get; private set; }
}
