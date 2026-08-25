using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class ToolPolicyVersion
{
    private ToolPolicyVersion()
    {
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = null!;

    public int VersionNo { get; private set; }

    public string AllowedTools { get; private set; } = null!;

    public int TimeoutSeconds { get; private set; }

    public string NetworkPolicy { get; private set; } = null!;

    public string FileSandboxPolicy { get; private set; } = null!;

    public int RateLimit { get; private set; }

    public decimal CostLimit { get; private set; }

    public string Checksum { get; private set; } = null!;

    public string Status { get; private set; } = null!;

    public Guid? ApprovedBy { get; private set; }

    public DateTimeOffset? ApprovedAt { get; private set; }

    public DateOnly EffectiveFrom { get; private set; }

    public DateOnly? EffectiveTo { get; private set; }

    public Guid? ActivationDecisionId { get; private set; }

    public Principal? Approver { get; private set; }

    public ActivationDecision? ActivationDecision { get; private set; }
}
