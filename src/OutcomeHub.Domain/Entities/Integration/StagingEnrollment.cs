using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Domain.Entities.Integration;

public sealed class StagingEnrollment
{
    private StagingEnrollment() { }

    public Guid Id { get; private set; }
    public Guid IngestionBatchId { get; private set; }
    public int RowNo { get; private set; }
    public long RawRecordId { get; private set; }
    public string StudentCode { get; private set; } = null!;
    public string OfferingCode { get; private set; } = null!;
    public string EnrollmentStatus { get; private set; } = null!;
    public Guid? ResolvedEnrollmentId { get; private set; }
    public string ValidationStatus { get; private set; } = null!;
    public string RowChecksum { get; private set; } = null!;

    public IngestionBatch IngestionBatch { get; private set; } = null!;
    public RawRecord RawRecord { get; private set; } = null!;
    public Enrollment? ResolvedEnrollment { get; private set; }
}
