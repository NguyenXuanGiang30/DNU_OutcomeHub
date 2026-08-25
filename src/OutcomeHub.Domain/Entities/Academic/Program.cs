namespace OutcomeHub.Domain.Entities.Academic;

public sealed class Program
{
    private Program() { }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string DegreeLevel { get; private set; } = null!;
    public string EducationMode { get; private set; } = null!;
    public Guid OwnerOrgUnitId { get; private set; }
    public string Status { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid UpdatedBy { get; private set; }
    public long RowVersion { get; private set; }

    public OrgUnit OwnerOrgUnit { get; private set; } = null!;

    public static Program Create(
        Guid id,
        string code,
        string name,
        string degreeLevel,
        string educationMode,
        Guid ownerOrgUnitId,
        string status,
        Guid createdBy,
        DateTimeOffset? createdAt = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(degreeLevel);
        ArgumentException.ThrowIfNullOrWhiteSpace(educationMode);

        var now = createdAt ?? DateTimeOffset.UtcNow;
        return new Program
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            DegreeLevel = degreeLevel.Trim().ToUpperInvariant(),
            EducationMode = educationMode.Trim().ToUpperInvariant(),
            OwnerOrgUnitId = ownerOrgUnitId,
            Status = string.IsNullOrWhiteSpace(status) ? "DRAFT" : status.Trim().ToUpperInvariant(),
            CreatedAt = now,
            CreatedBy = createdBy,
            UpdatedAt = now,
            UpdatedBy = createdBy,
            RowVersion = 1,
        };
    }

    public void Update(
        string name,
        string degreeLevel,
        string educationMode,
        string status,
        Guid updatedBy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(degreeLevel);
        ArgumentException.ThrowIfNullOrWhiteSpace(educationMode);

        Name = name.Trim();
        DegreeLevel = degreeLevel.Trim().ToUpperInvariant();
        EducationMode = educationMode.Trim().ToUpperInvariant();
        Status = string.IsNullOrWhiteSpace(status) ? Status : status.Trim().ToUpperInvariant();
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy;
    }
}
