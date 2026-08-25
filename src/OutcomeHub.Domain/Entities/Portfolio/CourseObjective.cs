namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class CourseObjective
{
    private CourseObjective() { }
    public Guid Id { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public ICollection<CourseObjectiveClo> CloMappings { get; private set; } = new List<CourseObjectiveClo>();
}
