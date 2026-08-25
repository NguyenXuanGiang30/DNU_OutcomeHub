namespace OutcomeHub.Domain.Entities.Result;

public sealed class CriterionPiContribution
{
    private CriterionPiContribution()
    {
    }

    public short AcademicYearStart { get; private set; }

    public Guid Id { get; private set; }

    public Guid BatchId { get; private set; }

    public Guid InputSnapshotId { get; private set; }

    public Guid OrgUnitId { get; private set; }

    public Guid ProgramId { get; private set; }

    public Guid ProgramVersionId { get; private set; }

    public Guid MeasurementPeriodId { get; private set; }

    public Guid CohortId { get; private set; }

    public Guid CurriculumPathId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid StudentPathId { get; private set; }

    public Guid CourseId { get; private set; }

    public Guid CourseOfferingId { get; private set; }

    public Guid AssessmentItemId { get; private set; }

    public Guid RubricCriterionId { get; private set; }

    public Guid ProgramPiId { get; private set; }

    public Guid SyllabusTraceabilityId { get; private set; }

    public Guid StudentCriterionResultId { get; private set; }

    public decimal NormalizedScore { get; private set; }

    public decimal DirectWeightRatio { get; private set; }

    public decimal AllocationRatio { get; private set; }

    public decimal WeightedContribution { get; private set; }

    public bool IsCore { get; private set; }

    public bool Included { get; private set; }

    public string? ExclusionReason { get; private set; }

    public ResultBatch Batch { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.InputSnapshot InputSnapshot { get; private set; } = null!;
    public StudentCriterionResult StudentCriterionResult { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.SnapshotDirectPiWeight SnapshotDirectPiWeight { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.OrgUnit OrgUnit { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Program Program { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramVersion ProgramVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Cohort Cohort { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CurriculumPath CurriculumPath { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Student Student { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.StudentPath StudentPath { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Course Course { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CourseOffering CourseOffering { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.AssessmentItem AssessmentItem { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.RubricCriterion RubricCriterion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramPi ProgramPi { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.SyllabusTraceability SyllabusTraceability { get; private set; } = null!;
}
