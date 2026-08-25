namespace OutcomeHub.Domain.Entities.Academic;

public sealed class Competency
{
    private Competency() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid? ParentId { get; private set; }
    public int LevelNo { get; private set; }
    public string Code { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int SortOrder { get; private set; }

    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public Competency? Parent { get; private set; }
}
