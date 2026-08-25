using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Academic;

public sealed class CourseRepository : ICourseRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public CourseRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PagedResult<CourseDto>> GetPagedCoursesAsync(
        PagedRequest request,
        Guid? ownerOrgUnitId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = _dbContext.Courses
            .AsNoTracking()
            .Include(c => c.OwnerOrgUnit)
            .AsQueryable();

        if (ownerOrgUnitId.HasValue)
        {
            query = query.Where(c => c.OwnerOrgUnitId == ownerOrgUnitId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var pattern = $"%{request.SearchTerm.Trim()}%";
            query = query.Where(c =>
                EF.Functions.ILike(c.Code, pattern) ||
                EF.Functions.ILike(c.Name, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.Code)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .Select(c => new CourseDto(
                c.Id,
                c.Code,
                c.Name,
                c.OwnerOrgUnitId,
                c.OwnerOrgUnit.Name,
                c.Status))
            .ToListAsync(cancellationToken);

        return PagedResult.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<Course?> GetCourseByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Courses
            .AsNoTracking()
            .Include(c => c.OwnerOrgUnit)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(course);

        await _dbContext.Courses.AddAsync(course, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return course;
    }

    public async Task<IReadOnlyList<CourseVersionDto>> GetCourseVersionsAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.CourseVersions
            .AsNoTracking()
            .Where(v => v.CourseId == courseId)
            .OrderByDescending(v => v.VersionNo)
            .Select(v => new CourseVersionDto(
                v.Id,
                v.CourseId,
                v.VersionNo,
                v.Name,
                v.CreditValue,
                v.CourseType,
                v.EffectiveFrom,
                v.EffectiveTo,
                v.SharedCoreFlag,
                v.Status,
                v.DecisionId,
                v.WorkflowInstanceId,
                v.SupersedesId,
                v.Checksum))
            .ToListAsync(cancellationToken);
    }

    public async Task<CourseVersion> CreateCourseVersionAsync(
        CourseVersion courseVersion,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(courseVersion);

        await _dbContext.CourseVersions.AddAsync(courseVersion, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return courseVersion;
    }

    public async Task<IReadOnlyList<ProgramCourseDto>> GetProgramCoursesAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ProgramCourses
            .AsNoTracking()
            .Where(pc => pc.ProgramVersionId == programVersionId)
            .Include(pc => pc.CourseVersion)
                .ThenInclude(cv => cv.Course)
            .OrderBy(pc => pc.CourseVersion.Course.Code)
            .Select(pc => new ProgramCourseDto(
                pc.Id,
                pc.ProgramVersionId,
                pc.CourseVersionId,
                pc.CourseVersion.Course.Code,
                pc.CourseVersion.Name,
                pc.CurriculumBlockId,
                pc.CatalogRole,
                pc.CourseVersion.CreditValue,
                pc.CreditOverride,
                pc.IsLocked,
                pc.Status))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProgramCourse> AddCourseToProgramAsync(
        ProgramCourse programCourse,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(programCourse);

        await _dbContext.ProgramCourses.AddAsync(programCourse, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return programCourse;
    }
}
