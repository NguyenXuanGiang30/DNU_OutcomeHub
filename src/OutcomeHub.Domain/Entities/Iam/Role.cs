namespace OutcomeHub.Domain.Entities.Iam;

public sealed class Role
{
    private readonly List<RoleVersion> _versions = [];

    private Role()
    {
    }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsSystem { get; private set; }
    public string Status { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<RoleVersion> Versions => _versions;
}
