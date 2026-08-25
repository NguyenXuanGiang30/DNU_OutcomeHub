namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CurriculumPathCourse
{
    private CurriculumPathCourse() { }

    public Guid Id { get; private set; }
    public Guid CurriculumPathId { get; private set; }
    public Guid ProgramCourseId { get; private set; }
    public int? PlannedTerm { get; private set; }
    public string RequirementType { get; private set; } = null!;
    public Guid? ElectiveGroupId { get; private set; }
    public int SortOrder { get; private set; }

    public CurriculumPath CurriculumPath { get; private set; } = null!;
    public ProgramCourse ProgramCourse { get; private set; } = null!;
    public CurriculumElectiveGroup? ElectiveGroup { get; private set; }
}
