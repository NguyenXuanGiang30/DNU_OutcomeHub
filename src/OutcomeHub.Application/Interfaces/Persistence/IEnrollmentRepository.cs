using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Measurement;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IEnrollmentRepository
{
    Task<PagedResult<EnrollmentDto>> GetPagedEnrollmentsAsync(
        PagedRequest request,
        Guid? courseOfferingId,
        Guid? studentId,
        CancellationToken cancellationToken = default);

    Task<EnrollmentDto?> GetEnrollmentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Enrollment> CreateEnrollmentAsync(
        Enrollment enrollment,
        EnrollmentRevision initialRevision,
        CancellationToken cancellationToken = default);
}
