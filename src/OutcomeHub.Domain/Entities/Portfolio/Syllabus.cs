using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class Syllabus
{
    private Syllabus() { }

    public Guid Id { get; private set; }
    public Guid ProgramCourseId { get; private set; }
    public string Code { get; private set; } = null!;
    public Guid OwnerOrgUnitId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public ProgramCourse ProgramCourse { get; private set; } = null!;
    public OrgUnit OwnerOrgUnit { get; private set; } = null!;
    public ICollection<SyllabusVersion> Versions { get; private set; } = new List<SyllabusVersion>();

    public static Syllabus Create(
        Guid id,
        Guid programCourseId,
        string code,
        Guid ownerOrgUnitId)
    {
        return new Syllabus
        {
            Id = id,
            ProgramCourseId = programCourseId,
            Code = code.Trim().ToUpperInvariant(),
            OwnerOrgUnitId = ownerOrgUnitId,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}
