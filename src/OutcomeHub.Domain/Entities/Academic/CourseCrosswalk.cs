namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CourseCrosswalk
{
    private CourseCrosswalk() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionCrosswalkId { get; private set; }
    public Guid FromProgramCourseId { get; private set; }
    public Guid? ToProgramCourseId { get; private set; }
    public string RelationType { get; private set; } = null!;
    public decimal? AllocationRatio { get; private set; }
    public string? Rationale { get; private set; }

    public ProgramVersionCrosswalk ProgramVersionCrosswalk { get; private set; } = null!;
    public ProgramCourse FromProgramCourse { get; private set; } = null!;
    public ProgramCourse? ToProgramCourse { get; private set; }
}
