using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Integration;

public sealed class IdempotencyRecord
{
    private IdempotencyRecord() { }

    public Guid Id { get; private set; }
    public Guid PrincipalId { get; private set; }
    public string OperationCode { get; private set; } = null!;
    public string IdempotencyKey { get; private set; } = null!;
    public string RequestHash { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public Guid? LockedBy { get; private set; }
    public DateTimeOffset? LockedUntil { get; private set; }
    public int? ResponseStatus { get; private set; }
    public string? ResponseHeaders { get; private set; }
    public string? ResponseBody { get; private set; }
    public Guid? ResourceId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    public Principal Principal { get; private set; } = null!;
    public Principal? LockedByPrincipal { get; private set; }
}
