using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Workflow;

public sealed class WorkflowComment
{
    private WorkflowComment()
    {
    }

    public Guid Id { get; private set; }
    public Guid InstanceId { get; private set; }
    public Guid AuthorPrincipalId { get; private set; }
    public string? TargetLocator { get; private set; }
    public string Body { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }

    public WorkflowInstance Instance { get; private set; } = null!;
    public Principal AuthorPrincipal { get; private set; } = null!;
}
