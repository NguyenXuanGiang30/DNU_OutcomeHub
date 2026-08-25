using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class EvaluationRun
{
    private EvaluationRun()
    {
    }

    public Guid Id { get; private set; }

    public Guid GovernedResourceId { get; private set; }

    public Guid SuiteVersionId { get; private set; }

    public string SuiteChecksum { get; private set; } = null!;

    public Guid EvaluationPolicyVersionId { get; private set; }

    public string EvaluationPolicyChecksum { get; private set; } = null!;

    public Guid ModelDeploymentVersionId { get; private set; }

    public Guid PromptVersionId { get; private set; }

    public Guid OutputSchemaVersionId { get; private set; }

    public Guid DataHandlingPolicyVersionId { get; private set; }

    public Guid ToolPolicyVersionId { get; private set; }

    public string ConfigBundleChecksum { get; private set; } = null!;

    public string Status { get; private set; } = null!;

    public string? ResultChecksum { get; private set; }

    public DateTimeOffset StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public GovernedResource GovernedResource { get; private set; } = null!;

    public GroundTruthSuiteVersion SuiteVersion { get; private set; } = null!;

    public EvaluationPolicyVersion EvaluationPolicyVersion { get; private set; } = null!;

    public ModelDeploymentVersion ModelDeploymentVersion { get; private set; } = null!;

    public PromptVersion PromptVersion { get; private set; } = null!;

    public OutputSchemaVersion OutputSchemaVersion { get; private set; } = null!;

    public DataHandlingPolicyVersion DataHandlingPolicyVersion { get; private set; } = null!;

    public ToolPolicyVersion ToolPolicyVersion { get; private set; } = null!;
}
