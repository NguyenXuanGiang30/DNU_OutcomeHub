namespace OutcomeHub.Domain.Entities.Academic;

public sealed class PloCrosswalk
{
    private PloCrosswalk() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionCrosswalkId { get; private set; }
    public Guid FromProgramPloId { get; private set; }
    public Guid? ToProgramPloId { get; private set; }
    public string RelationType { get; private set; } = null!;
    public decimal? AllocationRatio { get; private set; }
    public string? Rationale { get; private set; }

    public ProgramVersionCrosswalk ProgramVersionCrosswalk { get; private set; } = null!;
    public ProgramPlo FromProgramPlo { get; private set; } = null!;
    public ProgramPlo? ToProgramPlo { get; private set; }
}
