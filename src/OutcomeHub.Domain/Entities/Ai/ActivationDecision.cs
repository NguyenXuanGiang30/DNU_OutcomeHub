using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class ActivationDecision
{
    private ActivationDecision()
    {
    }

    public Guid Id { get; private set; }

    public Guid EvaluationRunId { get; private set; }

    public Guid ModelDeploymentVersionId { get; private set; }

    public Guid PromptVersionId { get; private set; }

    public Guid OutputSchemaVersionId { get; private set; }

    public Guid DataHandlingPolicyVersionId { get; private set; }

    public Guid ToolPolicyVersionId { get; private set; }

    public Guid DecisionRecordId { get; private set; }

    public Guid ApprovedBy { get; private set; }

    public DateTimeOffset ApprovedAt { get; private set; }

    public string Checksum { get; private set; } = null!;

    public EvaluationRun EvaluationRun { get; private set; } = null!;

    public ModelDeploymentVersion ModelDeploymentVersion { get; private set; } = null!;

    public PromptVersion PromptVersion { get; private set; } = null!;

    public OutputSchemaVersion OutputSchemaVersion { get; private set; } = null!;

    public DataHandlingPolicyVersion DataHandlingPolicyVersion { get; private set; } = null!;

    public ToolPolicyVersion ToolPolicyVersion { get; private set; } = null!;

    public DecisionRecord DecisionRecord { get; private set; } = null!;

    public Principal Approver { get; private set; } = null!;
}
