using System.Net;

namespace OutcomeHub.Domain.Entities.Iam;

public sealed class AuthSession
{
    private AuthSession()
    {
    }

    public Guid Id { get; private set; }
    public Guid PrincipalId { get; private set; }
    public string SessionTokenHash { get; private set; } = null!;
    public string? IdpSessionHash { get; private set; }
    public DateTimeOffset IssuedAt { get; private set; }
    public DateTimeOffset LastSeenAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public IPAddress? IpAddress { get; private set; }
    public string? UserAgentHash { get; private set; }
    public string AuthStrength { get; private set; } = null!;
    public bool MfaUsed { get; private set; }

    public Principal Principal { get; private set; } = null!;
}
