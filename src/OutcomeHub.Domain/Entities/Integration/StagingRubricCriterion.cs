using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Domain.Entities.Integration;

public sealed class StagingRubricCriterion
{
    private StagingRubricCriterion() { }

    public Guid Id { get; private set; }
    public Guid IngestionBatchId { get; private set; }
    public int RowNo { get; private set; }
    public long RawRecordId { get; private set; }
    public string RubricCode { get; private set; } = null!;
    public string CriterionCode { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public decimal? MaxScore { get; private set; }
    public Guid? ResolvedRubricCriterionId { get; private set; }
    public string ValidationStatus { get; private set; } = null!;
    public string RowChecksum { get; private set; } = null!;

    public IngestionBatch IngestionBatch { get; private set; } = null!;
    public RawRecord RawRecord { get; private set; } = null!;
    public RubricCriterion? ResolvedRubricCriterion { get; private set; }
}
