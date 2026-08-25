using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class ModelDeployment
{
    private ModelDeployment()
    {
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public Guid OwnerOrgUnitId { get; private set; }

    public OrgUnit OwnerOrgUnit { get; private set; } = null!;
}
