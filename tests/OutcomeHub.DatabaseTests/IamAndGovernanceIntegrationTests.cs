using Microsoft.EntityFrameworkCore;
using Npgsql;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.Common.Security;
using OutcomeHub.Application.DTOs.Iam;
using OutcomeHub.Infrastructure.Persistence;
using OutcomeHub.Infrastructure.Persistence.Interceptors;
using OutcomeHub.Infrastructure.Persistence.Repositories.Audit;
using OutcomeHub.Infrastructure.Persistence.Repositories.Iam;
using OutcomeHub.Infrastructure.Persistence.Rls;
using OutcomeHub.Infrastructure.Services;
using OutcomeHub.Migrations;
using Testcontainers.PostgreSql;
using Xunit;

namespace OutcomeHub.DatabaseTests;

public sealed class IamAndGovernanceIntegrationTests
{
    [Fact(Timeout = 180_000)]
    public async Task CompleteIamAndGovernanceLifecycleSucceedsUnderRls()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using PostgreSqlContainer postgreSql = new PostgreSqlBuilder("postgres:18")
            .WithDatabase("outcomehub_iam_tests")
            .WithUsername("outcomehub_test_owner")
            .WithPassword("outcomehub_test_owner_password")
            .Build();

        await postgreSql.StartAsync(cancellationToken);
        string ownerConnectionString = postgreSql.GetConnectionString();

        // ── Step 1: Provision database roles and run all 14 migrations ──
        string migrationConnectionString = await DatabaseBaselineTests.ProvisionDatabaseRolesAsync(
            ownerConnectionString,
            cancellationToken);

        string migrationRoot = Path.Combine(AppContext.BaseDirectory, "MigrationSql");
        var runner = new SqlMigrationRunner(migrationConnectionString, migrationRoot);
        var migrationResult = await runner.RunAsync(cancellationToken);
        Assert.Equal(17, migrationResult.AppliedCount);

        // ── Step 2: Seed development dataset ──
        await DatabaseBaselineTests.RunDatabaseScriptAsync(
            ownerConnectionString,
            "seed_development_dataset.sql",
            cancellationToken);

        // ── Step 3: App-role connection (RLS-enforced) ──
        var appConnectionString = new NpgsqlConnectionStringBuilder(ownerConnectionString)
        {
            Username = "outcomehub_app",
            Password = "outcomehub_test_app_password",
            Pooling = false,
        }.ConnectionString;

        var dbOptions = new DbContextOptionsBuilder<OutcomeHubDbContext>()
            .UseNpgsql(appConnectionString)
            .AddInterceptors(new RowVersionSaveChangesInterceptor())
            .Options;

        var adminPrincipalId = Guid.Parse("10000000-0000-7000-8000-000000000001");
        var adminContext = new DatabaseRequestContext(adminPrincipalId, Guid.NewGuid(), "IAM Integration Test");

        // ── Step 4: User Account Management ──
        await using var ctx1 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor1 = new RlsTransactionExecutor(ctx1);
        var iamRepo1 = new IamRepository(ctx1);
        var auditRepo1 = new AuditRepository(ctx1);
        var iamService1 = new IamService(iamRepo1, auditRepo1);

        var newUserReq = new CreateUserAccountRequest(
            DisplayName: "TS. Nguyễn Văn Quản Trị",
            PrincipalType: "HUMAN_USER",
            Status: "ACTIVE",
            Username: "quantri_fit",
            PersonId: null);

        var createdUser = await rlsExecutor1.ExecuteAsync(
            adminContext,
            ct => iamService1.CreateUserAsync(newUserReq, ct),
            cancellationToken);

        Assert.NotEqual(Guid.Empty, createdUser.PrincipalId);
        Assert.Equal("TS. Nguyễn Văn Quản Trị", createdUser.DisplayName);
        Assert.Equal("Active", createdUser.Status);
        Assert.Equal("quantri_fit", createdUser.Username);

        // Verify user list & detail
        var userDetail = await rlsExecutor1.ExecuteAsync(
            adminContext,
            ct => iamRepo1.GetUserDetailByIdAsync(createdUser.PrincipalId, ct),
            cancellationToken);

        Assert.NotNull(userDetail);
        Assert.Equal("quantri_fit", userDetail.Username);
        Assert.Empty(userDetail.RoleAssignments);

        // ── Step 5: Role & Permissions ──
        await using var ctx2 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor2 = new RlsTransactionExecutor(ctx2);
        var iamRepo2 = new IamRepository(ctx2);
        var auditRepo2 = new AuditRepository(ctx2);
        var iamService2 = new IamService(iamRepo2, auditRepo2);

        var allPermissions = await rlsExecutor2.ExecuteAsync(
            adminContext,
            ct => iamRepo2.GetAllPermissionsAsync(ct),
            cancellationToken);

        Assert.NotEmpty(allPermissions);
        var samplePermIds = allPermissions.Take(3).Select(p => p.Id).ToList();

        var createRoleReq = new CreateRoleRequest(
            Code: "ROLE_DEAN_ASSISTANT",
            Name: "Trợ lý Ban Chủ nhiệm Khoa",
            IsSystem: false,
            PermissionIds: samplePermIds);

        var createdRole = await rlsExecutor2.ExecuteAsync(
            adminContext,
            ct => iamService2.CreateRoleAsync(createRoleReq, adminPrincipalId, ct),
            cancellationToken);

        Assert.NotEqual(Guid.Empty, createdRole.Id);
        Assert.Equal("ROLE_DEAN_ASSISTANT", createdRole.Code);
        Assert.Equal("ACTIVE", createdRole.Status);

        var roleDetail = await rlsExecutor2.ExecuteAsync(
            adminContext,
            ct => iamRepo2.GetRoleDetailByIdAsync(createdRole.Id, ct),
            cancellationToken);

        Assert.NotNull(roleDetail);
        Assert.Equal(samplePermIds.Count, roleDetail.Permissions.Count);

        // ── Step 6: Access Scopes ──
        await using var ctx3 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor3 = new RlsTransactionExecutor(ctx3);
        var iamRepo3 = new IamRepository(ctx3);
        var auditRepo3 = new AuditRepository(ctx3);
        var iamService3 = new IamService(iamRepo3, auditRepo3);

        // Get an OrgUnit ID for scope
        Guid fitOrgId = await rlsExecutor3.ExecuteAsync(
            adminContext,
            async ct =>
            {
                return await ctx3.Set<OutcomeHub.Domain.Entities.Academic.OrgUnit>()
                    .AsNoTracking()
                    .Where(o => o.Code == "FIT")
                    .Select(o => o.Id)
                    .FirstAsync(ct);
            },
            cancellationToken);

        var createScopeReq = new CreateAccessScopeRequest(
            ScopeType: "ORG_UNIT",
            OrgUnitId: fitOrgId,
            ProgramId: null,
            ProgramVersionId: null,
            CohortId: null,
            CurriculumPathId: null,
            CourseId: null,
            CourseOfferingId: null,
            MeasurementPeriodId: null,
            SubjectPrincipalId: null,
            IncludeDescendants: true);

        var createdScope = await rlsExecutor3.ExecuteAsync(
            adminContext,
            ct => iamService3.CreateAccessScopeAsync(createScopeReq, ct),
            cancellationToken);

        Assert.NotEqual(Guid.Empty, createdScope.Id);
        Assert.Equal("ORG_UNIT", createdScope.ScopeType);
        Assert.Equal(fitOrgId, createdScope.OrgUnitId);
        Assert.True(createdScope.IncludeDescendants);
        Assert.NotEmpty(createdScope.Checksum);

        // ── Step 7: Separation of Duties (SoD) ──
        await using var ctx4 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor4 = new RlsTransactionExecutor(ctx4);
        var iamRepo4 = new IamRepository(ctx4);
        var auditRepo4 = new AuditRepository(ctx4);
        var iamService4 = new IamService(iamRepo4, auditRepo4);

        var sodRules = await rlsExecutor4.ExecuteAsync(
            adminContext,
            ct => iamRepo4.GetSodRulesAsync(ct),
            cancellationToken);

        // Check SoD validation - no violation for clean role
        var sodCheck = await rlsExecutor4.ExecuteAsync(
            adminContext,
            ct => iamService4.CheckSodViolationAsync(
                new CheckSodViolationRequest(createdUser.PrincipalId, createdRole.Id, createdScope.Id),
                ct),
            cancellationToken);

        Assert.NotNull(sodCheck);
        Assert.False(sodCheck.HasViolation);

        // ── Step 8: Role Assignment & Revocation ──
        await using var ctx5 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor5 = new RlsTransactionExecutor(ctx5);
        var iamRepo5 = new IamRepository(ctx5);
        var auditRepo5 = new AuditRepository(ctx5);
        var iamService5 = new IamService(iamRepo5, auditRepo5);

        var assignReq = new AssignRoleRequest(
            PrincipalId: createdUser.PrincipalId,
            RoleId: createdRole.Id,
            AccessScopeId: createdScope.Id,
            EffectiveFrom: DateTimeOffset.UtcNow.AddMinutes(-5),
            EffectiveTo: DateTimeOffset.UtcNow.AddYears(1),
            Reason: "Phân công nhiệm vụ Trợ lý Ban chủ nhiệm Khoa CNTT năm học 2026-2027");

        var assignment = await rlsExecutor5.ExecuteAsync(
            adminContext,
            ct => iamService5.AssignRoleAsync(assignReq, adminPrincipalId, ct),
            cancellationToken);

        Assert.NotEqual(Guid.Empty, assignment.Id);
        Assert.Equal(createdUser.PrincipalId, assignment.PrincipalId);
        Assert.Equal(createdRole.Id, assignment.RoleId);
        Assert.Equal("ACTIVE", assignment.Status);

        // Verify active assignments for user
        var userAssignments = await rlsExecutor5.ExecuteAsync(
            adminContext,
            ct => iamRepo5.GetRoleAssignmentsByPrincipalAsync(createdUser.PrincipalId, ct),
            cancellationToken);

        Assert.Single(userAssignments);
        Assert.Equal("ROLE_DEAN_ASSISTANT", userAssignments[0].RoleCode);

        // Revoke the role assignment
        var revokedAssignment = await rlsExecutor5.ExecuteAsync(
            adminContext,
            ct => iamService5.RevokeRoleAssignmentAsync(
                assignment.Id,
                new RevokeRoleAssignmentRequest("Hết thời gian phân công nhiệm vụ"),
                ct),
            cancellationToken);

        Assert.Equal("REVOKED", revokedAssignment.Status);
        Assert.NotNull(revokedAssignment.RevokedAt);

        // ── Step 9: Immutable Audit Trail ──
        await using var ctx6 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor6 = new RlsTransactionExecutor(ctx6);
        var iamRepo6 = new IamRepository(ctx6);
        var auditRepo6 = new AuditRepository(ctx6);
        var iamService6 = new IamService(iamRepo6, auditRepo6);

        var auditLog = await rlsExecutor6.ExecuteAsync(
            adminContext,
            ct => iamService6.LogSecurityEventAsync(
                actorPrincipalId: adminPrincipalId,
                actorKind: "HUMAN_USER",
                action: "ASSIGN_ROLE",
                category: "IAM",
                outcome: "SUCCESS",
                resourceType: "iam.role_assignment",
                resourceId: assignment.Id,
                purpose: "Phân công quyền quản trị",
                reason: "Bảo đảm tuân thủ ISO/IEC 27001",
                classification: "CONFIDENTIAL",
                cancellationToken: ct),
            cancellationToken);

        Assert.NotEqual(Guid.Empty, auditLog.Id);
        Assert.Equal("ASSIGN_ROLE", auditLog.Action);
        Assert.Equal("SUCCESS", auditLog.Outcome);
        Assert.NotEmpty(auditLog.EventHash);

        // Query audit logs
        var queriedLogs = await rlsExecutor6.ExecuteAsync(
            adminContext,
            ct => auditRepo6.QueryAuditLogsAsync(
                new QueryAuditLogsRequest(adminPrincipalId, "ASSIGN_ROLE", "IAM", null, null, null, 1, 10),
                ct),
            cancellationToken);

        Assert.NotEmpty(queriedLogs);
        Assert.Contains(queriedLogs, l => l.Id == auditLog.Id);

        // ── Step 10: Governance & Legal Hold ──
        await using var ctx7 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor7 = new RlsTransactionExecutor(ctx7);
        var iamRepo7 = new IamRepository(ctx7);
        var auditRepo7 = new AuditRepository(ctx7);
        var iamService7 = new IamService(iamRepo7, auditRepo7);

        var holdReq = new CreateLegalHoldRequest(
            Code: "HOLD-2026-EXAM-AUDIT",
            Title: "Đóng băng dữ liệu đợt thi học kỳ 2 năm học 2025-2026",
            Reason: "Thực hiện thanh tra khảo thí định kỳ",
            EffectiveFrom: DateTimeOffset.UtcNow);

        var legalHold = await rlsExecutor7.ExecuteAsync(
            adminContext,
            ct => iamService7.CreateLegalHoldAsync(holdReq, adminPrincipalId, ct),
            cancellationToken);

        Assert.NotEqual(Guid.Empty, legalHold.Id);
        Assert.Equal("HOLD-2026-EXAM-AUDIT", legalHold.Code);
        Assert.Equal("ACTIVE", legalHold.Status);

        // Release legal hold
        var releasedHold = await rlsExecutor7.ExecuteAsync(
            adminContext,
            ct => iamService7.ReleaseLegalHoldAsync(legalHold.Id, ct),
            cancellationToken);

        Assert.Equal("RELEASED", releasedHold.Status);
        Assert.NotNull(releasedHold.ReleasedAt);
    }
}
