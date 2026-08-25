namespace OutcomeHub.Domain.Entities.Result;

public sealed class StudentPloResult
{
    private StudentPloResult()
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

    public Guid StudentPathId { get; private set; }

    public Guid ProgramPloId { get; private set; }

    public string Method { get; private set; } = null!;

    public decimal? Score { get; private set; }

    public decimal ThetaInd { get; private set; }

    public string AttainmentStatus { get; private set; } = null!;

    public string CoreGateStatus { get; private set; } = null!;

    public string DataStatus { get; private set; } = null!;

    public decimal? Alpha { get; private set; }

    public ResultBatch Batch { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.OrgUnit OrgUnit { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Program Program { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramVersion ProgramVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Cohort Cohort { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CurriculumPath CurriculumPath { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Student Student { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.StudentPath StudentPath { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramPlo ProgramPlo { get; private set; } = null!;

    public static StudentPloResult Create(
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
        Guid studentPathId,
        Guid programPloId,
        string method,
        decimal? score,
        decimal thetaInd,
        string attainmentStatus,
        string coreGateStatus,
        string dataStatus,
        decimal? alpha = null)
    {
        return new StudentPloResult
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
            StudentPathId = studentPathId,
            ProgramPloId = programPloId,
            Method = method,
            Score = score,
            ThetaInd = thetaInd,
            AttainmentStatus = attainmentStatus,
            CoreGateStatus = coreGateStatus,
            DataStatus = dataStatus,
            Alpha = alpha
        };
    }
}
