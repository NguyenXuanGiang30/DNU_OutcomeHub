using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Academic;

public sealed class StudentRepository : IStudentRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public StudentRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PagedResult<StudentDto>> GetPagedStudentsAsync(
        PagedRequest request,
        Guid? admissionCohortId,
        Guid? programId,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Students
            .AsNoTracking()
            .Include(s => s.Person)
            .Include(s => s.AdmissionCohort)
                .ThenInclude(c => c.Program)
            .AsQueryable();

        if (admissionCohortId.HasValue)
        {
            query = query.Where(s => s.AdmissionCohortId == admissionCohortId.Value);
        }

        if (programId.HasValue)
        {
            query = query.Where(s => s.AdmissionCohort.ProgramId == programId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var pattern = $"%{request.SearchTerm.Trim()}%";
            query = query.Where(s =>
                EF.Functions.ILike(s.StudentCode, pattern) ||
                EF.Functions.ILike(s.Person.FullName, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.StudentCode)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .Select(s => new StudentDto(
                s.PersonId,
                s.StudentCode,
                s.Person.FullName,
                s.AdmissionCohortId,
                s.AdmissionCohort.Code,
                s.AdmissionCohort.Name,
                s.CurrentStatus,
                s.Person.EffectiveFrom,
                s.Person.EffectiveTo,
                null))
            .ToListAsync(cancellationToken);

        return PagedResult.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<StudentDto?> GetStudentByIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        var student = await _dbContext.Students
            .AsNoTracking()
            .Include(s => s.Person)
            .Include(s => s.AdmissionCohort)
            .FirstOrDefaultAsync(s => s.PersonId == personId, cancellationToken);

        if (student == null)
        {
            return null;
        }

        var paths = await _dbContext.StudentPaths
            .AsNoTracking()
            .Include(p => p.Program)
            .Include(p => p.ProgramVersion)
            .Where(p => p.StudentId == personId)
            .OrderByDescending(p => p.IsPrimary)
            .ThenBy(p => p.EffectiveFrom)
            .Select(p => new StudentPathDto(
                p.Id,
                p.StudentId,
                p.ProgramId,
                p.Program.Code,
                p.Program.Name,
                p.ProgramVersionId,
                p.ProgramVersion.Code,
                p.CurriculumPathId,
                p.PathStatus,
                p.EffectiveFrom,
                p.EffectiveTo,
                p.DecisionId,
                p.IsPrimary))
            .ToListAsync(cancellationToken);

        return new StudentDto(
            student.PersonId,
            student.StudentCode,
            student.Person.FullName,
            student.AdmissionCohortId,
            student.AdmissionCohort.Code,
            student.AdmissionCohort.Name,
            student.CurrentStatus,
            student.Person.EffectiveFrom,
            student.Person.EffectiveTo,
            paths);
    }

    public async Task<Student> CreateStudentAsync(
        Person person,
        Student student,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Persons.AddAsync(person, cancellationToken);
        await _dbContext.Students.AddAsync(student, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return student;
    }

    public async Task<Student> UpdateStudentAsync(
        Guid personId,
        string fullName,
        string currentStatus,
        DateOnly? effectiveTo,
        CancellationToken cancellationToken = default)
    {
        var student = await _dbContext.Students
            .Include(s => s.Person)
            .FirstOrDefaultAsync(s => s.PersonId == personId, cancellationToken);

        if (student == null)
        {
            throw new NotFoundException("Student", personId);
        }

        student.Person.Update(fullName, currentStatus, effectiveTo);
        student.Update(currentStatus);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return student;
    }

    public async Task<IReadOnlyList<StudentPathDto>> GetStudentPathsAsync(
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.StudentPaths
            .AsNoTracking()
            .Include(p => p.Program)
            .Include(p => p.ProgramVersion)
            .Where(p => p.StudentId == studentId)
            .OrderByDescending(p => p.IsPrimary)
            .ThenBy(p => p.EffectiveFrom)
            .Select(p => new StudentPathDto(
                p.Id,
                p.StudentId,
                p.ProgramId,
                p.Program.Code,
                p.Program.Name,
                p.ProgramVersionId,
                p.ProgramVersion.Code,
                p.CurriculumPathId,
                p.PathStatus,
                p.EffectiveFrom,
                p.EffectiveTo,
                p.DecisionId,
                p.IsPrimary))
            .ToListAsync(cancellationToken);
    }

    public async Task<StudentPath> AssignStudentPathAsync(
        StudentPath studentPath,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.StudentPaths.AddAsync(studentPath, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return studentPath;
    }
}
