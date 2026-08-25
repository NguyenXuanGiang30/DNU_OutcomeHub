namespace OutcomeHub.Domain.Entities.Academic;

public sealed class Staff
{
    private Staff() { }

    public Guid PersonId { get; private set; }
    public string StaffCode { get; private set; } = null!;
    public Guid HomeOrgUnitId { get; private set; }
    public string StaffType { get; private set; } = null!;
    public string CurrentStatus { get; private set; } = null!;

    public Person Person { get; private set; } = null!;
    public OrgUnit HomeOrgUnit { get; private set; } = null!;

    public static Staff Create(
        Guid personId,
        string staffCode,
        Guid homeOrgUnitId,
        string staffType = "LECTURER",
        string currentStatus = "ACTIVE")
    {
        return new Staff
        {
            PersonId = personId,
            StaffCode = staffCode.Trim().ToUpperInvariant(),
            HomeOrgUnitId = homeOrgUnitId,
            StaffType = staffType.Trim().ToUpperInvariant(),
            CurrentStatus = currentStatus.Trim().ToUpperInvariant(),
        };
    }

    public void Update(Guid homeOrgUnitId, string staffType, string currentStatus)
    {
        HomeOrgUnitId = homeOrgUnitId;
        StaffType = staffType.Trim().ToUpperInvariant();
        CurrentStatus = currentStatus.Trim().ToUpperInvariant();
    }
}
