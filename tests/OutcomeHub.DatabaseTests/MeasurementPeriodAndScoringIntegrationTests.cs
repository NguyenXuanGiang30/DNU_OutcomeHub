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
using OutcomeHub.Infrastructure.Persistence.Rls;
using OutcomeHub.Migrations;
using Testcontainers.PostgreSql;
using Xunit;

namespace OutcomeHub.DatabaseTests;

public sealed class MeasurementPeriodAndScoringIntegrationTests
{
    [Fact(Timeout = 180_000)]
    public async Task MeasurementPeriodEnrollmentAndScoresExecuteSuccessfullyUnderRls()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using PostgreSqlContainer postgreSql = new PostgreSqlBuilder("postgres:18")
            .WithDatabase("outcomehub_measurement_tests")
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
        Assert.Equal(15, migrationResult.AppliedCount);

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
        var offeringRepo = new CourseOfferingRepository(dbContext);
        var courseRepo = new CourseRepository(dbContext);
        var syllabusRepo = new SyllabusRepository(dbContext);
        var rubricRepo = new RubricRepository(dbContext);
        var cloRepo = new CloRepository(dbContext);
        var periodRepo = new MeasurementPeriodRepository(dbContext);
        var enrollmentRepo = new EnrollmentRepository(dbContext);
        var scoreRepo = new ScoreRepository(dbContext);

        var adminPrincipalId = Guid.Parse("10000000-0000-7000-8000-000000000001");
        var adminContext = new DatabaseRequestContext(adminPrincipalId, Guid.NewGuid(), "Admin manages measurement periods and scores");

        var fitOrgId = Guid.Parse("00000000-0000-7000-8000-000000000002");
        var programId = Guid.Parse("30000000-0000-7000-8000-000000000001");
        var k17VersionId = Guid.Parse("53000000-0000-7000-8000-000000000001");
        var decisionId = Guid.Parse("50000000-0000-7000-8000-000000000001");
        var defaultWorkflowDefId = Guid.Parse("00000000-0000-7000-8000-000000000401");
        var sourceSystemId = Guid.Parse("00000000-0000-7000-8000-000000000301");

        // 1. Create Cohort (Khóa tuyển sinh K17) & Link to ProgramVersionCohort
        var cohortId = Guid.NewGuid();
        var cohort = Cohort.Create(
            cohortId,
            programId,
            "K17_IT_MEAS",
            "Khóa 17 CNTT Đo lường",
            admissionYear: 2023,
            startDate: new DateOnly(2023, 9, 1),
            endDate: new DateOnly(2027, 6, 30));

        await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => cohortRepo.CreateCohortAsync(cohort, ct),
            cancellationToken);

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
        var studentPerson = Person.Create(
            studentPersonId,
            "Lê Thị Hoa",
            effectiveFrom: new DateOnly(2023, 9, 1),
            status: "ACTIVE");

        var student = Student.Create(
            studentPersonId,
            "23010002",
            cohortId,
            currentStatus: "ACTIVE");

        await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => studentRepo.CreateStudentAsync(studentPerson, student, ct),
            cancellationToken);

        // 3. Create Course, CourseVersion, ProgramCourse, Syllabus, SyllabusVersion, AssessmentItem, RubricCriterion
        var courseId = Guid.NewGuid();
        var course = Course.Create(courseId, "IT4201", "Kiểm thử Phần mềm", fitOrgId, "DRAFT");
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
            "Kiểm thử Phần mềm",
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
                    $"INSERT INTO academic.curriculum_plan (id, program_version_id, code, name, declared_total_credits, status, checksum) VALUES ({curriculumPlanId}, {k17VersionId}, 'PLAN_K17_TEST', 'Kế hoạch Test K17', 132.0, 'ACTIVE', repeat('8', 64))",
                    ct);
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO academic.curriculum_block (id, curriculum_plan_id, parent_id, code, name, block_type, required_credits, sort_order) VALUES ({curriculumBlockId}, {curriculumPlanId}, NULL, 'BLOCK_TEST', 'Khối Kiểm thử', 'SPECIALIZED', 12.0, 1)",
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
        var syllabus = Syllabus.Create(syllabusId, programCourseId, "DCCT-IT4201", fitOrgId);
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
                    $"INSERT INTO portfolio.syllabus_template (id, code, name, owner_org_unit_id, description) VALUES ({syllabusTemplateId}, 'BM13_TEST', 'Biểu mẫu BM13 Test', {fitOrgId}, 'Mẫu đề cương chi tiết học phần')",
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

        // CLO & AssessmentItem & RubricCriterion
        var cloId = Guid.NewGuid();
        var clo = Clo.Create(
            cloId,
            syllabusVersionId,
            "CLO1",
            "Thiết kế ca kiểm thử đơn vị và tích hợp",
            "SKILL",
            "APPLY",
            isCore: true,
            sortOrder: 1);

        await rlsExecutor.ExecuteAsync(adminContext, ct => cloRepo.CreateCloAsync(clo, ct), cancellationToken);

        var assessmentItemId = Guid.NewGuid();
        var assessmentItem = AssessmentItem.Create(
            assessmentItemId,
            syllabusVersionId,
            parentId: null,
            assessmentCode: "A1",
            name: "Bài tập lớn Kiểm thử Tự động",
            assessmentType: "PROJECT",
            courseWeightRatio: 0.3m,
            individualComponentRatio: 1.0m,
            isGroupAssessment: false,
            countsTowardCourseGrade: true,
            maxScore: 10.0m,
            sortOrder: 1);

        await rlsExecutor.ExecuteAsync(adminContext, ct => rubricRepo.CreateAssessmentItemAsync(assessmentItem, ct), cancellationToken);

        var rubricId = Guid.NewGuid();
        var rubric = Rubric.Create(
            rubricId,
            syllabusVersionId,
            syllabusTemplateVersionId,
            assessmentItemId,
            "RUBRIC_A1",
            "Tiêu chí đánh giá Kiểm thử Tự động",
            maxScore: 10.0m,
            rubricScaleId: rubricScaleId,
            checksum: Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));

        var rubricCriterionId = Guid.NewGuid();
        var rubricCriterion = RubricCriterion.Create(
            rubricCriterionId,
            rubricId,
            assessmentItemId,
            syllabusVersionId,
            "CRIT_01",
            "Độ bao phủ kiểm thử (Test Coverage)",
            maxScore: 10.0m,
            rubricWeightRatio: 1.0m,
            scoreSourceMode: "CRITERION",
            isCore: true,
            individualEvidence: true,
            sortOrder: 1);

        var level1 = RubricLevel.Create(
            Guid.NewGuid(),
            rubricCriterionId,
            "LEVEL_EXCELLENT",
            1,
            "Xuất sắc",
            "Bao phủ > 90% dòng mã",
            scoreFrom: 8.5m,
            scoreTo: 10.0m,
            numericValue: 10.0m);

        rubricCriterion.Levels.Add(level1);

        await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => rubricRepo.CreateRubricAsync(rubric, new[] { rubricCriterion }, ct),
            cancellationToken);

        // 4. Create CourseOffering (Lớp học phần)
        var offeringId = Guid.NewGuid();
        var offering = CourseOffering.Create(
            offeringId,
            "IT4201_2023_HK1_01",
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

        await rlsExecutor.ExecuteAsync(adminContext, ct => offeringRepo.CreateOfferingAsync(offering, ct), cancellationToken);

        // 5. Create MeasurementPeriod (Đợt đo lường)
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
                    $"INSERT INTO measurement.calculation_policy (id, code, name, owner_org_unit_id, description, created_at) VALUES ({calcPolicyId}, 'POLICY_DIRECT_MEAN', 'Chính sách đo lường trực tiếp trung bình', {fitOrgId}, 'Mô tả chính sách', CURRENT_TIMESTAMP)",
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
            "DOT_DO_2023_HK1_IT",
            "Đợt đo lường CĐR Học kỳ 1 Năm học 2023-2024 Khoa CNTT",
            fitOrgId,
            k17VersionId,
            academicYearStart: 2023,
            termCode: "HK1",
            programPolicyBindingId: calcPolicyBindingId,
            workflowInstanceId: periodWorkflowId,
            status: "DRAFT",
            collectionOpenAt: DateTimeOffset.UtcNow.AddDays(-10),
            collectionCloseAt: DateTimeOffset.UtcNow.AddDays(30));

        var createdPeriod = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => periodRepo.CreatePeriodAsync(period, ct),
            cancellationToken);

        Assert.Equal("DOT_DO_2023_HK1_IT", createdPeriod.Code);

        // 6. Attach Cohort, Offering, and Target to MeasurementPeriod
        var periodCohort = MeasurementPeriodCohort.Create(periodId, k17VersionId, cohortId);
        await rlsExecutor.ExecuteAsync(adminContext, ct => periodRepo.AttachCohortAsync(periodCohort, ct), cancellationToken);

        var periodOffering = MeasurementPeriodOffering.Create(
            periodId,
            k17VersionId,
            academicYearStart: 2023,
            offeringId,
            plannedSourceRole: "OFFICIAL",
            collectionStatus: "IN_PROGRESS",
            dueAt: DateTimeOffset.UtcNow.AddDays(15));

        await rlsExecutor.ExecuteAsync(adminContext, ct => periodRepo.AttachOfferingAsync(periodOffering, ct), cancellationToken);

        var targetId = Guid.NewGuid();
        var periodTarget = MeasurementPeriodTarget.Create(
            targetId,
            periodId,
            k17VersionId,
            outcomeLevel: "CLO",
            targetRole: "PRIMARY",
            courseOfferingId: offeringId,
            syllabusVersionId: syllabusVersionId,
            cloId: cloId);

        await rlsExecutor.ExecuteAsync(adminContext, ct => periodRepo.CreateTargetAsync(periodTarget, ct), cancellationToken);

        var loadedPeriod = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => periodRepo.GetPeriodByIdAsync(periodId, ct),
            cancellationToken);

        Assert.NotNull(loadedPeriod);
        Assert.NotEmpty(loadedPeriod.Cohorts!);
        Assert.NotEmpty(loadedPeriod.Offerings!);
        Assert.NotEmpty(loadedPeriod.Targets!);
        Assert.Equal("IT4201_2023_HK1_01", loadedPeriod.Offerings![0].CourseOfferingCode);

        // 7. Create Enrollment (Đăng ký học phần)
        var enrollmentId = Guid.NewGuid();
        var enrollment = Enrollment.Create(
            enrollmentId,
            offeringId,
            studentPersonId,
            attemptNo: 1,
            sourceSystemId: sourceSystemId,
            sourceRecordId: "SIS_ENR_23010002_IT4201");

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
                $"INSERT INTO iam.service_account (principal_id, client_id, owner_org_unit_id, purpose, expires_at, technical_contact) VALUES ({servicePrincipalId}, 'sis-service-client', {fitOrgId}, 'SIS Integration Service Account', NULL, 'admin@dnu.edu.vn')",
                cancellationToken);

            await ownerDbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO integration.source_system (id, code, name, system_type, base_url, owner_org_unit_id, service_principal_id, status, data_classification, created_at) VALUES ({sourceSystemId}, 'SIS_SYSTEM', 'Hệ thống Quản lý Đào tạo SIS', 'SIS', 'https://sis.dnu.edu.vn', {fitOrgId}, {servicePrincipalId}, 'ACTIVE', 'INTERNAL', CURRENT_TIMESTAMP)",
                cancellationToken);

            await ownerDbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO governance.governed_resource (id, resource_type, classification, disposition_status, created_at) VALUES ({ingestionGovResId}, 'integration.ingestion_batch', 'INTERNAL', 'ACTIVE', CURRENT_TIMESTAMP)",
                cancellationToken);

            await ownerDbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO integration.ingestion_batch (id, governed_resource_id, source_system_id, data_type, source_batch_id, idempotency_key, schema_version, payload_checksum, file_object_id, classification, status, received_at, completed_at, total_count, accepted_count, rejected_count) VALUES ({ingestionBatchId}, {ingestionGovResId}, {sourceSystemId}, 'ENROLLMENT', 'SIS_BATCH_01', {Guid.NewGuid().ToString()}, 1, repeat('6', 64), NULL, 'INTERNAL', 'COMPLETED', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 100, 100, 0)",
                cancellationToken);

            await ownerDbContext.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO governance.governed_resource (id, resource_type, classification, disposition_status, created_at) VALUES ({governedResourceId}, 'measurement.score_dataset', 'CONFIDENTIAL', 'ACTIVE', CURRENT_TIMESTAMP)",
                cancellationToken);
        }

        var initialRevision = EnrollmentRevision.Create(
            Guid.NewGuid(),
            enrollmentId,
            revisionNo: 1,
            enrollmentStatus: "ENROLLED",
            effectiveFrom: DateTimeOffset.UtcNow.AddDays(-20),
            ingestionBatchId: ingestionBatchId,
            checksum: Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
            recordedAt: DateTimeOffset.UtcNow);

        await using (var ownerDbContext = new OutcomeHubDbContext(ownerOptions))
        {
            await ownerDbContext.Set<Enrollment>().AddAsync(enrollment, cancellationToken);
            await ownerDbContext.Set<EnrollmentRevision>().AddAsync(initialRevision, cancellationToken);
            await ownerDbContext.SaveChangesAsync(cancellationToken);
        }

        var loadedEnrollment = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => enrollmentRepo.GetEnrollmentByIdAsync(enrollmentId, ct),
            cancellationToken);

        Assert.NotNull(loadedEnrollment);
        Assert.Equal("ENROLLED", loadedEnrollment.CurrentStatus);
        Assert.Equal("SIS_ENR_23010002_IT4201", loadedEnrollment.SourceRecordId);

        // 8. Submit ScoreRecord (Nhập điểm tiêu chí Rubric)
        var scoreDatasetId = Guid.NewGuid();
        var scoreDataset = ScoreDataset.Create(
            scoreDatasetId,
            governedResourceId,
            sourceSystemId,
            academicYearStart: 2023,
            courseOfferingId: offeringId,
            classification: "CONFIDENTIAL");

        await using (var ownerDbContext = new OutcomeHubDbContext(ownerOptions))
        {
            await ownerDbContext.Set<ScoreDataset>().AddAsync(scoreDataset, cancellationToken);
            await ownerDbContext.SaveChangesAsync(cancellationToken);
        }

        var scoreIdentityId = Guid.NewGuid();
        var scoreIdentity = ScoreIdentity.Create(
            scoreIdentityId,
            scoreDatasetId,
            academicYearStart: 2023,
            studentId: studentPersonId,
            courseOfferingId: offeringId,
            programVersionId: k17VersionId,
            syllabusVersionId: syllabusVersionId,
            attemptNo: 1,
            enrollmentId: enrollmentId,
            assessmentItemId: assessmentItemId,
            scoreLevel: "CRITERION",
            rubricCriterionId: rubricCriterionId);

        var scoreRecordId = Guid.NewGuid();
        var scoreRecord = ScoreRecord.Create(
            academicYearStart: 2023,
            id: scoreRecordId,
            scoreIdentityId: scoreIdentityId,
            studentId: studentPersonId,
            courseOfferingId: offeringId,
            orgUnitId: fitOrgId,
            programId: programId,
            programVersionId: k17VersionId,
            courseId: courseId,
            revisionNo: 1,
            rawScore: 8.5m,
            maxScore: 10.0m,
            scoreStatus: "SCORED",
            sourceSystemId: sourceSystemId,
            sourceRecordId: "LMS_SCORE_001",
            sourceRevision: "v1",
            ingestionBatchId: ingestionBatchId,
            recordedBy: adminPrincipalId,
            recordedAt: DateTimeOffset.UtcNow,
            checksum: Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));

        // Ingest score identity & record (via ingestion pipeline)
        await using (var ownerDbContext = new OutcomeHubDbContext(ownerOptions))
        {
            await ownerDbContext.Set<ScoreIdentity>().AddAsync(scoreIdentity, cancellationToken);
            await ownerDbContext.ScoreRecords.AddAsync(scoreRecord, cancellationToken);
            await ownerDbContext.SaveChangesAsync(cancellationToken);
        }

        // Verify reading score records under RLS via outcomehub_app
        var scores = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => scoreRepo.GetPagedScoresAsync(
                new PagedRequest { PageNumber = 1, PageSize = 10 },
                offeringId,
                studentPersonId,
                assessmentItemId,
                2023,
                ct),
            cancellationToken);

        Assert.NotEmpty(scores.Items);
        Assert.Equal(8.5m, scores.Items[0].RawScore);
        Assert.Equal("CRIT_01", scores.Items[0].RubricCriterionCode);
    }
}
