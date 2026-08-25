using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class AiArtifact
{
    private AiArtifact()
    {
    }

    public Guid Id { get; private set; }

    public Guid GovernedResourceId { get; private set; }

    public Guid AiJobId { get; private set; }

    public string ArtifactType { get; private set; } = null!;

    public string TargetResourceType { get; private set; } = null!;

    public Guid TargetResourceId { get; private set; }

    public string FieldPath { get; private set; } = null!;

    public string ProposedValue { get; private set; } = null!;

    public decimal Confidence { get; private set; }

    public bool IsInferred { get; private set; }

    public string ReviewStatus { get; private set; } = null!;

    public Guid? ReviewedBy { get; private set; }

    public DateTimeOffset? ReviewedAt { get; private set; }

    public long? AppliedResourceVersion { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public GovernedResource GovernedResource { get; private set; } = null!;

    public AiJob AiJob { get; private set; } = null!;

    public Principal? Reviewer { get; private set; }
}
