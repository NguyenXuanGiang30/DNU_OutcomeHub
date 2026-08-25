using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Workflow;

public sealed class WorkflowTask
{
    private WorkflowTask()
    {
    }

    public Guid Id { get; private set; }
    public Guid InstanceId { get; private set; }
    public string StepCode { get; private set; } = null!;
    public Guid? AssigneePrincipalId { get; private set; }
    public Guid? AssigneeRoleId { get; private set; }
    public string Status { get; private set; } = null!;
    public DateTimeOffset? DueAt { get; private set; }
    public string? Decision { get; private set; }
    public string? DecisionReason { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    public WorkflowInstance Instance { get; private set; } = null!;
    public Principal? AssigneePrincipal { get; private set; }
    public Role? AssigneeRole { get; private set; }
}
