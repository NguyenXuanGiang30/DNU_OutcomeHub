namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class AssessmentQuestion
{
    private AssessmentQuestion() { }
    public Guid Id { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public Guid AssessmentItemId { get; private set; }
    public string QuestionCode { get; private set; } = null!;
    public decimal MaxScore { get; private set; }
    public int SortOrder { get; private set; }
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public AssessmentItem AssessmentItem { get; private set; } = null!;
}
