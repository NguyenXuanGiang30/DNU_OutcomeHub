using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Domain.Entities.Integration;

public sealed class StagingCourseOffering
{
    private StagingCourseOffering() { }

    public Guid Id { get; private set; }
    public Guid IngestionBatchId { get; private set; }
    public int RowNo { get; private set; }
    public long RawRecordId { get; private set; }
    public string OfferingCode { get; private set; } = null!;
    public string CourseCode { get; private set; } = null!;
    public string AcademicYear { get; private set; } = null!;
    public string TermCode { get; private set; } = null!;
    public string? SectionCode { get; private set; }
    public Guid? ResolvedCourseOfferingId { get; private set; }
    public string ValidationStatus { get; private set; } = null!;
    public string RowChecksum { get; private set; } = null!;

    public IngestionBatch IngestionBatch { get; private set; } = null!;
    public RawRecord RawRecord { get; private set; } = null!;
    public CourseOffering? ResolvedCourseOffering { get; private set; }
}
