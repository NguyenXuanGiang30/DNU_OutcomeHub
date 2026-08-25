namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class TeachingSession
{
    private TeachingSession() { }
    public Guid Id { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public int SessionNo { get; private set; }
    public string Title { get; private set; } = null!;
    public decimal PlannedHours { get; private set; }
    public string TeachingMethod { get; private set; } = null!;
    public string? AssessmentMethod { get; private set; }
    public string? SelfStudyTask { get; private set; }
    public int SortOrder { get; private set; }
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
}
