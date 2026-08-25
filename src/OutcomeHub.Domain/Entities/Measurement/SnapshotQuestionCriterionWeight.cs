namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class SnapshotQuestionCriterionWeight
{
    private SnapshotQuestionCriterionWeight()
    {
    }

    public Guid InputSnapshotId { get; private set; }

    public Guid AssessmentQuestionId { get; private set; }

    public Guid RubricCriterionId { get; private set; }

    public string SourceMode { get; private set; } = null!;

    public decimal CriterionWeightRatio { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public InputSnapshot InputSnapshot { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.AssessmentQuestion AssessmentQuestion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.RubricCriterion RubricCriterion { get; private set; } = null!;
}
