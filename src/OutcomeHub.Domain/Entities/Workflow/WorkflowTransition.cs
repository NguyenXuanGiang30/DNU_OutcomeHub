using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Workflow;

public sealed class WorkflowTransition
{
    private WorkflowTransition()
    {
    }

    public Guid Id { get; private set; }
    public Guid InstanceId { get; private set; }
    public string FromState { get; private set; } = null!;
    public string ToState { get; private set; } = null!;
    public string EventCode { get; private set; } = null!;
    public Guid ActorPrincipalId { get; private set; }
    public string? Reason { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public Guid RequestId { get; private set; }

    public WorkflowInstance Instance { get; private set; } = null!;
    public Principal ActorPrincipal { get; private set; } = null!;
}
