namespace OutcomeHub.Domain.Entities.Result;

public sealed class StudentPiSourceContribution
{
    private StudentPiSourceContribution()
    {
    }

    public short AcademicYearStart { get; private set; }

    public Guid BatchId { get; private set; }

    public Guid InputSnapshotId { get; private set; }

    public Guid OrgUnitId { get; private set; }

    public Guid ProgramId { get; private set; }

    public Guid ProgramVersionId { get; private set; }

    public Guid MeasurementPeriodId { get; private set; }

    public Guid CohortId { get; private set; }

    public Guid CurriculumPathId { get; private set; }

    public Guid CourseId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid StudentPathId { get; private set; }

    public Guid ProgramPiId { get; private set; }

    public string Method { get; private set; } = null!;

    public Guid StudentPiResultId { get; private set; }

    public Guid CoursePiResultId { get; private set; }

    public Guid CourseOfferingId { get; private set; }

    public decimal SourceWeightRatio { get; private set; }

    public decimal WeightedContribution { get; private set; }

    public string SourceRole { get; private set; } = null!;

    public Guid? AnchorAssessmentId { get; private set; }

    public ResultBatch Batch { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.InputSnapshot InputSnapshot { get; private set; } = null!;
    public StudentPiResult StudentPiResult { get; private set; } = null!;
    public CoursePiResult CoursePiResult { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.SnapshotPiSourceWeight SnapshotPiSourceWeight { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.OrgUnit OrgUnit { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Program Program { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramVersion ProgramVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Cohort Cohort { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CurriculumPath CurriculumPath { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Course Course { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Student Student { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.StudentPath StudentPath { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramPi ProgramPi { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CourseOffering CourseOffering { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.AnchorAssessment? AnchorAssessment { get; private set; }
}
