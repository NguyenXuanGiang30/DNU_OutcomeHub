namespace OutcomeHub.Domain.Entities.Academic;

public sealed class ProgramObjective
{
    private ProgramObjective() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int SortOrder { get; private set; }

    public ProgramVersion ProgramVersion { get; private set; } = null!;
}
