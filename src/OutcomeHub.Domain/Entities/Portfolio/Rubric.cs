namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class Rubric
{
    private Rubric() { }
    public Guid Id { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public Guid SyllabusTemplateVersionId { get; private set; }
    public Guid AssessmentItemId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public decimal MaxScore { get; private set; }
    public Guid RubricScaleId { get; private set; }
    public string Checksum { get; private set; } = null!;
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public SyllabusTemplateVersion SyllabusTemplateVersion { get; private set; } = null!;
    public AssessmentItem AssessmentItem { get; private set; } = null!;
    public SyllabusTemplateRubricScale RubricScale { get; private set; } = null!;
    public ICollection<RubricCriterion> Criteria { get; private set; } = new List<RubricCriterion>();

    public static Rubric Create(
        Guid id,
        Guid syllabusVersionId,
        Guid syllabusTemplateVersionId,
        Guid assessmentItemId,
        string code,
        string name,
        decimal maxScore,
        Guid rubricScaleId,
        string checksum)
    {
        return new Rubric
        {
            Id = id,
            SyllabusVersionId = syllabusVersionId,
            SyllabusTemplateVersionId = syllabusTemplateVersionId,
            AssessmentItemId = assessmentItemId,
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            MaxScore = maxScore,
            RubricScaleId = rubricScaleId,
            Checksum = checksum.ToLowerInvariant(),
        };
    }
}
