using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class AiReviewEvent
{
    private AiReviewEvent()
    {
    }

    public Guid Id { get; private set; }

    public Guid ArtifactId { get; private set; }

    public string Decision { get; private set; } = null!;

    public string ProposedBefore { get; private set; } = null!;

    public string? FinalValue { get; private set; }

    public string? Reason { get; private set; }

    public Guid ReviewerPrincipalId { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }

    public AiArtifact Artifact { get; private set; } = null!;

    public Principal Reviewer { get; private set; } = null!;
}
