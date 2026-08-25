namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CompetencyPloMapping
{
    private CompetencyPloMapping() { }

    public Guid CompetencyId { get; private set; }
    public Guid ProgramPloId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public string MappingLevel { get; private set; } = null!;
    public string? Rationale { get; private set; }

    public Competency Competency { get; private set; } = null!;
    public ProgramPlo ProgramPlo { get; private set; } = null!;
    public ProgramVersion ProgramVersion { get; private set; } = null!;
}
