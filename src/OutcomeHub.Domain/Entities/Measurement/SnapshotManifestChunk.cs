namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class SnapshotManifestChunk
{
    private SnapshotManifestChunk()
    {
    }

    public Guid InputSnapshotId { get; private set; }

    public string EntityType { get; private set; } = null!;

    public int ChunkNo { get; private set; }

    public long RowCount { get; private set; }

    public string FirstKey { get; private set; } = null!;

    public string LastKey { get; private set; } = null!;

    public string Checksum { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public InputSnapshot InputSnapshot { get; private set; } = null!;
}
