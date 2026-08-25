using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.DTOs.Quality;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Quality;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Quality;

public sealed class ImprovementPlanRepository : IImprovementPlanRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public ImprovementPlanRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<ImprovementPlanDto?> GetPlanByIdAsync(
        Guid planId,
        CancellationToken cancellationToken)
    {
        var plan = await _dbContext.ImprovementPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == planId, cancellationToken);

        return plan == null ? null : MapToDto(plan);
    }

    public async Task<ImprovementPlanDetailDto?> GetPlanDetailByIdAsync(
        Guid planId,
        CancellationToken cancellationToken)
    {
        var plan = await _dbContext.ImprovementPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == planId, cancellationToken);

        if (plan == null) return null;

        var actions = await _dbContext.ImprovementActions
            .AsNoTracking()
            .Where(a => a.ImprovementPlanId == planId)
            .OrderBy(a => a.ActionNo)
            .Select(a => new ImprovementActionDto(
                a.Id, a.ImprovementPlanId, a.ActionNo, a.Description,
                a.OwnerPrincipalId, a.OwnerOrgUnitId,
                a.StartDate, a.DueDate, a.Status,
                a.CompletionRatio, a.CompletedAt))
            .ToListAsync(cancellationToken);

        var findings = await _dbContext.ImprovementFindings
            .AsNoTracking()
            .Where(f => f.ImprovementPlanId == planId)
            .Select(f => new ImprovementFindingDto(
                f.Id, f.ImprovementPlanId, f.FindingType,
                f.AcademicYearStart, f.CohortOutcomeResultId,
                f.Description, f.SourceChecksum, f.CreatedAt))
            .ToListAsync(cancellationToken);

        var evidences = await _dbContext.ImprovementEvidences
            .AsNoTracking()
            .Where(e => e.ImprovementPlanId == planId)
            .Select(e => new ImprovementEvidenceDto(
                e.Id, e.ImprovementPlanId, e.ImprovementActionId,
                e.EvidenceVersionId, e.LinkRole,
                e.VerifiedBy, e.VerifiedAt))
            .ToListAsync(cancellationToken);

        var remeasurements = await _dbContext.RemeasurementEvaluations
            .AsNoTracking()
            .Where(r => r.ImprovementPlanId == planId)
            .Select(r => new RemeasurementEvaluationDto(
                r.Id, r.ImprovementPlanId,
                r.BeforeBatchId, r.AfterBatchId,
                r.ComparabilityStatus,
                r.BaselineValue, r.AfterValue, r.DeltaValue,
                r.Conclusion, r.VerifiedBy, r.VerifiedAt))
            .ToListAsync(cancellationToken);

        return new ImprovementPlanDetailDto(
            plan.Id, plan.Code, plan.OrgUnitId, plan.ProgramVersionId,
            plan.Title, plan.ProblemStatement, plan.RootCauseSummary,
            plan.BaselineValue, plan.TargetValue, plan.KpiDefinition,
            plan.OwnerPrincipalId, plan.DueDate, plan.Status, plan.CreatedAt,
            actions, findings, evidences, remeasurements);
    }

    public async Task<IReadOnlyList<ImprovementPlanDto>> GetPlansAsync(
        Guid? programVersionId,
        Guid? orgUnitId,
        string? status,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.ImprovementPlans.AsNoTracking().AsQueryable();

        if (programVersionId.HasValue)
            query = query.Where(p => p.ProgramVersionId == programVersionId.Value);

        if (orgUnitId.HasValue)
            query = query.Where(p => p.OrgUnitId == orgUnitId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(p => p.Status == status);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ImprovementPlanDto(
                p.Id, p.Code, p.OrgUnitId, p.ProgramVersionId,
                p.Title, p.ProblemStatement, p.RootCauseSummary,
                p.BaselineValue, p.TargetValue, p.KpiDefinition,
                p.OwnerPrincipalId, p.DueDate, p.Status, p.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<ImprovementPlanDto> SavePlanAsync(
        ImprovementPlan plan,
        IReadOnlyList<ImprovementFinding>? findings,
        CancellationToken cancellationToken)
    {
        // Create governed resource
        await _dbContext.Database.ExecuteSqlAsync(
            $"INSERT INTO governance.governed_resource (id, resource_type, classification, disposition_status, created_at) VALUES ({plan.GovernedResourceId}, 'quality.improvement_plan', 'CONFIDENTIAL', 'ACTIVE', {plan.CreatedAt}) ON CONFLICT (id) DO NOTHING",
            cancellationToken);

        // Create workflow instance
        var workflowDefId = await _dbContext.Database
            .SqlQuery<Guid>(
                $"SELECT id AS \"Value\" FROM workflow.definition WHERE code = 'CQI_IMPROVEMENT_PLAN' LIMIT 1")
            .FirstOrDefaultAsync(cancellationToken);

        if (workflowDefId == Guid.Empty)
        {
            // Use any existing workflow definition as fallback
            workflowDefId = await _dbContext.Database
                .SqlQuery<Guid>(
                    $"SELECT id AS \"Value\" FROM workflow.definition ORDER BY id LIMIT 1")
                .FirstOrDefaultAsync(cancellationToken);
        }

        await _dbContext.Database.ExecuteSqlAsync(
            $"INSERT INTO workflow.instance (id, definition_id, current_state, started_by, started_at, row_version) VALUES ({plan.WorkflowInstanceId}, {workflowDefId}, 'DRAFT', {plan.CreatedBy}, {plan.CreatedAt}, 1) ON CONFLICT (id) DO NOTHING",
            cancellationToken);

        _dbContext.ImprovementPlans.Add(plan);

        if (findings != null)
        {
            foreach (var finding in findings)
            {
                _dbContext.ImprovementFindings.Add(finding);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(plan);
    }

    public async Task UpdatePlanAsync(
        ImprovementPlan plan,
        CancellationToken cancellationToken)
    {
        _dbContext.ImprovementPlans.Update(plan);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ImprovementActionDto> SaveActionAsync(
        ImprovementAction action,
        CancellationToken cancellationToken)
    {
        _dbContext.ImprovementActions.Add(action);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ImprovementActionDto(
            action.Id, action.ImprovementPlanId, action.ActionNo,
            action.Description, action.OwnerPrincipalId, action.OwnerOrgUnitId,
            action.StartDate, action.DueDate, action.Status,
            action.CompletionRatio, action.CompletedAt);
    }

    public async Task UpdateActionAsync(
        ImprovementAction action,
        CancellationToken cancellationToken)
    {
        _dbContext.ImprovementActions.Update(action);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ImprovementEvidenceDto> SaveEvidenceAsync(
        ImprovementEvidence evidence,
        CancellationToken cancellationToken)
    {
        _dbContext.ImprovementEvidences.Add(evidence);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ImprovementEvidenceDto(
            evidence.Id, evidence.ImprovementPlanId,
            evidence.ImprovementActionId, evidence.EvidenceVersionId,
            evidence.LinkRole, evidence.VerifiedBy, evidence.VerifiedAt);
    }

    public async Task UpdateEvidenceAsync(
        ImprovementEvidence evidence,
        CancellationToken cancellationToken)
    {
        _dbContext.ImprovementEvidences.Update(evidence);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<RemeasurementEvaluationDto> SaveRemeasurementAsync(
        RemeasurementEvaluation evaluation,
        CancellationToken cancellationToken)
    {
        _dbContext.RemeasurementEvaluations.Add(evaluation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RemeasurementEvaluationDto(
            evaluation.Id, evaluation.ImprovementPlanId,
            evaluation.BeforeBatchId, evaluation.AfterBatchId,
            evaluation.ComparabilityStatus,
            evaluation.BaselineValue, evaluation.AfterValue,
            evaluation.DeltaValue, evaluation.Conclusion,
            evaluation.VerifiedBy, evaluation.VerifiedAt);
    }

    public async Task<CqiDashboardSummaryDto> GetCqiDashboardAsync(
        Guid? programVersionId,
        Guid? orgUnitId,
        CancellationToken cancellationToken)
    {
        var planQuery = _dbContext.ImprovementPlans.AsNoTracking().AsQueryable();

        if (programVersionId.HasValue)
            planQuery = planQuery.Where(p => p.ProgramVersionId == programVersionId.Value);
        if (orgUnitId.HasValue)
            planQuery = planQuery.Where(p => p.OrgUnitId == orgUnitId.Value);

        var planIds = await planQuery.Select(p => p.Id).ToListAsync(cancellationToken);

        int totalPlans = planIds.Count;
        int draftCount = await planQuery.CountAsync(p => p.Status == "DRAFT", cancellationToken);
        int executingCount = await planQuery.CountAsync(
            p => p.Status == "EXECUTING" || p.Status == "IN_REVIEW" || p.Status == "APPROVED" || p.Status == "VERIFYING",
            cancellationToken);
        int closedCount = await planQuery.CountAsync(p => p.Status == "CLOSED", cancellationToken);

        int overdueActionCount = await _dbContext.ImprovementActions
            .AsNoTracking()
            .Where(a => planIds.Contains(a.ImprovementPlanId) && a.Status == "OVERDUE")
            .CountAsync(cancellationToken);

        int verifiedEvidenceCount = await _dbContext.ImprovementEvidences
            .AsNoTracking()
            .Where(e => planIds.Contains(e.ImprovementPlanId) && e.VerifiedBy != null)
            .CountAsync(cancellationToken);

        int totalEvidenceCount = await _dbContext.ImprovementEvidences
            .AsNoTracking()
            .Where(e => planIds.Contains(e.ImprovementPlanId))
            .CountAsync(cancellationToken);

        int remeasurementCount = await _dbContext.RemeasurementEvaluations
            .AsNoTracking()
            .Where(r => planIds.Contains(r.ImprovementPlanId))
            .CountAsync(cancellationToken);

        decimal closureRate = totalPlans > 0
            ? Math.Round(100m * closedCount / totalPlans, 2)
            : 0m;

        return new CqiDashboardSummaryDto(
            totalPlans, draftCount, executingCount, closedCount,
            overdueActionCount, verifiedEvidenceCount, totalEvidenceCount,
            remeasurementCount, closureRate);
    }

    public async Task<ImprovementAction?> GetActionByIdAsync(
        Guid actionId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ImprovementActions
            .FirstOrDefaultAsync(a => a.Id == actionId, cancellationToken);
    }

    public async Task<ImprovementEvidence?> GetEvidenceByIdAsync(
        Guid evidenceId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ImprovementEvidences
            .FirstOrDefaultAsync(e => e.Id == evidenceId, cancellationToken);
    }

    public async Task<ImprovementPlan?> GetPlanEntityByIdAsync(
        Guid planId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ImprovementPlans
            .FirstOrDefaultAsync(p => p.Id == planId, cancellationToken);
    }

    public async Task<int> GetNextActionNoAsync(
        Guid planId,
        CancellationToken cancellationToken)
    {
        var maxNo = await _dbContext.ImprovementActions
            .AsNoTracking()
            .Where(a => a.ImprovementPlanId == planId)
            .Select(a => (int?)a.ActionNo)
            .MaxAsync(cancellationToken);

        return (maxNo ?? 0) + 1;
    }

    private static ImprovementPlanDto MapToDto(ImprovementPlan plan)
    {
        return new ImprovementPlanDto(
            plan.Id, plan.Code, plan.OrgUnitId, plan.ProgramVersionId,
            plan.Title, plan.ProblemStatement, plan.RootCauseSummary,
            plan.BaselineValue, plan.TargetValue, plan.KpiDefinition,
            plan.OwnerPrincipalId, plan.DueDate, plan.Status, plan.CreatedAt);
    }
}
