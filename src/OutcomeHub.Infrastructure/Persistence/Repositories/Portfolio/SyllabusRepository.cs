using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Portfolio;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Portfolio;

public sealed class SyllabusRepository : ISyllabusRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public SyllabusRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PagedResult<SyllabusDto>> GetPagedSyllabusesAsync(
        PagedRequest request,
        Guid? programCourseId,
        Guid? ownerOrgUnitId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = _dbContext.Syllabuses
            .AsNoTracking()
            .AsQueryable();

        if (programCourseId.HasValue)
        {
            query = query.Where(s => s.ProgramCourseId == programCourseId.Value);
        }

        if (ownerOrgUnitId.HasValue)
        {
            query = query.Where(s => s.OwnerOrgUnitId == ownerOrgUnitId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var pattern = $"%{request.SearchTerm.Trim()}%";
            query = query.Where(s => EF.Functions.ILike(s.Code, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.Code)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .Select(s => new SyllabusDto(
                s.Id,
                s.ProgramCourseId,
                s.Code,
                s.OwnerOrgUnitId,
                s.CreatedAt,
                s.Versions.Count))
            .ToListAsync(cancellationToken);

        return PagedResult.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<Syllabus?> GetSyllabusByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Syllabuses
            .AsNoTracking()
            .Include(s => s.Versions)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Syllabus> CreateSyllabusAsync(Syllabus syllabus, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(syllabus);

        await _dbContext.Syllabuses.AddAsync(syllabus, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return syllabus;
    }

    public async Task<IReadOnlyList<SyllabusVersionDto>> GetSyllabusVersionsAsync(
        Guid syllabusId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.SyllabusVersions
            .AsNoTracking()
            .Where(v => v.SyllabusId == syllabusId)
            .OrderByDescending(v => v.VersionNo)
            .Select(v => new SyllabusVersionDto(
                v.Id,
                v.SyllabusId,
                v.ProgramCourseId,
                v.ProgramVersionId,
                v.InstitutionTemplateVersionId,
                v.CourseVersionId,
                v.SyllabusTemplateVersionId,
                v.VersionNo,
                v.ApplicableFrom,
                v.ApplicableTo,
                v.Status,
                v.ContentChecksum))
            .ToListAsync(cancellationToken);
    }

    public async Task<SyllabusVersion?> GetSyllabusVersionByIdAsync(
        Guid versionId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.SyllabusVersions
            .AsNoTracking()
            .Include(v => v.Syllabus)
            .Include(v => v.CourseVersion)
            .FirstOrDefaultAsync(v => v.Id == versionId, cancellationToken);
    }

    public async Task<SyllabusVersion> CreateSyllabusVersionAsync(
        SyllabusVersion syllabusVersion,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(syllabusVersion);

        await _dbContext.SyllabusVersions.AddAsync(syllabusVersion, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return syllabusVersion;
    }
}
