namespace OutcomeHub.Domain.Entities.Integration;

public sealed class SourceRecordMap
{
    private SourceRecordMap() { }

    public Guid SourceSystemId { get; private set; }
    public string EntityType { get; private set; } = null!;
    public string SourceRecordId { get; private set; } = null!;
    public Guid TargetId { get; private set; }
    public DateTimeOffset? SourceUpdatedAt { get; private set; }
    public string LastPayloadChecksum { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public DateTimeOffset UpdatedAt { get; private set; }

    public SourceSystem SourceSystem { get; private set; } = null!;
}
