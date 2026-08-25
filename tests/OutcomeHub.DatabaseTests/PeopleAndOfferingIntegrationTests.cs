using Microsoft.EntityFrameworkCore;
using Npgsql;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.Common.Security;
using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Portfolio;
using OutcomeHub.Infrastructure.Persistence;
using OutcomeHub.Infrastructure.Persistence.Interceptors;
using OutcomeHub.Infrastructure.Persistence.Repositories.Academic;
using OutcomeHub.Infrastructure.Persistence.Repositories.Portfolio;
using OutcomeHub.Infrastructure.Persistence.Rls;
using OutcomeHub.Migrations;
using Testcontainers.PostgreSql;
using Xunit;

namespace OutcomeHub.DatabaseTests;

public sealed class PeopleAndOfferingIntegrationTests
{
    [Fact(Timeout = 180_000)]
    public async Task StudentsStaffAndCourseOfferingsExecuteSuccessfullyUnderRls()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using PostgreSqlContainer postgreSql = new PostgreSqlBuilder("postgres:18")
            .WithDatabase("outcomehub_people_tests")
            .WithUsername("outcomehub_test_owner")
            .WithPassword("outcomehub_test_owner_password")
            .Build();

        await postgreSql.StartAsync(cancellationToken);
        string ownerConnectionString = postgreSql.GetConnectionString();

        string migrationConnectionString = await DatabaseBaselineTests.ProvisionDatabaseRolesAsync(
            ownerConnectionString,
            cancellationToken);

        string migrationRoot = Path.Combine(AppContext.BaseDirectory, "MigrationSql");
        var runner = new SqlMigrationRunner(migrationConnectionString, migrationRoot);
        var migrationResult = await runner.RunAsync(cancellationToken);
        Assert.Equal(12, migrationResult.AppliedCount);

        // Seed development dataset
        await DatabaseBaselineTests.RunDatabaseScriptAsync(
            ownerConnectionString,
            "seed_development_dataset.sql",
            cancellationToken);

        // App role connection (RLS-enforced)
        var appConnectionString = new NpgsqlConnectionStringBuilder(ownerConnectionString)
        {
            Username = "outcomehub_app",
            Password = "outcomehub_test_app_password",
            Pooling = false,
        }.ConnectionString;

        var optionsBuilder = new DbContextOptionsBuilder<OutcomeHubDbContext>()
            .UseNpgsql(appConnectionString)
            .AddInterceptors(new RowVersionSaveChangesInterceptor());

        await using var dbContext = new OutcomeHubDbContext(optionsBuilder.Options);
        var rlsExecutor = new RlsTransactionExecutor(dbContext);

        var cohortRepo = new CohortRepository(dbContext);
        var studentRepo = new StudentRepository(dbContext);
        var staffRepo = new StaffRepository(dbContext);
        var offeringRepo = new CourseOfferingRepository(dbContext);
        var courseRepo = new CourseRepository(dbContext);
        var syllabusRepo = new SyllabusRepository(dbContext);

        var adminPrincipalId = Guid.Parse("10000000-0000-7000-8000-000000000001");
        var adminContext = new DatabaseRequestContext(adminPrincipalId, Guid.NewGuid(), "Admin manages people and offerings");

        var fitOrgId = Guid.Parse("00000000-0000-7000-8000-000000000002");
        var programId = Guid.Parse("30000000-0000-7000-8000-000000000001");
        var k17VersionId = Guid.Parse("53000000-0000-7000-8000-000000000001");
        var decisionId = Guid.Parse("50000000-0000-7000-8000-000000000001");
        var defaultWorkflowDefId = Guid.Parse("00000000-0000-7000-8000-000000000401");

        // 1. Create Cohort (Khóa tuyển sinh K17)
        var cohortId = Guid.NewGuid();
        var cohort = Cohort.Create(
            cohortId,
            programId,
            "K17_IT",
            "Khóa 17 Công nghệ Thông tin (2023 - 2027)",
            admissionYear: 2023,
            startDate: new DateOnly(2023, 9, 1),
            endDate: new DateOnly(2027, 6, 30));

        var createdCohort = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => cohortRepo.CreateCohortAsync(cohort, ct),
            cancellationToken);

        Assert.Equal("K17_IT", createdCohort.Code);

        // 2. Create Student & StudentPath (Sinh viên và Lộ trình CTĐT)
        var studentPersonId = Guid.NewGuid();
        var studentPerson = Person.Create(
            studentPersonId,
            "Nguyễn Văn An",
            effectiveFrom: new DateOnly(2023, 9, 1),
            status: "ACTIVE");

        var student = Student.Create(
            studentPersonId,
            "23010001",
            cohortId,
            currentStatus: "ACTIVE");

        var createdStudent = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => studentRepo.CreateStudentAsync(studentPerson, student, ct),
            cancellationToken);

        Assert.Equal("23010001", createdStudent.StudentCode);

        // Insert curriculum path for test
        var curriculumPathId = Guid.NewGuid();
        var pathWorkflowId = Guid.NewGuid();
        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO workflow.instance (id, definition_id, current_state, started_by, started_at, row_version) VALUES ({pathWorkflowId}, {defaultWorkflowDefId}, 'APPROVED', {adminPrincipalId}, CURRENT_TIMESTAMP, 1)",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO academic.curriculum_path (id, program_version_id, code, name, path_type, effective_from, effective_to, is_default, workflow_instance_id) VALUES ({curriculumPathId}, {k17VersionId}, 'PATH_SE', 'Chuyên ngành Kỹ thuật Phần mềm', 'MAJOR', DATE '2023-09-01', NULL, true, {pathWorkflowId})",
                    ct);
                return true;
            },
            cancellationToken);

        var studentPathId = Guid.NewGuid();
        var studentPath = StudentPath.Create(
            studentPathId,
            studentPersonId,
            programId,
            k17VersionId,
            curriculumPathId,
            effectiveFrom: new DateOnly(2023, 9, 1),
            effectiveTo: null,
            pathStatus: "ACTIVE",
            decisionId: decisionId,
            isPrimary: true);

        await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => studentRepo.AssignStudentPathAsync(studentPath, ct),
            cancellationToken);

        var loadedStudent = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => studentRepo.GetStudentByIdAsync(studentPersonId, ct),
            cancellationToken);

        Assert.NotNull(loadedStudent);
        Assert.Equal("Nguyễn Văn An", loadedStudent.FullName);
        Assert.NotEmpty(loadedStudent.Paths!);
        Assert.Equal("7480201_K17", loadedStudent.Paths![0].ProgramVersionCode);

        // 3. Create Staff (Giảng viên & Cán bộ)
        var staffPersonId = Guid.NewGuid();
        var staffPerson = Person.Create(
            staffPersonId,
            "TS. Trần Văn Bình",
            effectiveFrom: new DateOnly(2020, 1, 1),
            status: "ACTIVE");

        var staff = Staff.Create(
            staffPersonId,
            "GV_CNTT_001",
            fitOrgId,
            staffType: "LECTURER",
            currentStatus: "ACTIVE");

        var createdStaff = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => staffRepo.CreateStaffAsync(staffPerson, staff, ct),
            cancellationToken);

        Assert.Equal("GV_CNTT_001", createdStaff.StaffCode);

        var loadedStaff = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => staffRepo.GetStaffByIdAsync(staffPersonId, ct),
            cancellationToken);

        Assert.NotNull(loadedStaff);
        Assert.Equal("TS. Trần Văn Bình", loadedStaff.FullName);

        // 4. Create Course, CourseVersion, ProgramCourse, Syllabus, SyllabusVersion for Offering
        var courseId = Guid.NewGuid();
        var course = Course.Create(courseId, "IT4105", "Phát triển Web Nâng cao", fitOrgId, "DRAFT");
        await rlsExecutor.ExecuteAsync(adminContext, ct => courseRepo.CreateCourseAsync(course, ct), cancellationToken);

        var courseWorkflowId = Guid.NewGuid();
        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO workflow.instance (id, definition_id, current_state, started_by, started_at, row_version) VALUES ({courseWorkflowId}, {defaultWorkflowDefId}, 'DRAFT', {adminPrincipalId}, CURRENT_TIMESTAMP, 1)",
                    ct);
                return true;
            },
            cancellationToken);

        var courseVersionId = Guid.NewGuid();
        var courseVersion = CourseVersion.Create(
            courseVersionId,
            courseId,
            1,
            "Phát triển Web Nâng cao",
            3.0m,
            "STANDARD",
            new DateOnly(2023, 9, 1),
            null,
            false,
            "DRAFT",
            decisionId,
            courseWorkflowId,
            null,
            Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));

        await rlsExecutor.ExecuteAsync(adminContext, ct => courseRepo.CreateCourseVersionAsync(courseVersion, ct), cancellationToken);

        var curriculumPlanId = Guid.NewGuid();
        var curriculumBlockId = Guid.NewGuid();
        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO academic.curriculum_plan (id, program_version_id, code, name, declared_total_credits, status, checksum) VALUES ({curriculumPlanId}, {k17VersionId}, 'PLAN_K17_WEB', 'Kế hoạch Web K17', 132.0, 'ACTIVE', repeat('8', 64))",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO academic.curriculum_block (id, curriculum_plan_id, parent_id, code, name, block_type, required_credits, sort_order) VALUES ({curriculumBlockId}, {curriculumPlanId}, NULL, 'BLOCK_WEB', 'Khối Chuyên đề Web', 'SPECIALIZED', 12.0, 1)",
                    ct);
                return true;
            },
            cancellationToken);

        var programCourseId = Guid.NewGuid();
        var programCourse = ProgramCourse.Create(
            programCourseId,
            k17VersionId,
            courseVersionId,
            curriculumBlockId,
            "REQUIRED",
            null,
            false,
            "DRAFT");

        await rlsExecutor.ExecuteAsync(adminContext, ct => courseRepo.AddCourseToProgramAsync(programCourse, ct), cancellationToken);

        var syllabusId = Guid.NewGuid();
        var syllabus = Syllabus.Create(syllabusId, programCourseId, "DCCT-IT4105", fitOrgId);
        await rlsExecutor.ExecuteAsync(adminContext, ct => syllabusRepo.CreateSyllabusAsync(syllabus, ct), cancellationToken);

        var syllabusTemplateId = Guid.NewGuid();
        var syllabusTemplateVersionId = Guid.NewGuid();
        var instTemplateVersionId = Guid.Parse("52000000-0000-7000-8000-000000000001");
        var syllabusTemplateWorkflowId = Guid.Parse("00000000-0000-7000-8000-000000000402");

        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO portfolio.syllabus_template (id, code, name, owner_org_unit_id, description) VALUES ({syllabusTemplateId}, 'BM13_WEB', 'Biểu mẫu BM13 Web', {fitOrgId}, 'Mẫu đề cương chi tiết học phần')",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO portfolio.syllabus_template_version (id, syllabus_template_id, institution_template_version_id, version_no, decision_id, effective_from, effective_to, status, workflow_instance_id, supersedes_id, checksum, created_at, created_by, updated_at, updated_by, row_version) VALUES ({syllabusTemplateVersionId}, {syllabusTemplateId}, {instTemplateVersionId}, 1, {decisionId}, DATE '2023-09-01', NULL, 'ACTIVE', {syllabusTemplateWorkflowId}, NULL, repeat('7', 64), CURRENT_TIMESTAMP, {adminPrincipalId}, CURRENT_TIMESTAMP, {adminPrincipalId}, 1)",
                    ct);
                return true;
            },
            cancellationToken);

        var syllabusWorkflowId = Guid.NewGuid();
        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO workflow.instance (id, definition_id, current_state, started_by, started_at, row_version) VALUES ({syllabusWorkflowId}, {defaultWorkflowDefId}, 'DRAFT', {adminPrincipalId}, CURRENT_TIMESTAMP, 1)",
                    ct);
                return true;
            },
            cancellationToken);

        var syllabusVersionId = Guid.NewGuid();
        var syllabusVersion = SyllabusVersion.Create(
            syllabusVersionId,
            syllabusId,
            programCourseId,
            k17VersionId,
            instTemplateVersionId,
            courseVersionId,
            syllabusTemplateVersionId,
            1,
            new DateOnly(2023, 9, 1),
            null,
            "DRAFT",
            null,
            syllabusWorkflowId,
            null,
            Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));

        await rlsExecutor.ExecuteAsync(adminContext, ct => syllabusRepo.CreateSyllabusVersionAsync(syllabusVersion, ct), cancellationToken);

        // 5. Create CourseOffering (Lớp học phần)
        var offeringId = Guid.NewGuid();
        var offering = CourseOffering.Create(
            offeringId,
            "IT4105_2023_HK1_01",
            programCourseId,
            courseVersionId,
            k17VersionId,
            syllabusVersionId,
            academicYearStart: 2023,
            termCode: "HK1",
            orgUnitId: fitOrgId,
            startDate: new DateOnly(2023, 9, 5),
            endDate: new DateOnly(2024, 1, 15),
            status: "PLANNED");

        var createdOffering = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => offeringRepo.CreateOfferingAsync(offering, ct),
            cancellationToken);

        Assert.Equal("IT4105_2023_HK1_01", createdOffering.Code);

        // 6. Assign Instructor to CourseOffering (Phân công giảng dạy)
        var assignmentId = Guid.NewGuid();
        var instructorAssignment = CourseOfferingInstructor.Create(
            assignmentId,
            offeringId,
            staffPersonId,
            "PRIMARY_INSTRUCTOR",
            effectiveFrom: new DateOnly(2023, 9, 5),
            effectiveTo: new DateOnly(2024, 1, 15),
            isPrimary: true);

        var createdAssignment = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => offeringRepo.AssignInstructorAsync(instructorAssignment, ct),
            cancellationToken);

        Assert.True(createdAssignment.IsPrimary);
        Assert.Equal("PRIMARY_INSTRUCTOR", createdAssignment.AssignmentRole);

        var loadedOffering = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => offeringRepo.GetOfferingByIdAsync(offeringId, ct),
            cancellationToken);

        Assert.NotNull(loadedOffering);
        Assert.NotEmpty(loadedOffering.Instructors!);
        Assert.Equal("GV_CNTT_001", loadedOffering.Instructors![0].StaffCode);
    }
}
