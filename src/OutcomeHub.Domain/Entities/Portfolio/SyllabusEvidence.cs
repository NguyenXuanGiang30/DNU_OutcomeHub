using OutcomeHub.Domain.Entities.Document;

namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class SyllabusEvidence
{
    private SyllabusEvidence() { }
    public Guid SyllabusVersionId { get; private set; }
    public Guid EvidenceVersionId { get; private set; }
    public string LinkRole { get; private set; } = null!;
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public EvidenceVersion EvidenceVersion { get; private set; } = null!;
}
