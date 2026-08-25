using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Academic;

public sealed class ProgramRepository : IProgramRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public ProgramRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PagedResult<ProgramDto>> GetPagedAsync(
        PagedRequest request,
        Guid? ownerOrgUnitId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = _dbContext.Programs
            .AsNoTracking()
            .Include(p => p.OwnerOrgUnit)
            .AsQueryable();

        if (ownerOrgUnitId.HasValue && ownerOrgUnitId.Value != Guid.Empty)
        {
            query = query.Where(p => p.OwnerOrgUnitId == ownerOrgUnitId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var pattern = $"%{request.SearchTerm.Trim()}%";
            query = query.Where(p =>
                EF.Functions.ILike(p.Code, pattern) ||
                EF.Functions.ILike(p.Name, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Code)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProgramDto(
                p.Id,
                p.Code,
                p.Name,
                p.DegreeLevel,
                p.EducationMode,
                p.OwnerOrgUnitId,
                p.OwnerOrgUnit.Name,
                p.Status,
                p.CreatedAt,
                p.UpdatedAt))
            .ToListAsync(cancellationToken);

        return PagedResult.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<ProgramDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Programs
            .AsNoTracking()
            .Include(p => p.OwnerOrgUnit)
            .Where(p => p.Id == id)
            .Select(p => new ProgramDto(
                p.Id,
                p.Code,
                p.Name,
                p.DegreeLevel,
                p.EducationMode,
                p.OwnerOrgUnitId,
                p.OwnerOrgUnit.Name,
                p.Status,
                p.CreatedAt,
                p.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProgramDto> CreateAsync(Program program, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(program);

        _dbContext.Programs.Add(program);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(program.Id, cancellationToken))!;
    }

    public async Task<ProgramDto> UpdateAsync(Program program, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(program);

        var existing = await _dbContext.Programs.FindAsync([program.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(Program), program.Id);

        existing.Update(
            program.Name,
            program.DegreeLevel,
            program.EducationMode,
            program.Status,
            program.UpdatedBy);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(program.Id, cancellationToken))!;
    }

    public async Task<IReadOnlyList<ProgramVersionDto>> GetVersionsByProgramIdAsync(
        Guid programId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProgramVersions
            .AsNoTracking()
            .Where(pv => pv.ProgramId == programId)
            .OrderByDescending(pv => pv.VersionNo)
            .Select(pv => new ProgramVersionDto(
                pv.Id,
                pv.ProgramId,
                pv.VersionNo,
                pv.Code,
                pv.InstitutionTemplateVersionId,
                pv.DecisionId,
                pv.EffectiveFrom,
                pv.EffectiveTo,
                pv.Status,
                pv.TotalCredits,
                pv.Checksum,
                pv.RowVersion))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProgramVersionDto?> GetVersionByIdAsync(
        Guid versionId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProgramVersions
            .AsNoTracking()
            .Where(pv => pv.Id == versionId)
            .Select(pv => new ProgramVersionDto(
                pv.Id,
                pv.ProgramId,
                pv.VersionNo,
                pv.Code,
                pv.InstitutionTemplateVersionId,
                pv.DecisionId,
                pv.EffectiveFrom,
                pv.EffectiveTo,
                pv.Status,
                pv.TotalCredits,
                pv.Checksum,
                pv.RowVersion))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProgramVersionDto> CreateVersionAsync(
        ProgramVersion version,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(version);

        _dbContext.ProgramVersions.Add(version);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await GetVersionByIdAsync(version.Id, cancellationToken))!;
    }

    public async Task<ProgramVersionDto> PublishVersionAsync(
        Guid versionId,
        CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.ProgramVersions.FindAsync([versionId], cancellationToken)
            ?? throw new NotFoundException(nameof(ProgramVersion), versionId);

        existing.Publish();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await GetVersionByIdAsync(versionId, cancellationToken))!;
    }
}
