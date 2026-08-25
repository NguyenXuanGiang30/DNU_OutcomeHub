using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Integration;

public sealed class QuarantineRecord
{
    private readonly List<QuarantineCorrection> _corrections = [];

    private QuarantineRecord() { }

    public Guid Id { get; private set; }
    public Guid IngestionBatchId { get; private set; }
    public long RawRecordId { get; private set; }
    public string ReasonCode { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public Guid? OwnerPrincipalId { get; private set; }
    public Guid? CurrentCorrectionId { get; private set; }
    public string? ResolutionReason { get; private set; }
    public Guid? ResolvedBy { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }
    public Guid? ReprocessBatchId { get; private set; }
    public long RowVersion { get; private set; }

    public IngestionBatch IngestionBatch { get; private set; } = null!;
    public RawRecord RawRecord { get; private set; } = null!;
    public Principal? OwnerPrincipal { get; private set; }
    public QuarantineCorrection? CurrentCorrection { get; private set; }
    public Principal? ResolvedByPrincipal { get; private set; }
    public IngestionBatch? ReprocessBatch { get; private set; }
    public IReadOnlyCollection<QuarantineCorrection> Corrections => _corrections;
}
