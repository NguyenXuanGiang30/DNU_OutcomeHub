namespace OutcomeHub.Domain.Entities.Iam;

public sealed class SodRule
{
    private readonly List<SodException> _exceptions = [];

    private SodRule()
    {
    }

    public Guid Id { get; private set; }
    public Guid PolicyVersionId { get; private set; }
    public string ResourceType { get; private set; } = null!;
    public Guid PermissionAId { get; private set; }
    public Guid PermissionBId { get; private set; }
    public string ConflictMode { get; private set; } = null!;
    public string Severity { get; private set; } = null!;

    public SodPolicyVersion PolicyVersion { get; private set; } = null!;
    public Permission PermissionA { get; private set; } = null!;
    public Permission PermissionB { get; private set; } = null!;
    public IReadOnlyCollection<SodException> Exceptions => _exceptions;

    public static SodRule Create(
        Guid id,
        Guid policyVersionId,
        string resourceType,
        Guid permissionAId,
        Guid permissionBId,
        string conflictMode,
        string severity)
    {
        if (permissionAId == permissionBId)
        {
            throw new ArgumentException("PermissionA and PermissionB must be different.", nameof(permissionBId));
        }

        return new SodRule
        {
            Id = id,
            PolicyVersionId = policyVersionId,
            ResourceType = resourceType.Trim(),
            PermissionAId = permissionAId,
            PermissionBId = permissionBId,
            ConflictMode = conflictMode,
            Severity = severity
        };
    }
}
