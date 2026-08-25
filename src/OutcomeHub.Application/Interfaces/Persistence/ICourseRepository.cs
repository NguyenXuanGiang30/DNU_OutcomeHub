using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface ICourseRepository
{
    Task<PagedResult<CourseDto>> GetPagedCoursesAsync(
        PagedRequest request,
        Guid? ownerOrgUnitId,
        CancellationToken cancellationToken);

    Task<Course?> GetCourseByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken);

    Task<IReadOnlyList<CourseVersionDto>> GetCourseVersionsAsync(
        Guid courseId,
        CancellationToken cancellationToken);

    Task<CourseVersion> CreateCourseVersionAsync(
        CourseVersion courseVersion,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ProgramCourseDto>> GetProgramCoursesAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<ProgramCourse> AddCourseToProgramAsync(
        ProgramCourse programCourse,
        CancellationToken cancellationToken);
}
