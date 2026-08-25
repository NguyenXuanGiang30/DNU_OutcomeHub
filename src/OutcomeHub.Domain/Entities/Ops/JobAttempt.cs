namespace OutcomeHub.Domain.Entities.Ops;

public sealed class JobAttempt
{
    private JobAttempt() { }

    public Guid OperationJobId { get; private set; }
    public int AttemptNo { get; private set; }
    public string WorkerId { get; private set; } = null!;
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? HeartbeatAt { get; private set; }
    public DateTimeOffset? FinishedAt { get; private set; }
    public string? Outcome { get; private set; }
    public string? ErrorCode { get; private set; }
    public string? LogReference { get; private set; }

    public OperationJob OperationJob { get; private set; } = null!;
}
