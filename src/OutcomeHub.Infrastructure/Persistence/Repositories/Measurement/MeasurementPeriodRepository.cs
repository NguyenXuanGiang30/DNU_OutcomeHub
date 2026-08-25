using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Measurement;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Measurement;

public sealed class MeasurementPeriodRepository : IMeasurementPeriodRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public MeasurementPeriodRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PagedResult<MeasurementPeriodDto>> GetPagedPeriodsAsync(
        PagedRequest request,
        Guid? orgUnitId,
        Guid? programVersionId,
        short? academicYearStart,
        string? status,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.MeasurementPeriods
            .AsNoTracking()
            .Include(p => p.OrgUnit)
            .Include(p => p.ProgramVersion)
            .AsQueryable();

        if (orgUnitId.HasValue)
        {
            query = query.Where(p => p.OrgUnitId == orgUnitId.Value);
        }

        if (programVersionId.HasValue)
        {
            query = query.Where(p => p.ProgramVersionId == programVersionId.Value);
        }

        if (academicYearStart.HasValue)
        {
            query = query.Where(p => p.AcademicYearStart == academicYearStart.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalized = status.Trim().ToUpperInvariant();
            query = query.Where(p => p.Status == normalized);
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
            .OrderByDescending(p => p.AcademicYearStart)
            .ThenBy(p => p.TermCode)
            .ThenBy(p => p.Code)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .Select(p => new MeasurementPeriodDto(
                p.Id,
                p.Code,
                p.Name,
                p.OrgUnitId,
                p.OrgUnit.Name,
                p.ProgramVersionId,
                p.ProgramVersion.Code,
                p.AcademicYearStart,
                p.TermCode,
                p.Status,
                p.ProgramPolicyBindingId,
                p.WorkflowInstanceId,
                p.CollectionOpenAt,
                p.CollectionCloseAt,
                p.DataCutoffAt,
                null,
                null,
                null))
            .ToListAsync(cancellationToken);

        return PagedResult.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<MeasurementPeriodDto?> GetPeriodByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var period = await _dbContext.MeasurementPeriods
            .AsNoTracking()
            .Include(p => p.OrgUnit)
            .Include(p => p.ProgramVersion)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (period == null)
        {
            return null;
        }

        var cohorts = await _dbContext.MeasurementPeriodCohorts
            .AsNoTracking()
            .Include(c => c.Cohort)
            .Where(c => c.MeasurementPeriodId == id)
            .OrderBy(c => c.Cohort.Code)
            .Select(c => new MeasurementPeriodCohortDto(
                c.MeasurementPeriodId,
                c.ProgramVersionId,
                c.CohortId,
                c.Cohort.Code,
                c.Cohort.Name))
            .ToListAsync(cancellationToken);

        var offerings = await _dbContext.MeasurementPeriodOfferings
            .AsNoTracking()
            .Include(o => o.CourseOffering)
                .ThenInclude(co => co.CourseVersion)
            .Where(o => o.MeasurementPeriodId == id)
            .OrderBy(o => o.CourseOffering.Code)
            .Select(o => new MeasurementPeriodOfferingDto(
                o.MeasurementPeriodId,
                o.ProgramVersionId,
                o.AcademicYearStart,
                o.CourseOfferingId,
                o.CourseOffering.Code,
                o.CourseOffering.CourseVersion.Name,
                o.PlannedSourceRole,
                o.CollectionStatus,
                o.DueAt))
            .ToListAsync(cancellationToken);

        var targets = await _dbContext.MeasurementPeriodTargets
            .AsNoTracking()
            .Include(t => t.CourseOffering)
            .Include(t => t.Clo)
            .Include(t => t.ProgramPi)
            .Include(t => t.ProgramPlo)
            .Where(t => t.MeasurementPeriodId == id)
            .OrderBy(t => t.OutcomeLevel)
            .Select(t => new MeasurementPeriodTargetDto(
                t.Id,
                t.MeasurementPeriodId,
                t.ProgramVersionId,
                t.OutcomeLevel,
                t.TargetRole,
                t.CourseOfferingId,
                t.CourseOffering != null ? t.CourseOffering.Code : null,
                t.SyllabusVersionId,
                t.CloId,
                t.Clo != null ? t.Clo.Code : null,
                t.ProgramPiId,
                t.ProgramPi != null ? t.ProgramPi.Code : null,
                t.ProgramPloId,
                t.ProgramPlo != null ? t.ProgramPlo.Code : null))
            .ToListAsync(cancellationToken);

        return new MeasurementPeriodDto(
            period.Id,
            period.Code,
            period.Name,
            period.OrgUnitId,
            period.OrgUnit.Name,
            period.ProgramVersionId,
            period.ProgramVersion.Code,
            period.AcademicYearStart,
            period.TermCode,
            period.Status,
            period.ProgramPolicyBindingId,
            period.WorkflowInstanceId,
            period.CollectionOpenAt,
            period.CollectionCloseAt,
            period.DataCutoffAt,
            cohorts,
            offerings,
            targets);
    }

    public async Task<MeasurementPeriod> CreatePeriodAsync(
        MeasurementPeriod period,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.MeasurementPeriods.AddAsync(period, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return period;
    }

    public async Task<MeasurementPeriod> UpdatePeriodAsync(
        Guid id,
        string name,
        string status,
        DateTimeOffset? collectionOpenAt,
        DateTimeOffset? collectionCloseAt,
        DateTimeOffset? dataCutoffAt,
        CancellationToken cancellationToken = default)
    {
        var period = await _dbContext.MeasurementPeriods
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (period == null)
        {
            throw new NotFoundException("MeasurementPeriod", id);
        }

        period.Update(name, status, collectionOpenAt, collectionCloseAt, dataCutoffAt);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return period;
    }

    public async Task<MeasurementPeriodCohort> AttachCohortAsync(
        MeasurementPeriodCohort cohort,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.MeasurementPeriodCohorts.AddAsync(cohort, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return cohort;
    }

    public async Task<MeasurementPeriodOffering> AttachOfferingAsync(
        MeasurementPeriodOffering offering,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.MeasurementPeriodOfferings.AddAsync(offering, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return offering;
    }

    public async Task<MeasurementPeriodTarget> CreateTargetAsync(
        MeasurementPeriodTarget target,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.MeasurementPeriodTargets.AddAsync(target, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return target;
    }
}
