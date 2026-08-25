namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class CalculationPolicy
{
    private CalculationPolicy()
    {
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public Guid OwnerOrgUnitId { get; private set; }

    public string? Description { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public OutcomeHub.Domain.Entities.Academic.OrgUnit OwnerOrgUnit { get; private set; } = null!;
    public ICollection<CalculationPolicyVersion> Versions { get; private set; } = new List<CalculationPolicyVersion>();
}
