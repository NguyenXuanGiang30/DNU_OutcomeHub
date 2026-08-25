using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class SharedSyllabusCoreVersion
{
    private SharedSyllabusCoreVersion() { }

    public Guid Id { get; private set; }
    public Guid SharedSyllabusCoreId { get; private set; }
    public Guid CourseVersionId { get; private set; }
    public int VersionNo { get; private set; }
    public string Status { get; private set; } = null!;
    public Guid? DecisionId { get; private set; }
    public Guid? WorkflowInstanceId { get; private set; }
    public Guid? SupersedesId { get; private set; }
    public string Checksum { get; private set; } = null!;
    public SharedSyllabusCore SharedSyllabusCore { get; private set; } = null!;
    public CourseVersion CourseVersion { get; private set; } = null!;
    public DecisionRecord? Decision { get; private set; }
    public WorkflowInstance? WorkflowInstance { get; private set; }
    public SharedSyllabusCoreVersion? Supersedes { get; private set; }
    public ICollection<SharedSyllabusCoreVersion> Successors { get; private set; } = new List<SharedSyllabusCoreVersion>();
}
