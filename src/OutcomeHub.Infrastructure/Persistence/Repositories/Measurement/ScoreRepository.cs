using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Measurement;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Measurement;

public sealed class ScoreRepository : IScoreRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public ScoreRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PagedResult<ScoreRecordDto>> GetPagedScoresAsync(
        PagedRequest request,
        Guid? courseOfferingId,
        Guid? studentId,
        Guid? assessmentItemId,
        short? academicYearStart,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ScoreRecords
            .AsNoTracking()
            .Include(r => r.Student)
                .ThenInclude(s => s.Person)
            .Include(r => r.CourseOffering)
            .Include(r => r.ScoreIdentity)
                .ThenInclude(i => i.AssessmentItem)
            .Include(r => r.ScoreIdentity)
                .ThenInclude(i => i.RubricCriterion)
            .AsQueryable();

        if (courseOfferingId.HasValue)
        {
            query = query.Where(r => r.CourseOfferingId == courseOfferingId.Value);
        }

        if (studentId.HasValue)
        {
            query = query.Where(r => r.StudentId == studentId.Value);
        }

        if (assessmentItemId.HasValue)
        {
            query = query.Where(r => r.ScoreIdentity.AssessmentItemId == assessmentItemId.Value);
        }

        if (academicYearStart.HasValue)
        {
            query = query.Where(r => r.AcademicYearStart == academicYearStart.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var pattern = $"%{request.SearchTerm.Trim()}%";
            query = query.Where(r =>
                EF.Functions.ILike(r.Student.StudentCode, pattern) ||
                EF.Functions.ILike(r.Student.Person.FullName, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.RecordedAt)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .Select(r => new ScoreRecordDto(
                r.Id,
                r.AcademicYearStart,
                r.ScoreIdentityId,
                r.StudentId,
                r.Student.StudentCode,
                r.Student.Person.FullName,
                r.CourseOfferingId,
                r.CourseOffering.Code,
                r.ScoreIdentity.AssessmentItemId,
                r.ScoreIdentity.AssessmentItem.Name,
                r.ScoreIdentity.RubricCriterionId,
                r.ScoreIdentity.RubricCriterion != null ? r.ScoreIdentity.RubricCriterion.CriterionCode : null,
                r.RevisionNo,
                r.RawScore,
                r.MaxScore,
                r.ScoreStatus,
                r.RecordedBy,
                r.RecordedAt,
                r.Checksum,
                r.CorrectionReason))
            .ToListAsync(cancellationToken);

        return PagedResult.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<ScoreRecord> SubmitScoreRecordAsync(
        ScoreIdentity identity,
        ScoreRecord record,
        CancellationToken cancellationToken = default)
    {
        var existingIdentity = await _dbContext.Set<ScoreIdentity>()
            .FirstOrDefaultAsync(i => i.Id == identity.Id, cancellationToken);

        if (existingIdentity == null)
        {
            await _dbContext.Set<ScoreIdentity>().AddAsync(identity, cancellationToken);
        }

        await _dbContext.ScoreRecords.AddAsync(record, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return record;
    }
}
