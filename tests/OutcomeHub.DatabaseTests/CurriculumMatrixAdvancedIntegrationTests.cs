using Microsoft.EntityFrameworkCore;
using Npgsql;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.Common.Security;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Infrastructure.Persistence;
using OutcomeHub.Infrastructure.Persistence.Interceptors;
using OutcomeHub.Infrastructure.Persistence.Repositories.Academic;
using OutcomeHub.Infrastructure.Persistence.Rls;
using OutcomeHub.Infrastructure.Services;
using OutcomeHub.Migrations;
using Testcontainers.PostgreSql;
using Xunit;

namespace OutcomeHub.DatabaseTests;

public sealed class CurriculumMatrixAdvancedIntegrationTests
{
    [Fact(Timeout = 180_000)]
    public async Task CompleteCurriculumMatrixAndBloomValidationSucceedsUnderRls()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using PostgreSqlContainer postgreSql = new PostgreSqlBuilder("postgres:18")
            .WithDatabase("outcomehub_matrix_tests")
            .WithUsername("outcomehub_test_owner")
            .WithPassword("outcomehub_test_owner_password")
            .Build();

        await postgreSql.StartAsync(cancellationToken);
        string ownerConnectionString = postgreSql.GetConnectionString();

        // ── Step 1: Provision database roles and run all 16 migrations ──
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
        var adminContext = new DatabaseRequestContext(adminPrincipalId, Guid.NewGuid(), "Curriculum Matrix Test");

        await using var ctx = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor = new RlsTransactionExecutor(ctx);
        var matrixRepo = new CurriculumMatrixRepository(ctx);
        var matrixService = new CurriculumMatrixService(matrixRepo);

        var programVersionId = Guid.Parse("53000000-0000-7000-8000-000000000001");
        var curriculumPathId = Guid.NewGuid();

        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                var path = CurriculumPath.Create(
                    curriculumPathId,
                    programVersionId,
                    "PATH_STD",
                    "Lộ trình Tiêu chuẩn",
                    "MAJOR",
                    new DateOnly(2023, 9, 1),
                    null,
                    true,
                    Guid.Parse("00000000-0000-7000-8000-000000000402"));

                ctx.CurriculumPaths.Add(path);
                await ctx.SaveChangesAsync(ct);
                return true;
            },
            cancellationToken);

        // ── Step 4: Coverage Analysis (FR-CTD-10) ──
        var coverage = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => matrixService.AnalyzeCoverageAsync(programVersionId, curriculumPathId, ct),
            cancellationToken);

        Assert.NotNull(coverage);
        Assert.Equal(programVersionId, coverage.ProgramVersionId);
        Assert.True(coverage.TotalPlos > 0);
        Assert.True(coverage.TotalPis > 0);

        // ── Step 5: Competency Roadmap & Bloom Progression (FR-CTD-11) ──
        var roadmap = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => matrixService.GetCompetencyRoadmapAsync(programVersionId, ct),
            cancellationToken);

        Assert.NotNull(roadmap);
        Assert.NotEmpty(roadmap.Terms);
        Assert.NotEmpty(roadmap.PloBloomEvolutions);

        // ── Step 6: ProgramVersion Diff & Crosswalk (FR-CTD-14 & FR-CTD-23) ──
        var diff = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => matrixService.CompareVersionsAsync(programVersionId, programVersionId, ct),
            cancellationToken);

        Assert.NotNull(diff);
        Assert.NotEmpty(diff.PloDiffs);

        var crosswalk = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => matrixService.GetPloCrosswalkAsync(programVersionId, programVersionId, ct),
            cancellationToken);

        Assert.NotNull(crosswalk);
        Assert.NotEmpty(crosswalk.Rows);

        // ── Step 7: Direct Measurement Plans (DMP) (FR-CTD-15 & FR-CTD-16) ──
        var dmps = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => matrixService.GetDirectMeasurementPlansAsync(programVersionId, curriculumPathId, ct),
            cancellationToken);

        Assert.NotNull(dmps);
        Assert.NotEmpty(dmps);

        var firstPi = dmps[0];
        var saveRequest = new CreateDirectMeasurementPlanRequest(
            programVersionId,
            curriculumPathId,
            firstPi.ProgramPiId,
            [
                new(Guid.NewGuid(), Guid.NewGuid(), 100.0m, true, false)
            ]);

        var savedPlan = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => matrixService.SaveDirectMeasurementPlanAsync(saveRequest, ct),
            cancellationToken);

        Assert.NotNull(savedPlan);
        Assert.Equal(firstPi.ProgramPiId, savedPlan.ProgramPiId);
        Assert.True(savedPlan.MeetsLevelAPolicy);

        // ── Step 8: PO - PLO & 3-Tier Competencies (FR-CTD-20) ──
        var poMatrix = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => matrixService.GetProgramObjectiveMatrixAsync(programVersionId, ct),
            cancellationToken);

        Assert.NotNull(poMatrix);
        Assert.NotEmpty(poMatrix.ProgramObjectives);
        Assert.NotEmpty(poMatrix.PoPloMatrix);
        Assert.Equal(3, poMatrix.CompetencyTiers.Count);

        // ── Step 9: Prerequisite Graph (FR-CTD-21 & FR-CTD-22) ──
        var graph = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => matrixService.GetPrerequisiteGraphAsync(programVersionId, ct),
            cancellationToken);

        Assert.NotNull(graph);
        Assert.NotEmpty(graph.Nodes);
        Assert.NotEmpty(graph.Edges);

        var knowledgeBlocks = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => matrixService.GetKnowledgeBlockStructureAsync(programVersionId, ct),
            cancellationToken);

        Assert.NotNull(knowledgeBlocks);
        Assert.NotEmpty(knowledgeBlocks.KnowledgeBlocks);

        // ── Step 10: Curriculum Specification Document (FR-CTD-24) ──
        var specification = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => matrixService.GetCurriculumSpecificationAsync(programVersionId, ct),
            cancellationToken);

        Assert.NotNull(specification);
        Assert.NotEmpty(specification.IntegrityChecksum);
        Assert.True(specification.TotalCredits > 0);

        // ── Step 11: Publishing Readiness Checklist (FR-CTD-25) ──
        var checklist = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => matrixService.CheckPublishingReadinessAsync(programVersionId, ct),
            cancellationToken);

        Assert.NotNull(checklist);
        Assert.Equal(5, checklist.ChecklistItems.Count);
        Assert.True(checklist.IsReadyForPublishing);
    }
}
