using OutcomeHub.Application.DTOs.Result;
using OutcomeHub.Domain.Entities.Measurement;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IResultRepository
{
    Task<ResultBatchDto?> GetResultBatchByIdAsync(
        Guid batchId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ResultBatchDto>> GetBatchesByPeriodIdAsync(
        Guid periodId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentCloResultDto>> GetStudentCloResultsAsync(
        Guid? batchId,
        Guid? studentId,
        Guid? courseOfferingId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentPiResultDto>> GetStudentPiResultsAsync(
        Guid? batchId,
        Guid? studentId,
        Guid? programVersionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentPloResultDto>> GetStudentPloResultsAsync(
        Guid? batchId,
        Guid? studentId,
        Guid? programVersionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CohortOutcomeResultDto>> GetCohortOutcomeResultsAsync(
        Guid? batchId,
        Guid? cohortId,
        string? outcomeLevel,
        CancellationToken cancellationToken);

    Task<ProgramOutcomeDashboardDto?> GetProgramOutcomeDashboardAsync(
        Guid periodId,
        Guid programVersionId,
        Guid cohortId,
        CancellationToken cancellationToken);

    Task<ResultBatchDto> SaveCalculationBatchAsync(
        InputSnapshot inputSnapshot,
        ResultBatch resultBatch,
        IReadOnlyList<StudentCloResult> cloResults,
        IReadOnlyList<StudentPiResult> piResults,
        IReadOnlyList<StudentPloResult> ploResults,
        IReadOnlyList<CohortOutcomeResult> cohortResults,
        CancellationToken cancellationToken);
}
