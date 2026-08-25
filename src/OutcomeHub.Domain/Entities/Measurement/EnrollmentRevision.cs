using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class EnrollmentRevision
{
    private EnrollmentRevision() { }

    public Guid Id { get; private set; }
    public Guid EnrollmentId { get; private set; }
    public int RevisionNo { get; private set; }
    public string EnrollmentStatus { get; private set; } = null!;
    public bool RepeatFlag { get; private set; }
    public bool ImprovementFlag { get; private set; }
    public DateTimeOffset EffectiveFrom { get; private set; }
    public DateTimeOffset? EffectiveTo { get; private set; }
    public DateTimeOffset? SourceUpdatedAt { get; private set; }
    public Guid IngestionBatchId { get; private set; }
    public Guid? SupersedesId { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }
    public string Checksum { get; private set; } = null!;

    public Enrollment Enrollment { get; private set; } = null!;
    public IngestionBatch IngestionBatch { get; private set; } = null!;
    public EnrollmentRevision? Supersedes { get; private set; }
    public ICollection<EnrollmentRevision> Successors { get; private set; } = new List<EnrollmentRevision>();

    public static EnrollmentRevision Create(
        Guid id,
        Guid enrollmentId,
        int revisionNo,
        string enrollmentStatus,
        DateTimeOffset effectiveFrom,
        Guid ingestionBatchId,
        string checksum,
        DateTimeOffset recordedAt,
        bool repeatFlag = false,
        bool improvementFlag = false,
        DateTimeOffset? effectiveTo = null,
        DateTimeOffset? sourceUpdatedAt = null,
        Guid? supersedesId = null)
    {
        return new EnrollmentRevision
        {
            Id = id,
            EnrollmentId = enrollmentId,
            RevisionNo = revisionNo,
            EnrollmentStatus = enrollmentStatus.Trim().ToUpperInvariant(),
            EffectiveFrom = effectiveFrom,
            IngestionBatchId = ingestionBatchId,
            Checksum = checksum.Trim().ToLowerInvariant(),
            RecordedAt = recordedAt,
            RepeatFlag = repeatFlag,
            ImprovementFlag = improvementFlag,
            EffectiveTo = effectiveTo,
            SourceUpdatedAt = sourceUpdatedAt,
            SupersedesId = supersedesId,
        };
    }
}
