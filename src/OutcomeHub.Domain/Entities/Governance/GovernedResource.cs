namespace OutcomeHub.Domain.Entities.Governance;

public sealed class GovernedResource
{
    private GovernedResource() { }
    public Guid Id { get; private set; }
    public string ResourceType { get; private set; } = null!;
    public string Classification { get; private set; } = null!;
    public string DispositionStatus { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public ICollection<ResourceSecurityScope> SecurityScopes { get; private set; } = new List<ResourceSecurityScope>();
    public ICollection<RetentionBinding> RetentionBindings { get; private set; } = new List<RetentionBinding>();
}
