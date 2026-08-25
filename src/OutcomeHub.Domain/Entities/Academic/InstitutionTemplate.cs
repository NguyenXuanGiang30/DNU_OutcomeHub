namespace OutcomeHub.Domain.Entities.Academic;

public sealed class InstitutionTemplate
{
    private InstitutionTemplate() { }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Guid OwnerOrgUnitId { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public OrgUnit OwnerOrgUnit { get; private set; } = null!;
}
