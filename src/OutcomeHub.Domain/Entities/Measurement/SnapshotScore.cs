namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class SnapshotScore
{
    private SnapshotScore()
    {
    }

    public Guid InputSnapshotId { get; private set; }

    public short AcademicYearStart { get; private set; }

    public Guid ScoreRecordId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid CourseOfferingId { get; private set; }

    public decimal? RawScore { get; private set; }

    public decimal MaxScore { get; private set; }

    public string ScoreStatus { get; private set; } = null!;

    public decimal? NormalizedScore { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public InputSnapshot InputSnapshot { get; private set; } = null!;
    public ScoreRecord ScoreRecord { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Student Student { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CourseOffering CourseOffering { get; private set; } = null!;
}
