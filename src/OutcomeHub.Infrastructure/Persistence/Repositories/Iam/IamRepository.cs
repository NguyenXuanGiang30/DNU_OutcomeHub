using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.DTOs.Iam;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Iam;

public sealed class IamRepository : IIamRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public IamRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    // ──────────────── User / Principal ────────────────

    public async Task<UserAccountDto?> GetUserByIdAsync(Guid principalId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.UserAccounts
            .AsNoTracking()
            .Include(u => u.Principal)
            .FirstOrDefaultAsync(u => u.PrincipalId == principalId, cancellationToken);

        return user == null ? null : MapToUserDto(user);
    }

    public async Task<UserDetailDto?> GetUserDetailByIdAsync(Guid principalId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.UserAccounts
            .AsNoTracking()
            .Include(u => u.Principal)
            .FirstOrDefaultAsync(u => u.PrincipalId == principalId, cancellationToken);

        if (user == null) return null;

        var assignments = await _dbContext.RoleAssignments
            .AsNoTracking()
            .Include(ra => ra.Role)
            .Include(ra => ra.AccessScope)
            .Where(ra => ra.PrincipalId == principalId)
            .Select(ra => new RoleAssignmentDto(
                ra.Id, ra.PrincipalId, user.Principal.DisplayName,
                ra.RoleId, ra.Role.Code, ra.Role.Name,
                ra.AccessScopeId, ra.AccessScope.ScopeType,
                ra.EffectiveFrom, ra.EffectiveTo, ra.Status,
                ra.Reason, ra.RevokedAt))
            .ToListAsync(cancellationToken);

        return new UserDetailDto(
            user.PrincipalId,
            user.Principal.DisplayName,
            user.Principal.PrincipalType.ToString(),
            user.Principal.Status.ToString(),
            user.Username,
            user.PersonId,
            user.LastLoginAt,
            user.Principal.CreatedAt,
            assignments);
    }

    public async Task<IReadOnlyList<UserAccountDto>> GetUsersAsync(string? status, CancellationToken cancellationToken)
    {
        var query = _dbContext.UserAccounts
            .AsNoTracking()
            .Include(u => u.Principal)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(u => u.Principal.Status.ToString() == status);
        }

        return await query
            .OrderBy(u => u.Principal.DisplayName)
            .Select(u => MapToUserDto(u))
            .ToListAsync(cancellationToken);
    }

    public async Task<UserAccountDto> SaveUserAsync(
        Principal principal,
        UserAccount userAccount,
        CancellationToken cancellationToken)
    {
        _dbContext.Principals.Add(principal);
        _dbContext.UserAccounts.Add(userAccount);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToUserDto(userAccount);
    }

    public async Task UpdateUserAccountAsync(UserAccount userAccount, CancellationToken cancellationToken)
    {
        _dbContext.UserAccounts.Update(userAccount);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // ──────────────── Role & Permission ────────────────

    public async Task<RoleDto?> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        return role == null ? null : MapToRoleDto(role);
    }

    public async Task<RoleDetailDto?> GetRoleDetailByIdAsync(Guid roleId, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        if (role == null) return null;

        var permissions = await (
            from rv in _dbContext.RoleVersions
            where rv.RoleId == roleId
            join rvp in _dbContext.RoleVersionPermissions on rv.Id equals rvp.RoleVersionId
            join p in _dbContext.Permissions on rvp.PermissionId equals p.Id
            select new PermissionDto(p.Id, p.ResourceType, p.Action, p.FieldScope, p.Description)
        ).Distinct().ToListAsync(cancellationToken);

        return new RoleDetailDto(
            role.Id, role.Code, role.Name, role.IsSystem, role.Status, role.CreatedAt, permissions);
    }

    public async Task<IReadOnlyList<RoleDto>> GetRolesAsync(string? status, CancellationToken cancellationToken)
    {
        var query = _dbContext.Roles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(r => r.Status == status);
        }

        return await query
            .OrderBy(r => r.Code)
            .Select(r => MapToRoleDto(r))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Permissions
            .AsNoTracking()
            .OrderBy(p => p.ResourceType)
            .ThenBy(p => p.Action)
            .Select(p => new PermissionDto(p.Id, p.ResourceType, p.Action, p.FieldScope, p.Description))
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid?> GetLatestRoleVersionIdAsync(Guid roleId, CancellationToken cancellationToken)
    {
        return await _dbContext.RoleVersions
            .AsNoTracking()
            .Where(rv => rv.RoleId == roleId && (rv.Status == "ACTIVE" || rv.Status == "APPROVED"))
            .OrderByDescending(rv => rv.VersionNo)
            .Select(rv => (Guid?)rv.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<RoleDto> SaveRoleAsync(
        Role role,
        RoleVersion roleVersion,
        IReadOnlyList<RoleVersionPermission> permissions,
        CancellationToken cancellationToken)
    {
        // Ensure workflow instance exists
        var workflowDefId = await _dbContext.Database
            .SqlQuery<Guid>($"SELECT id AS \"Value\" FROM workflow.definition ORDER BY id LIMIT 1")
            .FirstOrDefaultAsync(cancellationToken);

        await _dbContext.Database.ExecuteSqlAsync(
            $"INSERT INTO workflow.instance (id, definition_id, current_state, started_by, started_at, row_version) VALUES ({roleVersion.WorkflowInstanceId}, {workflowDefId}, 'APPROVED', {roleVersion.CreatedBy}, {roleVersion.CreatedAt}, 1) ON CONFLICT (id) DO NOTHING",
            cancellationToken);

        _dbContext.Roles.Add(role);
        _dbContext.RoleVersions.Add(roleVersion);

        foreach (var p in permissions)
        {
            _dbContext.RoleVersionPermissions.Add(p);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToRoleDto(role);
    }

    // ──────────────── Access Scope ────────────────

    public async Task<AccessScopeDto?> GetAccessScopeByIdAsync(Guid accessScopeId, CancellationToken cancellationToken)
    {
        var scope = await _dbContext.AccessScopes
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == accessScopeId, cancellationToken);

        return scope == null ? null : MapToScopeDto(scope);
    }

    public async Task<IReadOnlyList<AccessScopeDto>> GetAccessScopesAsync(string? scopeType, CancellationToken cancellationToken)
    {
        var query = _dbContext.AccessScopes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(scopeType))
        {
            query = query.Where(s => s.ScopeType == scopeType);
        }

        return await query
            .OrderBy(s => s.ScopeType)
            .Select(s => MapToScopeDto(s))
            .ToListAsync(cancellationToken);
    }

    public async Task<AccessScopeDto> SaveAccessScopeAsync(AccessScope accessScope, CancellationToken cancellationToken)
    {
        _dbContext.AccessScopes.Add(accessScope);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToScopeDto(accessScope);
    }

    // ──────────────── Role Assignment ────────────────

    public async Task<RoleAssignmentDto?> GetRoleAssignmentByIdAsync(Guid assignmentId, CancellationToken cancellationToken)
    {
        var assignment = await _dbContext.RoleAssignments
            .AsNoTracking()
            .Include(ra => ra.Principal)
            .Include(ra => ra.Role)
            .Include(ra => ra.AccessScope)
            .FirstOrDefaultAsync(ra => ra.Id == assignmentId, cancellationToken);

        return assignment == null ? null : MapToAssignmentDto(assignment);
    }

    public async Task<IReadOnlyList<RoleAssignmentDto>> GetRoleAssignmentsByPrincipalAsync(Guid principalId, CancellationToken cancellationToken)
    {
        return await _dbContext.RoleAssignments
            .AsNoTracking()
            .Include(ra => ra.Principal)
            .Include(ra => ra.Role)
            .Include(ra => ra.AccessScope)
            .Where(ra => ra.PrincipalId == principalId)
            .OrderByDescending(ra => ra.RequestedAt)
            .Select(ra => MapToAssignmentDto(ra))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RoleAssignmentDto>> GetRoleAssignmentsAsync(Guid? principalId, Guid? roleId, string? status, CancellationToken cancellationToken)
    {
        var query = _dbContext.RoleAssignments
            .AsNoTracking()
            .Include(ra => ra.Principal)
            .Include(ra => ra.Role)
            .Include(ra => ra.AccessScope)
            .AsQueryable();

        if (principalId.HasValue) query = query.Where(ra => ra.PrincipalId == principalId.Value);
        if (roleId.HasValue) query = query.Where(ra => ra.RoleId == roleId.Value);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(ra => ra.Status == status);

        return await query
            .OrderByDescending(ra => ra.RequestedAt)
            .Select(ra => MapToAssignmentDto(ra))
            .ToListAsync(cancellationToken);
    }

    public async Task<RoleAssignmentDto> SaveRoleAssignmentAsync(RoleAssignment assignment, CancellationToken cancellationToken)
    {
        // Ensure workflow instance exists
        var workflowDefId = await _dbContext.Database
            .SqlQuery<Guid>($"SELECT id AS \"Value\" FROM workflow.definition ORDER BY id LIMIT 1")
            .FirstOrDefaultAsync(cancellationToken);

        await _dbContext.Database.ExecuteSqlAsync(
            $"INSERT INTO workflow.instance (id, definition_id, current_state, started_by, started_at, row_version) VALUES ({assignment.WorkflowInstanceId}, {workflowDefId}, 'APPROVED', {assignment.GrantedBy}, {assignment.RequestedAt}, 1) ON CONFLICT (id) DO NOTHING",
            cancellationToken);

        _dbContext.RoleAssignments.Add(assignment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await GetRoleAssignmentByIdAsync(assignment.Id, cancellationToken))!;
    }

    public async Task UpdateRoleAssignmentAsync(RoleAssignment assignment, CancellationToken cancellationToken)
    {
        _dbContext.RoleAssignments.Update(assignment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<RoleAssignment?> GetRoleAssignmentEntityByIdAsync(Guid assignmentId, CancellationToken cancellationToken)
    {
        return await _dbContext.RoleAssignments
            .FirstOrDefaultAsync(ra => ra.Id == assignmentId, cancellationToken);
    }

    // ──────────────── Separation of Duties (SoD) ────────────────

    public async Task<IReadOnlyList<SodRuleDto>> GetSodRulesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SodRules
            .AsNoTracking()
            .Select(sr => new SodRuleDto(
                sr.Id, sr.PolicyVersionId, sr.ResourceType,
                sr.PermissionAId, sr.PermissionBId,
                sr.ConflictMode, sr.Severity))
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid> GetActiveSodPolicyVersionIdAsync(CancellationToken cancellationToken)
    {
        var policyId = await _dbContext.SodPolicyVersions
            .AsNoTracking()
            .Where(p => p.Status == "ACTIVE")
            .OrderByDescending(p => p.VersionNo)
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return policyId ?? Guid.Parse("00000000-0000-7000-8000-000000000601");
    }

    public async Task<IReadOnlyList<Guid>> GetPermissionsForRoleAsync(Guid roleId, CancellationToken cancellationToken)
    {
        return await (
            from rv in _dbContext.RoleVersions
            where rv.RoleId == roleId
            join rvp in _dbContext.RoleVersionPermissions on rv.Id equals rvp.RoleVersionId
            select rvp.PermissionId
        ).Distinct().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetPrincipalActiveRoleIdsAsync(Guid principalId, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        return await _dbContext.RoleAssignments
            .AsNoTracking()
            .Where(ra => ra.PrincipalId == principalId &&
                         ra.Status == "ACTIVE" &&
                         ra.EffectiveFrom <= now &&
                         ra.EffectiveTo >= now)
            .Select(ra => ra.RoleId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    // ──────────────── Governance & Legal Hold ────────────────

    public async Task<LegalHoldDto?> GetLegalHoldByIdAsync(Guid holdId, CancellationToken cancellationToken)
    {
        var hold = await _dbContext.LegalHolds
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == holdId, cancellationToken);

        return hold == null ? null : MapToLegalHoldDto(hold);
    }

    public async Task<IReadOnlyList<LegalHoldDto>> GetLegalHoldsAsync(string? status, CancellationToken cancellationToken)
    {
        var query = _dbContext.LegalHolds.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(h => h.Status == status);
        }

        return await query
            .OrderByDescending(h => h.EffectiveFrom)
            .Select(h => MapToLegalHoldDto(h))
            .ToListAsync(cancellationToken);
    }

    public async Task<LegalHoldDto> SaveLegalHoldAsync(LegalHold legalHold, CancellationToken cancellationToken)
    {
        _dbContext.LegalHolds.Add(legalHold);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToLegalHoldDto(legalHold);
    }

    public async Task UpdateLegalHoldAsync(LegalHold legalHold, CancellationToken cancellationToken)
    {
        _dbContext.LegalHolds.Update(legalHold);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<LegalHold?> GetLegalHoldEntityByIdAsync(Guid holdId, CancellationToken cancellationToken)
    {
        return await _dbContext.LegalHolds
            .FirstOrDefaultAsync(h => h.Id == holdId, cancellationToken);
    }

    // ──────────────── Private Mappers ────────────────

    private static UserAccountDto MapToUserDto(UserAccount u)
    {
        return new UserAccountDto(
            u.PrincipalId,
            u.Principal?.DisplayName ?? string.Empty,
            u.Principal?.PrincipalType.ToString() ?? string.Empty,
            u.Principal?.Status.ToString() ?? string.Empty,
            u.Username,
            u.PersonId,
            u.LastLoginAt,
            u.Principal?.CreatedAt ?? DateTimeOffset.UtcNow);
    }

    private static RoleDto MapToRoleDto(Role r)
    {
        return new RoleDto(r.Id, r.Code, r.Name, r.IsSystem, r.Status, r.CreatedAt);
    }

    private static AccessScopeDto MapToScopeDto(AccessScope s)
    {
        return new AccessScopeDto(
            s.Id, s.ScopeType, s.OrgUnitId, s.ProgramId, s.ProgramVersionId,
            s.CohortId, s.CurriculumPathId, s.CourseId, s.CourseOfferingId,
            s.MeasurementPeriodId, s.SubjectPrincipalId, s.IncludeDescendants, s.Checksum);
    }

    private static RoleAssignmentDto MapToAssignmentDto(RoleAssignment ra)
    {
        return new RoleAssignmentDto(
            ra.Id, ra.PrincipalId, ra.Principal?.DisplayName ?? string.Empty,
            ra.RoleId, ra.Role?.Code ?? string.Empty, ra.Role?.Name ?? string.Empty,
            ra.AccessScopeId, ra.AccessScope?.ScopeType ?? string.Empty,
            ra.EffectiveFrom, ra.EffectiveTo, ra.Status,
            ra.Reason, ra.RevokedAt);
    }

    private static LegalHoldDto MapToLegalHoldDto(LegalHold h)
    {
        return new LegalHoldDto(
            h.Id, h.Code, h.Title, h.Reason, h.Status,
            h.EffectiveFrom, h.ReleasedAt, h.CreatedBy);
    }
}
