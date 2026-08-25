using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Iam;

public sealed class IdpGroupRoleMapping
{
    private IdpGroupRoleMapping()
    {
    }

    public Guid Id { get; private set; }
    public Guid IdentityProviderId { get; private set; }
    public string ExternalGroupId { get; private set; } = null!;
    public Guid RoleId { get; private set; }
    public Guid RoleVersionId { get; private set; }
    public Guid AccessScopeId { get; private set; }
    public int VersionNo { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string Status { get; private set; } = null!;
    public Guid WorkflowInstanceId { get; private set; }
    public Guid? SupersedesId { get; private set; }
    public string Checksum { get; private set; } = null!;

    public IdentityProvider IdentityProvider { get; private set; } = null!;
    public Role Role { get; private set; } = null!;
    public RoleVersion RoleVersion { get; private set; } = null!;
    public AccessScope AccessScope { get; private set; } = null!;
    public WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public IdpGroupRoleMapping? Supersedes { get; private set; }
}
