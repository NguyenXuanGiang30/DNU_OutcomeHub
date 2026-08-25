using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class SyllabusVersion
{
    private SyllabusVersion() { }

    public Guid Id { get; private set; }
    public Guid SyllabusId { get; private set; }
    public Guid ProgramCourseId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid InstitutionTemplateVersionId { get; private set; }
    public Guid CourseVersionId { get; private set; }
    public Guid SyllabusTemplateVersionId { get; private set; }
    public int VersionNo { get; private set; }
    public DateOnly ApplicableFrom { get; private set; }
    public DateOnly? ApplicableTo { get; private set; }
    public string Status { get; private set; } = null!;
    public Guid? SharedSyllabusCoreVersionId { get; private set; }
    public Guid? WorkflowInstanceId { get; private set; }
    public Guid? SupersedesId { get; private set; }
    public string ContentChecksum { get; private set; } = null!;
    public long RowVersion { get; private set; }
    public Syllabus Syllabus { get; private set; } = null!;
    public ProgramCourse ProgramCourse { get; private set; } = null!;
    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public InstitutionTemplateVersion InstitutionTemplateVersion { get; private set; } = null!;
    public CourseVersion CourseVersion { get; private set; } = null!;
    public SyllabusTemplateVersion SyllabusTemplateVersion { get; private set; } = null!;
    public SharedSyllabusCoreVersion? SharedSyllabusCoreVersion { get; private set; }
    public WorkflowInstance? WorkflowInstance { get; private set; }
    public SyllabusVersion? Supersedes { get; private set; }
    public ICollection<SyllabusVersion> Successors { get; private set; } = new List<SyllabusVersion>();

    public static SyllabusVersion Create(
        Guid id,
        Guid syllabusId,
        Guid programCourseId,
        Guid programVersionId,
        Guid institutionTemplateVersionId,
        Guid courseVersionId,
        Guid syllabusTemplateVersionId,
        int versionNo,
        DateOnly applicableFrom,
        DateOnly? applicableTo,
        string status,
        Guid? sharedSyllabusCoreVersionId,
        Guid? workflowInstanceId,
        Guid? supersedesId,
        string contentChecksum)
    {
        return new SyllabusVersion
        {
            Id = id,
            SyllabusId = syllabusId,
            ProgramCourseId = programCourseId,
            ProgramVersionId = programVersionId,
            InstitutionTemplateVersionId = institutionTemplateVersionId,
            CourseVersionId = courseVersionId,
            SyllabusTemplateVersionId = syllabusTemplateVersionId,
            VersionNo = versionNo,
            ApplicableFrom = applicableFrom,
            ApplicableTo = applicableTo,
            Status = status.Trim().ToUpperInvariant(),
            SharedSyllabusCoreVersionId = sharedSyllabusCoreVersionId,
            WorkflowInstanceId = workflowInstanceId,
            SupersedesId = supersedesId,
            ContentChecksum = contentChecksum.ToLowerInvariant(),
        };
    }
}
