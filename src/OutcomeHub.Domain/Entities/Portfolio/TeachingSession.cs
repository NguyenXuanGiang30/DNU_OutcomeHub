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

    public static TeachingSession Create(
        Guid id,
        Guid syllabusVersionId,
        int sessionNo,
        string title,
        decimal plannedHours,
        string teachingMethod,
        string? assessmentMethod = null,
        string? selfStudyTask = null,
        int sortOrder = 1)
    {
        return new TeachingSession
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            SyllabusVersionId = syllabusVersionId,
            SessionNo = sessionNo,
            Title = string.IsNullOrWhiteSpace(title) ? $"Session {sessionNo}" : title.Trim(),
            PlannedHours = plannedHours <= 0 ? 3.0m : plannedHours,
            TeachingMethod = string.IsNullOrWhiteSpace(teachingMethod) ? "LECTURE_PRACTICE" : teachingMethod.Trim(),
            AssessmentMethod = assessmentMethod,
            SelfStudyTask = selfStudyTask,
            SortOrder = sortOrder
        };
    }
}
