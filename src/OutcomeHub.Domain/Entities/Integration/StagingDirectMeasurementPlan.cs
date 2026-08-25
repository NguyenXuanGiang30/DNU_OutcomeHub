using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Domain.Entities.Integration;

public sealed class StagingDirectMeasurementPlan
{
    private StagingDirectMeasurementPlan() { }

    public Guid Id { get; private set; }
    public Guid IngestionBatchId { get; private set; }
    public int RowNo { get; private set; }
    public long RawRecordId { get; private set; }
    public string CourseCode { get; private set; } = null!;
    public string PiCode { get; private set; } = null!;
    public string AssessmentCode { get; private set; } = null!;
    public string? CriterionCode { get; private set; }
    public decimal? Weight { get; private set; }
    public Guid? ResolvedDirectMeasurementPlanId { get; private set; }
    public string ValidationStatus { get; private set; } = null!;
    public string RowChecksum { get; private set; } = null!;

    public IngestionBatch IngestionBatch { get; private set; } = null!;
    public RawRecord RawRecord { get; private set; } = null!;
    public DirectMeasurementPlan? ResolvedDirectMeasurementPlan { get; private set; }
}
