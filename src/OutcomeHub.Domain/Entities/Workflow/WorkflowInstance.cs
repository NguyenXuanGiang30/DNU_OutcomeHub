using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Workflow;

public sealed class WorkflowInstance
{
    private readonly List<WorkflowTask> _tasks = [];
    private readonly List<WorkflowTransition> _transitions = [];
    private readonly List<WorkflowComment> _comments = [];

    private WorkflowInstance()
    {
    }

    public Guid Id { get; private set; }
    public Guid DefinitionId { get; private set; }
    public string CurrentState { get; private set; } = null!;
    public Guid StartedBy { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public long RowVersion { get; private set; }

    public WorkflowDefinition Definition { get; private set; } = null!;
    public Principal StartedByPrincipal { get; private set; } = null!;
    public IReadOnlyCollection<WorkflowTask> Tasks => _tasks;
    public IReadOnlyCollection<WorkflowTransition> Transitions => _transitions;
    public IReadOnlyCollection<WorkflowComment> Comments => _comments;
}
