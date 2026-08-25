using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IStaffRepository
{
    Task<PagedResult<StaffDto>> GetPagedStaffAsync(
        PagedRequest request,
        Guid? homeOrgUnitId,
        string? staffType,
        CancellationToken cancellationToken = default);

    Task<StaffDto?> GetStaffByIdAsync(Guid personId, CancellationToken cancellationToken = default);

    Task<Staff> CreateStaffAsync(
        Person person,
        Staff staff,
        CancellationToken cancellationToken = default);

    Task<Staff> UpdateStaffAsync(
        Guid personId,
        string fullName,
        Guid homeOrgUnitId,
        string staffType,
        string currentStatus,
        DateOnly? effectiveTo,
        CancellationToken cancellationToken = default);
}
