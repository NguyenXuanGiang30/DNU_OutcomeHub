namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class SnapshotPopulationMember
{
    private SnapshotPopulationMember()
    {
    }

    public Guid InputSnapshotId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid CohortId { get; private set; }

    public Guid StudentPathId { get; private set; }

    public Guid CurriculumPathId { get; private set; }

    public string Decision { get; private set; } = null!;

    public string? ExclusionReasonCode { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public InputSnapshot InputSnapshot { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Student Student { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Cohort Cohort { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.StudentPath StudentPath { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CurriculumPath CurriculumPath { get; private set; } = null!;
}
