namespace OutcomeHub.Domain.Entities.Quality;

public sealed class ImprovementAction
{
    private ImprovementAction()
    {
    }

    public Guid Id { get; private set; }

    public Guid ImprovementPlanId { get; private set; }

    public int ActionNo { get; private set; }

    public string Description { get; private set; } = null!;

    public Guid OwnerPrincipalId { get; private set; }

    public Guid OwnerOrgUnitId { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly DueDate { get; private set; }

    public string Status { get; private set; } = null!;

    public decimal CompletionRatio { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public long RowVersion { get; private set; }

    public ImprovementPlan ImprovementPlan { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal OwnerPrincipal { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.OrgUnit OwnerOrgUnit { get; private set; } = null!;
}
