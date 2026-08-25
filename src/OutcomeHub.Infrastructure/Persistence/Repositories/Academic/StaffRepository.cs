using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Academic;

public sealed class StaffRepository : IStaffRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public StaffRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PagedResult<StaffDto>> GetPagedStaffAsync(
        PagedRequest request,
        Guid? homeOrgUnitId,
        string? staffType,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Staff
            .AsNoTracking()
            .Include(s => s.Person)
            .Include(s => s.HomeOrgUnit)
            .AsQueryable();

        if (homeOrgUnitId.HasValue)
        {
            query = query.Where(s => s.HomeOrgUnitId == homeOrgUnitId.Value);
        }

        if (!string.IsNullOrWhiteSpace(staffType))
        {
            var normalizedType = staffType.Trim().ToUpperInvariant();
            query = query.Where(s => s.StaffType == normalizedType);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var pattern = $"%{request.SearchTerm.Trim()}%";
            query = query.Where(s =>
                EF.Functions.ILike(s.StaffCode, pattern) ||
                EF.Functions.ILike(s.Person.FullName, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.StaffCode)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .Select(s => new StaffDto(
                s.PersonId,
                s.StaffCode,
                s.Person.FullName,
                s.HomeOrgUnitId,
                s.HomeOrgUnit.Name,
                s.StaffType,
                s.CurrentStatus,
                s.Person.EffectiveFrom,
                s.Person.EffectiveTo))
            .ToListAsync(cancellationToken);

        return PagedResult.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<StaffDto?> GetStaffByIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        var staff = await _dbContext.Staff
            .AsNoTracking()
            .Include(s => s.Person)
            .Include(s => s.HomeOrgUnit)
            .FirstOrDefaultAsync(s => s.PersonId == personId, cancellationToken);

        if (staff == null)
        {
            return null;
        }

        return new StaffDto(
            staff.PersonId,
            staff.StaffCode,
            staff.Person.FullName,
            staff.HomeOrgUnitId,
            staff.HomeOrgUnit.Name,
            staff.StaffType,
            staff.CurrentStatus,
            staff.Person.EffectiveFrom,
            staff.Person.EffectiveTo);
    }

    public async Task<Staff> CreateStaffAsync(
        Person person,
        Staff staff,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Persons.AddAsync(person, cancellationToken);
        await _dbContext.Staff.AddAsync(staff, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return staff;
    }

    public async Task<Staff> UpdateStaffAsync(
        Guid personId,
        string fullName,
        Guid homeOrgUnitId,
        string staffType,
        string currentStatus,
        DateOnly? effectiveTo,
        CancellationToken cancellationToken = default)
    {
        var staff = await _dbContext.Staff
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.PersonId == personId, cancellationToken);

        if (staff == null)
        {
            throw new NotFoundException("Staff", personId);
        }

        staff.Person.Update(fullName, currentStatus, effectiveTo);
        staff.Update(homeOrgUnitId, staffType, currentStatus);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return staff;
    }
}
