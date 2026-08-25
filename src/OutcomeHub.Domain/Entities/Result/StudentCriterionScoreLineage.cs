namespace OutcomeHub.Domain.Entities.Result;

public sealed class StudentCriterionScoreLineage
{
    private StudentCriterionScoreLineage()
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

    public Guid CourseOfferingId { get; private set; }

    public Guid RubricCriterionId { get; private set; }

    public Guid StudentCriterionResultId { get; private set; }

    public Guid ScoreRecordId { get; private set; }

    public Guid? AssessmentQuestionId { get; private set; }

    public decimal SourceWeightRatio { get; private set; }

    public decimal WeightedContribution { get; private set; }

    public ResultBatch Batch { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.InputSnapshot InputSnapshot { get; private set; } = null!;
    public StudentCriterionResult StudentCriterionResult { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.SnapshotScore SnapshotScore { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.SnapshotQuestionCriterionWeight? QuestionCriterionWeight { get; private set; }
    public OutcomeHub.Domain.Entities.Academic.OrgUnit OrgUnit { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Program Program { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramVersion ProgramVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Cohort Cohort { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CurriculumPath CurriculumPath { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Course Course { get; private set; } = null!;
}
