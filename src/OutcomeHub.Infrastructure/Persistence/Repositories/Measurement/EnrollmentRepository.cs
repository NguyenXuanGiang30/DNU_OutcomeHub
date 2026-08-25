using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Measurement;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Measurement;

public sealed class EnrollmentRepository : IEnrollmentRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public EnrollmentRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PagedResult<EnrollmentDto>> GetPagedEnrollmentsAsync(
        PagedRequest request,
        Guid? courseOfferingId,
        Guid? studentId,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Enrollments
            .AsNoTracking()
            .Include(e => e.CourseOffering)
            .Include(e => e.Student)
                .ThenInclude(s => s.Person)
            .Include(e => e.Revisions)
            .AsQueryable();

        if (courseOfferingId.HasValue)
        {
            query = query.Where(e => e.CourseOfferingId == courseOfferingId.Value);
        }

        if (studentId.HasValue)
        {
            query = query.Where(e => e.StudentId == studentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var pattern = $"%{request.SearchTerm.Trim()}%";
            query = query.Where(e =>
                EF.Functions.ILike(e.Student.StudentCode, pattern) ||
                EF.Functions.ILike(e.Student.Person.FullName, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(e => e.Student.StudentCode)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .Select(e => new EnrollmentDto(
                e.Id,
                e.CourseOfferingId,
                e.CourseOffering.Code,
                e.StudentId,
                e.Student.StudentCode,
                e.Student.Person.FullName,
                e.AttemptNo,
                e.SourceSystemId,
                e.SourceRecordId,
                e.Revisions.OrderByDescending(r => r.RevisionNo).Select(r => r.EnrollmentStatus).FirstOrDefault(),
                e.Revisions.OrderByDescending(r => r.RevisionNo).Select(r => r.RepeatFlag).FirstOrDefault(),
                e.Revisions.OrderByDescending(r => r.RevisionNo).Select(r => r.ImprovementFlag).FirstOrDefault(),
                e.Revisions.OrderByDescending(r => r.RevisionNo).Select(r => (DateTimeOffset?)r.EffectiveFrom).FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return PagedResult.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<EnrollmentDto?> GetEnrollmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var enrollment = await _dbContext.Enrollments
            .AsNoTracking()
            .Include(e => e.CourseOffering)
            .Include(e => e.Student)
                .ThenInclude(s => s.Person)
            .Include(e => e.Revisions)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (enrollment == null)
        {
            return null;
        }

        var latestRev = enrollment.Revisions.OrderByDescending(r => r.RevisionNo).FirstOrDefault();

        return new EnrollmentDto(
            enrollment.Id,
            enrollment.CourseOfferingId,
            enrollment.CourseOffering.Code,
            enrollment.StudentId,
            enrollment.Student.StudentCode,
            enrollment.Student.Person.FullName,
            enrollment.AttemptNo,
            enrollment.SourceSystemId,
            enrollment.SourceRecordId,
            latestRev?.EnrollmentStatus,
            latestRev?.RepeatFlag ?? false,
            latestRev?.ImprovementFlag ?? false,
            latestRev?.EffectiveFrom);
    }

    public async Task<Enrollment> CreateEnrollmentAsync(
        Enrollment enrollment,
        EnrollmentRevision initialRevision,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Enrollments.AddAsync(enrollment, cancellationToken);
        await _dbContext.Set<EnrollmentRevision>().AddAsync(initialRevision, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return enrollment;
    }
}
