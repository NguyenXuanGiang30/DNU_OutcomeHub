namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class TeachingSessionAssessment
{
    private TeachingSessionAssessment() { }
    public Guid TeachingSessionId { get; private set; }
    public Guid AssessmentItemId { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public TeachingSession TeachingSession { get; private set; } = null!;
    public AssessmentItem AssessmentItem { get; private set; } = null!;
}
