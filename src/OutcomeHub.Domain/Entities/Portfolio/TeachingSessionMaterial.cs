namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class TeachingSessionMaterial
{
    private TeachingSessionMaterial() { }
    public Guid TeachingSessionId { get; private set; }
    public Guid LearningMaterialId { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public TeachingSession TeachingSession { get; private set; } = null!;
    public LearningMaterial LearningMaterial { get; private set; } = null!;
}
