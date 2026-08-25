using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IStudentRepository
{
    Task<PagedResult<StudentDto>> GetPagedStudentsAsync(
        PagedRequest request,
        Guid? admissionCohortId,
        Guid? programId,
        CancellationToken cancellationToken = default);

    Task<StudentDto?> GetStudentByIdAsync(Guid personId, CancellationToken cancellationToken = default);

    Task<Student> CreateStudentAsync(
        Person person,
        Student student,
        CancellationToken cancellationToken = default);

    Task<Student> UpdateStudentAsync(
        Guid personId,
        string fullName,
        string currentStatus,
        DateOnly? effectiveTo,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StudentPathDto>> GetStudentPathsAsync(
        Guid studentId,
        CancellationToken cancellationToken = default);

    Task<StudentPath> AssignStudentPathAsync(
        StudentPath studentPath,
        CancellationToken cancellationToken = default);
}
