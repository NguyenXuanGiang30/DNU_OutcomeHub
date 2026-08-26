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

public sealed class SyllabusRubricIntegrationTests
{
    [Fact(Timeout = 180_000)]
    public async Task CourseSyllabusCloAndRubricsExecuteSuccessfullyUnderRls()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using PostgreSqlContainer postgreSql = new PostgreSqlBuilder("postgres:18")
            .WithDatabase("outcomehub_syllabus_tests")
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
        Assert.Equal(16, migrationResult.AppliedCount);

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

        var courseRepo = new CourseRepository(dbContext);
        var syllabusRepo = new SyllabusRepository(dbContext);
        var cloRepo = new CloRepository(dbContext);
        var rubricRepo = new RubricRepository(dbContext);

        var adminPrincipalId = Guid.Parse("10000000-0000-7000-8000-000000000001");
        var adminContext = new DatabaseRequestContext(adminPrincipalId, Guid.NewGuid(), "Admin creates syllabus & rubrics");

        // 1. Create Course & CourseVersion
        var fitOrgId = Guid.Parse("00000000-0000-7000-8000-000000000002");
        var decisionId = Guid.Parse("50000000-0000-7000-8000-000000000001");
        var defaultWorkflowDefId = Guid.Parse("00000000-0000-7000-8000-000000000401");

        var courseId = Guid.NewGuid();
        var course = Course.Create(
            courseId,
            "IT4102",
            "Kiến trúc Phần mềm và Mẫu thiết kế",
            fitOrgId,
            "DRAFT");

        var createdCourse = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => courseRepo.CreateCourseAsync(course, ct),
            cancellationToken);

        Assert.Equal("IT4102", createdCourse.Code);

        // Provision workflow instance for CourseVersion
        var courseVersionWorkflowId = Guid.NewGuid();
        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO workflow.instance (id, definition_id, current_state, started_by, started_at, row_version) VALUES ({courseVersionWorkflowId}, {defaultWorkflowDefId}, 'DRAFT', {adminPrincipalId}, CURRENT_TIMESTAMP, 1)",
                    ct);
                return true;
            },
            cancellationToken);

        var courseVersionId = Guid.NewGuid();
        var courseVersion = CourseVersion.Create(
            courseVersionId,
            courseId,
            versionNo: 1,
            name: "Kiến trúc Phần mềm và Mẫu thiết kế",
            creditValue: 3.0m,
            courseType: "STANDARD",
            effectiveFrom: new DateOnly(2023, 9, 1),
            effectiveTo: null,
            sharedCoreFlag: false,
            status: "DRAFT",
            decisionId: decisionId,
            workflowInstanceId: courseVersionWorkflowId,
            supersedesId: null,
            checksum: Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));

        var createdVersion = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => courseRepo.CreateCourseVersionAsync(courseVersion, ct),
            cancellationToken);

        Assert.Equal(1, createdVersion.VersionNo);
        Assert.Equal(3.0m, createdVersion.CreditValue);

        // 2. Add Course to ProgramVersion K17
        var k17VersionId = Guid.Parse("53000000-0000-7000-8000-000000000001");
        var curriculumPlanId = Guid.NewGuid();
        var curriculumBlockId = Guid.NewGuid();

        // Insert curriculum plan & block for test
        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO academic.curriculum_plan (id, program_version_id, code, name, declared_total_credits, status, checksum) VALUES ({curriculumPlanId}, {k17VersionId}, 'PLAN_K17_IT', 'Kế hoạch đào tạo chuẩn K17 CNTT', 132.0, 'ACTIVE', repeat('9', 64))",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO academic.curriculum_block (id, curriculum_plan_id, parent_id, code, name, block_type, required_credits, sort_order) VALUES ({curriculumBlockId}, {curriculumPlanId}, NULL, 'BLOCK_SPEC', 'Khối Kiến thức Chuyên ngành', 'SPECIALIZED', 24.0, 1)",
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
            creditOverride: null,
            isLocked: false,
            status: "DRAFT");

        var createdProgramCourse = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => courseRepo.AddCourseToProgramAsync(programCourse, ct),
            cancellationToken);

        Assert.Equal("REQUIRED", createdProgramCourse.CatalogRole);

        // 3. Create Syllabus & SyllabusVersion (BM13 standard)
        var syllabusId = Guid.NewGuid();
        var syllabus = Syllabus.Create(
            syllabusId,
            programCourseId,
            "DCCT-IT4102-K17",
            fitOrgId);

        var createdSyllabus = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => syllabusRepo.CreateSyllabusAsync(syllabus, ct),
            cancellationToken);

        Assert.Equal("DCCT-IT4102-K17", createdSyllabus.Code);

        // Create Syllabus Template & Scale
        var syllabusTemplateId = Guid.NewGuid();
        var syllabusTemplateVersionId = Guid.NewGuid();
        var rubricScaleId = Guid.NewGuid();
        var instTemplateVersionId = Guid.Parse("52000000-0000-7000-8000-000000000001");

        var syllabusTemplateWorkflowId = Guid.Parse("00000000-0000-7000-8000-000000000402");
        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO portfolio.syllabus_template (id, code, name, owner_org_unit_id, description) VALUES ({syllabusTemplateId}, 'BM13_TEMPLATE', 'Biểu mẫu BM13 Chuẩn DNU', {fitOrgId}, 'Mẫu đề cương chi tiết học phần')",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO portfolio.syllabus_template_version (id, syllabus_template_id, institution_template_version_id, version_no, decision_id, effective_from, effective_to, status, workflow_instance_id, supersedes_id, checksum, created_at, created_by, updated_at, updated_by, row_version) VALUES ({syllabusTemplateVersionId}, {syllabusTemplateId}, {instTemplateVersionId}, 1, {decisionId}, DATE '2023-09-01', NULL, 'ACTIVE', {syllabusTemplateWorkflowId}, NULL, repeat('7', 64), CURRENT_TIMESTAMP, {adminPrincipalId}, CURRENT_TIMESTAMP, {adminPrincipalId}, 1)",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO portfolio.syllabus_template_rubric_scale (id, syllabus_template_version_id, code, name) VALUES ({rubricScaleId}, {syllabusTemplateVersionId}, 'SCALE_4_LEVELS', 'Thang đánh giá 4 mức chuẩn OBE')",
                    ct);
                return true;
            },
            cancellationToken);

        var syllabusVersionWorkflowId = Guid.NewGuid();
        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO workflow.instance (id, definition_id, current_state, started_by, started_at, row_version) VALUES ({syllabusVersionWorkflowId}, {defaultWorkflowDefId}, 'DRAFT', {adminPrincipalId}, CURRENT_TIMESTAMP, 1)",
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
            versionNo: 1,
            applicableFrom: new DateOnly(2023, 9, 1),
            applicableTo: null,
            status: "DRAFT",
            sharedSyllabusCoreVersionId: null,
            workflowInstanceId: syllabusVersionWorkflowId,
            supersedesId: null,
            contentChecksum: Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));

        var createdSylVersion = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => syllabusRepo.CreateSyllabusVersionAsync(syllabusVersion, ct),
            cancellationToken);

        Assert.Equal(1, createdSylVersion.VersionNo);

        // 4. Create CLOs (Course Learning Outcomes)
        var clo1 = Clo.Create(
            Guid.NewGuid(),
            syllabusVersionId,
            "CLO1",
            "Trình bày các nguyên lý thiết kế SOLID và mẫu kiến trúc phần mềm",
            "KNOWLEDGE",
            "UNDERSTAND",
            isCore: true,
            sortOrder: 1);

        var clo2 = Clo.Create(
            Guid.NewGuid(),
            syllabusVersionId,
            "CLO2",
            "Áp dụng mẫu kiến trúc Clean Architecture / Microservices vào bài toán thực tế",
            "SKILL",
            "APPLY",
            isCore: true,
            sortOrder: 2);

        var createdClo1 = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => cloRepo.CreateCloAsync(clo1, ct),
            cancellationToken);

        var createdClo2 = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => cloRepo.CreateCloAsync(clo2, ct),
            cancellationToken);

        Assert.Equal("CLO1", createdClo1.Code);
        Assert.Equal("CLO2", createdClo2.Code);

        var clos = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => cloRepo.GetClosBySyllabusVersionIdAsync(syllabusVersionId, ct),
            cancellationToken);

        Assert.Equal(2, clos.Count);

        // 5. Course - PI Matrix Mapping
        var pi51Id = Guid.Parse("55000000-0000-7000-8000-000000000001");
        var mapping = CoursePiMapping.Create(
            Guid.NewGuid(),
            k17VersionId,
            programCourseId,
            pi51Id,
            contributionLevel: "M",
            isDirectAssessment: true,
            rationale: "Học phần Kiến trúc phần mềm đóng vai trò Mastery (M) và là điểm đo trực tiếp (A) cho PI5.1");

        var createdMapping = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => cloRepo.SetCoursePiMappingAsync(mapping, ct),
            cancellationToken);

        Assert.Equal("M", createdMapping.ContributionLevel);
        Assert.True(createdMapping.IsDirectAssessment);

        // 6. Assessment Items & Rubrics
        var assessmentItem1 = AssessmentItem.Create(
            Guid.NewGuid(),
            syllabusVersionId,
            parentId: null,
            assessmentCode: "A1",
            name: "Đánh giá Chuyên cần & Ý thức",
            assessmentType: "ATTENDANCE",
            courseWeightRatio: 0.1m,
            individualComponentRatio: 1.0m,
            isGroupAssessment: false,
            countsTowardCourseGrade: true,
            maxScore: 10.0m,
            sortOrder: 1);

        var assessmentItem2 = AssessmentItem.Create(
            Guid.NewGuid(),
            syllabusVersionId,
            parentId: null,
            assessmentCode: "A2",
            name: "Bài tập lớn Thiết kế Kiến trúc Hệ thống",
            assessmentType: "PROJECT",
            courseWeightRatio: 0.3m,
            individualComponentRatio: 0.5m,
            isGroupAssessment: true,
            countsTowardCourseGrade: true,
            maxScore: 10.0m,
            sortOrder: 2);

        await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => rubricRepo.CreateAssessmentItemAsync(assessmentItem1, ct),
            cancellationToken);

        var createdA2 = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => rubricRepo.CreateAssessmentItemAsync(assessmentItem2, ct),
            cancellationToken);

        Assert.Equal("A2", createdA2.AssessmentCode);
        Assert.True(createdA2.IsGroupAssessment);

        // 7. Create Rubric for A2
        var rubricId = Guid.NewGuid();
        var rubric = Rubric.Create(
            rubricId,
            syllabusVersionId,
            syllabusTemplateVersionId,
            createdA2.Id,
            "RUBRIC_A2",
            "Tiêu chí đánh giá Bài tập lớn Kiến trúc Hệ thống",
            maxScore: 10.0m,
            rubricScaleId: rubricScaleId,
            checksum: Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));

        var criterion1Id = Guid.NewGuid();
        var criterion1 = RubricCriterion.Create(
            criterion1Id,
            rubricId,
            createdA2.Id,
            syllabusVersionId,
            "CRIT_ARCH_DESIGN",
            "Chất lượng thiết kế kiến trúc và lược đồ CSDL",
            maxScore: 6.0m,
            rubricWeightRatio: 0.6m,
            scoreSourceMode: "CRITERION",
            isCore: true,
            individualEvidence: false,
            sortOrder: 1);

        var level1 = RubricLevel.Create(
            Guid.NewGuid(),
            criterion1Id,
            "LEVEL_EXCELLENT",
            1,
            "Xuất sắc",
            "Thiết kế kiến trúc phân tầng chuẩn mực, CSDL chuẩn hóa 3NF, đáp ứng tải cao",
            scoreFrom: 5.1m,
            scoreTo: 6.0m,
            numericValue: 6.0m);

        var level2 = RubricLevel.Create(
            Guid.NewGuid(),
            criterion1Id,
            "LEVEL_GOOD",
            2,
            "Khá",
            "Thiết kế cấu trúc rõ ràng, đúng chuẩn OOP/SOLID, CSDL hoàn chỉnh",
            scoreFrom: 4.2m,
            scoreTo: 5.0m,
            numericValue: 5.0m);

        criterion1.Levels.Add(level1);
        criterion1.Levels.Add(level2);

        var createdRubric = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => rubricRepo.CreateRubricAsync(rubric, new[] { criterion1 }, ct),
            cancellationToken);

        Assert.Equal("RUBRIC_A2", createdRubric.Code);

        var loadedRubrics = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => rubricRepo.GetRubricsBySyllabusVersionIdAsync(syllabusVersionId, ct),
            cancellationToken);

        Assert.NotEmpty(loadedRubrics);
        var loadedRubric = loadedRubrics.First(r => r.Code == "RUBRIC_A2");
        Assert.NotEmpty(loadedRubric.Criteria);
        var loadedCriterion = loadedRubric.Criteria.First(c => c.CriterionCode == "CRIT_ARCH_DESIGN");
        Assert.NotEmpty(loadedCriterion.Levels);
    }
}
