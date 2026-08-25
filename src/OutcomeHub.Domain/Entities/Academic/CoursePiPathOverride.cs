namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CoursePiPathOverride
{
    private CoursePiPathOverride() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid CoursePiMappingId { get; private set; }
    public Guid CurriculumPathId { get; private set; }
    public string ContributionLevel { get; private set; } = null!;
    public bool DirectAssessmentEnabled { get; private set; }
    public Guid ExceptionDecisionId { get; private set; }
    public string Rationale { get; private set; } = null!;

    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public CoursePiMapping CoursePiMapping { get; private set; } = null!;
    public CurriculumPath CurriculumPath { get; private set; } = null!;
    public DecisionRecord ExceptionDecision { get; private set; } = null!;
}
