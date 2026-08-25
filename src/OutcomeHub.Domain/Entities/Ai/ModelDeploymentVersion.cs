using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class ModelDeploymentVersion
{
    private ModelDeploymentVersion()
    {
    }

    public Guid Id { get; private set; }

    public Guid ModelDeploymentId { get; private set; }

    public int VersionNo { get; private set; }

    public string Provider { get; private set; } = null!;

    public string ProviderModelId { get; private set; } = null!;

    public string? ProviderModelRevision { get; private set; }

    public string DeploymentName { get; private set; } = null!;

    public string Region { get; private set; } = null!;

    public string Capability { get; private set; } = null!;

    public string SecretReference { get; private set; } = null!;

    public string Configuration { get; private set; } = null!;

    public string Checksum { get; private set; } = null!;

    public string Status { get; private set; } = null!;

    public DateOnly EffectiveFrom { get; private set; }

    public DateOnly? EffectiveTo { get; private set; }

    public Guid? ApprovedBy { get; private set; }

    public DateTimeOffset? ApprovedAt { get; private set; }

    public Guid? ActivationDecisionId { get; private set; }

    public ModelDeployment ModelDeployment { get; private set; } = null!;

    public Principal? Approver { get; private set; }

    public ActivationDecision? ActivationDecision { get; private set; }
}
