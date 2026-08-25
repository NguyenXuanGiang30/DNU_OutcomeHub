using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Domain.Entities.Iam;

public sealed class ServiceAccount
{
    private readonly List<ServiceCredential> _credentials = [];

    private ServiceAccount()
    {
    }

    public Guid PrincipalId { get; private set; }
    public string ClientId { get; private set; } = null!;
    public Guid OwnerOrgUnitId { get; private set; }
    public string Purpose { get; private set; } = null!;
    public DateTimeOffset? ExpiresAt { get; private set; }
    public string TechnicalContact { get; private set; } = null!;

    public Principal Principal { get; private set; } = null!;
    public OrgUnit OwnerOrgUnit { get; private set; } = null!;
    public IReadOnlyCollection<ServiceCredential> Credentials => _credentials;
}
