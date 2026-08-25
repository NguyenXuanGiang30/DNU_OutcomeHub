namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CoursePrerequisiteGroup
{
    private CoursePrerequisiteGroup() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid TargetProgramCourseId { get; private set; }
    public int GroupNo { get; private set; }
    public int MinimumItemsSatisfied { get; private set; }
    public string RelationType { get; private set; } = null!;

    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public ProgramCourse TargetProgramCourse { get; private set; } = null!;
}
