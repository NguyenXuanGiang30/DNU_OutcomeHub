using OutcomeHub.Application.DTOs.Iam;

namespace OutcomeHub.Application.Interfaces.Services;

public interface IIamService
{
    // User management
    Task<UserAccountDto> CreateUserAsync(
        CreateUserAccountRequest request,
        CancellationToken cancellationToken);

    // Role management
    Task<RoleDto> CreateRoleAsync(
        CreateRoleRequest request,
        Guid createdByPrincipalId,
        CancellationToken cancellationToken);

    // Access Scope management
    Task<AccessScopeDto> CreateAccessScopeAsync(
        CreateAccessScopeRequest request,
        CancellationToken cancellationToken);

    // Role Assignment & SoD checking
    Task<RoleAssignmentDto> AssignRoleAsync(
        AssignRoleRequest request,
        Guid grantedByPrincipalId,
        CancellationToken cancellationToken);

    Task<RoleAssignmentDto> RevokeRoleAssignmentAsync(
        Guid assignmentId,
        RevokeRoleAssignmentRequest request,
        CancellationToken cancellationToken);

    Task<SodViolationCheckResultDto> CheckSodViolationAsync(
        CheckSodViolationRequest request,
        CancellationToken cancellationToken);

    // Audit Logging
    Task<AuditLogEntryDto> LogSecurityEventAsync(
        Guid? actorPrincipalId,
        string actorKind,
        string action,
        string category,
        string outcome,
        string resourceType,
        Guid? resourceId,
        string? purpose,
        string? reason,
        string classification,
        CancellationToken cancellationToken);

    // Governance & Legal Hold
    Task<LegalHoldDto> CreateLegalHoldAsync(
        CreateLegalHoldRequest request,
        Guid createdByPrincipalId,
        CancellationToken cancellationToken);

    Task<LegalHoldDto> ReleaseLegalHoldAsync(
        Guid holdId,
        CancellationToken cancellationToken);
}
