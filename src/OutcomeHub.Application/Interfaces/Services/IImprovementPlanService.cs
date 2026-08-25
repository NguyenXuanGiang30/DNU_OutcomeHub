using OutcomeHub.Application.DTOs.Quality;

namespace OutcomeHub.Application.Interfaces.Services;

public interface IImprovementPlanService
{
    Task<ImprovementPlanDto> CreatePlanAsync(
        CreateImprovementPlanRequest request,
        Guid createdByPrincipalId,
        CancellationToken cancellationToken);

    Task<ImprovementPlanDto> UpdatePlanAsync(
        Guid planId,
        UpdateImprovementPlanRequest request,
        CancellationToken cancellationToken);

    Task<ImprovementPlanDto> TransitionPlanStatusAsync(
        Guid planId,
        TransitionPlanStatusRequest request,
        CancellationToken cancellationToken);

    Task<ImprovementActionDto> AddActionAsync(
        Guid planId,
        CreateImprovementActionRequest request,
        CancellationToken cancellationToken);

    Task<ImprovementActionDto> UpdateActionProgressAsync(
        Guid actionId,
        UpdateActionProgressRequest request,
        CancellationToken cancellationToken);

    Task<ImprovementActionDto> CompleteActionAsync(
        Guid actionId,
        CancellationToken cancellationToken);

    Task<ImprovementEvidenceDto> AttachEvidenceAsync(
        Guid planId,
        AttachImprovementEvidenceRequest request,
        CancellationToken cancellationToken);

    Task<ImprovementEvidenceDto> VerifyEvidenceAsync(
        Guid evidenceId,
        VerifyEvidenceRequest request,
        CancellationToken cancellationToken);

    Task<RemeasurementEvaluationDto> CreateRemeasurementAsync(
        Guid planId,
        CreateRemeasurementEvaluationRequest request,
        Guid verifiedByPrincipalId,
        CancellationToken cancellationToken);
}
