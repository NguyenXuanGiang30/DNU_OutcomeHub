namespace OutcomeHub.Domain.Entities.Governance;

public sealed class RetentionBinding
{
    private RetentionBinding() { }
    public Guid Id { get; private set; }
    public Guid GovernedResourceId { get; private set; }
    public Guid RetentionPolicyVersionId { get; private set; }
    public DateTimeOffset TriggerEventAt { get; private set; }
    public DateTimeOffset CalculatedUntil { get; private set; }
    public string Status { get; private set; } = null!;
    public string SourceReason { get; private set; } = null!;
    public GovernedResource GovernedResource { get; private set; } = null!;
    public RetentionPolicyVersion RetentionPolicyVersion { get; private set; } = null!;
}
