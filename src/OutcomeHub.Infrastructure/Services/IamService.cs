using System.Security.Cryptography;
using System.Text;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.DTOs.Iam;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;
using OutcomeHub.Domain.Entities.Audit;
using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Iam;
using OutcomeHub.Domain.Enums.Iam;

namespace OutcomeHub.Infrastructure.Services;

public sealed class IamService : IIamService
{
    private readonly IIamRepository _iamRepository;
    private readonly IAuditRepository _auditRepository;

    public IamService(
        IIamRepository iamRepository,
        IAuditRepository auditRepository)
    {
        _iamRepository = iamRepository ?? throw new ArgumentNullException(nameof(iamRepository));
        _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
    }

    public async Task<UserAccountDto> CreateUserAsync(
        CreateUserAccountRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!Enum.TryParse<PrincipalType>(request.PrincipalType, true, out var principalType))
        {
            principalType = PrincipalType.User;
        }

        if (!Enum.TryParse<PrincipalStatus>(request.Status, true, out var status))
        {
            status = PrincipalStatus.Active;
        }

        var principal = Principal.Create(
            principalType,
            status,
            request.DisplayName,
            DateTimeOffset.UtcNow);

        var userAccount = UserAccount.Create(
            principal.Id,
            request.PersonId,
            request.Username,
            null,
            null);

        return await _iamRepository.SaveUserAsync(principal, userAccount, cancellationToken);
    }

    public async Task<RoleDto> CreateRoleAsync(
        CreateRoleRequest request,
        Guid createdByPrincipalId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var now = DateTimeOffset.UtcNow;
        var roleId = Guid.NewGuid();
        var roleVersionId = Guid.NewGuid();
        var workflowInstanceId = Guid.NewGuid();

        var role = Role.Create(
            roleId,
            request.Code,
            request.Name,
            request.IsSystem,
            "ACTIVE",
            now);

        var permChecksum = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(string.Join(";", (request.PermissionIds ?? []).OrderBy(x => x))))).ToLowerInvariant();

        var roleChecksum = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes($"{request.Code}|{request.Name}|1|{permChecksum}"))).ToLowerInvariant();

        var roleVersion = RoleVersion.Create(
            roleVersionId,
            roleId,
            1,
            "ACTIVE",
            DateOnly.FromDateTime(DateTime.Today),
            null,
            workflowInstanceId,
            null,
            permChecksum,
            roleChecksum,
            createdByPrincipalId,
            now);

        var permissions = (request.PermissionIds ?? [])
            .Select(pId => RoleVersionPermission.Create(roleVersionId, pId, now, createdByPrincipalId))
            .ToList();

        return await _iamRepository.SaveRoleAsync(role, roleVersion, permissions, cancellationToken);
    }

    public async Task<AccessScopeDto> CreateAccessScopeAsync(
        CreateAccessScopeRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var accessScope = AccessScope.Create(
            Guid.NewGuid(),
            request.ScopeType,
            request.OrgUnitId,
            request.ProgramId,
            request.ProgramVersionId,
            request.CohortId,
            request.CurriculumPathId,
            request.CourseId,
            request.CourseOfferingId,
            request.MeasurementPeriodId,
            request.SubjectPrincipalId,
            request.IncludeDescendants,
            DateTimeOffset.UtcNow);

        return await _iamRepository.SaveAccessScopeAsync(accessScope, cancellationToken);
    }

    public async Task<RoleAssignmentDto> AssignRoleAsync(
        AssignRoleRequest request,
        Guid grantedByPrincipalId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // 1. Verify SoD compliance
        var sodCheck = await CheckSodViolationAsync(
            new CheckSodViolationRequest(request.PrincipalId, request.RoleId, request.AccessScopeId),
            cancellationToken);

        if (sodCheck.HasViolation)
        {
            throw new InvalidOperationException(
                $"Cannot assign role due to Separation of Duties (SoD) violation: {string.Join("; ", sodCheck.Violations)}");
        }

        // 2. Fetch latest active role version
        var latestVersionId = await _iamRepository.GetLatestRoleVersionIdAsync(request.RoleId, cancellationToken)
            ?? throw new NotFoundException($"Active RoleVersion for Role {request.RoleId}", request.RoleId);

        var now = DateTimeOffset.UtcNow;
        var assignmentId = Guid.NewGuid();
        var workflowInstanceId = Guid.NewGuid();
        var sodPolicyVersionId = await _iamRepository.GetActiveSodPolicyVersionIdAsync(cancellationToken);

        var authChecksum = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes($"{request.PrincipalId}|{request.RoleId}|{request.AccessScopeId}|{request.EffectiveFrom:O}|{request.EffectiveTo:O}"))).ToLowerInvariant();

        var assignment = RoleAssignment.Create(
            assignmentId,
            request.PrincipalId,
            request.RoleId,
            latestVersionId,
            request.AccessScopeId,
            request.EffectiveFrom,
            request.EffectiveTo,
            "ACTIVE",
            "MANUAL",
            null,
            grantedByPrincipalId,
            grantedByPrincipalId,
            workflowInstanceId,
            sodPolicyVersionId,
            authChecksum,
            grantedByPrincipalId,
            now,
            now,
            request.Reason);

        return await _iamRepository.SaveRoleAssignmentAsync(assignment, cancellationToken);
    }

    public async Task<RoleAssignmentDto> RevokeRoleAssignmentAsync(
        Guid assignmentId,
        RevokeRoleAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var assignment = await _iamRepository.GetRoleAssignmentEntityByIdAsync(assignmentId, cancellationToken)
            ?? throw new NotFoundException("RoleAssignment", assignmentId);

        assignment.Revoke(request.RevokeReason, DateTimeOffset.UtcNow);
        await _iamRepository.UpdateRoleAssignmentAsync(assignment, cancellationToken);

        return (await _iamRepository.GetRoleAssignmentByIdAsync(assignmentId, cancellationToken))!;
    }

    public async Task<SodViolationCheckResultDto> CheckSodViolationAsync(
        CheckSodViolationRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Get permissions of target role
        var targetRolePerms = await _iamRepository.GetPermissionsForRoleAsync(request.TargetRoleId, cancellationToken);

        // Get all active roles of the principal
        var currentRoleIds = await _iamRepository.GetPrincipalActiveRoleIdsAsync(request.PrincipalId, cancellationToken);

        var currentPerms = new HashSet<Guid>();
        foreach (var rId in currentRoleIds)
        {
            var pIds = await _iamRepository.GetPermissionsForRoleAsync(rId, cancellationToken);
            foreach (var p in pIds) currentPerms.Add(p);
        }

        // Get all SoD rules
        var sodRules = await _iamRepository.GetSodRulesAsync(cancellationToken);
        var violations = new List<string>();

        foreach (var rule in sodRules)
        {
            bool hasAInCurrent = currentPerms.Contains(rule.PermissionAId);
            bool hasBInCurrent = currentPerms.Contains(rule.PermissionBId);
            bool hasAInTarget = targetRolePerms.Contains(rule.PermissionAId);
            bool hasBInTarget = targetRolePerms.Contains(rule.PermissionBId);

            if ((hasAInCurrent && hasBInTarget) || (hasBInCurrent && hasAInTarget) || (hasAInTarget && hasBInTarget))
            {
                violations.Add($"SoD Rule [{rule.Severity}] conflict on resource '{rule.ResourceType}' (Conflict Mode: {rule.ConflictMode})");
            }
        }

        return new SodViolationCheckResultDto(
            violations.Count > 0,
            violations);
    }

    public async Task<AuditLogEntryDto> LogSecurityEventAsync(
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
        CancellationToken cancellationToken)
    {
        var chainId = Guid.Parse("90000000-0000-4000-8000-000000000001"); // Default security audit chain
        var nextSeq = await _auditRepository.GetNextChainSequenceAsync(chainId, cancellationToken);
        var prevHash = await _auditRepository.GetLastEventHashAsync(chainId, cancellationToken);

        var auditEvent = AuditEvent.Create(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            null,
            null,
            actorPrincipalId,
            actorKind,
            action,
            category,
            outcome,
            resourceType,
            resourceId,
            purpose,
            reason,
            classification,
            chainId,
            nextSeq,
            prevHash);

        return await _auditRepository.SaveAuditEventAsync(auditEvent, cancellationToken);
    }

    public async Task<LegalHoldDto> CreateLegalHoldAsync(
        CreateLegalHoldRequest request,
        Guid createdByPrincipalId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var hold = LegalHold.Create(
            Guid.NewGuid(),
            request.Code,
            request.Title,
            request.Reason,
            request.EffectiveFrom,
            createdByPrincipalId,
            createdByPrincipalId);

        return await _iamRepository.SaveLegalHoldAsync(hold, cancellationToken);
    }

    public async Task<LegalHoldDto> ReleaseLegalHoldAsync(
        Guid holdId,
        CancellationToken cancellationToken)
    {
        var hold = await _iamRepository.GetLegalHoldEntityByIdAsync(holdId, cancellationToken)
            ?? throw new NotFoundException("LegalHold", holdId);

        hold.Release(DateTimeOffset.UtcNow);
        await _iamRepository.UpdateLegalHoldAsync(hold, cancellationToken);

        return (await _iamRepository.GetLegalHoldByIdAsync(holdId, cancellationToken))!;
    }
}
