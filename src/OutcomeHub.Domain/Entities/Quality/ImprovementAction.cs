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

    /// <summary>
    /// Creates a new ImprovementAction in PLANNED status.
    /// </summary>
    public static ImprovementAction Create(
        Guid id,
        Guid improvementPlanId,
        int actionNo,
        string description,
        Guid ownerPrincipalId,
        Guid ownerOrgUnitId,
        DateOnly startDate,
        DateOnly dueDate)
    {
        if (dueDate < startDate)
        {
            throw new ArgumentException("DueDate must be >= StartDate.", nameof(dueDate));
        }

        return new ImprovementAction
        {
            Id = id,
            ImprovementPlanId = improvementPlanId,
            ActionNo = actionNo,
            Description = description,
            OwnerPrincipalId = ownerPrincipalId,
            OwnerOrgUnitId = ownerOrgUnitId,
            StartDate = startDate,
            DueDate = dueDate,
            Status = "PLANNED",
            CompletionRatio = 0m,
            CompletedAt = null,
            RowVersion = 1
        };
    }

    /// <summary>
    /// Updates progress ratio (0.0 to 1.0) and transitions to IN_PROGRESS if needed.
    /// </summary>
    public void UpdateProgress(decimal completionRatio)
    {
        if (completionRatio < 0m || completionRatio > 1m)
        {
            throw new ArgumentOutOfRangeException(nameof(completionRatio), "Must be between 0 and 1.");
        }

        CompletionRatio = completionRatio;

        if (Status == "PLANNED" && completionRatio > 0m)
        {
            Status = "IN_PROGRESS";
        }
    }

    /// <summary>
    /// Marks the action as COMPLETED with a completion timestamp.
    /// Sets CompletionRatio to 1.0.
    /// </summary>
    public void Complete(DateTimeOffset completedAt)
    {
        CompletionRatio = 1.0m;
        CompletedAt = completedAt;
        Status = "COMPLETED";
    }

    /// <summary>
    /// Marks the action as OVERDUE if past its due date without completion.
    /// </summary>
    public void MarkOverdue()
    {
        if (Status != "COMPLETED")
        {
            Status = "OVERDUE";
        }
    }
}
