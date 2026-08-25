namespace OutcomeHub.Domain.Entities.Academic;

public sealed class Course
{
    private Course() { }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Guid OwnerOrgUnitId { get; private set; }
    public string Status { get; private set; } = null!;

    public OrgUnit OwnerOrgUnit { get; private set; } = null!;

    public static Course Create(
        Guid id,
        string code,
        string name,
        Guid ownerOrgUnitId,
        string status = "DRAFT")
    {
        return new Course
        {
            Id = id,
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            OwnerOrgUnitId = ownerOrgUnitId,
            Status = status.Trim().ToUpperInvariant(),
        };
    }

    public void Update(string name, string status)
    {
        Name = name.Trim();
        Status = status.Trim().ToUpperInvariant();
    }
}
