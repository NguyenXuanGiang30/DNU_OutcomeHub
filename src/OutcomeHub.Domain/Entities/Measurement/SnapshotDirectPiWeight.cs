namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class SnapshotDirectPiWeight
{
    private SnapshotDirectPiWeight()
    {
    }

    public Guid InputSnapshotId { get; private set; }

    public Guid SyllabusTraceabilityId { get; private set; }

    public Guid ProgramPiId { get; private set; }

    public Guid CourseOfferingId { get; private set; }

    public Guid RubricCriterionId { get; private set; }

    public decimal DirectWeightRatio { get; private set; }

    public decimal? AllocationRatio { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public InputSnapshot InputSnapshot { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.SyllabusTraceability SyllabusTraceability { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramPi ProgramPi { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CourseOffering CourseOffering { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.RubricCriterion RubricCriterion { get; private set; } = null!;
}
