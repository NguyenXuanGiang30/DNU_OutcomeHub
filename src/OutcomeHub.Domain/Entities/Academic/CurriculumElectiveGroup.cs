namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CurriculumElectiveGroup
{
    private CurriculumElectiveGroup() { }

    public Guid Id { get; private set; }
    public Guid CurriculumPathId { get; private set; }
    public Guid CurriculumBlockId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public int MinimumCourseCount { get; private set; }
    public int? MaximumCourseCount { get; private set; }
    public decimal MinimumCredits { get; private set; }
    public decimal? MaximumCredits { get; private set; }

    public CurriculumPath CurriculumPath { get; private set; } = null!;
    public CurriculumBlock CurriculumBlock { get; private set; } = null!;
}
