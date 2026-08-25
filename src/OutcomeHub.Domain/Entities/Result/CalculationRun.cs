namespace OutcomeHub.Domain.Entities.Result;

public sealed class CalculationRun
{
    private CalculationRun()
    {
    }

    public Guid Id { get; private set; }

    public Guid BatchId { get; private set; }

    public int AttemptNo { get; private set; }

    public string WorkerId { get; private set; } = null!;

    public string Status { get; private set; } = null!;

    public DateTimeOffset StartedAt { get; private set; }

    public DateTimeOffset? HeartbeatAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public decimal ProgressRatio { get; private set; }

    public string? ErrorCode { get; private set; }

    public string? ErrorDetail { get; private set; }

    public string? LogReference { get; private set; }

    public ResultBatch Batch { get; private set; } = null!;
}
