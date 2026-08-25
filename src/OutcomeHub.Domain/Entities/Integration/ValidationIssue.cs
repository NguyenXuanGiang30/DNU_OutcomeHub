using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Integration;

public sealed class ValidationIssue
{
    private ValidationIssue() { }

    public Guid Id { get; private set; }
    public Guid IngestionBatchId { get; private set; }
    public long? RawRecordId { get; private set; }
    public string? StagingTable { get; private set; }
    public Guid? StagingRowId { get; private set; }
    public string? FieldName { get; private set; }
    public string ErrorCode { get; private set; } = null!;
    public string Severity { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public string? SuggestedAction { get; private set; }
    public string Status { get; private set; } = null!;
    public Guid? ResolvedBy { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }

    public IngestionBatch IngestionBatch { get; private set; } = null!;
    public RawRecord? RawRecord { get; private set; }
    public Principal? ResolvedByPrincipal { get; private set; }
}
