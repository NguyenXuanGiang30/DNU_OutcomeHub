namespace OutcomeHub.Domain.Entities.Result;

public sealed class StudentCloResult
{
    private StudentCloResult()
    {
    }

    public short AcademicYearStart { get; private set; }

    public Guid Id { get; private set; }

    public Guid BatchId { get; private set; }

    public Guid OrgUnitId { get; private set; }

    public Guid ProgramId { get; private set; }

    public Guid ProgramVersionId { get; private set; }

    public Guid MeasurementPeriodId { get; private set; }

    public Guid CohortId { get; private set; }

    public Guid CurriculumPathId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid CourseId { get; private set; }

    public Guid CourseOfferingId { get; private set; }

    public Guid CloId { get; private set; }

    public decimal? Score { get; private set; }

    public decimal ThetaInd { get; private set; }

    public string AttainmentStatus { get; private set; } = null!;

    public string DataStatus { get; private set; } = null!;

    public decimal? Numerator { get; private set; }

    public decimal? Denominator { get; private set; }

    public ResultBatch Batch { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.OrgUnit OrgUnit { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Program Program { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramVersion ProgramVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Cohort Cohort { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CurriculumPath CurriculumPath { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Student Student { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Course Course { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CourseOffering CourseOffering { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.Clo Clo { get; private set; } = null!;

    public static StudentCloResult Create(
        short academicYearStart,
        Guid id,
        Guid batchId,
        Guid orgUnitId,
        Guid programId,
        Guid programVersionId,
        Guid measurementPeriodId,
        Guid cohortId,
        Guid curriculumPathId,
        Guid studentId,
        Guid courseId,
        Guid courseOfferingId,
        Guid cloId,
        decimal? score,
        decimal thetaInd,
        string attainmentStatus,
        string dataStatus,
        decimal? numerator = null,
        decimal? denominator = null)
    {
        return new StudentCloResult
        {
            AcademicYearStart = academicYearStart,
            Id = id,
            BatchId = batchId,
            OrgUnitId = orgUnitId,
            ProgramId = programId,
            ProgramVersionId = programVersionId,
            MeasurementPeriodId = measurementPeriodId,
            CohortId = cohortId,
            CurriculumPathId = curriculumPathId,
            StudentId = studentId,
            CourseId = courseId,
            CourseOfferingId = courseOfferingId,
            CloId = cloId,
            Score = score,
            ThetaInd = thetaInd,
            AttainmentStatus = attainmentStatus,
            DataStatus = dataStatus,
            Numerator = numerator,
            Denominator = denominator
        };
    }
}
