namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CurriculumBlock
{
    private CurriculumBlock() { }

    public Guid Id { get; private set; }
    public Guid CurriculumPlanId { get; private set; }
    public Guid? ParentId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string BlockType { get; private set; } = null!;
    public decimal RequiredCredits { get; private set; }
    public decimal? MaximumCredits { get; private set; }
    public int SortOrder { get; private set; }

    public CurriculumPlan CurriculumPlan { get; private set; } = null!;
    public CurriculumBlock? Parent { get; private set; }
}
