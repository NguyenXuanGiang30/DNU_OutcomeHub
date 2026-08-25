using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Iam;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class ScoreRecord
{
    private ScoreRecord() { }

    public short AcademicYearStart { get; private set; }
    public Guid Id { get; private set; }
    public Guid ScoreIdentityId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid CourseOfferingId { get; private set; }
    public Guid OrgUnitId { get; private set; }
    public Guid ProgramId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid CourseId { get; private set; }
    public int RevisionNo { get; private set; }
    public decimal? RawScore { get; private set; }
    public decimal MaxScore { get; private set; }
    public string ScoreStatus { get; private set; } = null!;
    public Guid SourceSystemId { get; private set; }
    public string SourceRecordId { get; private set; } = null!;
    public string SourceRevision { get; private set; } = null!;
    public Guid IngestionBatchId { get; private set; }
    public Guid? SupersedesId { get; private set; }
    public string? CorrectionReason { get; private set; }
    public Guid RecordedBy { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }
    public string Checksum { get; private set; } = null!;

    public ScoreIdentity ScoreIdentity { get; private set; } = null!;
    public Student Student { get; private set; } = null!;
    public CourseOffering CourseOffering { get; private set; } = null!;
    public OrgUnit OrgUnit { get; private set; } = null!;
    public Program Program { get; private set; } = null!;
    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public Course Course { get; private set; } = null!;
    public SourceSystem SourceSystem { get; private set; } = null!;
    public IngestionBatch IngestionBatch { get; private set; } = null!;
    public ScoreRecord? Supersedes { get; private set; }
    public ICollection<ScoreRecord> Successors { get; private set; } = new List<ScoreRecord>();
    public Principal Recorder { get; private set; } = null!;

    public static ScoreRecord Create(
        short academicYearStart,
        Guid id,
        Guid scoreIdentityId,
        Guid studentId,
        Guid courseOfferingId,
        Guid orgUnitId,
        Guid programId,
        Guid programVersionId,
        Guid courseId,
        int revisionNo,
        decimal? rawScore,
        decimal maxScore,
        string scoreStatus,
        Guid sourceSystemId,
        string sourceRecordId,
        string sourceRevision,
        Guid ingestionBatchId,
        Guid recordedBy,
        DateTimeOffset recordedAt,
        string checksum,
        Guid? supersedesId = null,
        string? correctionReason = null)
    {
        return new ScoreRecord
        {
            AcademicYearStart = academicYearStart,
            Id = id,
            ScoreIdentityId = scoreIdentityId,
            StudentId = studentId,
            CourseOfferingId = courseOfferingId,
            OrgUnitId = orgUnitId,
            ProgramId = programId,
            ProgramVersionId = programVersionId,
            CourseId = courseId,
            RevisionNo = revisionNo,
            RawScore = rawScore,
            MaxScore = maxScore,
            ScoreStatus = scoreStatus.Trim().ToUpperInvariant(),
            SourceSystemId = sourceSystemId,
            SourceRecordId = sourceRecordId.Trim(),
            SourceRevision = sourceRevision.Trim(),
            IngestionBatchId = ingestionBatchId,
            RecordedBy = recordedBy,
            RecordedAt = recordedAt,
            Checksum = checksum.Trim().ToLowerInvariant(),
            SupersedesId = supersedesId,
            CorrectionReason = correctionReason,
        };
    }
}
