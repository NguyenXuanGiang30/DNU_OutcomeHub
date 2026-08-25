namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class SnapshotResource
{
    private SnapshotResource()
    {
    }

    public Guid InputSnapshotId { get; private set; }

    public string ResourceType { get; private set; } = null!;

    public Guid ResourceId { get; private set; }

    public Guid VersionId { get; private set; }

    public string Checksum { get; private set; } = null!;

    public string CanonicalPayload { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public InputSnapshot InputSnapshot { get; private set; } = null!;
}
