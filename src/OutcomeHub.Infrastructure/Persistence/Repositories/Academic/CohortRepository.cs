using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Academic;

public sealed class CohortRepository : ICohortRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public CohortRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PagedResult<CohortDto>> GetPagedCohortsAsync(
        PagedRequest request,
        Guid? programId,
        int? admissionYear,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Cohorts
            .AsNoTracking()
            .Include(c => c.Program)
            .AsQueryable();

        if (programId.HasValue)
        {
            query = query.Where(c => c.ProgramId == programId.Value);
        }

        if (admissionYear.HasValue)
        {
            query = query.Where(c => c.AdmissionYear == admissionYear.Value);
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
            .OrderByDescending(c => c.AdmissionYear)
            .ThenBy(c => c.Code)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .Select(c => new CohortDto(
                c.Id,
                c.ProgramId,
                c.Program.Code,
                c.Program.Name,
                c.Code,
                c.Name,
                c.AdmissionYear,
                c.StartDate,
                c.EndDate))
            .ToListAsync(cancellationToken);

        return PagedResult.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<CohortDto?> GetCohortByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cohort = await _dbContext.Cohorts
            .AsNoTracking()
            .Include(c => c.Program)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (cohort == null)
        {
            return null;
        }

        return new CohortDto(
            cohort.Id,
            cohort.ProgramId,
            cohort.Program.Code,
            cohort.Program.Name,
            cohort.Code,
            cohort.Name,
            cohort.AdmissionYear,
            cohort.StartDate,
            cohort.EndDate);
    }

    public async Task<Cohort> CreateCohortAsync(Cohort cohort, CancellationToken cancellationToken = default)
    {
        await _dbContext.Cohorts.AddAsync(cohort, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return cohort;
    }

    public async Task<Cohort> UpdateCohortAsync(
        Guid id,
        string name,
        DateOnly? endDate,
        CancellationToken cancellationToken = default)
    {
        var cohort = await _dbContext.Cohorts
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (cohort == null)
        {
            throw new NotFoundException("Cohort", id);
        }

        cohort.Update(name, endDate);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return cohort;
    }
}
