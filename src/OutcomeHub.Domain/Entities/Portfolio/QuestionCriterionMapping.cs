namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class QuestionCriterionMapping
{
    private QuestionCriterionMapping() { }
    public Guid QuestionId { get; private set; }
    public Guid RubricCriterionId { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public decimal CriterionWeightRatio { get; private set; }
    public AssessmentQuestion Question { get; private set; } = null!;
    public RubricCriterion RubricCriterion { get; private set; } = null!;
}
