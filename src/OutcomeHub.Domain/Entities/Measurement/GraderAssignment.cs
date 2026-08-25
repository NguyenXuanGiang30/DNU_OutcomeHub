namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class GraderAssignment
{
    private GraderAssignment()
    {
    }

    public Guid Id { get; private set; }

    public Guid MeasurementPeriodId { get; private set; }

    public Guid CourseOfferingId { get; private set; }

    public Guid SyllabusVersionId { get; private set; }

    public Guid AssessmentItemId { get; private set; }

    public Guid RubricCriterionId { get; private set; }

    public Guid PrincipalId { get; private set; }

    public string AssignmentRole { get; private set; } = null!;

    public DateTimeOffset EffectiveFrom { get; private set; }

    public DateTimeOffset? EffectiveTo { get; private set; }

    public Guid AssignedBy { get; private set; }

    public MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public MeasurementPeriodOffering PeriodOffering { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CourseOffering CourseOffering { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.AssessmentItem AssessmentItem { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.RubricCriterion RubricCriterion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal Principal { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal Assigner { get; private set; } = null!;
}
