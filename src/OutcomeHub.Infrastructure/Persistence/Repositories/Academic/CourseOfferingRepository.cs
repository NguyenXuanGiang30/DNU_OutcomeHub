using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Academic;

public sealed class CourseOfferingRepository : ICourseOfferingRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public CourseOfferingRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PagedResult<CourseOfferingDto>> GetPagedOfferingsAsync(
        PagedRequest request,
        Guid? programVersionId,
        short? academicYearStart,
        string? termCode,
        Guid? orgUnitId,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.CourseOfferings
            .AsNoTracking()
            .Include(o => o.CourseVersion)
            .Include(o => o.OrgUnit)
            .AsQueryable();

        if (programVersionId.HasValue)
        {
            query = query.Where(o => o.ProgramVersionId == programVersionId.Value);
        }

        if (academicYearStart.HasValue)
        {
            query = query.Where(o => o.AcademicYearStart == academicYearStart.Value);
        }

        if (!string.IsNullOrWhiteSpace(termCode))
        {
            var normalizedTerm = termCode.Trim().ToUpperInvariant();
            query = query.Where(o => o.TermCode == normalizedTerm);
        }

        if (orgUnitId.HasValue)
        {
            query = query.Where(o => o.OrgUnitId == orgUnitId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var pattern = $"%{request.SearchTerm.Trim()}%";
            query = query.Where(o =>
                EF.Functions.ILike(o.Code, pattern) ||
                EF.Functions.ILike(o.CourseVersion.Name, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.AcademicYearStart)
            .ThenBy(o => o.TermCode)
            .ThenBy(o => o.Code)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .Select(o => new CourseOfferingDto(
                o.Id,
                o.Code,
                o.ProgramCourseId,
                o.CourseVersionId,
                o.CourseVersion.Name,
                o.ProgramVersionId,
                o.SyllabusVersionId,
                o.AcademicYearStart,
                o.TermCode,
                o.OrgUnitId,
                o.OrgUnit.Name,
                o.Status,
                o.StartDate,
                o.EndDate,
                null))
            .ToListAsync(cancellationToken);

        return PagedResult.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<CourseOfferingDto?> GetOfferingByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var offering = await _dbContext.CourseOfferings
            .AsNoTracking()
            .Include(o => o.CourseVersion)
            .Include(o => o.OrgUnit)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (offering == null)
        {
            return null;
        }

        var instructors = await _dbContext.CourseOfferingInstructors
            .AsNoTracking()
            .Include(i => i.Staff)
                .ThenInclude(s => s.Person)
            .Where(i => i.CourseOfferingId == id)
            .OrderByDescending(i => i.IsPrimary)
            .ThenBy(i => i.EffectiveFrom)
            .Select(i => new CourseOfferingInstructorDto(
                i.Id,
                i.CourseOfferingId,
                i.StaffId,
                i.Staff.StaffCode,
                i.Staff.Person.FullName,
                i.AssignmentRole,
                i.EffectiveFrom,
                i.EffectiveTo,
                i.IsPrimary))
            .ToListAsync(cancellationToken);

        return new CourseOfferingDto(
            offering.Id,
            offering.Code,
            offering.ProgramCourseId,
            offering.CourseVersionId,
            offering.CourseVersion.Name,
            offering.ProgramVersionId,
            offering.SyllabusVersionId,
            offering.AcademicYearStart,
            offering.TermCode,
            offering.OrgUnitId,
            offering.OrgUnit.Name,
            offering.Status,
            offering.StartDate,
            offering.EndDate,
            instructors);
    }

    public async Task<CourseOffering> CreateOfferingAsync(
        CourseOffering offering,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.CourseOfferings.AddAsync(offering, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return offering;
    }

    public async Task<CourseOffering> UpdateOfferingAsync(
        Guid id,
        string status,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        var offering = await _dbContext.CourseOfferings
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (offering == null)
        {
            throw new NotFoundException("CourseOffering", id);
        }

        offering.Update(status, startDate, endDate);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return offering;
    }

    public async Task<IReadOnlyList<CourseOfferingInstructorDto>> GetOfferingInstructorsAsync(
        Guid courseOfferingId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.CourseOfferingInstructors
            .AsNoTracking()
            .Include(i => i.Staff)
                .ThenInclude(s => s.Person)
            .Where(i => i.CourseOfferingId == courseOfferingId)
            .OrderByDescending(i => i.IsPrimary)
            .ThenBy(i => i.EffectiveFrom)
            .Select(i => new CourseOfferingInstructorDto(
                i.Id,
                i.CourseOfferingId,
                i.StaffId,
                i.Staff.StaffCode,
                i.Staff.Person.FullName,
                i.AssignmentRole,
                i.EffectiveFrom,
                i.EffectiveTo,
                i.IsPrimary))
            .ToListAsync(cancellationToken);
    }

    public async Task<CourseOfferingInstructor> AssignInstructorAsync(
        CourseOfferingInstructor instructor,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.CourseOfferingInstructors.AddAsync(instructor, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return instructor;
    }

    public async Task<bool> RemoveInstructorAsync(
        Guid instructorAssignmentId,
        CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.CourseOfferingInstructors
            .FirstOrDefaultAsync(i => i.Id == instructorAssignmentId, cancellationToken);

        if (item == null)
        {
            return false;
        }

        _dbContext.CourseOfferingInstructors.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
