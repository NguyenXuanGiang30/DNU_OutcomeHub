using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Iam;

public sealed class RoleAssignment
{
    private RoleAssignment()
    {
    }

    public Guid Id { get; private set; }
    public Guid PrincipalId { get; private set; }
    public Guid RoleId { get; private set; }
    public Guid RoleVersionId { get; private set; }
    public Guid AccessScopeId { get; private set; }
    public DateTimeOffset EffectiveFrom { get; private set; }
    public DateTimeOffset EffectiveTo { get; private set; }
    public string Status { get; private set; } = null!;
    public string Source { get; private set; } = null!;
    public string? SourceReference { get; private set; }
    public Guid GrantedBy { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public Guid WorkflowInstanceId { get; private set; }
    public Guid SodPolicyVersionId { get; private set; }
    public string AuthorizationSnapshotChecksum { get; private set; } = null!;
    public Guid RequestedBy { get; private set; }
    public DateTimeOffset RequestedAt { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string Reason { get; private set; } = null!;
    public string? RevokeReason { get; private set; }

    public Principal Principal { get; private set; } = null!;
    public Role Role { get; private set; } = null!;
    public RoleVersion RoleVersion { get; private set; } = null!;
    public AccessScope AccessScope { get; private set; } = null!;
    public Principal GrantedByPrincipal { get; private set; } = null!;
    public Principal? ApprovedByPrincipal { get; private set; }
    public WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public SodPolicyVersion SodPolicyVersion { get; private set; } = null!;
    public Principal RequestedByPrincipal { get; private set; } = null!;
}
