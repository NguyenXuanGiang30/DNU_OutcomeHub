using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class SyllabusTemplate
{
    private SyllabusTemplate() { }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Guid OwnerOrgUnitId { get; private set; }
    public string? Description { get; private set; }
    public OrgUnit OwnerOrgUnit { get; private set; } = null!;
    public ICollection<SyllabusTemplateVersion> Versions { get; private set; } = new List<SyllabusTemplateVersion>();
}
