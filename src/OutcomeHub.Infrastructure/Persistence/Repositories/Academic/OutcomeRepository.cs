using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Academic;

public sealed class OutcomeRepository : IOutcomeRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public OutcomeRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<ProgramOutcomeTreeDto?> GetOutcomeTreeAsync(
        Guid programVersionId,
        CancellationToken cancellationToken = default)
    {
        var version = await _dbContext.ProgramVersions
            .AsNoTracking()
            .Where(pv => pv.Id == programVersionId)
            .Select(pv => new { pv.Id, pv.Code })
            .FirstOrDefaultAsync(cancellationToken);

        if (version == null)
        {
            return null;
        }

        var plos = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == programVersionId)
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.Code)
            .ToListAsync(cancellationToken);

        var pis = await _dbContext.ProgramPis
            .AsNoTracking()
            .Where(pi => pi.ProgramVersionId == programVersionId)
            .OrderBy(pi => pi.SortOrder)
            .ThenBy(pi => pi.Code)
            .ToListAsync(cancellationToken);

        var piLookup = pis.ToLookup(pi => pi.ProgramPloId);

        var ploDtos = plos.Select(plo => new ProgramPloDto(
            plo.Id,
            plo.ProgramVersionId,
            plo.Code,
            plo.Description,
            plo.Domain,
            plo.BloomLevel,
            plo.IsLocked,
            plo.SortOrder,
            piLookup[plo.Id].Select(pi => new ProgramPiDto(
                pi.Id,
                pi.ProgramVersionId,
                pi.ProgramPloId,
                pi.Code,
                pi.Description,
                pi.IsLocked,
                pi.IsCore,
                pi.WeightRatio,
                pi.SortOrder)).ToList())).ToList();

        return new ProgramOutcomeTreeDto(
            version.Id,
            version.Code,
            ploDtos);
    }

    public async Task<ProgramPloDto?> GetPloByIdAsync(Guid ploId, CancellationToken cancellationToken = default)
    {
        var plo = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Where(p => p.Id == ploId)
            .FirstOrDefaultAsync(cancellationToken);

        if (plo == null)
        {
            return null;
        }

        var pis = await _dbContext.ProgramPis
            .AsNoTracking()
            .Where(pi => pi.ProgramPloId == ploId)
            .OrderBy(pi => pi.SortOrder)
            .ThenBy(pi => pi.Code)
            .Select(pi => new ProgramPiDto(
                pi.Id,
                pi.ProgramVersionId,
                pi.ProgramPloId,
                pi.Code,
                pi.Description,
                pi.IsLocked,
                pi.IsCore,
                pi.WeightRatio,
                pi.SortOrder))
            .ToListAsync(cancellationToken);

        return new ProgramPloDto(
            plo.Id,
            plo.ProgramVersionId,
            plo.Code,
            plo.Description,
            plo.Domain,
            plo.BloomLevel,
            plo.IsLocked,
            plo.SortOrder,
            pis);
    }

    public async Task<ProgramPloDto> CreatePloAsync(ProgramPlo plo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(plo);

        _dbContext.ProgramPlos.Add(plo);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await GetPloByIdAsync(plo.Id, cancellationToken))!;
    }

    public async Task<ProgramPiDto> CreatePiAsync(ProgramPi pi, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pi);

        _dbContext.ProgramPis.Add(pi);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ProgramPiDto(
            pi.Id,
            pi.ProgramVersionId,
            pi.ProgramPloId,
            pi.Code,
            pi.Description,
            pi.IsLocked,
            pi.IsCore,
            pi.WeightRatio,
            pi.SortOrder);
    }
}
