namespace OutcomeHub.Domain.Entities.Quality;

public sealed class PlanWaiver
{
    private PlanWaiver()
    {
    }

    public Guid Id { get; private set; }

    public Guid FindingId { get; private set; }

    public string Reason { get; private set; } = null!;

    public Guid RequestedBy { get; private set; }

    public Guid WorkflowInstanceId { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public ImprovementFinding Finding { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal RequestedByPrincipal { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Workflow.WorkflowInstance WorkflowInstance { get; private set; } = null!;
}
