using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Measurement;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IMeasurementPeriodRepository
{
    Task<PagedResult<MeasurementPeriodDto>> GetPagedPeriodsAsync(
        PagedRequest request,
        Guid? orgUnitId,
        Guid? programVersionId,
        short? academicYearStart,
        string? status,
        CancellationToken cancellationToken = default);

    Task<MeasurementPeriodDto?> GetPeriodByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<MeasurementPeriod> CreatePeriodAsync(
        MeasurementPeriod period,
        CancellationToken cancellationToken = default);

    Task<MeasurementPeriod> UpdatePeriodAsync(
        Guid id,
        string name,
        string status,
        DateTimeOffset? collectionOpenAt,
        DateTimeOffset? collectionCloseAt,
        DateTimeOffset? dataCutoffAt,
        CancellationToken cancellationToken = default);

    Task<MeasurementPeriodCohort> AttachCohortAsync(
        MeasurementPeriodCohort cohort,
        CancellationToken cancellationToken = default);

    Task<MeasurementPeriodOffering> AttachOfferingAsync(
        MeasurementPeriodOffering offering,
        CancellationToken cancellationToken = default);

    Task<MeasurementPeriodTarget> CreateTargetAsync(
        MeasurementPeriodTarget target,
        CancellationToken cancellationToken = default);
}
