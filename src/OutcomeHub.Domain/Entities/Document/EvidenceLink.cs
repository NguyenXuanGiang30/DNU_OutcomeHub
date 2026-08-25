namespace OutcomeHub.Domain.Entities.Document;

public sealed class EvidenceLink
{
    private EvidenceLink() { }
    public Guid EvidenceVersionId { get; private set; }
    public string ResourceType { get; private set; } = null!;
    public Guid ResourceId { get; private set; }
    public string LinkRole { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public EvidenceVersion EvidenceVersion { get; private set; } = null!;
}
