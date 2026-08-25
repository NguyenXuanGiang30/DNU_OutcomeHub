namespace OutcomeHub.Domain.Entities.Iam;

public sealed class DatabasePrincipalBinding
{
    private DatabasePrincipalBinding()
    {
    }

    public string DatabaseRoleName { get; private set; } = null!;
    public Guid ServicePrincipalId { get; private set; }
    public Guid AccessScopeId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string Status { get; private set; } = null!;
    public string Checksum { get; private set; } = null!;

    public ServiceAccount ServiceAccount { get; private set; } = null!;
    public AccessScope AccessScope { get; private set; } = null!;
}
