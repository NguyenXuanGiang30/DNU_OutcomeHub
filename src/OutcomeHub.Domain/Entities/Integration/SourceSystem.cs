using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Integration;

public sealed class SourceSystem
{
    private SourceSystem() { }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string SystemType { get; private set; } = null!;
    public string? BaseUrl { get; private set; }
    public Guid OwnerOrgUnitId { get; private set; }
    public Guid ServicePrincipalId { get; private set; }
    public string Status { get; private set; } = null!;
    public string DataClassification { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public OrgUnit OwnerOrgUnit { get; private set; } = null!;
    public ServiceAccount ServiceAccount { get; private set; } = null!;
}
