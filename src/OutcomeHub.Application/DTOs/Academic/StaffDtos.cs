namespace OutcomeHub.Application.DTOs.Academic;

public sealed record StaffDto(
    Guid PersonId,
    string StaffCode,
    string FullName,
    Guid HomeOrgUnitId,
    string HomeOrgUnitName,
    string StaffType,
    string CurrentStatus,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo);

public sealed record CreateStaffRequest(
    string StaffCode,
    string FullName,
    Guid HomeOrgUnitId,
    DateOnly EffectiveFrom,
    string StaffType = "LECTURER",
    string CurrentStatus = "ACTIVE",
    Guid? SourceSystemId = null,
    string? SourcePersonId = null);

public sealed record UpdateStaffRequest(
    string FullName,
    Guid HomeOrgUnitId,
    string StaffType,
    string CurrentStatus,
    DateOnly? EffectiveTo);
