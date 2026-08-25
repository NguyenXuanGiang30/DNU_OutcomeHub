using OutcomeHub.Domain.Entities.Integration;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CourseOffering
{
    private CourseOffering() { }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public Guid ProgramCourseId { get; private set; }
    public Guid CourseVersionId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public short AcademicYearStart { get; private set; }
    public string TermCode { get; private set; } = null!;
    public Guid OrgUnitId { get; private set; }
    public string Status { get; private set; } = null!;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public Guid? SourceSystemId { get; private set; }
    public string? SourceRecordId { get; private set; }

    public ProgramCourse ProgramCourse { get; private set; } = null!;
    public CourseVersion CourseVersion { get; private set; } = null!;
    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public OrgUnit OrgUnit { get; private set; } = null!;
    public SourceSystem? SourceSystem { get; private set; }
    public ICollection<CourseOfferingInstructor> Instructors { get; } = new List<CourseOfferingInstructor>();

    public static CourseOffering Create(
        Guid id,
        string code,
        Guid programCourseId,
        Guid courseVersionId,
        Guid programVersionId,
        Guid syllabusVersionId,
        short academicYearStart,
        string termCode,
        Guid orgUnitId,
        DateOnly startDate,
        DateOnly endDate,
        string status = "PLANNED",
        Guid? sourceSystemId = null,
        string? sourceRecordId = null)
    {
        return new CourseOffering
        {
            Id = id,
            Code = code.Trim().ToUpperInvariant(),
            ProgramCourseId = programCourseId,
            CourseVersionId = courseVersionId,
            ProgramVersionId = programVersionId,
            SyllabusVersionId = syllabusVersionId,
            AcademicYearStart = academicYearStart,
            TermCode = termCode.Trim().ToUpperInvariant(),
            OrgUnitId = orgUnitId,
            StartDate = startDate,
            EndDate = endDate,
            Status = status.Trim().ToUpperInvariant(),
            SourceSystemId = sourceSystemId,
            SourceRecordId = string.IsNullOrWhiteSpace(sourceRecordId) ? null : sourceRecordId.Trim(),
        };
    }

    public void Update(string status, DateOnly startDate, DateOnly endDate)
    {
        Status = status.Trim().ToUpperInvariant();
        StartDate = startDate;
        EndDate = endDate;
    }
}
