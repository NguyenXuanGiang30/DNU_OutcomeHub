using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Domain.Entities.Academic;

public sealed class AnchorAssessment
{
    private AnchorAssessment() { }

    public Guid Id { get; private set; }
    public Guid DirectMeasurementSourceId { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public Guid AssessmentItemId { get; private set; }
    public string AnchorRole { get; private set; } = null!;
    public string EvidenceRequirement { get; private set; } = null!;
    public DateTimeOffset? ApprovedAt { get; private set; }

    public DirectMeasurementSource DirectMeasurementSource { get; private set; } = null!;
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public AssessmentItem AssessmentItem { get; private set; } = null!;
}
