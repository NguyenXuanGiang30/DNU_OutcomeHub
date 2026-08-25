using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class MeasurementPeriodOffering
{
    private MeasurementPeriodOffering() { }

    public Guid MeasurementPeriodId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public short AcademicYearStart { get; private set; }
    public Guid CourseOfferingId { get; private set; }
    public string PlannedSourceRole { get; private set; } = null!;
    public string CollectionStatus { get; private set; } = null!;
    public DateTimeOffset? DueAt { get; private set; }

    public MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public CourseOffering CourseOffering { get; private set; } = null!;

    public static MeasurementPeriodOffering Create(
        Guid measurementPeriodId,
        Guid programVersionId,
        short academicYearStart,
        Guid courseOfferingId,
        string plannedSourceRole = "OFFICIAL",
        string collectionStatus = "PENDING",
        DateTimeOffset? dueAt = null)
    {
        return new MeasurementPeriodOffering
        {
            MeasurementPeriodId = measurementPeriodId,
            ProgramVersionId = programVersionId,
            AcademicYearStart = academicYearStart,
            CourseOfferingId = courseOfferingId,
            PlannedSourceRole = plannedSourceRole.Trim().ToUpperInvariant(),
            CollectionStatus = collectionStatus.Trim().ToUpperInvariant(),
            DueAt = dueAt,
        };
    }

    public void UpdateStatus(string collectionStatus, DateTimeOffset? dueAt)
    {
        CollectionStatus = collectionStatus.Trim().ToUpperInvariant();
        DueAt = dueAt;
    }
}
