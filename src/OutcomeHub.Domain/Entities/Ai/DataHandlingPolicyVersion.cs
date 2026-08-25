using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class DataHandlingPolicyVersion
{
    private DataHandlingPolicyVersion()
    {
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = null!;

    public int VersionNo { get; private set; }

    public string AllowedProviders { get; private set; } = null!;

    public string AllowedRegions { get; private set; } = null!;

    public int InputRetentionDays { get; private set; }

    public int OutputRetentionDays { get; private set; }

    public bool ProviderTrainingOptOut { get; private set; }

    public string MaximumClassification { get; private set; } = null!;

    public string RedactionRules { get; private set; } = null!;

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
