using OutcomeHub.Application.DTOs.Quality;
using OutcomeHub.Domain.Entities.Quality;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IImprovementPlanRepository
{
    Task<ImprovementPlanDto?> GetPlanByIdAsync(
        Guid planId,
        CancellationToken cancellationToken);

    Task<ImprovementPlanDetailDto?> GetPlanDetailByIdAsync(
        Guid planId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ImprovementPlanDto>> GetPlansAsync(
        Guid? programVersionId,
        Guid? orgUnitId,
        string? status,
        CancellationToken cancellationToken);

    Task<ImprovementPlanDto> SavePlanAsync(
        ImprovementPlan plan,
        IReadOnlyList<ImprovementFinding>? findings,
        CancellationToken cancellationToken);

    Task UpdatePlanAsync(
        ImprovementPlan plan,
        CancellationToken cancellationToken);

    Task<ImprovementActionDto> SaveActionAsync(
        ImprovementAction action,
        CancellationToken cancellationToken);

    Task UpdateActionAsync(
        ImprovementAction action,
        CancellationToken cancellationToken);

    Task<ImprovementEvidenceDto> SaveEvidenceAsync(
        ImprovementEvidence evidence,
        CancellationToken cancellationToken);

    Task UpdateEvidenceAsync(
        ImprovementEvidence evidence,
        CancellationToken cancellationToken);

    Task<RemeasurementEvaluationDto> SaveRemeasurementAsync(
        RemeasurementEvaluation evaluation,
        CancellationToken cancellationToken);

    Task<CqiDashboardSummaryDto> GetCqiDashboardAsync(
        Guid? programVersionId,
        Guid? orgUnitId,
        CancellationToken cancellationToken);

    Task<ImprovementAction?> GetActionByIdAsync(
        Guid actionId,
        CancellationToken cancellationToken);

    Task<ImprovementEvidence?> GetEvidenceByIdAsync(
        Guid evidenceId,
        CancellationToken cancellationToken);

    Task<ImprovementPlan?> GetPlanEntityByIdAsync(
        Guid planId,
        CancellationToken cancellationToken);

    Task<int> GetNextActionNoAsync(
        Guid planId,
        CancellationToken cancellationToken);
}
