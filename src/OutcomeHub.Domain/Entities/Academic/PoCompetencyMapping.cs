namespace OutcomeHub.Domain.Entities.Academic;

public sealed class PoCompetencyMapping
{
    private PoCompetencyMapping() { }

    public Guid ProgramObjectiveId { get; private set; }
    public Guid CompetencyId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public string MappingLevel { get; private set; } = null!;
    public string? Rationale { get; private set; }

    public ProgramObjective ProgramObjective { get; private set; } = null!;
    public Competency Competency { get; private set; } = null!;
    public ProgramVersion ProgramVersion { get; private set; } = null!;
}
