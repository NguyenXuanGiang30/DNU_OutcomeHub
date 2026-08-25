namespace OutcomeHub.Application.DTOs.Academic;

public sealed record OrgUnitDto(
    Guid Id,
    Guid? ParentId,
    string Code,
    string Name,
    string UnitType,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record OrgUnitTreeDto(
    Guid Id,
    Guid? ParentId,
    string Code,
    string Name,
    string UnitType,
    string Status,
    IReadOnlyList<OrgUnitTreeDto> Children);

public sealed record CreateOrgUnitRequest(
    Guid? ParentId,
    string Code,
    string Name,
    string UnitType,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo = null,
    string Status = "ACTIVE");

public sealed record UpdateOrgUnitRequest(
    string Name,
    string UnitType,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo = null,
    string Status = "ACTIVE",
    Guid? ParentId = null);
