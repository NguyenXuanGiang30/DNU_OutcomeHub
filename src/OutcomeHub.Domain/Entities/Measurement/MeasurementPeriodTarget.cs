using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class MeasurementPeriodTarget
{
    private MeasurementPeriodTarget() { }

    public Guid Id { get; private set; }
    public Guid MeasurementPeriodId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public string OutcomeLevel { get; private set; } = null!;
    public Guid? CourseOfferingId { get; private set; }
    public Guid? SyllabusVersionId { get; private set; }
    public Guid? CloId { get; private set; }
    public Guid? ProgramPiId { get; private set; }
    public Guid? ProgramPloId { get; private set; }
    public string TargetRole { get; private set; } = null!;

    public MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public MeasurementPeriodOffering? PeriodOffering { get; private set; }
    public CourseOffering? CourseOffering { get; private set; }
    public SyllabusVersion? SyllabusVersion { get; private set; }
    public Clo? Clo { get; private set; }
    public ProgramPi? ProgramPi { get; private set; }
    public ProgramPlo? ProgramPlo { get; private set; }

    public static MeasurementPeriodTarget Create(
        Guid id,
        Guid measurementPeriodId,
        Guid programVersionId,
        string outcomeLevel,
        string targetRole = "PRIMARY",
        Guid? courseOfferingId = null,
        Guid? syllabusVersionId = null,
        Guid? cloId = null,
        Guid? programPiId = null,
        Guid? programPloId = null)
    {
        return new MeasurementPeriodTarget
        {
            Id = id,
            MeasurementPeriodId = measurementPeriodId,
            ProgramVersionId = programVersionId,
            OutcomeLevel = outcomeLevel.Trim().ToUpperInvariant(),
            TargetRole = targetRole.Trim().ToUpperInvariant(),
            CourseOfferingId = courseOfferingId,
            SyllabusVersionId = syllabusVersionId,
            CloId = cloId,
            ProgramPiId = programPiId,
            ProgramPloId = programPloId,
        };
    }
}
