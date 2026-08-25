namespace OutcomeHub.Domain.Entities.Iam;

public sealed class ExternalIdentity
{
    private ExternalIdentity()
    {
    }

    public Guid Id { get; private set; }
    public Guid UserPrincipalId { get; private set; }
    public Guid IdentityProviderId { get; private set; }
    public string Subject { get; private set; } = null!;
    public string? ClaimsSnapshot { get; private set; }
    public string ClaimsHash { get; private set; } = null!;
    public DateTimeOffset FirstSeenAt { get; private set; }
    public DateTimeOffset LastSeenAt { get; private set; }

    public UserAccount UserAccount { get; private set; } = null!;
    public IdentityProvider IdentityProvider { get; private set; } = null!;
}
