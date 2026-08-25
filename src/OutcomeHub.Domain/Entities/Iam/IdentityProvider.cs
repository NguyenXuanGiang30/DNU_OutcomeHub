namespace OutcomeHub.Domain.Entities.Iam;

public sealed class IdentityProvider
{
    private readonly List<ExternalIdentity> _externalIdentities = [];
    private readonly List<IdpGroupRoleMapping> _groupRoleMappings = [];

    private IdentityProvider()
    {
    }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Protocol { get; private set; } = null!;
    public string IssuerOrEntityId { get; private set; } = null!;
    public string? ClientId { get; private set; }
    public string? MetadataUrl { get; private set; }
    public string ClaimsMapping { get; private set; } = null!;
    public int ClaimsMappingVersion { get; private set; }
    public string? SecretReference { get; private set; }
    public string Status { get; private set; } = null!;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }

    public IReadOnlyCollection<ExternalIdentity> ExternalIdentities => _externalIdentities;
    public IReadOnlyCollection<IdpGroupRoleMapping> GroupRoleMappings => _groupRoleMappings;
}
