using Microsoft.EntityFrameworkCore;
using Npgsql;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.Common.Security;
using OutcomeHub.Domain.Entities.Quality;
using OutcomeHub.Infrastructure.Persistence;
using OutcomeHub.Infrastructure.Persistence.Interceptors;
using OutcomeHub.Infrastructure.Persistence.Repositories.Quality;
using OutcomeHub.Infrastructure.Persistence.Rls;
using OutcomeHub.Infrastructure.Services;
using OutcomeHub.Migrations;
using Testcontainers.PostgreSql;
using Xunit;

namespace OutcomeHub.DatabaseTests;

public sealed class CqiImprovementIntegrationTests
{
    [Fact(Timeout = 180_000)]
    public async Task CompleteCqiImprovementPlanLifecycleSucceedsUnderRls()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using PostgreSqlContainer postgreSql = new PostgreSqlBuilder("postgres:18")
            .WithDatabase("outcomehub_cqi_tests")
            .WithUsername("outcomehub_test_owner")
            .WithPassword("outcomehub_test_owner_password")
            .Build();

        await postgreSql.StartAsync(cancellationToken);
        string ownerConnectionString = postgreSql.GetConnectionString();

        // ── Step 1: Provision database roles and run all 13 migrations ──
        string migrationConnectionString = await DatabaseBaselineTests.ProvisionDatabaseRolesAsync(
            ownerConnectionString,
            cancellationToken);

        string migrationRoot = Path.Combine(AppContext.BaseDirectory, "MigrationSql");
        var runner = new SqlMigrationRunner(migrationConnectionString, migrationRoot);
        var migrationResult = await runner.RunAsync(cancellationToken);
        Assert.Equal(18, migrationResult.AppliedCount);

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
        Guid orgUnitId, programVersionId, principalId;
        await using (var seedCtx = new OutcomeHubDbContext(dbOptions))
        {
            var rlsExec = new RlsTransactionExecutor(seedCtx);
            var adminContext = new DatabaseRequestContext(adminPrincipalId, Guid.NewGuid(), "CQI test seed data lookup");

            orgUnitId = await rlsExec.ExecuteAsync(
                adminContext,
                async ct =>
                {
                    return await seedCtx.Set<OutcomeHub.Domain.Entities.Academic.OrgUnit>()
                        .AsNoTracking()
                        .Select(o => o.Id)
                        .FirstAsync(ct);
                },
                cancellationToken);

            programVersionId = await rlsExec.ExecuteAsync(
                adminContext,
                async ct =>
                {
                    return await seedCtx.Set<OutcomeHub.Domain.Entities.Academic.ProgramVersion>()
                        .AsNoTracking()
                        .Select(pv => pv.Id)
                        .FirstAsync(ct);
                },
                cancellationToken);

            principalId = adminContext.PrincipalId;
        }

        // ── Step 4: Create CQI Improvement Plan ──
        await using var ctx = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor = new RlsTransactionExecutor(ctx);
        var repository = new ImprovementPlanRepository(ctx);
        var service = new ImprovementPlanService(repository);

        var context = new DatabaseRequestContext(principalId, Guid.NewGuid(), "CQI Integration Test");

        var createRequest = new Application.DTOs.Quality.CreateImprovementPlanRequest(
            OrgUnitId: orgUnitId,
            ProgramVersionId: programVersionId,
            Title: "Kế hoạch cải tiến PLO1 - Khóa 2024",
            ProblemStatement: "Tỷ lệ đạt PLO1 của khóa K24 thấp hơn ngưỡng 70% (đạt 62.5%)",
            RootCauseSummary: "Sinh viên yếu phần tư duy phản biện ở PI1.2, PI1.3",
            BaselineValue: 62.5m,
            TargetValue: 75.0m,
            KpiDefinition: "Tỷ lệ đạt PLO1 cấp khóa (attainment_rate)",
            OwnerPrincipalId: principalId,
            DueDate: DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
            Findings: new[]
            {
                new Application.DTOs.Quality.CreateImprovementFindingRequest(
                    FindingType: "QUALITATIVE_OBSERVATION",
                    AcademicYearStart: null,
                    CohortOutcomeResultId: null,
                    Description: "Sinh viên K24 thiếu kỹ năng tư duy phản biện qua đánh giá gián tiếp",
                    SourceChecksum: null)
            });

        var planDto = await rlsExecutor.ExecuteAsync(
            context,
            ct => service.CreatePlanAsync(createRequest, principalId, ct),
            cancellationToken);

        Assert.NotEqual(Guid.Empty, planDto.Id);
        Assert.Equal("DRAFT", planDto.Status);
        Assert.StartsWith("CQI-", planDto.Code);
        Assert.Equal(62.5m, planDto.BaselineValue);
        Assert.Equal(75.0m, planDto.TargetValue);

        // ── Step 5: Verify plan detail includes findings ──
        var planDetail = await rlsExecutor.ExecuteAsync(
            context,
            ct => repository.GetPlanDetailByIdAsync(planDto.Id, ct),
            cancellationToken);

        Assert.NotNull(planDetail);
        Assert.Single(planDetail.Findings);
        Assert.Equal("QUALITATIVE_OBSERVATION", planDetail.Findings[0].FindingType);

        // ── Step 6: Add improvement actions ──
        await using var ctx2 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor2 = new RlsTransactionExecutor(ctx2);
        var repository2 = new ImprovementPlanRepository(ctx2);
        var service2 = new ImprovementPlanService(repository2);

        var action1 = await rlsExecutor2.ExecuteAsync(
            context,
            ct => service2.AddActionAsync(planDto.Id,
                new Application.DTOs.Quality.CreateImprovementActionRequest(
                    Description: "Tổ chức workshop bổ trợ tư duy phản biện cho K24",
                    OwnerPrincipalId: principalId,
                    OwnerOrgUnitId: orgUnitId,
                    StartDate: DateOnly.FromDateTime(DateTime.Today),
                    DueDate: DateOnly.FromDateTime(DateTime.Today.AddMonths(2))),
                ct),
            cancellationToken);

        Assert.Equal(1, action1.ActionNo);
        Assert.Equal("PLANNED", action1.Status);
        Assert.Equal(0m, action1.CompletionRatio);

        var action2 = await rlsExecutor2.ExecuteAsync(
            context,
            ct => service2.AddActionAsync(planDto.Id,
                new Application.DTOs.Quality.CreateImprovementActionRequest(
                    Description: "Cập nhật rubric PI1.2 bổ sung tiêu chí đánh giá tư duy phản biện",
                    OwnerPrincipalId: principalId,
                    OwnerOrgUnitId: orgUnitId,
                    StartDate: DateOnly.FromDateTime(DateTime.Today),
                    DueDate: DateOnly.FromDateTime(DateTime.Today.AddMonths(3))),
                ct),
            cancellationToken);

        Assert.Equal(2, action2.ActionNo);

        // ── Step 7: Update action progress ──
        await using var ctx3 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor3 = new RlsTransactionExecutor(ctx3);
        var repository3 = new ImprovementPlanRepository(ctx3);
        var service3 = new ImprovementPlanService(repository3);

        var updatedAction = await rlsExecutor3.ExecuteAsync(
            context,
            ct => service3.UpdateActionProgressAsync(action1.Id,
                new Application.DTOs.Quality.UpdateActionProgressRequest(CompletionRatio: 0.5m),
                ct),
            cancellationToken);

        Assert.Equal("IN_PROGRESS", updatedAction.Status);
        Assert.Equal(0.5m, updatedAction.CompletionRatio);

        // ── Step 8: Complete action ──
        await using var ctx4 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor4 = new RlsTransactionExecutor(ctx4);
        var repository4 = new ImprovementPlanRepository(ctx4);
        var service4 = new ImprovementPlanService(repository4);

        var completedAction = await rlsExecutor4.ExecuteAsync(
            context,
            ct => service4.CompleteActionAsync(action1.Id, ct),
            cancellationToken);

        Assert.Equal("COMPLETED", completedAction.Status);
        Assert.Equal(1.0m, completedAction.CompletionRatio);
        Assert.NotNull(completedAction.CompletedAt);

        // ── Step 9: Transition plan status through workflow ──
        await using var ctx5 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor5 = new RlsTransactionExecutor(ctx5);
        var repository5 = new ImprovementPlanRepository(ctx5);
        var service5 = new ImprovementPlanService(repository5);

        // DRAFT → IN_REVIEW
        var planAfterReview = await rlsExecutor5.ExecuteAsync(
            context,
            ct => service5.TransitionPlanStatusAsync(planDto.Id,
                new Application.DTOs.Quality.TransitionPlanStatusRequest("IN_REVIEW", "Gửi thẩm định"),
                ct),
            cancellationToken);
        Assert.Equal("IN_REVIEW", planAfterReview.Status);

        // IN_REVIEW → APPROVED
        await using var ctx6 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor6 = new RlsTransactionExecutor(ctx6);
        var repository6 = new ImprovementPlanRepository(ctx6);
        var service6 = new ImprovementPlanService(repository6);

        var planApproved = await rlsExecutor6.ExecuteAsync(
            context,
            ct => service6.TransitionPlanStatusAsync(planDto.Id,
                new Application.DTOs.Quality.TransitionPlanStatusRequest("APPROVED", "Phê duyệt"),
                ct),
            cancellationToken);
        Assert.Equal("APPROVED", planApproved.Status);

        // APPROVED → EXECUTING
        await using var ctx7 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor7 = new RlsTransactionExecutor(ctx7);
        var repository7 = new ImprovementPlanRepository(ctx7);
        var service7 = new ImprovementPlanService(repository7);

        var planExecuting = await rlsExecutor7.ExecuteAsync(
            context,
            ct => service7.TransitionPlanStatusAsync(planDto.Id,
                new Application.DTOs.Quality.TransitionPlanStatusRequest("EXECUTING", "Bắt đầu triển khai"),
                ct),
            cancellationToken);
        Assert.Equal("EXECUTING", planExecuting.Status);

        // EXECUTING → VERIFYING
        await using var ctx8 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor8 = new RlsTransactionExecutor(ctx8);
        var repository8 = new ImprovementPlanRepository(ctx8);
        var service8 = new ImprovementPlanService(repository8);

        var planVerifying = await rlsExecutor8.ExecuteAsync(
            context,
            ct => service8.TransitionPlanStatusAsync(planDto.Id,
                new Application.DTOs.Quality.TransitionPlanStatusRequest("VERIFYING", "Bắt đầu xác minh"),
                ct),
            cancellationToken);
        Assert.Equal("VERIFYING", planVerifying.Status);

        // VERIFYING → CLOSED
        await using var ctx9 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor9 = new RlsTransactionExecutor(ctx9);
        var repository9 = new ImprovementPlanRepository(ctx9);
        var service9 = new ImprovementPlanService(repository9);

        var planClosed = await rlsExecutor9.ExecuteAsync(
            context,
            ct => service9.TransitionPlanStatusAsync(planDto.Id,
                new Application.DTOs.Quality.TransitionPlanStatusRequest("CLOSED", "Đã hoàn thành cải tiến"),
                ct),
            cancellationToken);
        Assert.Equal("CLOSED", planClosed.Status);

        // ── Step 10: Reopen plan ──
        await using var ctx10 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor10 = new RlsTransactionExecutor(ctx10);
        var repository10 = new ImprovementPlanRepository(ctx10);
        var service10 = new ImprovementPlanService(repository10);

        var planReopened = await rlsExecutor10.ExecuteAsync(
            context,
            ct => service10.TransitionPlanStatusAsync(planDto.Id,
                new Application.DTOs.Quality.TransitionPlanStatusRequest("REOPENED", "Cần bổ sung biện pháp"),
                ct),
            cancellationToken);
        Assert.Equal("REOPENED", planReopened.Status);

        // REOPENED → EXECUTING again
        await using var ctx11 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor11 = new RlsTransactionExecutor(ctx11);
        var repository11 = new ImprovementPlanRepository(ctx11);
        var service11 = new ImprovementPlanService(repository11);

        var planExecuting2 = await rlsExecutor11.ExecuteAsync(
            context,
            ct => service11.TransitionPlanStatusAsync(planDto.Id,
                new Application.DTOs.Quality.TransitionPlanStatusRequest("EXECUTING", "Triển khai lại"),
                ct),
            cancellationToken);
        Assert.Equal("EXECUTING", planExecuting2.Status);

        // ── Step 11: List plans and verify filters ──
        await using var ctx12 = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor12 = new RlsTransactionExecutor(ctx12);
        var repository12 = new ImprovementPlanRepository(ctx12);

        var allPlans = await rlsExecutor12.ExecuteAsync(
            context,
            ct => repository12.GetPlansAsync(null, null, null, ct),
            cancellationToken);

        Assert.NotEmpty(allPlans);
        Assert.Contains(allPlans, p => p.Id == planDto.Id);

        // Filter by status
        var executingPlans = await rlsExecutor12.ExecuteAsync(
            context,
            ct => repository12.GetPlansAsync(null, null, "EXECUTING", ct),
            cancellationToken);

        Assert.Contains(executingPlans, p => p.Id == planDto.Id);

        // ── Step 12: CQI Dashboard ──
        var dashboard = await rlsExecutor12.ExecuteAsync(
            context,
            ct => repository12.GetCqiDashboardAsync(null, null, ct),
            cancellationToken);

        Assert.True(dashboard.TotalPlans >= 1);
        Assert.True(dashboard.ExecutingCount >= 1);

        // ── Step 13: Verify final plan detail ──
        var finalDetail = await rlsExecutor12.ExecuteAsync(
            context,
            ct => repository12.GetPlanDetailByIdAsync(planDto.Id, ct),
            cancellationToken);

        Assert.NotNull(finalDetail);
        Assert.Equal("EXECUTING", finalDetail.Status);
        Assert.NotEmpty(finalDetail.Findings);
        Assert.True(finalDetail.Actions.Count >= 2);
    }
}
