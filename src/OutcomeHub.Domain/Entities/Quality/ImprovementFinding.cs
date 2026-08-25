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

    /// <summary>
    /// Creates a finding linked to a CohortOutcomeResult (PLO/PI not attained).
    /// </summary>
    public static ImprovementFinding CreateFromCohortResult(
        Guid id,
        Guid improvementPlanId,
        string findingType,
        short academicYearStart,
        Guid cohortOutcomeResultId,
        string? description,
        string? sourceChecksum,
        DateTimeOffset createdAt)
    {
        return new ImprovementFinding
        {
            Id = id,
            ImprovementPlanId = improvementPlanId,
            FindingType = findingType,
            AcademicYearStart = academicYearStart,
            CohortOutcomeResultId = cohortOutcomeResultId,
            ResultAlertId = null,
            Description = description,
            SourceChecksum = sourceChecksum,
            CreatedAt = createdAt
        };
    }

    /// <summary>
    /// Creates a finding from a qualitative/manual observation (no result link).
    /// </summary>
    public static ImprovementFinding CreateFromDescription(
        Guid id,
        Guid improvementPlanId,
        string findingType,
        string description,
        DateTimeOffset createdAt)
    {
        return new ImprovementFinding
        {
            Id = id,
            ImprovementPlanId = improvementPlanId,
            FindingType = findingType,
            AcademicYearStart = null,
            CohortOutcomeResultId = null,
            ResultAlertId = null,
            Description = description,
            SourceChecksum = null,
            CreatedAt = createdAt
        };
    }
}
