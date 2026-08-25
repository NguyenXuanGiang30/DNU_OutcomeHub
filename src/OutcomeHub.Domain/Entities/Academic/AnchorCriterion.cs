using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Domain.Entities.Academic;

public sealed class AnchorCriterion
{
    private AnchorCriterion() { }

    public Guid AnchorAssessmentId { get; private set; }
    public Guid SyllabusTraceabilityId { get; private set; }

    public AnchorAssessment AnchorAssessment { get; private set; } = null!;
    public SyllabusTraceability SyllabusTraceability { get; private set; } = null!;
}
