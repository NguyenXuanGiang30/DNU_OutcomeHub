namespace OutcomeHub.Domain.Entities.Iam;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "RoleVersionPermission is the established IAM association entity name.")]
public sealed class RoleVersionPermission
{
    private RoleVersionPermission()
    {
    }

    public Guid RoleVersionId { get; private set; }
    public Guid PermissionId { get; private set; }
    public DateTimeOffset GrantedAt { get; private set; }
    public Guid GrantedBy { get; private set; }

    public RoleVersion RoleVersion { get; private set; } = null!;
    public Permission Permission { get; private set; } = null!;
    public Principal GrantedByPrincipal { get; private set; } = null!;
}
