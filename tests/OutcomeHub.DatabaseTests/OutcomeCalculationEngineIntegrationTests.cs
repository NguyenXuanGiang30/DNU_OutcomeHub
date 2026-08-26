using Microsoft.EntityFrameworkCore;
using Npgsql;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.Common.Security;
using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Measurement;
using OutcomeHub.Domain.Entities.Portfolio;
using OutcomeHub.Infrastructure.Persistence;
using OutcomeHub.Infrastructure.Persistence.Interceptors;
using OutcomeHub.Infrastructure.Persistence.Repositories.Academic;
using OutcomeHub.Infrastructure.Persistence.Repositories.Measurement;
using OutcomeHub.Infrastructure.Persistence.Repositories.Portfolio;
using OutcomeHub.Infrastructure.Persistence.Repositories.Result;
using OutcomeHub.Infrastructure.Persistence.Rls;
using OutcomeHub.Infrastructure.Services;
using OutcomeHub.Migrations;
using Testcontainers.PostgreSql;
using Xunit;

namespace OutcomeHub.DatabaseTests;

public sealed class OutcomeCalculationEngineIntegrationTests
{
    private static readonly string[] ValidAttainmentStatuses = ["ATTAINED", "NOT_ATTAINED"];

    [Fact(Timeout = 180_000)]
    public async Task CompleteOutcomeCalculationEngineEndToEndLifecycleSucceedsUnderRls()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using PostgreSqlContainer postgreSql = new PostgreSqlBuilder("postgres:18")
            .WithDatabase("outcomehub_calc_tests")
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
        Assert.Equal(18, migrationResult.AppliedCount);

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

        var dbContext = new OutcomeHubDbContext(optionsBuilder.Options);
        var rlsExecutor = new RlsTransactionExecutor(dbContext);

        var cohortRepo = new CohortRepository(dbContext);
        var studentRepo = new StudentRepository(dbContext);
        var offeringRepo = new CourseOfferingRepository(dbContext);
        var courseRepo = new CourseRepository(dbContext);
        var syllabusRepo = new SyllabusRepository(dbContext);
        var rubricRepo = new RubricRepository(dbContext);
        var cloRepo = new CloRepository(dbContext);
        var periodRepo = new MeasurementPeriodRepository(dbContext);
        var enrollmentRepo = new EnrollmentRepository(dbContext);
        var scoreRepo = new ScoreRepository(dbContext);
        var resultRepo = new ResultRepository(dbContext);
        var calculationService = new OutcomeCalculationService(dbContext, resultRepo);

        var adminPrincipalId = Guid.Parse("10000000-0000-7000-8000-000000000001");
        var adminContext = new DatabaseRequestContext(adminPrincipalId, Guid.NewGuid(), "Admin runs outcome calculation test");

        var fitOrgId = Guid.Parse("00000000-0000-7000-8000-000000000002");
        var programId = Guid.Parse("30000000-0000-7000-8000-000000000001");
        var k17VersionId = Guid.Parse("53000000-0000-7000-8000-000000000001");
        var decisionId = Guid.Parse("50000000-0000-7000-8000-000000000001");
        var defaultWorkflowDefId = Guid.Parse("00000000-0000-7000-8000-000000000401");
        var sourceSystemId = Guid.Parse("00000000-0000-7000-8000-000000000301");

        // 1. Create Cohort K17
        var cohortId = Guid.NewGuid();
        var cohort = Cohort.Create(cohortId, programId, "K17_IT_CALC", "Khóa 17 CNTT Tính toán", 2023, new DateOnly(2023, 9, 1), new DateOnly(2027, 6, 30));
        await rlsExecutor.ExecuteAsync(adminContext, ct => cohortRepo.CreateCohortAsync(cohort, ct), cancellationToken);

        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO academic.program_version_cohort (program_version_id, cohort_id, effective_from, effective_to, is_default) VALUES ({k17VersionId}, {cohortId}, DATE '2023-09-01', NULL, true)",
                    ct);
                return true;
            },
            cancellationToken);

        // 2. Create Student
        var studentPersonId = Guid.NewGuid();
        var studentPerson = Person.Create(studentPersonId, "Nguyễn Văn Đạt", new DateOnly(2023, 9, 1), null, "ACTIVE");
        var student = Student.Create(studentPersonId, "23010099", cohortId, "ACTIVE");
        await rlsExecutor.ExecuteAsync(adminContext, ct => studentRepo.CreateStudentAsync(studentPerson, student, ct), cancellationToken);

        // 2.1 Create CurriculumPath and StudentPath
        var curriculumPathWorkflowId = Guid.NewGuid();
        var curriculumPathId = Guid.NewGuid();
        var studentPathId = Guid.NewGuid();

        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO workflow.instance (id, definition_id, current_state, started_by, started_at, row_version) VALUES ({curriculumPathWorkflowId}, {defaultWorkflowDefId}, 'APPROVED', {adminPrincipalId}, CURRENT_TIMESTAMP, 1)",
                    ct);

                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO academic.curriculum_path (id, program_version_id, code, name, path_type, effective_from, effective_to, is_default, workflow_instance_id) VALUES ({curriculumPathId}, {k17VersionId}, 'PATH_K17_CALC', 'Lộ trình Chuẩn K17 Calc', 'COMMON', DATE '2023-09-01', NULL, true, {curriculumPathWorkflowId})",
                    ct);

                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO academic.student_path (id, student_id, program_id, program_version_id, curriculum_path_id, path_status, effective_from, effective_to, decision_id, is_primary) VALUES ({studentPathId}, {studentPersonId}, {programId}, {k17VersionId}, {curriculumPathId}, 'ACTIVE', DATE '2023-09-01', NULL, {decisionId}, true)",
                    ct);

                return true;
            },
            cancellationToken);

        // 3. Create Course, Syllabus & CLO
        var courseId = Guid.NewGuid();
        var course = Course.Create(courseId, "IT4301", "Kiến trúc Phần mềm", fitOrgId, "DRAFT");
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
        var courseVersion = CourseVersion.Create(courseVersionId, courseId, 1, "Kiến trúc Phần mềm", 3.0m, "STANDARD", new DateOnly(2023, 9, 1), null, false, "DRAFT", decisionId, courseWorkflowId, null, Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));
        await rlsExecutor.ExecuteAsync(adminContext, ct => courseRepo.CreateCourseVersionAsync(courseVersion, ct), cancellationToken);

        var curriculumPlanId = Guid.NewGuid();
        var curriculumBlockId = Guid.NewGuid();
        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO academic.curriculum_plan (id, program_version_id, code, name, declared_total_credits, status, checksum) VALUES ({curriculumPlanId}, {k17VersionId}, 'PLAN_K17_CALC', 'Kế hoạch Calc K17', 132.0, 'ACTIVE', repeat('9', 64))",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO academic.curriculum_block (id, curriculum_plan_id, parent_id, code, name, block_type, required_credits, sort_order) VALUES ({curriculumBlockId}, {curriculumPlanId}, NULL, 'BLOCK_CALC', 'Khối Tính toán', 'SPECIALIZED', 12.0, 1)",
                    ct);
                return true;
            },
            cancellationToken);

        var programCourseId = Guid.NewGuid();
        var programCourse = ProgramCourse.Create(programCourseId, k17VersionId, courseVersionId, curriculumBlockId, "REQUIRED", null, false, "DRAFT");
        await rlsExecutor.ExecuteAsync(adminContext, ct => courseRepo.AddCourseToProgramAsync(programCourse, ct), cancellationToken);

        var syllabusId = Guid.NewGuid();
        var syllabus = Syllabus.Create(syllabusId, programCourseId, "DCCT-IT4301", fitOrgId);
        await rlsExecutor.ExecuteAsync(adminContext, ct => syllabusRepo.CreateSyllabusAsync(syllabus, ct), cancellationToken);

        var instTemplateVersionId = Guid.Parse("52000000-0000-7000-8000-000000000001");
        var syllabusTemplateId = Guid.NewGuid();
        var syllabusTemplateVersionId = Guid.NewGuid();
        var rubricScaleId = Guid.NewGuid();
        var syllabusTemplateWorkflowId = Guid.Parse("00000000-0000-7000-8000-000000000402");

        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO portfolio.syllabus_template (id, code, name, owner_org_unit_id, description) VALUES ({syllabusTemplateId}, 'BM13_CALC', 'Biểu mẫu BM13 Calc', {fitOrgId}, 'Mẫu đề cương chi tiết')",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO portfolio.syllabus_template_version (id, syllabus_template_id, institution_template_version_id, version_no, decision_id, effective_from, effective_to, status, workflow_instance_id, supersedes_id, checksum, created_at, created_by, updated_at, updated_by, row_version) VALUES ({syllabusTemplateVersionId}, {syllabusTemplateId}, {instTemplateVersionId}, 1, {decisionId}, DATE '2023-09-01', NULL, 'ACTIVE', {syllabusTemplateWorkflowId}, NULL, repeat('7', 64), CURRENT_TIMESTAMP, {adminPrincipalId}, CURRENT_TIMESTAMP, {adminPrincipalId}, 1)",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO portfolio.syllabus_template_rubric_scale (id, syllabus_template_version_id, code, name) VALUES ({rubricScaleId}, {syllabusTemplateVersionId}, 'SCALE_4_LEVELS_CALC', 'Thang 4 mức')",
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
        var syllabusVersion = SyllabusVersion.Create(syllabusVersionId, syllabusId, programCourseId, k17VersionId, instTemplateVersionId, courseVersionId, syllabusTemplateVersionId, 1, new DateOnly(2023, 9, 1), null, "DRAFT", null, syllabusWorkflowId, null, Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));
        await rlsExecutor.ExecuteAsync(adminContext, ct => syllabusRepo.CreateSyllabusVersionAsync(syllabusVersion, ct), cancellationToken);

        var cloId = Guid.NewGuid();
        var clo = Clo.Create(cloId, syllabusVersionId, "CLO1", "Thiết kế kiến trúc hệ thống phân tán", "COGNITIVE", "APPLY", true, 1);
        await rlsExecutor.ExecuteAsync(adminContext, ct => cloRepo.CreateCloAsync(clo, ct), cancellationToken);

        var assessmentItemId = Guid.NewGuid();
        var assessmentItem = AssessmentItem.Create(assessmentItemId, syllabusVersionId, null, "A1", "Đồ án Kiến trúc", "PROJECT", 1.0m, 1.0m, false, true, 10.0m, 1);
        await rlsExecutor.ExecuteAsync(adminContext, ct => rubricRepo.CreateAssessmentItemAsync(assessmentItem, ct), cancellationToken);

        var rubricId = Guid.NewGuid();
        var rubric = Rubric.Create(rubricId, syllabusVersionId, syllabusTemplateVersionId, assessmentItemId, "RUBRIC_CALC", "Rubric Đồ án", 10.0m, rubricScaleId, Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));

        var rubricCriterionId = Guid.NewGuid();
        var rubricCriterion = RubricCriterion.Create(rubricCriterionId, rubricId, assessmentItemId, syllabusVersionId, "RC1", "Thiết kế mô hình C4", 10.0m, 1.0m, "CRITERION", true, true, 1);

        await rlsExecutor.ExecuteAsync(adminContext, ct => rubricRepo.CreateRubricAsync(rubric, new[] { rubricCriterion }, ct), cancellationToken);

        // 4. Create Course Offering
        var offeringId = Guid.NewGuid();
        var offering = CourseOffering.Create(offeringId, "IT4301_2023_CALC", programCourseId, courseVersionId, k17VersionId, syllabusVersionId, 2023, "HK1", fitOrgId, new DateOnly(2023, 9, 5), new DateOnly(2024, 1, 15), "PLANNED");
        await rlsExecutor.ExecuteAsync(adminContext, ct => offeringRepo.CreateOfferingAsync(offering, ct), cancellationToken);

        // 5. Create Measurement Period
        var calcPolicyId = Guid.NewGuid();
        var calcPolicyVersionWorkflowId = Guid.NewGuid();
        var calcPolicyVersionId = Guid.NewGuid();
        var calcPolicyBindingWorkflowId = Guid.NewGuid();
        var calcPolicyBindingId = Guid.NewGuid();
        var periodWorkflowId = Guid.NewGuid();

        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO measurement.calculation_policy (id, code, name, owner_org_unit_id, description, created_at) VALUES ({calcPolicyId}, 'POLICY_CALC', 'Chính sách tính toán', {fitOrgId}, 'Mô tả', CURRENT_TIMESTAMP)",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO workflow.instance (id, definition_id, current_state, started_by, started_at, row_version) VALUES ({calcPolicyVersionWorkflowId}, {defaultWorkflowDefId}, 'DRAFT', {adminPrincipalId}, CURRENT_TIMESTAMP, 1)",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO measurement.calculation_policy_version (id, policy_id, version_no, effective_from, effective_to, status, formula_family, engine_contract_version, direct_source_min, direct_source_max, missing_data_rule, repeat_attempt_rule, withdrawal_rule, recognition_rule, direct_indirect_mode, alpha, core_gate_mode, default_min_sample_size, definition, schema_version, workflow_instance_id, checksum, supersedes_id) VALUES ({calcPolicyVersionId}, {calcPolicyId}, 1, DATE '2023-09-01', NULL, 'ACTIVE', 'STANDARD_MEAN', 'v1.0', 1, 10, 'EXCLUDE', 'LATEST', 'EXCLUDE', 'INCLUDE', 'DIRECT', NULL, 'STRICT', 1, '{{}}'::jsonb, '1.0', {calcPolicyVersionWorkflowId}, repeat('9', 64), NULL)",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO workflow.instance (id, definition_id, current_state, started_by, started_at, row_version) VALUES ({calcPolicyBindingWorkflowId}, {defaultWorkflowDefId}, 'DRAFT', {adminPrincipalId}, CURRENT_TIMESTAMP, 1)",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO measurement.program_policy_binding (id, program_version_id, policy_version_id, effective_from, effective_to, status, decision_id, workflow_instance_id, checksum) VALUES ({calcPolicyBindingId}, {k17VersionId}, {calcPolicyVersionId}, DATE '2023-09-01', NULL, 'ACTIVE', {decisionId}, {calcPolicyBindingWorkflowId}, repeat('8', 64))",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO workflow.instance (id, definition_id, current_state, started_by, started_at, row_version) VALUES ({periodWorkflowId}, {defaultWorkflowDefId}, 'DRAFT', {adminPrincipalId}, CURRENT_TIMESTAMP, 1)",
                    ct);
                return true;
            },
            cancellationToken);

        var periodId = Guid.NewGuid();
        var period = MeasurementPeriod.Create(
            periodId,
            "MEAS_2023_SEM1_CALC",
            "Đợt đo lường HK1 2023-2024 Calc",
            fitOrgId,
            k17VersionId,
            academicYearStart: 2023,
            termCode: "HK1",
            programPolicyBindingId: calcPolicyBindingId,
            workflowInstanceId: periodWorkflowId,
            status: "OPEN");

        await rlsExecutor.ExecuteAsync(adminContext, ct => periodRepo.CreatePeriodAsync(period, ct), cancellationToken);
        await rlsExecutor.ExecuteAsync(adminContext, ct => periodRepo.AttachCohortAsync(MeasurementPeriodCohort.Create(periodId, k17VersionId, cohortId), ct), cancellationToken);
        await rlsExecutor.ExecuteAsync(adminContext, ct => periodRepo.AttachOfferingAsync(MeasurementPeriodOffering.Create(periodId, k17VersionId, 2023, offeringId, "OFFICIAL", "IN_PROGRESS", DateTimeOffset.UtcNow.AddDays(15)), ct), cancellationToken);

        // 6. Create Enrollment & ScoreRecord
        var enrollmentId = Guid.NewGuid();
        var enrollment = Enrollment.Create(enrollmentId, offeringId, studentPersonId, 1, sourceSystemId, "SIS_ENR_CALC_01");

        var servicePrincipalId = Guid.NewGuid();
        var ingestionGovResId = Guid.NewGuid();
        var ingestionBatchId = Guid.NewGuid();
        var governedResourceId = Guid.NewGuid();

        var ownerOptions = new DbContextOptionsBuilder<OutcomeHubDbContext>()
            .UseNpgsql(ownerConnectionString)
            .Options;

        await using (var ownerDbContext = new OutcomeHubDbContext(ownerOptions))
        {
            await ownerDbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO iam.principal (id, principal_type, status, display_name, created_at) VALUES ({servicePrincipalId}, 'SERVICE_ACCOUNT', 'ACTIVE', 'SIS Sync Agent', CURRENT_TIMESTAMP)",
                cancellationToken);
            await ownerDbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO iam.service_account (principal_id, client_id, owner_org_unit_id, purpose, expires_at, technical_contact) VALUES ({servicePrincipalId}, 'sis-client-calc', {fitOrgId}, 'SIS Service', NULL, 'admin@dnu.edu.vn')",
                cancellationToken);
            await ownerDbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO integration.source_system (id, code, name, system_type, base_url, owner_org_unit_id, service_principal_id, status, data_classification, created_at) VALUES ({sourceSystemId}, 'SIS_CALC', 'SIS Calc', 'SIS', 'https://sis.dnu.edu.vn', {fitOrgId}, {servicePrincipalId}, 'ACTIVE', 'INTERNAL', CURRENT_TIMESTAMP)",
                cancellationToken);
            await ownerDbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO governance.governed_resource (id, resource_type, classification, disposition_status, created_at) VALUES ({ingestionGovResId}, 'integration.ingestion_batch', 'INTERNAL', 'ACTIVE', CURRENT_TIMESTAMP)",
                cancellationToken);
            await ownerDbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO integration.ingestion_batch (id, governed_resource_id, source_system_id, data_type, source_batch_id, idempotency_key, schema_version, payload_checksum, file_object_id, classification, status, received_at, completed_at, total_count, accepted_count, rejected_count) VALUES ({ingestionBatchId}, {ingestionGovResId}, {sourceSystemId}, 'ENROLLMENT', 'SIS_BATCH_CALC', {Guid.NewGuid().ToString()}, 1, repeat('6', 64), NULL, 'INTERNAL', 'COMPLETED', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 100, 100, 0)",
                cancellationToken);
            await ownerDbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO governance.governed_resource (id, resource_type, classification, disposition_status, created_at) VALUES ({governedResourceId}, 'measurement.score_dataset', 'CONFIDENTIAL', 'ACTIVE', CURRENT_TIMESTAMP)",
                cancellationToken);
        }

        var initialRevision = EnrollmentRevision.Create(Guid.NewGuid(), enrollmentId, 1, "ENROLLED", DateTimeOffset.UtcNow.AddDays(-20), ingestionBatchId, Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"), DateTimeOffset.UtcNow);

        await using (var ownerDbContext = new OutcomeHubDbContext(ownerOptions))
        {
            await ownerDbContext.Set<Enrollment>().AddAsync(enrollment, cancellationToken);
            await ownerDbContext.Set<EnrollmentRevision>().AddAsync(initialRevision, cancellationToken);
            await ownerDbContext.SaveChangesAsync(cancellationToken);
        }

        var scoreDatasetId = Guid.NewGuid();
        var scoreDataset = ScoreDataset.Create(scoreDatasetId, governedResourceId, sourceSystemId, 2023, offeringId, "CONFIDENTIAL");

        await using (var ownerDbContext = new OutcomeHubDbContext(ownerOptions))
        {
            await ownerDbContext.Set<ScoreDataset>().AddAsync(scoreDataset, cancellationToken);
            await ownerDbContext.SaveChangesAsync(cancellationToken);
        }

        var scoreIdentityId = Guid.NewGuid();
        var scoreIdentity = ScoreIdentity.Create(scoreIdentityId, scoreDatasetId, 2023, studentPersonId, offeringId, k17VersionId, syllabusVersionId, 1, enrollmentId, assessmentItemId, "CRITERION", rubricCriterionId);

        var scoreRecordId = Guid.NewGuid();
        var scoreRecord = ScoreRecord.Create(2023, scoreRecordId, scoreIdentityId, studentPersonId, offeringId, fitOrgId, programId, k17VersionId, courseId, 1, 8.5m, 10.0m, "SCORED", sourceSystemId, "LMS_SCORE_001", "v1", ingestionBatchId, adminPrincipalId, DateTimeOffset.UtcNow, Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));

        await using (var ownerDbContext = new OutcomeHubDbContext(ownerOptions))
        {
            await ownerDbContext.Set<ScoreIdentity>().AddAsync(scoreIdentity, cancellationToken);
            await ownerDbContext.ScoreRecords.AddAsync(scoreRecord, cancellationToken);
            await ownerDbContext.SaveChangesAsync(cancellationToken);
        }

        // 7. Execute Two-Tier Calculation Engine
        var batchDto = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => calculationService.CalculatePeriodOutcomesAsync(
                periodId,
                "Kiểm thử tính toán chuẩn đầu ra tự động đợt đo 2023-SEM1",
                adminContext.PrincipalId,
                ct),
            cancellationToken);

        Assert.NotNull(batchDto);
        Assert.Equal("CALCULATED", batchDto.Status);
        Assert.Equal("2.0.0-OBE", batchDto.EngineVersion);
        Assert.False(string.IsNullOrWhiteSpace(batchDto.ResultChecksum));

        var batchId = batchDto.Id;

        // 8. Query Student CLO Results (Tier 1)
        var cloResults = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => resultRepo.GetStudentCloResultsAsync(batchId, studentPersonId, null, ct),
            cancellationToken);

        Assert.NotEmpty(cloResults);
        Assert.All(cloResults, clo =>
        {
            Assert.Equal(batchId, clo.BatchId);
            Assert.Equal(studentPersonId, clo.StudentId);
            Assert.True(clo.Score.HasValue);
            Assert.Equal(5.0m, clo.ThetaInd);
            Assert.Contains(clo.AttainmentStatus, ValidAttainmentStatuses);
        });

        // 9. Query Student PI Results (Tier 2)
        var piResults = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => resultRepo.GetStudentPiResultsAsync(batchId, studentPersonId, k17VersionId, ct),
            cancellationToken);

        Assert.NotEmpty(piResults);
        Assert.All(piResults, pi =>
        {
            Assert.Equal(batchId, pi.BatchId);
            Assert.Equal(studentPersonId, pi.StudentId);
            Assert.True(pi.Score.HasValue);
            Assert.Equal("PASSED", pi.CoreGateStatus);
            Assert.Equal("ATTAINED", pi.AttainmentStatus);
        });

        // 10. Query Student PLO Results (Tier 2)
        var ploResults = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => resultRepo.GetStudentPloResultsAsync(batchId, studentPersonId, k17VersionId, ct),
            cancellationToken);

        Assert.NotEmpty(ploResults);
        Assert.All(ploResults, plo =>
        {
            Assert.Equal(batchId, plo.BatchId);
            Assert.Equal(studentPersonId, plo.StudentId);
            Assert.True(plo.Score.HasValue);
            Assert.Equal("ATTAINED", plo.AttainmentStatus);
        });

        // 11. Query Cohort Outcomes (Khóa / CTĐT)
        var cohortResults = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => resultRepo.GetCohortOutcomeResultsAsync(batchId, cohortId, null, ct),
            cancellationToken);

        Assert.NotEmpty(cohortResults);
        Assert.All(cohortResults, co =>
        {
            Assert.Equal(batchId, co.BatchId);
            Assert.Equal(cohortId, co.CohortId);
            Assert.True(co.AttainmentRate >= 0m);
            Assert.Equal(70.0m, co.ThetaCoh);
            Assert.Equal("ATTAINED", co.OutcomeStatus);
        });

        // 12. Query Program Outcome Dashboard
        var dashboard = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => resultRepo.GetProgramOutcomeDashboardAsync(periodId, k17VersionId, cohortId, ct),
            cancellationToken);

        Assert.NotNull(dashboard);
        Assert.Equal(k17VersionId, dashboard.ProgramVersionId);
        Assert.Equal(cohortId, dashboard.CohortId);
        Assert.True(dashboard.TotalPlos > 0);
        Assert.True(dashboard.AttainedPlos > 0);
        Assert.Equal(100.0m, dashboard.PloAttainmentRate);
        Assert.NotEmpty(dashboard.PloResults);
        Assert.NotEmpty(dashboard.PiResults);
    }
}
