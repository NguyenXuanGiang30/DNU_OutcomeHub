namespace OutcomeHub.Domain.Entities.Quality;

public sealed class RemeasurementEvaluation
{
    private RemeasurementEvaluation()
    {
    }

    public Guid Id { get; private set; }

    public Guid ImprovementPlanId { get; private set; }

    public Guid BeforeBatchId { get; private set; }

    public Guid AfterBatchId { get; private set; }

    public string ComparabilityStatus { get; private set; } = null!;

    public decimal? BaselineValue { get; private set; }

    public decimal? AfterValue { get; private set; }

    public decimal? DeltaValue { get; private set; }

    public string Conclusion { get; private set; } = null!;

    public Guid VerifiedBy { get; private set; }

    public DateTimeOffset VerifiedAt { get; private set; }

    public ImprovementPlan ImprovementPlan { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Result.ResultBatch BeforeBatch { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Result.ResultBatch AfterBatch { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal VerifiedByPrincipal { get; private set; } = null!;
}
