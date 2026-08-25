using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class AiJob
{
    private AiJob()
    {
    }

    public Guid Id { get; private set; }

    public Guid GovernedResourceId { get; private set; }

    public string JobType { get; private set; } = null!;

    public string Status { get; private set; } = null!;

    public string Classification { get; private set; } = null!;

    public Guid RequestedBy { get; private set; }

    public Guid AccessScopeId { get; private set; }

    public Guid ModelDeploymentVersionId { get; private set; }

    public Guid PromptVersionId { get; private set; }

    public Guid OutputSchemaVersionId { get; private set; }

    public Guid DataHandlingPolicyVersionId { get; private set; }

    public Guid ToolPolicyVersionId { get; private set; }

    public string GenerationParameters { get; private set; } = null!;

    public string InputChecksum { get; private set; } = null!;

    public Guid RequestId { get; private set; }

    public Guid CorrelationId { get; private set; }

    public DateTimeOffset QueuedAt { get; private set; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public long? InputTokens { get; private set; }

    public long? OutputTokens { get; private set; }

    public decimal? EstimatedCost { get; private set; }

    public string? ErrorCode { get; private set; }

    public string? ErrorDetailRedacted { get; private set; }

    public string TargetResourceType { get; private set; } = null!;

    public Guid TargetResourceId { get; private set; }

    public long TargetResourceVersion { get; private set; }

    public string TargetContentChecksum { get; private set; } = null!;

    public long TargetRowVersion { get; private set; }

    public GovernedResource GovernedResource { get; private set; } = null!;

    public Principal RequestedByPrincipal { get; private set; } = null!;

    public AccessScope AccessScope { get; private set; } = null!;

    public ModelDeploymentVersion ModelDeploymentVersion { get; private set; } = null!;

    public PromptVersion PromptVersion { get; private set; } = null!;

    public OutputSchemaVersion OutputSchemaVersion { get; private set; } = null!;

    public DataHandlingPolicyVersion DataHandlingPolicyVersion { get; private set; } = null!;

    public ToolPolicyVersion ToolPolicyVersion { get; private set; } = null!;
}
