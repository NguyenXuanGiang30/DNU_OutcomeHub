using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface ICohortRepository
{
    Task<PagedResult<CohortDto>> GetPagedCohortsAsync(
        PagedRequest request,
        Guid? programId,
        int? admissionYear,
        CancellationToken cancellationToken = default);

    Task<CohortDto?> GetCohortByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Cohort> CreateCohortAsync(Cohort cohort, CancellationToken cancellationToken = default);

    Task<Cohort> UpdateCohortAsync(
        Guid id,
        string name,
        DateOnly? endDate,
        CancellationToken cancellationToken = default);
}
