using Microsoft.EntityFrameworkCore;
using Npgsql;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.Common.Security;
using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Infrastructure.Persistence;
using OutcomeHub.Infrastructure.Persistence.Interceptors;
using OutcomeHub.Infrastructure.Persistence.Repositories.Analytics;
using OutcomeHub.Infrastructure.Persistence.Rls;
using OutcomeHub.Infrastructure.Services;
using OutcomeHub.Migrations;
using Testcontainers.PostgreSql;
using Xunit;

namespace OutcomeHub.DatabaseTests;

public sealed class DashboardAndAccreditationIntegrationTests
{
    [Fact(Timeout = 180_000)]
    public async Task CompleteDashboardAndAccreditationReportsSucceedUnderRls()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using PostgreSqlContainer postgreSql = new PostgreSqlBuilder("postgres:18")
            .WithDatabase("outcomehub_dashboard_tests")
            .WithUsername("outcomehub_test_owner")
            .WithPassword("outcomehub_test_owner_password")
            .Build();

        await postgreSql.StartAsync(cancellationToken);
        string ownerConnectionString = postgreSql.GetConnectionString();

        // ── Step 1: Provision database roles and run all 15 migrations ──
        string migrationConnectionString = await DatabaseBaselineTests.ProvisionDatabaseRolesAsync(
            ownerConnectionString,
            cancellationToken);

        string migrationRoot = Path.Combine(AppContext.BaseDirectory, "MigrationSql");
        var runner = new SqlMigrationRunner(migrationConnectionString, migrationRoot);
        var migrationResult = await runner.RunAsync(cancellationToken);
        Assert.Equal(19, migrationResult.AppliedCount);

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
        var adminContext = new DatabaseRequestContext(adminPrincipalId, Guid.NewGuid(), "Dashboard Integration Test");

        await using var ctx = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor = new RlsTransactionExecutor(ctx);
        var dashboardRepo = new DashboardRepository(ctx);
        var reportRepo = new AccreditationReportRepository(ctx);
        var dashboardService = new DashboardService(dashboardRepo);
        var reportService = new AccreditationReportService(reportRepo);

        var fitOrgId = Guid.Parse("00000000-0000-7000-8000-000000000002");
        var programId = Guid.Parse("30000000-0000-7000-8000-000000000001");
        var itProgramVersionId = Guid.Parse("53000000-0000-7000-8000-000000000001");

        // Seed Cohort, Student, Staff for tests
        var cohortId = Guid.NewGuid();
        var studentPersonId = Guid.NewGuid();
        var staffPersonId = Guid.NewGuid();

        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                var cohort = Cohort.Create(cohortId, programId, "K17_IT_TEST", "Khóa 17 CNTT Test", 2023, new DateOnly(2023, 9, 1), new DateOnly(2027, 6, 30));
                ctx.Cohorts.Add(cohort);

                var studentPerson = Person.Create(studentPersonId, "Nguyễn Văn Sinh Viên", new DateOnly(2023, 9, 1));
                ctx.Persons.Add(studentPerson);

                var student = Student.Create(studentPersonId, "SV2023001", cohortId);
                ctx.Students.Add(student);

                var staffPerson = Person.Create(staffPersonId, "TS. Trần Văn Giảng Viên", new DateOnly(2020, 1, 1));
                ctx.Persons.Add(staffPerson);

                var staff = Staff.Create(staffPersonId, "GV2020001", fitOrgId, "LECTURER");
                ctx.Staff.Add(staff);

                await ctx.SaveChangesAsync(ct);
                return true;
            },
            cancellationToken);

        // ── Step 4: Executive Dashboard (University Level) ──
        var execDash = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => dashboardService.GetUniversityExecutiveDashboardAsync(ct),
            cancellationToken);

        Assert.NotNull(execDash);
        Assert.True(execDash.TotalPrograms > 0);
        Assert.True(execDash.OverallPloAttainmentRate > 0);
        Assert.NotEmpty(execDash.FacultySummaries);

        // ── Step 5: Faculty Dashboard ──
        var facultyDash = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => dashboardService.GetFacultyDashboardAsync(fitOrgId, ct),
            cancellationToken);

        Assert.NotNull(facultyDash);
        Assert.Equal("FIT", facultyDash.OrgUnitCode);
        Assert.True(facultyDash.TotalPrograms > 0);
        Assert.NotEmpty(facultyDash.ProgramSummaries);

        // ── Step 6: Program Dashboard ──
        var programDash = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => dashboardService.GetProgramDashboardAsync(itProgramVersionId, ct),
            cancellationToken);

        Assert.NotNull(programDash);
        Assert.Equal("7480201", programDash.ProgramCode);
        Assert.NotEmpty(programDash.PloAttainments);
        Assert.NotEmpty(programDash.CohortAttainments);

        // ── Step 7: Lecturer Dashboard ──
        var lecturerDash = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => dashboardService.GetLecturerDashboardAsync(staffPersonId, ct),
            cancellationToken);

        Assert.NotNull(lecturerDash);
        Assert.Equal(staffPersonId, lecturerDash.LecturerId);
        Assert.Equal("TS. Trần Văn Giảng Viên", lecturerDash.LecturerName);

        // ── Step 8: Student Outcome Dashboard ──
        var studentDash = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => dashboardService.GetStudentDashboardAsync(studentPersonId, ct),
            cancellationToken);

        Assert.NotNull(studentDash);
        Assert.Equal(studentPersonId, studentDash.StudentId);
        Assert.NotEmpty(studentDash.PloCompetencies);

        // ── Step 9: Multi-tier Drill-down Tree ──
        var drillDownTree = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => dashboardService.GetDrillDownTreeAsync("PROGRAM", itProgramVersionId, ct),
            cancellationToken);

        Assert.NotNull(drillDownTree);
        Assert.Equal("PROGRAM_VERSION", drillDownTree.NodeType);
        Assert.NotEmpty(drillDownTree.Children);
        Assert.Equal("PLO", drillDownTree.Children[0].NodeType);

        // ── Step 10: System Alerts ──
        var alerts = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => dashboardService.GetAlertsAsync(fitOrgId, itProgramVersionId, ct),
            cancellationToken);

        Assert.NotNull(alerts);

        // ── Step 11: MOET Accreditation Report ──
        var moetReport = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => reportService.GetMoetReportAsync(itProgramVersionId, null, ct),
            cancellationToken);

        Assert.NotNull(moetReport);
        Assert.Equal("7480201", moetReport.ProgramCode);
        Assert.NotEmpty(moetReport.PloMatrixAssessments);
        Assert.Contains("Thông tư", moetReport.StandardFramework);

        // ── Step 12: AUN-QA Accreditation Report ──
        var aunReport = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => reportService.GetAunQaReportAsync(itProgramVersionId, ct),
            cancellationToken);

        Assert.NotNull(aunReport);
        Assert.Equal("7480201", aunReport.ProgramCode);
        Assert.NotEmpty(aunReport.ExpectedLearningOutcomes);
        Assert.NotEmpty(aunReport.AlignmentMatrix);

        // ── Step 13: ABET Accreditation Report ──
        var abetReport = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => reportService.GetAbetReportAsync(itProgramVersionId, ct),
            cancellationToken);

        Assert.NotNull(abetReport);
        Assert.Equal("7480201", abetReport.ProgramCode);
        Assert.NotEmpty(abetReport.StudentOutcomes);

        // ── Step 14: Accreditation Dossier Package ──
        var dossier = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => reportService.GetAccreditationDossierAsync(itProgramVersionId, ct),
            cancellationToken);

        Assert.NotNull(dossier);
        Assert.Equal("7480201", dossier.ProgramCode);
        Assert.NotEmpty(dossier.DossierIntegrityChecksum);
        Assert.NotEmpty(dossier.ProgramLearningOutcomes);

        // ── Step 15: Student OBE Transcript Supplement ──
        var transcript = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => reportService.GetStudentObeTranscriptAsync(studentPersonId, ct),
            cancellationToken);

        Assert.NotNull(transcript);
        Assert.Equal(studentPersonId, transcript.StudentId);
        Assert.NotEmpty(transcript.TranscriptVerificationCode);
        Assert.NotEmpty(transcript.PloCompetencies);
    }
}
