using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class MeasurementPeriodCohort
{
    private MeasurementPeriodCohort() { }

    public Guid MeasurementPeriodId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid CohortId { get; private set; }

    public MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public Cohort Cohort { get; private set; } = null!;
    public ProgramVersionCohort ProgramVersionCohort { get; private set; } = null!;

    public static MeasurementPeriodCohort Create(
        Guid measurementPeriodId,
        Guid programVersionId,
        Guid cohortId)
    {
        return new MeasurementPeriodCohort
        {
            MeasurementPeriodId = measurementPeriodId,
            ProgramVersionId = programVersionId,
            CohortId = cohortId,
        };
    }
}
