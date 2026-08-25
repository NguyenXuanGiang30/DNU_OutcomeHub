namespace OutcomeHub.Domain.Entities.Quality;

public sealed class ImprovementDocument
{
    private ImprovementDocument()
    {
    }

    public Guid ImprovementPlanId { get; private set; }

    public Guid DocumentVersionId { get; private set; }

    public string DocumentRole { get; private set; } = null!;

    public ImprovementPlan ImprovementPlan { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Document.DocumentVersion DocumentVersion { get; private set; } = null!;
}
