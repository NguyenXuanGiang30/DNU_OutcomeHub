using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Ops;

public sealed class OperationJob
{
    private readonly List<JobAttempt> _attempts = [];

    private OperationJob() { }

    public Guid Id { get; private set; }
    public string JobType { get; private set; } = null!;
    public string SubjectType { get; private set; } = null!;
    public Guid SubjectId { get; private set; }
    public string Status { get; private set; } = null!;
    public long ProgressCurrent { get; private set; }
    public long? ProgressTotal { get; private set; }
    public string QueueName { get; private set; } = null!;
    public string? TransportMessageId { get; private set; }
    public DateTimeOffset AvailableAt { get; private set; }
    public int Priority { get; private set; }
    public int AttemptCount { get; private set; }
    public int MaxAttempts { get; private set; }
    public Guid RequestedBy { get; private set; }
    public Guid AccessScopeId { get; private set; }
    public Guid? LeasedByPrincipalId { get; private set; }
    public DateTimeOffset? LeaseUntil { get; private set; }
    public Guid RequestId { get; private set; }
    public Guid CorrelationId { get; private set; }
    public Guid? CancelRequestedBy { get; private set; }
    public DateTimeOffset? CancelRequestedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? HeartbeatAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public string? ErrorCode { get; private set; }
    public string? ErrorDetailRedacted { get; private set; }
    public long RowVersion { get; private set; }

    public Principal RequestedByPrincipal { get; private set; } = null!;
    public AccessScope AccessScope { get; private set; } = null!;
    public Principal? LeasedByPrincipal { get; private set; }
    public Principal? CancelRequestedByPrincipal { get; private set; }
    public IReadOnlyCollection<JobAttempt> Attempts => _attempts;
}
