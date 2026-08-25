namespace OutcomeHub.Domain.Entities.Academic;

public sealed class ProgramVersionCohort
{
    private ProgramVersionCohort() { }

    public Guid ProgramVersionId { get; private set; }
    public Guid CohortId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public bool IsDefault { get; private set; }

    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public Cohort Cohort { get; private set; } = null!;
}
