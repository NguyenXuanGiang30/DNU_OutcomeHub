using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IOrgUnitRepository
{
    Task<IReadOnlyList<OrgUnitDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<OrgUnitDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrgUnitTreeDto>> GetTreeAsync(CancellationToken cancellationToken = default);

    Task<OrgUnitDto> CreateAsync(OrgUnit orgUnit, CancellationToken cancellationToken = default);

    Task<OrgUnitDto> UpdateAsync(OrgUnit orgUnit, CancellationToken cancellationToken = default);
}
