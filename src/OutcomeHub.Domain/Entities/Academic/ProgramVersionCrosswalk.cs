namespace OutcomeHub.Domain.Entities.Academic;

public sealed class ProgramVersionCrosswalk
{
    private ProgramVersionCrosswalk() { }

    public Guid Id { get; private set; }
    public Guid FromProgramVersionId { get; private set; }
    public Guid ToProgramVersionId { get; private set; }
    public string Status { get; private set; } = null!;
    public Guid DecisionId { get; private set; }
    public string? Rationale { get; private set; }

    public ProgramVersion FromProgramVersion { get; private set; } = null!;
    public ProgramVersion ToProgramVersion { get; private set; } = null!;
    public DecisionRecord Decision { get; private set; } = null!;
}
