namespace OutcomeHub.Domain.Entities.Academic;

public sealed class OrgUnit
{
    private OrgUnit() { }

    public Guid Id { get; private set; }
    public Guid? ParentId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string UnitType { get; private set; } = null!;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string Status { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid UpdatedBy { get; private set; }
    public long RowVersion { get; private set; }

    public OrgUnit? Parent { get; private set; }

    public static OrgUnit Create(
        Guid id,
        Guid? parentId,
        string code,
        string name,
        string unitType,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo,
        string status,
        Guid createdBy,
        DateTimeOffset? createdAt = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(unitType);

        var now = createdAt ?? DateTimeOffset.UtcNow;
        return new OrgUnit
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            ParentId = parentId,
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            UnitType = unitType.Trim().ToUpperInvariant(),
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            Status = string.IsNullOrWhiteSpace(status) ? "ACTIVE" : status.Trim().ToUpperInvariant(),
            CreatedAt = now,
            CreatedBy = createdBy,
            UpdatedAt = now,
            UpdatedBy = createdBy,
            RowVersion = 1,
        };
    }

    public void Update(
        string name,
        string unitType,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo,
        string status,
        Guid updatedBy,
        Guid? parentId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(unitType);

        Name = name.Trim();
        UnitType = unitType.Trim().ToUpperInvariant();
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        Status = string.IsNullOrWhiteSpace(status) ? Status : status.Trim().ToUpperInvariant();
        ParentId = parentId ?? ParentId;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy;
    }
}
