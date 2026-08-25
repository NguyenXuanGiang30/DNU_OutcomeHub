namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class SnapshotEnrollment
{
    private SnapshotEnrollment()
    {
    }

    public Guid InputSnapshotId { get; private set; }

    public Guid EnrollmentRevisionId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid CourseOfferingId { get; private set; }

    public short AttemptNo { get; private set; }

    public int RevisionNo { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public InputSnapshot InputSnapshot { get; private set; } = null!;
    public EnrollmentRevision EnrollmentRevision { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Student Student { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CourseOffering CourseOffering { get; private set; } = null!;
}
