using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class PromptVersion
{
    private PromptVersion()
    {
    }

    public Guid Id { get; private set; }

    public Guid PromptId { get; private set; }

    public int VersionNo { get; private set; }

    public string SystemTemplate { get; private set; } = null!;

    public string InputContract { get; private set; } = null!;

    public Guid OutputSchemaVersionId { get; private set; }

    public string Checksum { get; private set; } = null!;

    public string Status { get; private set; } = null!;

    public Guid? ApprovedBy { get; private set; }

    public DateTimeOffset? ApprovedAt { get; private set; }

    public DateOnly EffectiveFrom { get; private set; }

    public DateOnly? EffectiveTo { get; private set; }

    public Guid? ActivationDecisionId { get; private set; }

    public Prompt Prompt { get; private set; } = null!;

    public OutputSchemaVersion OutputSchemaVersion { get; private set; } = null!;

    public Principal? Approver { get; private set; }

    public ActivationDecision? ActivationDecision { get; private set; }
}
