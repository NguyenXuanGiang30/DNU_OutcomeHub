using Microsoft.EntityFrameworkCore;
using Npgsql;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.Common.Security;
using OutcomeHub.Application.DTOs.Portfolio;
using OutcomeHub.Infrastructure.Persistence;
using OutcomeHub.Infrastructure.Persistence.Interceptors;
using OutcomeHub.Infrastructure.Persistence.Repositories.Portfolio;
using OutcomeHub.Infrastructure.Persistence.Rls;
using OutcomeHub.Infrastructure.Services;
using OutcomeHub.Migrations;
using Testcontainers.PostgreSql;
using Xunit;

namespace OutcomeHub.DatabaseTests;

public sealed class ExamBlueprintAndPortfolioIntegrationTests
{
    [Fact(Timeout = 180_000)]
    public async Task CompleteExamBlueprintAndPortfolioLifecycleSucceedsUnderRls()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using PostgreSqlContainer postgreSql = new PostgreSqlBuilder("postgres:18")
            .WithDatabase("outcomehub_exam_tests")
            .WithUsername("outcomehub_test_owner")
            .WithPassword("outcomehub_test_owner_password")
            .Build();

        await postgreSql.StartAsync(cancellationToken);
        string ownerConnectionString = postgreSql.GetConnectionString();

        // ── Step 1: Provision database roles and run all 17 migrations ──
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
        var adminContext = new DatabaseRequestContext(adminPrincipalId, Guid.NewGuid(), "Exam Blueprint Test");

        await using var ctx = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor = new RlsTransactionExecutor(ctx);
        var examRepo = new ExamBlueprintRepository(ctx);
        var examService = new ExamBlueprintService(examRepo);

        var syllabusVersionId = Guid.Parse("58000000-0000-7000-8000-000000000001");
        var assessmentItemId = Guid.Parse("59000000-0000-7000-8000-000000000001");

        // ── Step 4: Exam Blueprint / Matrix (FR-PRT-03, FR-PRT-05, FR-PRT-06) ──
        var blueprint = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => examService.GetExamBlueprintAsync(syllabusVersionId, assessmentItemId, ct),
            cancellationToken);

        Assert.NotNull(blueprint);
        Assert.Equal(assessmentItemId, blueprint.AssessmentItemId);
        Assert.NotEmpty(blueprint.Sections);
        Assert.NotEmpty(blueprint.IntegrityChecksum);

        var saveBlueprintRequest = new CreateExamBlueprintRequest(
            syllabusVersionId,
            assessmentItemId,
            90,
            10.0m,
            [
                new("SEC1", "Trắc nghiệm lý thuyết", 0.4m, [
                    new(1, "Câu 1", "Khái niệm", 2.0m, "UNDERSTAND", Guid.NewGuid(), null, 0m),
                    new(2, "Câu 2", "Nguyên lý", 2.0m, "UNDERSTAND", Guid.NewGuid(), null, 0m)
                ]),
                new("SEC2", "Tự luận và thực hành", 0.6m, [
                    new(3, "Câu 3", "Cài đặt thuật toán", 3.0m, "APPLY", Guid.NewGuid(), Guid.NewGuid(), 50.0m),
                    new(4, "Câu 4", "Tối ưu hóa hệ thống", 3.0m, "CREATE", Guid.NewGuid(), Guid.NewGuid(), 50.0m)
                ])
            ]);

        var savedBlueprint = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => examService.SaveExamBlueprintAsync(saveBlueprintRequest, ct),
            cancellationToken);

        Assert.NotNull(savedBlueprint);
        Assert.Equal(2, savedBlueprint.Sections.Count);

        // ── Step 5: Traceability Matrix Table 8.3.1 (FR-PRT-17) ──
        var table831 = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => examService.GetTraceabilityMatrix831Async(syllabusVersionId, ct),
            cancellationToken);

        Assert.NotNull(table831);
        Assert.NotEmpty(table831.Rows);
        Assert.Contains(table831.Rows, r => r.IsDirectAssessment);

        // ── Step 6: Direct Assessment Matrix Table 8.3.2 (FR-PRT-05, FR-PRT-18, FR-PRT-19) ──
        var table832 = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => examService.GetDirectAssessmentMatrix832Async(syllabusVersionId, ct),
            cancellationToken);

        Assert.NotNull(table832);
        Assert.NotEmpty(table832.Rows);
        Assert.True(table832.IsWeightSumValid);

        // ── Step 7: Teaching Schedule & Weekly Lesson Plan (FR-PRT-20) ──
        var weeklySchedule = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => examService.GetWeeklyScheduleAsync(syllabusVersionId, ct),
            cancellationToken);

        Assert.NotNull(weeklySchedule);
        Assert.NotEmpty(weeklySchedule.Sessions);

        var saveScheduleRequest = new SaveWeeklyScheduleRequest(
            syllabusVersionId,
            [
                new(1, 1, "Tổng quan kiến trúc phần mềm", ["LLO1"], ["CLO1"], 3, 6, "LECTURE", "Chương 1", "Quiz 1", "Cài đặt IDE"),
                new(2, 1, "Mô hình phân lớp và Repository pattern", ["LLO2"], ["CLO1"], 3, 6, "PRACTICE", "Chương 2", "Lab 1", "Làm bài Lab 1"),
                new(3, 2, "RESTful API và Middleware bảo mật", ["LLO3"], ["CLO2"], 3, 6, "WORKSHOP", "Chương 3", "Lab 2", "Hoàn thiện Lab 2")
            ]);

        var updatedSchedule = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => examService.SaveWeeklyScheduleAsync(saveScheduleRequest, ct),
            cancellationToken);

        Assert.NotNull(updatedSchedule);
        Assert.Equal(3, updatedSchedule.Sessions.Count);

        // ── Step 8: Academic Document Vault & Artifact Upload (FR-PRT-07, FR-PRT-08, FR-PRT-10, FR-PRT-11) ──
        var documents = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => examService.GetDocumentsAsync(syllabusVersionId, ct),
            cancellationToken);

        Assert.NotNull(documents);
        Assert.NotEmpty(documents);

        var uploadRequest = new UploadDocumentRequest(
            syllabusVersionId,
            "STUDENT_EVIDENCE",
            "Bài tập lớn mẫu của sinh viên đạt loại Giỏi",
            "Sample_Project_A_Plus.zip",
            "application/zip",
            1024 * 1024 * 5,
            "UEsDBBQAAAAIAAAAAAAAAAAAAAAAAAAAAAA",
            adminPrincipalId);

        var uploadedDoc = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => examService.UploadDocumentAsync(uploadRequest, ct),
            cancellationToken);

        Assert.NotNull(uploadedDoc);
        Assert.Equal("CLEAN", uploadedDoc.VirusScanStatus);
        Assert.NotEmpty(uploadedDoc.Sha256Checksum);

        // ── Step 9: Portfolio Package Engine Export (FR-PRT-12) ──
        var packageRequest = new ExportPortfolioPackageRequest(
            syllabusVersionId,
            "2023-2024",
            "HK1",
            "DNU OUTCOMEHUB - CONFIDENTIAL ACADEMIC PORTFOLIO");

        var package = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => examService.ExportPortfolioPackageAsync(packageRequest, ct),
            cancellationToken);

        Assert.NotNull(package);
        Assert.NotEmpty(package.TableOfContents);
        Assert.NotEmpty(package.ManifestChecksum);
        Assert.Equal("DNU OUTCOMEHUB - CONFIDENTIAL ACADEMIC PORTFOLIO", package.WatermarkText);

        // ── Step 10: AI Syllabus Draft Assistant (FR-PRT-09) ──
        var aiDraftRequest = new AiSyllabusDraftRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Lập trình Nâng cao",
            3,
            ["PLO5", "PLO6"],
            "Công nghệ Phần mềm");

        var aiDraft = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => examService.GenerateAiSyllabusDraftAsync(aiDraftRequest, ct),
            cancellationToken);

        Assert.NotNull(aiDraft);
        Assert.NotEmpty(aiDraft.GeneratedClos);
        Assert.NotEmpty(aiDraft.GeneratedTeachingSchedule);
        Assert.NotEmpty(aiDraft.GeneratedAssessmentBlueprint);

        // ── Step 11: Syllabus Publishing Readiness Gatekeeper (FR-PRT-21) ──
        var publishingChecklist = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => examService.ValidateSyllabusPublishingReadinessAsync(syllabusVersionId, ct),
            cancellationToken);

        Assert.NotNull(publishingChecklist);
        Assert.Equal(5, publishingChecklist.Gates.Count);
        Assert.True(publishingChecklist.IsReadyForPublishing);
    }
}
