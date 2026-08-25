using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class ScoreIdentity
{
    private ScoreIdentity() { }

    public Guid Id { get; private set; }
    public Guid ScoreDatasetId { get; private set; }
    public short AcademicYearStart { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid CourseOfferingId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public short AttemptNo { get; private set; }
    public Guid EnrollmentId { get; private set; }
    public Guid AssessmentItemId { get; private set; }
    public Guid? RubricCriterionId { get; private set; }
    public Guid? AssessmentQuestionId { get; private set; }
    public string ScoreLevel { get; private set; } = null!;

    public ScoreDataset ScoreDataset { get; private set; } = null!;
    public Student Student { get; private set; } = null!;
    public CourseOffering CourseOffering { get; private set; } = null!;
    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public Enrollment Enrollment { get; private set; } = null!;
    public AssessmentItem AssessmentItem { get; private set; } = null!;
    public RubricCriterion? RubricCriterion { get; private set; }
    public AssessmentQuestion? AssessmentQuestion { get; private set; }
    public ICollection<ScoreRecord> Records { get; private set; } = new List<ScoreRecord>();

    public static ScoreIdentity Create(
        Guid id,
        Guid scoreDatasetId,
        short academicYearStart,
        Guid studentId,
        Guid courseOfferingId,
        Guid programVersionId,
        Guid syllabusVersionId,
        short attemptNo,
        Guid enrollmentId,
        Guid assessmentItemId,
        string scoreLevel = "CRITERION",
        Guid? rubricCriterionId = null,
        Guid? assessmentQuestionId = null)
    {
        return new ScoreIdentity
        {
            Id = id,
            ScoreDatasetId = scoreDatasetId,
            AcademicYearStart = academicYearStart,
            StudentId = studentId,
            CourseOfferingId = courseOfferingId,
            ProgramVersionId = programVersionId,
            SyllabusVersionId = syllabusVersionId,
            AttemptNo = attemptNo,
            EnrollmentId = enrollmentId,
            AssessmentItemId = assessmentItemId,
            ScoreLevel = scoreLevel.Trim().ToUpperInvariant(),
            RubricCriterionId = rubricCriterionId,
            AssessmentQuestionId = assessmentQuestionId,
        };
    }
}
