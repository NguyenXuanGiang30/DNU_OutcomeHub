namespace OutcomeHub.Domain.Entities.Result;

public sealed class BatchDelta
{
    private BatchDelta()
    {
    }

    public Guid Id { get; private set; }

    public Guid OldBatchId { get; private set; }

    public Guid NewBatchId { get; private set; }

    public string EntityType { get; private set; } = null!;

    public string EntityKey { get; private set; } = null!;

    public decimal? OldValue { get; private set; }

    public decimal? NewValue { get; private set; }

    public decimal? Delta { get; private set; }

    public string? Reason { get; private set; }

    public ResultBatch OldBatch { get; private set; } = null!;
    public ResultBatch NewBatch { get; private set; } = null!;
}
