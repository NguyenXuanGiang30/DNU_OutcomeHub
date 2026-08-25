using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.DTOs.Portfolio;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Portfolio;

public sealed class CloRepository : ICloRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public CloRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IReadOnlyList<CloDto>> GetClosBySyllabusVersionIdAsync(
        Guid syllabusVersionId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Clos
            .AsNoTracking()
            .Where(c => c.SyllabusVersionId == syllabusVersionId)
            .OrderBy(c => c.SortOrder)
            .Select(c => new CloDto(
                c.Id,
                c.SyllabusVersionId,
                c.Code,
                c.Description,
                c.Domain,
                c.BloomLevel,
                c.IsCore,
                c.SortOrder))
            .ToListAsync(cancellationToken);
    }

    public async Task<Clo> CreateCloAsync(Clo clo, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(clo);

        await _dbContext.Clos.AddAsync(clo, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return clo;
    }

    public async Task<Clo> UpdateCloAsync(
        Guid id,
        string description,
        string domain,
        string bloomLevel,
        bool isCore,
        int sortOrder,
        CancellationToken cancellationToken)
    {
        var existing = await _dbContext.Clos.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"CLO with ID '{id}' not found.");

        existing.Update(description, domain, bloomLevel, isCore, sortOrder);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteCloAsync(Guid id, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.Clos.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (existing == null)
        {
            return false;
        }

        _dbContext.Clos.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<CoursePiMappingDto>> GetCoursePiMappingsAsync(
        Guid programVersionId,
        Guid? programCourseId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<CoursePiMapping>()
            .AsNoTracking()
            .Where(m => m.ProgramVersionId == programVersionId);

        if (programCourseId.HasValue)
        {
            query = query.Where(m => m.ProgramCourseId == programCourseId.Value);
        }

        return await query
            .Include(m => m.ProgramCourse)
                .ThenInclude(pc => pc.CourseVersion)
                    .ThenInclude(cv => cv.Course)
            .Include(m => m.ProgramPi)
            .OrderBy(m => m.ProgramCourse.CourseVersion.Course.Code)
            .ThenBy(m => m.ProgramPi.Code)
            .Select(m => new CoursePiMappingDto(
                m.Id,
                m.ProgramVersionId,
                m.ProgramCourseId,
                m.ProgramCourse.CourseVersion.Course.Code,
                m.ProgramCourse.CourseVersion.Name,
                m.ProgramPiId,
                m.ProgramPi.Code,
                m.ContributionLevel,
                m.IsDirectAssessment,
                m.Rationale,
                m.SourceType,
                m.IsLocked))
            .ToListAsync(cancellationToken);
    }

    public async Task<CoursePiMapping> SetCoursePiMappingAsync(
        CoursePiMapping mapping,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(mapping);

        var existing = await _dbContext.Set<CoursePiMapping>()
            .FirstOrDefaultAsync(m =>
                m.ProgramVersionId == mapping.ProgramVersionId &&
                m.ProgramCourseId == mapping.ProgramCourseId &&
                m.ProgramPiId == mapping.ProgramPiId,
                cancellationToken);

        if (existing != null)
        {
            _dbContext.Set<CoursePiMapping>().Remove(existing);
        }

        await _dbContext.Set<CoursePiMapping>().AddAsync(mapping, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return mapping;
    }
}
