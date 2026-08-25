namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class PeriodPopulationEnrollment
{
    private PeriodPopulationEnrollment()
    {
    }

    public Guid MeasurementPeriodId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid EnrollmentRevisionId { get; private set; }

    public string SelectionRole { get; private set; } = null!;

    public PeriodPopulationMember PopulationMember { get; private set; } = null!;
    public EnrollmentRevision EnrollmentRevision { get; private set; } = null!;
}
