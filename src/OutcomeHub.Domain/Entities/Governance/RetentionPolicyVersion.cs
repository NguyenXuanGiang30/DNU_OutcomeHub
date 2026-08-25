using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Governance;

public sealed class RetentionPolicyVersion
{
    private RetentionPolicyVersion() { }
    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public int VersionNo { get; private set; }
    public string Name { get; private set; } = null!;
    public string ResourceType { get; private set; } = null!;
    public string TriggerEvent { get; private set; } = null!;
    public int RetentionDays { get; private set; }
    public string DispositionAction { get; private set; } = null!;
    public string LegalBasis { get; private set; } = null!;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string Status { get; private set; } = null!;
    public Guid? ApprovedBy { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public Principal? Approver { get; private set; }
}
