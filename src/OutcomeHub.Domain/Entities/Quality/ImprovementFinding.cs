namespace OutcomeHub.Domain.Entities.Quality;

public sealed class ImprovementFinding
{
    private ImprovementFinding()
    {
    }

    public Guid Id { get; private set; }

    public Guid ImprovementPlanId { get; private set; }

    public string FindingType { get; private set; } = null!;

    public short? AcademicYearStart { get; private set; }

    public Guid? CohortOutcomeResultId { get; private set; }

    public Guid? ResultAlertId { get; private set; }

    public string? Description { get; private set; }

    public string? SourceChecksum { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public ImprovementPlan ImprovementPlan { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Result.CohortOutcomeResult? CohortOutcomeResult { get; private set; }
    public OutcomeHub.Domain.Entities.Result.ResultAlert? ResultAlert { get; private set; }
}
