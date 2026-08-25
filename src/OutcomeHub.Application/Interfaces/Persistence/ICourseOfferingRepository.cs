using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface ICourseOfferingRepository
{
    Task<PagedResult<CourseOfferingDto>> GetPagedOfferingsAsync(
        PagedRequest request,
        Guid? programVersionId,
        short? academicYearStart,
        string? termCode,
        Guid? orgUnitId,
        CancellationToken cancellationToken = default);

    Task<CourseOfferingDto?> GetOfferingByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CourseOffering> CreateOfferingAsync(
        CourseOffering offering,
        CancellationToken cancellationToken = default);

    Task<CourseOffering> UpdateOfferingAsync(
        Guid id,
        string status,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CourseOfferingInstructorDto>> GetOfferingInstructorsAsync(
        Guid courseOfferingId,
        CancellationToken cancellationToken = default);

    Task<CourseOfferingInstructor> AssignInstructorAsync(
        CourseOfferingInstructor instructor,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveInstructorAsync(
        Guid instructorAssignmentId,
        CancellationToken cancellationToken = default);
}
