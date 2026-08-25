namespace OutcomeHub.Domain.Entities.Integration;

public sealed class SyncJob
{
    private SyncJob() { }

    public Guid Id { get; private set; }
    public Guid SourceSystemId { get; private set; }
    public string DataType { get; private set; } = null!;
    public string Mode { get; private set; } = null!;
    public string? CursorFrom { get; private set; }
    public string? CursorTo { get; private set; }
    public DateTimeOffset? UpdatedSince { get; private set; }
    public string Status { get; private set; } = null!;
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public long ReadCount { get; private set; }
    public long AcceptedCount { get; private set; }
    public long RejectedCount { get; private set; }
    public string? ErrorSummary { get; private set; }
    public Guid RequestId { get; private set; }

    public SourceSystem SourceSystem { get; private set; } = null!;
}
