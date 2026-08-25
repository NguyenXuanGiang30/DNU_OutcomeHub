namespace OutcomeHub.Domain.Entities.Iam;

public sealed class ServiceCredential
{
    private ServiceCredential()
    {
    }

    public Guid Id { get; private set; }
    public Guid ServicePrincipalId { get; private set; }
    public string CredentialType { get; private set; } = null!;
    public string? KeyPrefix { get; private set; }
    public string? SecretHash { get; private set; }
    public string? SecretReference { get; private set; }
    public string? CertificateThumbprint { get; private set; }
    public string? PublicJwk { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? RevokedBy { get; private set; }
    public string? RevokeReason { get; private set; }
    public DateTimeOffset? LastUsedAt { get; private set; }

    public ServiceAccount ServiceAccount { get; private set; } = null!;
    public Principal? RevokedByPrincipal { get; private set; }
}
