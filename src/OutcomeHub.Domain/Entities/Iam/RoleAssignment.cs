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

    public static RoleAssignment Create(
        Guid id,
        Guid principalId,
        Guid roleId,
        Guid roleVersionId,
        Guid accessScopeId,
        DateTimeOffset effectiveFrom,
        DateTimeOffset effectiveTo,
        string status,
        string source,
        string? sourceReference,
        Guid grantedBy,
        Guid? approvedBy,
        Guid workflowInstanceId,
        Guid sodPolicyVersionId,
        string authorizationSnapshotChecksum,
        Guid requestedBy,
        DateTimeOffset requestedAt,
        DateTimeOffset? approvedAt,
        string reason)
    {
        if (effectiveTo <= effectiveFrom)
        {
            throw new ArgumentException("EffectiveTo must be after EffectiveFrom.", nameof(effectiveTo));
        }

        return new RoleAssignment
        {
            Id = id,
            PrincipalId = principalId,
            RoleId = roleId,
            RoleVersionId = roleVersionId,
            AccessScopeId = accessScopeId,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            Status = status,
            Source = source,
            SourceReference = sourceReference,
            GrantedBy = grantedBy,
            ApprovedBy = approvedBy,
            WorkflowInstanceId = workflowInstanceId,
            SodPolicyVersionId = sodPolicyVersionId,
            AuthorizationSnapshotChecksum = authorizationSnapshotChecksum,
            RequestedBy = requestedBy,
            RequestedAt = requestedAt,
            ApprovedAt = approvedAt,
            RevokedAt = null,
            Reason = reason.Trim(),
            RevokeReason = null
        };
    }

    public void Revoke(string revokeReason, DateTimeOffset revokedAt)
    {
        Status = "REVOKED";
        RevokeReason = revokeReason.Trim();
        RevokedAt = revokedAt;
    }
}
