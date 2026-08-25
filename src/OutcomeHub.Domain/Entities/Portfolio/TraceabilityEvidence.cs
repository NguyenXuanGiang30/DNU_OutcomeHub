using OutcomeHub.Domain.Entities.Document;

namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class TraceabilityEvidence
{
    private TraceabilityEvidence() { }
    public Guid SyllabusTraceabilityId { get; private set; }
    public Guid EvidenceVersionId { get; private set; }
    public string LinkRole { get; private set; } = null!;
    public SyllabusTraceability SyllabusTraceability { get; private set; } = null!;
    public EvidenceVersion EvidenceVersion { get; private set; } = null!;
}
