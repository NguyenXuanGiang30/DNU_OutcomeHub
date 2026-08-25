namespace OutcomeHub.Application.DTOs.Quality;

// ──────────────── Request DTOs ────────────────

public sealed record CreateImprovementPlanRequest(
    Guid OrgUnitId,
    Guid ProgramVersionId,
    string Title,
    string ProblemStatement,
    string? RootCauseSummary,
    decimal? BaselineValue,
    decimal? TargetValue,
    string KpiDefinition,
    Guid OwnerPrincipalId,
    DateOnly DueDate,
    IReadOnlyList<CreateImprovementFindingRequest>? Findings);

public sealed record UpdateImprovementPlanRequest(
    string Title,
    string ProblemStatement,
    string? RootCauseSummary,
    decimal? BaselineValue,
    decimal? TargetValue,
    string KpiDefinition,
    DateOnly DueDate);

public sealed record TransitionPlanStatusRequest(
    string NewStatus,
    string? Comment);

public sealed record CreateImprovementActionRequest(
    string Description,
    Guid OwnerPrincipalId,
    Guid OwnerOrgUnitId,
    DateOnly StartDate,
    DateOnly DueDate);

public sealed record UpdateActionProgressRequest(
    decimal CompletionRatio);

public sealed record CreateImprovementFindingRequest(
    string FindingType,
    short? AcademicYearStart,
    Guid? CohortOutcomeResultId,
    string? Description,
    string? SourceChecksum);

public sealed record AttachImprovementEvidenceRequest(
    Guid? ImprovementActionId,
    Guid EvidenceVersionId,
    string LinkRole);

public sealed record VerifyEvidenceRequest(
    Guid VerifiedBy);

public sealed record CreateRemeasurementEvaluationRequest(
    Guid BeforeBatchId,
    Guid AfterBatchId,
    string ComparabilityStatus,
    decimal? BaselineValue,
    decimal? AfterValue,
    string Conclusion);

// ──────────────── Response DTOs ────────────────

public sealed record ImprovementPlanDto(
    Guid Id,
    string Code,
    Guid OrgUnitId,
    Guid ProgramVersionId,
    string Title,
    string ProblemStatement,
    string? RootCauseSummary,
    decimal? BaselineValue,
    decimal? TargetValue,
    string KpiDefinition,
    Guid OwnerPrincipalId,
    DateOnly DueDate,
    string Status,
    DateTimeOffset CreatedAt);

public sealed record ImprovementPlanDetailDto(
    Guid Id,
    string Code,
    Guid OrgUnitId,
    Guid ProgramVersionId,
    string Title,
    string ProblemStatement,
    string? RootCauseSummary,
    decimal? BaselineValue,
    decimal? TargetValue,
    string KpiDefinition,
    Guid OwnerPrincipalId,
    DateOnly DueDate,
    string Status,
    DateTimeOffset CreatedAt,
    IReadOnlyList<ImprovementActionDto> Actions,
    IReadOnlyList<ImprovementFindingDto> Findings,
    IReadOnlyList<ImprovementEvidenceDto> Evidences,
    IReadOnlyList<RemeasurementEvaluationDto> Remeasurements);

public sealed record ImprovementActionDto(
    Guid Id,
    Guid ImprovementPlanId,
    int ActionNo,
    string Description,
    Guid OwnerPrincipalId,
    Guid OwnerOrgUnitId,
    DateOnly StartDate,
    DateOnly DueDate,
    string Status,
    decimal CompletionRatio,
    DateTimeOffset? CompletedAt);

public sealed record ImprovementFindingDto(
    Guid Id,
    Guid ImprovementPlanId,
    string FindingType,
    short? AcademicYearStart,
    Guid? CohortOutcomeResultId,
    string? Description,
    string? SourceChecksum,
    DateTimeOffset CreatedAt);

public sealed record ImprovementEvidenceDto(
    Guid Id,
    Guid ImprovementPlanId,
    Guid? ImprovementActionId,
    Guid EvidenceVersionId,
    string LinkRole,
    Guid? VerifiedBy,
    DateTimeOffset? VerifiedAt);

public sealed record RemeasurementEvaluationDto(
    Guid Id,
    Guid ImprovementPlanId,
    Guid BeforeBatchId,
    Guid AfterBatchId,
    string ComparabilityStatus,
    decimal? BaselineValue,
    decimal? AfterValue,
    decimal? DeltaValue,
    string Conclusion,
    Guid VerifiedBy,
    DateTimeOffset VerifiedAt);

public sealed record CqiDashboardSummaryDto(
    int TotalPlans,
    int DraftCount,
    int ExecutingCount,
    int ClosedCount,
    int OverdueActionCount,
    int VerifiedEvidenceCount,
    int TotalEvidenceCount,
    int RemeasurementCount,
    decimal ClosureRate);
