namespace OutcomeHub.Application.DTOs.Iam;

// ──────────────── User / Principal DTOs ────────────────

public sealed record CreateUserAccountRequest(
    string DisplayName,
    string PrincipalType,
    string Status,
    string? Username,
    Guid? PersonId);

public sealed record UserAccountDto(
    Guid PrincipalId,
    string DisplayName,
    string PrincipalType,
    string Status,
    string? Username,
    Guid? PersonId,
    DateTimeOffset? LastLoginAt,
    DateTimeOffset CreatedAt);

public sealed record UserDetailDto(
    Guid PrincipalId,
    string DisplayName,
    string PrincipalType,
    string Status,
    string? Username,
    Guid? PersonId,
    DateTimeOffset? LastLoginAt,
    DateTimeOffset CreatedAt,
    IReadOnlyList<RoleAssignmentDto> RoleAssignments);

// ──────────────── Role & Permission DTOs ────────────────

public sealed record CreateRoleRequest(
    string Code,
    string Name,
    bool IsSystem,
    IReadOnlyList<Guid>? PermissionIds);

public sealed record RoleDto(
    Guid Id,
    string Code,
    string Name,
    bool IsSystem,
    string Status,
    DateTimeOffset CreatedAt);

public sealed record RoleDetailDto(
    Guid Id,
    string Code,
    string Name,
    bool IsSystem,
    string Status,
    DateTimeOffset CreatedAt,
    IReadOnlyList<PermissionDto> Permissions);

public sealed record PermissionDto(
    Guid Id,
    string ResourceType,
    string Action,
    string FieldScope,
    string? Description);

// ──────────────── Role Assignment DTOs ────────────────

public sealed record AssignRoleRequest(
    Guid PrincipalId,
    Guid RoleId,
    Guid AccessScopeId,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset EffectiveTo,
    string Reason);

public sealed record RevokeRoleAssignmentRequest(
    string RevokeReason);

public sealed record RoleAssignmentDto(
    Guid Id,
    Guid PrincipalId,
    string PrincipalName,
    Guid RoleId,
    string RoleCode,
    string RoleName,
    Guid AccessScopeId,
    string ScopeType,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset EffectiveTo,
    string Status,
    string Reason,
    DateTimeOffset? RevokedAt);

// ──────────────── Access Scope DTOs ────────────────

public sealed record CreateAccessScopeRequest(
    string ScopeType,
    Guid? OrgUnitId,
    Guid? ProgramId,
    Guid? ProgramVersionId,
    Guid? CohortId,
    Guid? CurriculumPathId,
    Guid? CourseId,
    Guid? CourseOfferingId,
    Guid? MeasurementPeriodId,
    Guid? SubjectPrincipalId,
    bool IncludeDescendants);

public sealed record AccessScopeDto(
    Guid Id,
    string ScopeType,
    Guid? OrgUnitId,
    Guid? ProgramId,
    Guid? ProgramVersionId,
    Guid? CohortId,
    Guid? CurriculumPathId,
    Guid? CourseId,
    Guid? CourseOfferingId,
    Guid? MeasurementPeriodId,
    Guid? SubjectPrincipalId,
    bool IncludeDescendants,
    string Checksum);

// ──────────────── Separation of Duties (SoD) DTOs ────────────────

public sealed record SodRuleDto(
    Guid Id,
    Guid PolicyVersionId,
    string ResourceType,
    Guid PermissionAId,
    Guid PermissionBId,
    string ConflictMode,
    string Severity);

public sealed record CheckSodViolationRequest(
    Guid PrincipalId,
    Guid TargetRoleId,
    Guid TargetAccessScopeId);

public sealed record SodViolationCheckResultDto(
    bool HasViolation,
    IReadOnlyList<string> Violations);

// ──────────────── Audit Trail DTOs ────────────────

public sealed record AuditLogEntryDto(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid? ActorPrincipalId,
    string? ActorName,
    string Action,
    string Category,
    string Outcome,
    string ResourceType,
    Guid? ResourceId,
    string? Purpose,
    string? Reason,
    string EventHash);

public sealed record QueryAuditLogsRequest(
    Guid? ActorPrincipalId,
    string? Action,
    string? Category,
    string? ResourceType,
    DateTimeOffset? FromDate,
    DateTimeOffset? ToDate,
    int PageNumber,
    int PageSize);

// ──────────────── Governance & Legal Hold DTOs ────────────────

public sealed record CreateLegalHoldRequest(
    string Code,
    string Title,
    string Reason,
    DateTimeOffset EffectiveFrom);

public sealed record LegalHoldDto(
    Guid Id,
    string Code,
    string Title,
    string Reason,
    string Status,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset? ReleasedAt,
    Guid CreatedBy);
