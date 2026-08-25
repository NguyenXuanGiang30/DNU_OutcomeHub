using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Domain.Entities.Integration;

public sealed class StagingScore
{
    private StagingScore() { }

    public Guid Id { get; private set; }
    public Guid IngestionBatchId { get; private set; }
    public int RowNo { get; private set; }
    public long RawRecordId { get; private set; }
    public string StudentCode { get; private set; } = null!;
    public string OfferingCode { get; private set; } = null!;
    public string AssessmentCode { get; private set; } = null!;
    public string? CriterionCode { get; private set; }
    public decimal? RawScore { get; private set; }
    public decimal? MaxScore { get; private set; }
    public short? ResolvedScoreAcademicYearStart { get; private set; }
    public Guid? ResolvedScoreRecordId { get; private set; }
    public string ValidationStatus { get; private set; } = null!;
    public string RowChecksum { get; private set; } = null!;

    public IngestionBatch IngestionBatch { get; private set; } = null!;
    public RawRecord RawRecord { get; private set; } = null!;
    public ScoreRecord? ResolvedScoreRecord { get; private set; }
}
