using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class SharedSyllabusCore
{
    private SharedSyllabusCore() { }

    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid OwnerOrgUnitId { get; private set; }
    public string Code { get; private set; } = null!;
    public Course Course { get; private set; } = null!;
    public OrgUnit OwnerOrgUnit { get; private set; } = null!;
    public ICollection<SharedSyllabusCoreVersion> Versions { get; private set; } = new List<SharedSyllabusCoreVersion>();
}
