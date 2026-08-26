using OutcomeHub.Application.DTOs.Iam;
using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IIamRepository
{
    // Users
    Task<UserAccountDto?> GetUserByIdAsync(Guid principalId, CancellationToken cancellationToken);
    Task<UserDetailDto?> GetUserDetailByIdAsync(Guid principalId, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserAccountDto>> GetUsersAsync(string? status, CancellationToken cancellationToken);
    Task<UserAccountDto> SaveUserAsync(Principal principal, UserAccount userAccount, CancellationToken cancellationToken);
    Task UpdateUserAccountAsync(UserAccount userAccount, CancellationToken cancellationToken);

    // Roles & Permissions
    Task<RoleDto?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken);
    Task<RoleDetailDto?> GetRoleDetailByIdAsync(Guid roleId, CancellationToken cancellationToken);
    Task<IReadOnlyList<RoleDto>> GetRolesAsync(string? status, CancellationToken cancellationToken);
    Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync(CancellationToken cancellationToken);
    Task<Guid?> GetLatestRoleVersionIdAsync(Guid roleId, CancellationToken cancellationToken);
    Task<RoleDto> SaveRoleAsync(Role role, RoleVersion roleVersion, IReadOnlyList<RoleVersionPermission> permissions, CancellationToken cancellationToken);

    // Access Scopes
    Task<AccessScopeDto?> GetAccessScopeByIdAsync(Guid accessScopeId, CancellationToken cancellationToken);
    Task<IReadOnlyList<AccessScopeDto>> GetAccessScopesAsync(string? scopeType, CancellationToken cancellationToken);
    Task<AccessScopeDto> SaveAccessScopeAsync(AccessScope accessScope, CancellationToken cancellationToken);

    // Role Assignments
    Task<RoleAssignmentDto?> GetRoleAssignmentByIdAsync(Guid assignmentId, CancellationToken cancellationToken);
    Task<IReadOnlyList<RoleAssignmentDto>> GetRoleAssignmentsByPrincipalAsync(Guid principalId, CancellationToken cancellationToken);
    Task<IReadOnlyList<RoleAssignmentDto>> GetRoleAssignmentsAsync(Guid? principalId, Guid? roleId, string? status, CancellationToken cancellationToken);
    Task<RoleAssignmentDto> SaveRoleAssignmentAsync(RoleAssignment assignment, CancellationToken cancellationToken);
    Task UpdateRoleAssignmentAsync(RoleAssignment assignment, CancellationToken cancellationToken);
    Task<RoleAssignment?> GetRoleAssignmentEntityByIdAsync(Guid assignmentId, CancellationToken cancellationToken);

    // SoD (Separation of Duties)
    Task<IReadOnlyList<SodRuleDto>> GetSodRulesAsync(CancellationToken cancellationToken);
    Task<Guid> GetActiveSodPolicyVersionIdAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Guid>> GetPermissionsForRoleAsync(Guid roleId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Guid>> GetPrincipalActiveRoleIdsAsync(Guid principalId, CancellationToken cancellationToken);

    // Governance & Legal Holds
    Task<LegalHoldDto?> GetLegalHoldByIdAsync(Guid holdId, CancellationToken cancellationToken);
    Task<IReadOnlyList<LegalHoldDto>> GetLegalHoldsAsync(string? status, CancellationToken cancellationToken);
    Task<LegalHoldDto> SaveLegalHoldAsync(LegalHold legalHold, CancellationToken cancellationToken);
    Task UpdateLegalHoldAsync(LegalHold legalHold, CancellationToken cancellationToken);
    Task<LegalHold?> GetLegalHoldEntityByIdAsync(Guid holdId, CancellationToken cancellationToken);
}
