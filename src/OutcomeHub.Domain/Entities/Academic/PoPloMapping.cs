namespace OutcomeHub.Domain.Entities.Academic;

public sealed class PoPloMapping
{
    private PoPloMapping() { }

    public Guid ProgramObjectiveId { get; private set; }
    public Guid ProgramPloId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public string MappingLevel { get; private set; } = null!;
    public string? Rationale { get; private set; }

    public ProgramObjective ProgramObjective { get; private set; } = null!;
    public ProgramPlo ProgramPlo { get; private set; } = null!;
    public ProgramVersion ProgramVersion { get; private set; } = null!;
}
