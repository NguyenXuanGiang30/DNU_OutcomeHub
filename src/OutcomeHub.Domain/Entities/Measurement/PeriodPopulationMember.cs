namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class PeriodPopulationMember
{
    private PeriodPopulationMember()
    {
    }

    public Guid MeasurementPeriodId { get; private set; }

    public Guid ProgramVersionId { get; private set; }

    public Guid CohortId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid StudentPathId { get; private set; }

    public Guid CurriculumPathId { get; private set; }

    public string Decision { get; private set; } = null!;

    public string? ExclusionReasonCode { get; private set; }

    public string DecisionSource { get; private set; } = null!;

    public Guid DecidedBy { get; private set; }

    public DateTimeOffset DecidedAt { get; private set; }

    public MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Cohort Cohort { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Student Student { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.StudentPath StudentPath { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CurriculumPath CurriculumPath { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal Decider { get; private set; } = null!;
}
