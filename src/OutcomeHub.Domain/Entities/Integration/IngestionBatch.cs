using OutcomeHub.Domain.Entities.Document;
using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Domain.Entities.Integration;

public sealed class IngestionBatch
{
    private IngestionBatch() { }

    public Guid Id { get; private set; }
    public Guid GovernedResourceId { get; private set; }
    public Guid SourceSystemId { get; private set; }
    public string DataType { get; private set; } = null!;
    public string? SourceBatchId { get; private set; }
    public string IdempotencyKey { get; private set; } = null!;
    public int SchemaVersion { get; private set; }
    public string PayloadChecksum { get; private set; } = null!;
    public Guid? FileObjectId { get; private set; }
    public string Classification { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public DateTimeOffset ReceivedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public long TotalCount { get; private set; }
    public long AcceptedCount { get; private set; }
    public long RejectedCount { get; private set; }

    public GovernedResource GovernedResource { get; private set; } = null!;
    public SourceSystem SourceSystem { get; private set; } = null!;
    public FileObject? FileObject { get; private set; }
}
