using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Iam;

public sealed class RoleVersion
{
    private readonly List<RoleVersionPermission> _permissions = [];

    private RoleVersion()
    {
    }

    public Guid Id { get; private set; }
    public Guid RoleId { get; private set; }
    public int VersionNo { get; private set; }
    public string Status { get; private set; } = null!;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public Guid WorkflowInstanceId { get; private set; }
    public Guid? DecisionId { get; private set; }
    public string PermissionSetChecksum { get; private set; } = null!;
    public string Checksum { get; private set; } = null!;
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public Role Role { get; private set; } = null!;
    public WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public DecisionRecord? Decision { get; private set; }
    public Principal CreatedByPrincipal { get; private set; } = null!;
    public IReadOnlyCollection<RoleVersionPermission> Permissions => _permissions;
}
