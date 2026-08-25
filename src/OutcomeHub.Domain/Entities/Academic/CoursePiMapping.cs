namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CoursePiMapping
{
    private CoursePiMapping() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid ProgramCourseId { get; private set; }
    public Guid ProgramPiId { get; private set; }
    public string ContributionLevel { get; private set; } = null!;
    public bool IsDirectAssessment { get; private set; }
    public string? Rationale { get; private set; }
    public string SourceType { get; private set; } = null!;
    public Guid? SourceSharedMappingId { get; private set; }
    public bool IsLocked { get; private set; }
    public Guid? ExceptionDecisionId { get; private set; }

    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public ProgramCourse ProgramCourse { get; private set; } = null!;
    public ProgramPi ProgramPi { get; private set; } = null!;
    public SharedCoursePiMapping? SourceSharedMapping { get; private set; }
    public DecisionRecord? ExceptionDecision { get; private set; }

    public static CoursePiMapping Create(
        Guid id,
        Guid programVersionId,
        Guid programCourseId,
        Guid programPiId,
        string contributionLevel,
        bool isDirectAssessment,
        string? rationale,
        string sourceType = "PROGRAM",
        Guid? sourceSharedMappingId = null,
        bool isLocked = false,
        Guid? exceptionDecisionId = null)
    {
        return new CoursePiMapping
        {
            Id = id,
            ProgramVersionId = programVersionId,
            ProgramCourseId = programCourseId,
            ProgramPiId = programPiId,
            ContributionLevel = contributionLevel.Trim().ToUpperInvariant(),
            IsDirectAssessment = isDirectAssessment,
            Rationale = rationale?.Trim(),
            SourceType = sourceType.Trim().ToUpperInvariant(),
            SourceSharedMappingId = sourceSharedMappingId,
            IsLocked = isLocked,
            ExceptionDecisionId = exceptionDecisionId,
        };
    }
}
