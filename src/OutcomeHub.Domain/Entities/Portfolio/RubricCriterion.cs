namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class RubricCriterion
{
    private RubricCriterion() { }
    public Guid Id { get; private set; }
    public Guid RubricId { get; private set; }
    public Guid AssessmentItemId { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public string CriterionCode { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal MaxScore { get; private set; }
    public decimal RubricWeightRatio { get; private set; }
    public string ScoreSourceMode { get; private set; } = null!;
    public bool IsCore { get; private set; }
    public bool IndividualEvidence { get; private set; }
    public int SortOrder { get; private set; }
    public Rubric Rubric { get; private set; } = null!;
    public AssessmentItem AssessmentItem { get; private set; } = null!;
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public ICollection<RubricLevel> Levels { get; private set; } = new List<RubricLevel>();

    public static RubricCriterion Create(
        Guid id,
        Guid rubricId,
        Guid assessmentItemId,
        Guid syllabusVersionId,
        string criterionCode,
        string description,
        decimal maxScore,
        decimal rubricWeightRatio,
        string scoreSourceMode,
        bool isCore,
        bool individualEvidence,
        int sortOrder)
    {
        return new RubricCriterion
        {
            Id = id,
            RubricId = rubricId,
            AssessmentItemId = assessmentItemId,
            SyllabusVersionId = syllabusVersionId,
            CriterionCode = criterionCode.Trim().ToUpperInvariant(),
            Description = description.Trim(),
            MaxScore = maxScore,
            RubricWeightRatio = rubricWeightRatio,
            ScoreSourceMode = scoreSourceMode.Trim().ToUpperInvariant(),
            IsCore = isCore,
            IndividualEvidence = individualEvidence,
            SortOrder = sortOrder,
        };
    }
}
