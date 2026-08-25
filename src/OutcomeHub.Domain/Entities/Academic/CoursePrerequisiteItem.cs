namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CoursePrerequisiteItem
{
    private CoursePrerequisiteItem() { }

    public Guid GroupId { get; private set; }
    public Guid RequiredProgramCourseId { get; private set; }
    public decimal? MinimumGrade { get; private set; }
    public bool AllowConcurrent { get; private set; }
    public string? Rationale { get; private set; }

    public CoursePrerequisiteGroup Group { get; private set; } = null!;
    public ProgramCourse RequiredProgramCourse { get; private set; } = null!;
}
