using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.DTOs.Quality;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;
using OutcomeHub.Domain.Entities.Quality;

namespace OutcomeHub.Infrastructure.Services;

public sealed class ImprovementPlanService : IImprovementPlanService
{
    private readonly IImprovementPlanRepository _repository;

    public ImprovementPlanService(IImprovementPlanRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ImprovementPlanDto> CreatePlanAsync(
        CreateImprovementPlanRequest request,
        Guid createdByPrincipalId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var now = DateTimeOffset.UtcNow;
        var planId = Guid.NewGuid();
        var governedResourceId = Guid.NewGuid();
        var workflowInstanceId = Guid.NewGuid();
        var code = $"CQI-{now:yyyyMMdd}-{planId.ToString("N")[..8].ToUpperInvariant()}";

        var plan = ImprovementPlan.Create(
            id: planId,
            governedResourceId: governedResourceId,
            code: code,
            orgUnitId: request.OrgUnitId,
            programVersionId: request.ProgramVersionId,
            title: request.Title,
            problemStatement: request.ProblemStatement,
            rootCauseSummary: request.RootCauseSummary,
            baselineValue: request.BaselineValue,
            targetValue: request.TargetValue,
            kpiDefinition: request.KpiDefinition,
            ownerPrincipalId: request.OwnerPrincipalId,
            dueDate: request.DueDate,
            workflowInstanceId: workflowInstanceId,
            createdBy: createdByPrincipalId,
            createdAt: now);

        List<ImprovementFinding>? findings = null;
        if (request.Findings is { Count: > 0 })
        {
            findings = [];
            foreach (var findingReq in request.Findings)
            {
                ImprovementFinding finding;
                if (findingReq.CohortOutcomeResultId.HasValue)
                {
                    finding = ImprovementFinding.CreateFromCohortResult(
                        id: Guid.NewGuid(),
                        improvementPlanId: planId,
                        findingType: findingReq.FindingType,
                        academicYearStart: findingReq.AcademicYearStart
                            ?? throw new ArgumentException("AcademicYearStart is required when CohortOutcomeResultId is provided."),
                        cohortOutcomeResultId: findingReq.CohortOutcomeResultId.Value,
                        description: findingReq.Description,
                        sourceChecksum: findingReq.SourceChecksum,
                        createdAt: now);
                }
                else
                {
                    finding = ImprovementFinding.CreateFromDescription(
                        id: Guid.NewGuid(),
                        improvementPlanId: planId,
                        findingType: findingReq.FindingType,
                        description: findingReq.Description
                            ?? throw new ArgumentException("Description is required when no CohortOutcomeResultId is provided."),
                        createdAt: now);
                }

                findings.Add(finding);
            }
        }

        return await _repository.SavePlanAsync(plan, findings, cancellationToken);
    }

    public async Task<ImprovementPlanDto> UpdatePlanAsync(
        Guid planId,
        UpdateImprovementPlanRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var plan = await _repository.GetPlanEntityByIdAsync(planId, cancellationToken)
            ?? throw new NotFoundException("ImprovementPlan", planId);

        plan.UpdateDetails(
            request.Title,
            request.ProblemStatement,
            request.RootCauseSummary,
            request.BaselineValue,
            request.TargetValue,
            request.KpiDefinition,
            request.DueDate);

        await _repository.UpdatePlanAsync(plan, cancellationToken);

        return (await _repository.GetPlanByIdAsync(planId, cancellationToken))!;
    }

    public async Task<ImprovementPlanDto> TransitionPlanStatusAsync(
        Guid planId,
        TransitionPlanStatusRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var plan = await _repository.GetPlanEntityByIdAsync(planId, cancellationToken)
            ?? throw new NotFoundException("ImprovementPlan", planId);

        plan.TransitionStatus(request.NewStatus);
        await _repository.UpdatePlanAsync(plan, cancellationToken);

        return (await _repository.GetPlanByIdAsync(planId, cancellationToken))!;
    }

    public async Task<ImprovementActionDto> AddActionAsync(
        Guid planId,
        CreateImprovementActionRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Verify plan exists
        _ = await _repository.GetPlanEntityByIdAsync(planId, cancellationToken)
            ?? throw new NotFoundException("ImprovementPlan", planId);

        int nextNo = await _repository.GetNextActionNoAsync(planId, cancellationToken);

        var action = ImprovementAction.Create(
            id: Guid.NewGuid(),
            improvementPlanId: planId,
            actionNo: nextNo,
            description: request.Description,
            ownerPrincipalId: request.OwnerPrincipalId,
            ownerOrgUnitId: request.OwnerOrgUnitId,
            startDate: request.StartDate,
            dueDate: request.DueDate);

        return await _repository.SaveActionAsync(action, cancellationToken);
    }

    public async Task<ImprovementActionDto> UpdateActionProgressAsync(
        Guid actionId,
        UpdateActionProgressRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var action = await _repository.GetActionByIdAsync(actionId, cancellationToken)
            ?? throw new NotFoundException("ImprovementAction", actionId);

        action.UpdateProgress(request.CompletionRatio);
        await _repository.UpdateActionAsync(action, cancellationToken);

        return new ImprovementActionDto(
            action.Id, action.ImprovementPlanId, action.ActionNo,
            action.Description, action.OwnerPrincipalId, action.OwnerOrgUnitId,
            action.StartDate, action.DueDate, action.Status,
            action.CompletionRatio, action.CompletedAt);
    }

    public async Task<ImprovementActionDto> CompleteActionAsync(
        Guid actionId,
        CancellationToken cancellationToken)
    {
        var action = await _repository.GetActionByIdAsync(actionId, cancellationToken)
            ?? throw new NotFoundException("ImprovementAction", actionId);

        action.Complete(DateTimeOffset.UtcNow);
        await _repository.UpdateActionAsync(action, cancellationToken);

        return new ImprovementActionDto(
            action.Id, action.ImprovementPlanId, action.ActionNo,
            action.Description, action.OwnerPrincipalId, action.OwnerOrgUnitId,
            action.StartDate, action.DueDate, action.Status,
            action.CompletionRatio, action.CompletedAt);
    }

    public async Task<ImprovementEvidenceDto> AttachEvidenceAsync(
        Guid planId,
        AttachImprovementEvidenceRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        _ = await _repository.GetPlanEntityByIdAsync(planId, cancellationToken)
            ?? throw new NotFoundException("ImprovementPlan", planId);

        var evidence = ImprovementEvidence.Create(
            id: Guid.NewGuid(),
            improvementPlanId: planId,
            improvementActionId: request.ImprovementActionId,
            evidenceVersionId: request.EvidenceVersionId,
            linkRole: request.LinkRole);

        return await _repository.SaveEvidenceAsync(evidence, cancellationToken);
    }

    public async Task<ImprovementEvidenceDto> VerifyEvidenceAsync(
        Guid evidenceId,
        VerifyEvidenceRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var evidence = await _repository.GetEvidenceByIdAsync(evidenceId, cancellationToken)
            ?? throw new NotFoundException("ImprovementEvidence", evidenceId);

        evidence.Verify(request.VerifiedBy, DateTimeOffset.UtcNow);
        await _repository.UpdateEvidenceAsync(evidence, cancellationToken);

        return new ImprovementEvidenceDto(
            evidence.Id, evidence.ImprovementPlanId,
            evidence.ImprovementActionId, evidence.EvidenceVersionId,
            evidence.LinkRole, evidence.VerifiedBy, evidence.VerifiedAt);
    }

    public async Task<RemeasurementEvaluationDto> CreateRemeasurementAsync(
        Guid planId,
        CreateRemeasurementEvaluationRequest request,
        Guid verifiedByPrincipalId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        _ = await _repository.GetPlanEntityByIdAsync(planId, cancellationToken)
            ?? throw new NotFoundException("ImprovementPlan", planId);

        var evaluation = RemeasurementEvaluation.Create(
            id: Guid.NewGuid(),
            improvementPlanId: planId,
            beforeBatchId: request.BeforeBatchId,
            afterBatchId: request.AfterBatchId,
            comparabilityStatus: request.ComparabilityStatus,
            baselineValue: request.BaselineValue,
            afterValue: request.AfterValue,
            conclusion: request.Conclusion,
            verifiedBy: verifiedByPrincipalId,
            verifiedAt: DateTimeOffset.UtcNow);

        return await _repository.SaveRemeasurementAsync(evaluation, cancellationToken);
    }
}
