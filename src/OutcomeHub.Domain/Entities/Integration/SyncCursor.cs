namespace OutcomeHub.Domain.Entities.Integration;

public sealed class SyncCursor
{
    private SyncCursor() { }

    public Guid SourceSystemId { get; private set; }
    public string ResourceType { get; private set; } = null!;
    public byte[] CursorValueCiphertext { get; private set; } = null!;
    public DateTimeOffset? LastSourceUpdatedAt { get; private set; }
    public Guid? LastSuccessfulJobId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public SourceSystem SourceSystem { get; private set; } = null!;
    public SyncJob? LastSuccessfulJob { get; private set; }
}
