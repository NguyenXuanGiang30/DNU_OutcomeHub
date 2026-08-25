namespace OutcomeHub.Domain.Entities.Result;

public sealed class PublicationRevocation
{
    private PublicationRevocation()
    {
    }

    public Guid Id { get; private set; }

    public Guid PublicationId { get; private set; }

    public string Reason { get; private set; } = null!;

    public Guid RevokedBy { get; private set; }

    public DateTimeOffset RevokedAt { get; private set; }

    public Guid DecisionId { get; private set; }

    public Publication Publication { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal RevokedByPrincipal { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.DecisionRecord Decision { get; private set; } = null!;
}
