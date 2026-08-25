namespace OutcomeHub.Domain.Entities.Quality;

public sealed class ImprovementPlan
{
    private ImprovementPlan()
    {
    }

    public Guid Id { get; private set; }

    public Guid GovernedResourceId { get; private set; }

    public string Code { get; private set; } = null!;

    public Guid OrgUnitId { get; private set; }

    public Guid ProgramVersionId { get; private set; }

    public string Title { get; private set; } = null!;

    public string ProblemStatement { get; private set; } = null!;

    public string? RootCauseSummary { get; private set; }

    public decimal? BaselineValue { get; private set; }

    public decimal? TargetValue { get; private set; }

    public string KpiDefinition { get; private set; } = null!;

    public Guid OwnerPrincipalId { get; private set; }

    public DateOnly DueDate { get; private set; }

    public Guid WorkflowInstanceId { get; private set; }

    public string Status { get; private set; } = null!;

    public Guid CreatedBy { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public long RowVersion { get; private set; }

    public OutcomeHub.Domain.Entities.Governance.GovernedResource GovernedResource { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.OrgUnit OrgUnit { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramVersion ProgramVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal OwnerPrincipal { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Workflow.WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal CreatedByPrincipal { get; private set; } = null!;
}
