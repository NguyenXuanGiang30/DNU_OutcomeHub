using OutcomeHub.Application.DTOs.Result;

namespace OutcomeHub.Application.Interfaces.Services;

public interface IOutcomeCalculationService
{
    Task<ResultBatchDto> CalculatePeriodOutcomesAsync(
        Guid measurementPeriodId,
        string? calculationReason,
        Guid requestedByPrincipalId,
        CancellationToken cancellationToken);
}
