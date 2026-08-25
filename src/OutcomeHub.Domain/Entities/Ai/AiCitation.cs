namespace OutcomeHub.Domain.Entities.Ai;

public sealed class AiCitation
{
    private AiCitation()
    {
    }

    public Guid Id { get; private set; }

    public Guid ArtifactId { get; private set; }

    public Guid SourceSnapshotId { get; private set; }

    public int? PageNo { get; private set; }

    public string? RegionPolygon { get; private set; }

    public string? RowLocator { get; private set; }

    public string? SourceTextExcerpt { get; private set; }

    public string SourceChecksum { get; private set; } = null!;

    public AiArtifact Artifact { get; private set; } = null!;

    public AiSourceSnapshot SourceSnapshot { get; private set; } = null!;
}
