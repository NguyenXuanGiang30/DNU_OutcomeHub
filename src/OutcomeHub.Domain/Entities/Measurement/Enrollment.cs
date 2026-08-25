using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class Enrollment
{
    private Enrollment() { }

    public Guid Id { get; private set; }
    public Guid CourseOfferingId { get; private set; }
    public Guid StudentId { get; private set; }
    public short AttemptNo { get; private set; }
    public Guid SourceSystemId { get; private set; }
    public string SourceRecordId { get; private set; } = null!;

    public CourseOffering CourseOffering { get; private set; } = null!;
    public Student Student { get; private set; } = null!;
    public SourceSystem SourceSystem { get; private set; } = null!;
    public ICollection<EnrollmentRevision> Revisions { get; private set; } = new List<EnrollmentRevision>();

    public static Enrollment Create(
        Guid id,
        Guid courseOfferingId,
        Guid studentId,
        short attemptNo,
        Guid sourceSystemId,
        string sourceRecordId)
    {
        return new Enrollment
        {
            Id = id,
            CourseOfferingId = courseOfferingId,
            StudentId = studentId,
            AttemptNo = attemptNo,
            SourceSystemId = sourceSystemId,
            SourceRecordId = sourceRecordId.Trim(),
        };
    }
}
