namespace OutcomeHub.Domain.Entities.Governance;

public sealed class ResourceDependency
{
    private ResourceDependency() { }
    public Guid ParentGovernedResourceId { get; private set; }
    public Guid ChildGovernedResourceId { get; private set; }
    public string DependencyRole { get; private set; } = null!;
    public GovernedResource ParentGovernedResource { get; private set; } = null!;
    public GovernedResource ChildGovernedResource { get; private set; } = null!;
}
