namespace OutcomeHub.Domain.Entities.Result;

public sealed class BatchSupersession
{
    private BatchSupersession()
    {
    }

    public Guid OldBatchId { get; private set; }

    public Guid NewBatchId { get; private set; }

    public string Reason { get; private set; } = null!;

    public Guid CreatedBy { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public ResultBatch OldBatch { get; private set; } = null!;
    public ResultBatch NewBatch { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal CreatedByPrincipal { get; private set; } = null!;
}
