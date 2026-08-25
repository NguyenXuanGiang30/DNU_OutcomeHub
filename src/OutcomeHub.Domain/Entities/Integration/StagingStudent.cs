using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Domain.Entities.Integration;

public sealed class StagingStudent
{
    private StagingStudent() { }

    public Guid Id { get; private set; }
    public Guid IngestionBatchId { get; private set; }
    public int RowNo { get; private set; }
    public long RawRecordId { get; private set; }
    public string StudentCode { get; private set; } = null!;
    public string? FullName { get; private set; }
    public string? Email { get; private set; }
    public Guid? ResolvedStudentId { get; private set; }
    public string ValidationStatus { get; private set; } = null!;
    public string RowChecksum { get; private set; } = null!;

    public IngestionBatch IngestionBatch { get; private set; } = null!;
    public RawRecord RawRecord { get; private set; } = null!;
    public Student? ResolvedStudent { get; private set; }
}
