namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class SnapshotPiSourceWeight
{
    private SnapshotPiSourceWeight()
    {
    }

    public Guid InputSnapshotId { get; private set; }

    public Guid StudentPathId { get; private set; }

    public Guid ProgramPiId { get; private set; }

    public Guid CourseOfferingId { get; private set; }

    public decimal SourceWeightRatio { get; private set; }

    public string SourceRole { get; private set; } = null!;

    public Guid? AnchorAssessmentId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public InputSnapshot InputSnapshot { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.StudentPath StudentPath { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramPi ProgramPi { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CourseOffering CourseOffering { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.AnchorAssessment? AnchorAssessment { get; private set; }
}
