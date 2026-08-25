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

    /// <summary>
    /// Creates a new ImprovementPlan in DRAFT status.
    /// </summary>
    public static ImprovementPlan Create(
        Guid id,
        Guid governedResourceId,
        string code,
        Guid orgUnitId,
        Guid programVersionId,
        string title,
        string problemStatement,
        string? rootCauseSummary,
        decimal? baselineValue,
        decimal? targetValue,
        string kpiDefinition,
        Guid ownerPrincipalId,
        DateOnly dueDate,
        Guid workflowInstanceId,
        Guid createdBy,
        DateTimeOffset createdAt)
    {
        return new ImprovementPlan
        {
            Id = id,
            GovernedResourceId = governedResourceId,
            Code = code,
            OrgUnitId = orgUnitId,
            ProgramVersionId = programVersionId,
            Title = title,
            ProblemStatement = problemStatement,
            RootCauseSummary = rootCauseSummary,
            BaselineValue = baselineValue,
            TargetValue = targetValue,
            KpiDefinition = kpiDefinition,
            OwnerPrincipalId = ownerPrincipalId,
            DueDate = dueDate,
            WorkflowInstanceId = workflowInstanceId,
            Status = "DRAFT",
            CreatedBy = createdBy,
            CreatedAt = createdAt,
            RowVersion = 1
        };
    }

    /// <summary>
    /// Updates plan details (problem, root cause, KPI, target, due date).
    /// Only allowed when plan is in DRAFT or REOPENED status.
    /// </summary>
    public void UpdateDetails(
        string title,
        string problemStatement,
        string? rootCauseSummary,
        decimal? baselineValue,
        decimal? targetValue,
        string kpiDefinition,
        DateOnly dueDate)
    {
        if (Status != "DRAFT" && Status != "REOPENED")
        {
            throw new InvalidOperationException(
                $"Cannot update ImprovementPlan in status '{Status}'. Only DRAFT or REOPENED plans can be updated.");
        }

        Title = title;
        ProblemStatement = problemStatement;
        RootCauseSummary = rootCauseSummary;
        BaselineValue = baselineValue;
        TargetValue = targetValue;
        KpiDefinition = kpiDefinition;
        DueDate = dueDate;
    }

    /// <summary>
    /// Transitions the plan status following the workflow:
    /// DRAFT → IN_REVIEW → APPROVED → EXECUTING → VERIFYING → CLOSED
    /// CLOSED/VERIFYING → REOPENED → EXECUTING
    /// </summary>
    public void TransitionStatus(string newStatus)
    {
        var validTransitions = new Dictionary<string, string[]>
        {
            ["DRAFT"] = ["IN_REVIEW"],
            ["IN_REVIEW"] = ["APPROVED", "DRAFT"],
            ["APPROVED"] = ["EXECUTING"],
            ["EXECUTING"] = ["VERIFYING"],
            ["VERIFYING"] = ["CLOSED", "REOPENED"],
            ["CLOSED"] = ["REOPENED"],
            ["REOPENED"] = ["EXECUTING"]
        };

        if (!validTransitions.TryGetValue(Status, out var allowed) ||
            !allowed.Contains(newStatus))
        {
            throw new InvalidOperationException(
                $"Invalid status transition from '{Status}' to '{newStatus}'.");
        }

        Status = newStatus;
    }
}
