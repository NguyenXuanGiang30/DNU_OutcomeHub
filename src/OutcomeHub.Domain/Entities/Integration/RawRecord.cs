namespace OutcomeHub.Domain.Entities.Integration;

public sealed class RawRecord
{
    private RawRecord() { }

    public long Id { get; private set; }
    public Guid IngestionBatchId { get; private set; }
    public int RowNo { get; private set; }
    public string? SourceRecordId { get; private set; }
    public DateTimeOffset? SourceUpdatedAt { get; private set; }
    public string Payload { get; private set; } = null!;
    public string PayloadChecksum { get; private set; } = null!;
    public DateTimeOffset ReceivedAt { get; private set; }

    public IngestionBatch IngestionBatch { get; private set; } = null!;
}
