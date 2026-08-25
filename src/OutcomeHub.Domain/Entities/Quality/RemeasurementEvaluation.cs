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

    /// <summary>
    /// Creates a remeasurement evaluation linking before and after batches.
    /// Automatically computes delta = afterValue - baselineValue.
    /// </summary>
    public static RemeasurementEvaluation Create(
        Guid id,
        Guid improvementPlanId,
        Guid beforeBatchId,
        Guid afterBatchId,
        string comparabilityStatus,
        decimal? baselineValue,
        decimal? afterValue,
        string conclusion,
        Guid verifiedBy,
        DateTimeOffset verifiedAt)
    {
        if (beforeBatchId == afterBatchId)
        {
            throw new ArgumentException("Before and after batches must be different.", nameof(afterBatchId));
        }

        decimal? deltaValue = null;
        if (baselineValue.HasValue && afterValue.HasValue)
        {
            deltaValue = afterValue.Value - baselineValue.Value;
        }

        return new RemeasurementEvaluation
        {
            Id = id,
            ImprovementPlanId = improvementPlanId,
            BeforeBatchId = beforeBatchId,
            AfterBatchId = afterBatchId,
            ComparabilityStatus = comparabilityStatus,
            BaselineValue = baselineValue,
            AfterValue = afterValue,
            DeltaValue = deltaValue,
            Conclusion = conclusion,
            VerifiedBy = verifiedBy,
            VerifiedAt = verifiedAt
        };
    }
}
