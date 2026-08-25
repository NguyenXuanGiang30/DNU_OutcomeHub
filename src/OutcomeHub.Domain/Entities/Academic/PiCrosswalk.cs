namespace OutcomeHub.Domain.Entities.Academic;

public sealed class PiCrosswalk
{
    private PiCrosswalk() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionCrosswalkId { get; private set; }
    public Guid FromProgramPiId { get; private set; }
    public Guid? ToProgramPiId { get; private set; }
    public string RelationType { get; private set; } = null!;
    public decimal? AllocationRatio { get; private set; }
    public string? Rationale { get; private set; }

    public ProgramVersionCrosswalk ProgramVersionCrosswalk { get; private set; } = null!;
    public ProgramPi FromProgramPi { get; private set; } = null!;
    public ProgramPi? ToProgramPi { get; private set; }
}
