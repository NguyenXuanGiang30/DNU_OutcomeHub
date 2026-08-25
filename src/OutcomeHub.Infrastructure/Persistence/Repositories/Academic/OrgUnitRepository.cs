using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Academic;

public sealed class OrgUnitRepository : IOrgUnitRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public OrgUnitRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<OrgUnitDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.OrgUnits
            .AsNoTracking()
            .OrderBy(o => o.Code)
            .Select(o => new OrgUnitDto(
                o.Id,
                o.ParentId,
                o.Code,
                o.Name,
                o.UnitType,
                o.EffectiveFrom,
                o.EffectiveTo,
                o.Status,
                o.CreatedAt,
                o.UpdatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrgUnitDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OrgUnits
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrgUnitDto(
                o.Id,
                o.ParentId,
                o.Code,
                o.Name,
                o.UnitType,
                o.EffectiveFrom,
                o.EffectiveTo,
                o.Status,
                o.CreatedAt,
                o.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrgUnitTreeDto>> GetTreeAsync(CancellationToken cancellationToken = default)
    {
        var allUnits = await _dbContext.OrgUnits
            .AsNoTracking()
            .OrderBy(o => o.Code)
            .ToListAsync(cancellationToken);

        var lookup = allUnits.ToLookup(u => u.ParentId);

        List<OrgUnitTreeDto> BuildSubtree(Guid? parentId)
        {
            return lookup[parentId]
                .Select(u => new OrgUnitTreeDto(
                    u.Id,
                    u.ParentId,
                    u.Code,
                    u.Name,
                    u.UnitType,
                    u.Status,
                    BuildSubtree(u.Id)))
                .ToList();
        }

        return BuildSubtree(null);
    }

    public async Task<OrgUnitDto> CreateAsync(OrgUnit orgUnit, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orgUnit);

        _dbContext.OrgUnits.Add(orgUnit);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(orgUnit.Id, cancellationToken))!;
    }

    public async Task<OrgUnitDto> UpdateAsync(OrgUnit orgUnit, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orgUnit);

        var existing = await _dbContext.OrgUnits.FindAsync([orgUnit.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(OrgUnit), orgUnit.Id);

        existing.Update(
            orgUnit.Name,
            orgUnit.UnitType,
            orgUnit.EffectiveFrom,
            orgUnit.EffectiveTo,
            orgUnit.Status,
            orgUnit.UpdatedBy,
            orgUnit.ParentId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(orgUnit.Id, cancellationToken))!;
    }
}
