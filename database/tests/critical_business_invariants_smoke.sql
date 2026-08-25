\set ON_ERROR_STOP on

BEGIN;

SET LOCAL session_replication_role = replica;

INSERT INTO academic.program (
    id, code, name, degree_level, education_mode, owner_org_unit_id, status,
    created_at, created_by, updated_at, updated_by, row_version)
VALUES
    ('81000000-0000-7000-8000-000000000001', 'INV_PA', 'Invariant Program A', 'BACHELOR', 'FULL_TIME', '81000000-0000-7000-8000-000000000101', 'DRAFT', CURRENT_TIMESTAMP, '81000000-0000-7000-8000-000000000102', CURRENT_TIMESTAMP, '81000000-0000-7000-8000-000000000102', 1),
    ('81000000-0000-7000-8000-000000000002', 'INV_PB', 'Invariant Program B', 'BACHELOR', 'FULL_TIME', '81000000-0000-7000-8000-000000000101', 'DRAFT', CURRENT_TIMESTAMP, '81000000-0000-7000-8000-000000000102', CURRENT_TIMESTAMP, '81000000-0000-7000-8000-000000000102', 1);

INSERT INTO academic.program_version (
    id, program_id, institution_template_version_id, version_no, code,
    decision_id, effective_from, effective_to, status, total_credits,
    workflow_instance_id, supersedes_id, checksum, row_version)
VALUES
    ('81000000-0000-7000-8000-000000000011', '81000000-0000-7000-8000-000000000001', '81000000-0000-7000-8000-000000000111', 1, 'INV_PVA', '81000000-0000-7000-8000-000000000112', DATE '2026-01-01', NULL, 'DRAFT', 120, '81000000-0000-7000-8000-000000000113', NULL, repeat('a', 64), 1),
    ('81000000-0000-7000-8000-000000000012', '81000000-0000-7000-8000-000000000002', '81000000-0000-7000-8000-000000000114', 1, 'INV_PVB', '81000000-0000-7000-8000-000000000115', DATE '2026-01-01', NULL, 'DRAFT', 120, '81000000-0000-7000-8000-000000000116', NULL, repeat('b', 64), 1);

INSERT INTO academic.cohort (
    id, program_id, code, name, admission_year, start_date, end_date)
VALUES
    ('81000000-0000-7000-8000-000000000021', '81000000-0000-7000-8000-000000000001', 'INV_CA', 'Invariant Cohort A', 2026, DATE '2026-01-01', NULL),
    ('81000000-0000-7000-8000-000000000022', '81000000-0000-7000-8000-000000000002', 'INV_CB', 'Invariant Cohort B', 2026, DATE '2026-01-01', NULL);

INSERT INTO academic.curriculum_plan (
    id, program_version_id, code, name, declared_total_credits, status, checksum)
VALUES (
    '81000000-0000-7000-8000-000000000031',
    '81000000-0000-7000-8000-000000000011',
    'INV_PLAN',
    'Invariant Curriculum Plan',
    120,
    'DRAFT',
    repeat('c', 64));

INSERT INTO academic.curriculum_block (
    id, curriculum_plan_id, parent_id, code, name, block_type,
    required_credits, maximum_credits, sort_order)
VALUES
    ('81000000-0000-7000-8000-000000000041', '81000000-0000-7000-8000-000000000031', NULL, 'INV_BA', 'Invariant Block A', 'REQUIRED', 0, NULL, 1),
    ('81000000-0000-7000-8000-000000000042', '81000000-0000-7000-8000-000000000031', '81000000-0000-7000-8000-000000000041', 'INV_BB', 'Invariant Block B', 'REQUIRED', 0, NULL, 2),
    ('81000000-0000-7000-8000-000000000043', '81000000-0000-7000-8000-000000000031', '81000000-0000-7000-8000-000000000042', 'INV_BC', 'Invariant Block C', 'REQUIRED', 0, NULL, 3);

INSERT INTO academic.program_course (
    id, program_version_id, course_version_id, curriculum_block_id,
    catalog_role, credit_override, is_locked, status)
VALUES (
    '81000000-0000-7000-8000-000000000051',
    '81000000-0000-7000-8000-000000000011',
    '81000000-0000-7000-8000-000000000151',
    '81000000-0000-7000-8000-000000000041',
    'REQUIRED',
    NULL,
    false,
    'DRAFT');

INSERT INTO portfolio.assessment_item (
    id, syllabus_version_id, parent_id, assessment_code, name,
    assessment_type, course_weight_ratio, individual_component_ratio,
    is_group_assessment, counts_toward_course_grade, max_score, sort_order)
VALUES
    ('81000000-0000-7000-8000-000000000061', '81000000-0000-7000-8000-000000000161', NULL, 'INV_AA', 'Invariant Assessment A', 'EXAM', 0.2, NULL, false, true, 10, 1),
    ('81000000-0000-7000-8000-000000000062', '81000000-0000-7000-8000-000000000161', '81000000-0000-7000-8000-000000000061', 'INV_AB', 'Invariant Assessment B', 'PART', 0.2, NULL, false, true, 10, 2),
    ('81000000-0000-7000-8000-000000000063', '81000000-0000-7000-8000-000000000161', '81000000-0000-7000-8000-000000000062', 'INV_AC', 'Invariant Assessment C', 'PART', 0.2, NULL, false, true, 10, 3);

INSERT INTO governance.resource_dependency (
    parent_governed_resource_id, child_governed_resource_id, dependency_role)
VALUES
    ('81000000-0000-7000-8000-000000000071', '81000000-0000-7000-8000-000000000072', 'DERIVED_FROM'),
    ('81000000-0000-7000-8000-000000000072', '81000000-0000-7000-8000-000000000073', 'DERIVED_FROM');

INSERT INTO result.result_batch (
    id, governed_resource_id, measurement_period_id, input_snapshot_id,
    policy_version_id, program_policy_binding_id, org_unit_id,
    program_version_id, academic_year_start, batch_no, engine_version,
    source_commit, container_digest, status, idempotency_key,
    request_checksum, recalculates_batch_id, recalculation_reason,
    workflow_instance_id, sod_policy_version_id, result_checksum,
    started_at, completed_at, published_at)
VALUES
    ('81000000-0000-7000-8000-000000000081', '81000000-0000-7000-8000-000000000181', '81000000-0000-7000-8000-000000000191', '81000000-0000-7000-8000-000000000201', '81000000-0000-7000-8000-000000000211', '81000000-0000-7000-8000-000000000221', '81000000-0000-7000-8000-000000000101', '81000000-0000-7000-8000-000000000011', 2026, 1, 'invariant-test', 'test', NULL, 'QUEUED', 'invariant-batch-1', repeat('1', 64), NULL, NULL, '81000000-0000-7000-8000-000000000231', '81000000-0000-7000-8000-000000000241', NULL, NULL, NULL, NULL),
    ('81000000-0000-7000-8000-000000000082', '81000000-0000-7000-8000-000000000182', '81000000-0000-7000-8000-000000000191', '81000000-0000-7000-8000-000000000202', '81000000-0000-7000-8000-000000000211', '81000000-0000-7000-8000-000000000221', '81000000-0000-7000-8000-000000000101', '81000000-0000-7000-8000-000000000011', 2026, 2, 'invariant-test', 'test', NULL, 'QUEUED', 'invariant-batch-2', repeat('2', 64), NULL, NULL, '81000000-0000-7000-8000-000000000232', '81000000-0000-7000-8000-000000000241', NULL, NULL, NULL, NULL),
    ('81000000-0000-7000-8000-000000000083', '81000000-0000-7000-8000-000000000183', '81000000-0000-7000-8000-000000000191', '81000000-0000-7000-8000-000000000203', '81000000-0000-7000-8000-000000000211', '81000000-0000-7000-8000-000000000221', '81000000-0000-7000-8000-000000000101', '81000000-0000-7000-8000-000000000011', 2026, 3, 'invariant-test', 'test', NULL, 'QUEUED', 'invariant-batch-3', repeat('3', 64), NULL, NULL, '81000000-0000-7000-8000-000000000233', '81000000-0000-7000-8000-000000000241', NULL, NULL, NULL, NULL),
    ('81000000-0000-7000-8000-000000000084', '81000000-0000-7000-8000-000000000184', '81000000-0000-7000-8000-000000000192', '81000000-0000-7000-8000-000000000204', '81000000-0000-7000-8000-000000000211', '81000000-0000-7000-8000-000000000221', '81000000-0000-7000-8000-000000000101', '81000000-0000-7000-8000-000000000011', 2026, 1, 'invariant-test', 'test', NULL, 'QUEUED', 'invariant-batch-4', repeat('4', 64), NULL, NULL, '81000000-0000-7000-8000-000000000234', '81000000-0000-7000-8000-000000000241', NULL, NULL, NULL, NULL);

INSERT INTO result.batch_supersession (
    old_batch_id, new_batch_id, reason, created_by, created_at)
VALUES
    ('81000000-0000-7000-8000-000000000081', '81000000-0000-7000-8000-000000000082', 'Invariant chain 1', '81000000-0000-7000-8000-000000000102', CURRENT_TIMESTAMP),
    ('81000000-0000-7000-8000-000000000082', '81000000-0000-7000-8000-000000000083', 'Invariant chain 2', '81000000-0000-7000-8000-000000000102', CURRENT_TIMESTAMP);

INSERT INTO integration.ingestion_batch (
    id, governed_resource_id, source_system_id, data_type, source_batch_id,
    idempotency_key, schema_version, payload_checksum, file_object_id,
    classification, status, received_at, completed_at, total_count,
    accepted_count, rejected_count)
VALUES (
    '81000000-0000-7000-8000-000000000091',
    '81000000-0000-7000-8000-000000000291',
    '81000000-0000-7000-8000-000000000292',
    'STUDENT',
    'invariant-source-batch',
    'invariant-ingestion',
    1,
    repeat('d', 64),
    NULL,
    'INTERNAL',
    'RECEIVED',
    CURRENT_TIMESTAMP,
    NULL,
    2,
    0,
    0);

INSERT INTO integration.raw_record (
    id, ingestion_batch_id, row_no, source_record_id, source_updated_at,
    payload, payload_checksum, received_at)
VALUES
    (8100001, '81000000-0000-7000-8000-000000000091', 1, 'invariant-raw-1', NULL, '{}', repeat('e', 64), CURRENT_TIMESTAMP),
    (8100002, '81000000-0000-7000-8000-000000000099', 2, 'invariant-raw-2', NULL, '{}', repeat('f', 64), CURRENT_TIMESTAMP);

INSERT INTO integration.staging_student (
    id, ingestion_batch_id, row_no, raw_record_id, student_code,
    full_name, email, resolved_student_id, validation_status, row_checksum)
VALUES (
    '81000000-0000-7000-8000-000000000092',
    '81000000-0000-7000-8000-000000000091',
    1,
    8100001,
    'INV_STUDENT',
    'Invariant Student',
    NULL,
    NULL,
    'PENDING',
    repeat('0', 64));

SET LOCAL session_replication_role = origin;
SET CONSTRAINTS ALL IMMEDIATE;

INSERT INTO academic.program_version_cohort (
    program_version_id, cohort_id, effective_from, effective_to, is_default)
VALUES (
    '81000000-0000-7000-8000-000000000011',
    '81000000-0000-7000-8000-000000000021',
    DATE '2026-01-01',
    NULL,
    true);

DO $test$
BEGIN
    BEGIN
        INSERT INTO academic.program_version_cohort (
            program_version_id, cohort_id, effective_from, effective_to, is_default)
        VALUES (
            '81000000-0000-7000-8000-000000000011',
            '81000000-0000-7000-8000-000000000022',
            DATE '2026-01-01',
            NULL,
            false);
        RAISE EXCEPTION 'Cross-program ProgramVersionCohort unexpectedly succeeded.';
    EXCEPTION WHEN check_violation THEN NULL;
    END;

    BEGIN
        UPDATE academic.cohort
        SET program_id = '81000000-0000-7000-8000-000000000002'
        WHERE id = '81000000-0000-7000-8000-000000000021';
        RAISE EXCEPTION 'Parent-side Cohort update broke ProgramVersion binding.';
    EXCEPTION WHEN check_violation THEN NULL;
    END;

    BEGIN
        UPDATE academic.curriculum_plan
        SET program_version_id = '81000000-0000-7000-8000-000000000012'
        WHERE id = '81000000-0000-7000-8000-000000000031';
        RAISE EXCEPTION 'CurriculumPlan update broke ProgramCourse binding.';
    EXCEPTION WHEN check_violation THEN NULL;
    END;

    BEGIN
        UPDATE academic.curriculum_block
        SET parent_id = '81000000-0000-7000-8000-000000000043'
        WHERE id = '81000000-0000-7000-8000-000000000041';
        RAISE EXCEPTION 'CurriculumBlock cycle unexpectedly succeeded.';
    EXCEPTION WHEN check_violation THEN NULL;
    END;

    BEGIN
        UPDATE portfolio.assessment_item
        SET parent_id = '81000000-0000-7000-8000-000000000063'
        WHERE id = '81000000-0000-7000-8000-000000000061';
        RAISE EXCEPTION 'AssessmentItem cycle unexpectedly succeeded.';
    EXCEPTION WHEN check_violation THEN NULL;
    END;

    BEGIN
        INSERT INTO governance.resource_dependency (
            parent_governed_resource_id, child_governed_resource_id, dependency_role)
        VALUES (
            '81000000-0000-7000-8000-000000000073',
            '81000000-0000-7000-8000-000000000071',
            'DERIVED_FROM');
        RAISE EXCEPTION 'Governed-resource dependency cycle unexpectedly succeeded.';
    EXCEPTION WHEN check_violation THEN NULL;
    END;

    BEGIN
        INSERT INTO result.batch_supersession (
            old_batch_id, new_batch_id, reason, created_by, created_at)
        VALUES (
            '81000000-0000-7000-8000-000000000083',
            '81000000-0000-7000-8000-000000000081',
            'Invariant cycle denied',
            '81000000-0000-7000-8000-000000000102',
            CURRENT_TIMESTAMP);
        RAISE EXCEPTION 'ResultBatch supersession cycle unexpectedly succeeded.';
    EXCEPTION WHEN check_violation THEN NULL;
    END;

    BEGIN
        INSERT INTO result.batch_supersession (
            old_batch_id, new_batch_id, reason, created_by, created_at)
        VALUES (
            '81000000-0000-7000-8000-000000000083',
            '81000000-0000-7000-8000-000000000084',
            'Cross-period supersession denied',
            '81000000-0000-7000-8000-000000000102',
            CURRENT_TIMESTAMP);
        RAISE EXCEPTION 'Cross-period ResultBatch supersession unexpectedly succeeded.';
    EXCEPTION WHEN check_violation THEN NULL;
    END;
END;
$test$;

INSERT INTO integration.validation_issue (
    id, ingestion_batch_id, raw_record_id, staging_table, staging_row_id,
    field_name, error_code, severity, message, suggested_action, status,
    resolved_by, resolved_at)
VALUES (
    '81000000-0000-7000-8000-000000000093',
    '81000000-0000-7000-8000-000000000091',
    8100001,
    'staging_student',
    '81000000-0000-7000-8000-000000000092',
    'student_code',
    'INV_TEST',
    'ERROR',
    'Invariant validation issue',
    NULL,
    'OPEN',
    NULL,
    NULL);

DO $test$
BEGIN
    BEGIN
        INSERT INTO integration.validation_issue (
            id, ingestion_batch_id, raw_record_id, staging_table, staging_row_id,
            error_code, severity, message, status)
        VALUES (
            '81000000-0000-7000-8000-000000000094',
            '81000000-0000-7000-8000-000000000091',
            8100001,
            'integration.staging_student',
            '81000000-0000-7000-8000-000000000092',
            'INV_UNKNOWN_TABLE',
            'ERROR',
            'Unknown locator denied',
            'OPEN');
        RAISE EXCEPTION 'Unknown staging-table locator unexpectedly succeeded.';
    EXCEPTION WHEN check_violation THEN NULL;
    END;

    BEGIN
        INSERT INTO integration.validation_issue (
            id, ingestion_batch_id, raw_record_id, error_code, severity, message, status)
        VALUES (
            '81000000-0000-7000-8000-000000000095',
            '81000000-0000-7000-8000-000000000091',
            8100002,
            'INV_WRONG_BATCH',
            'ERROR',
            'Wrong raw batch denied',
            'OPEN');
        RAISE EXCEPTION 'Cross-batch RawRecord locator unexpectedly succeeded.';
    EXCEPTION WHEN check_violation THEN NULL;
    END;

    BEGIN
        UPDATE integration.staging_student
        SET raw_record_id = 8100002
        WHERE id = '81000000-0000-7000-8000-000000000092';
        RAISE EXCEPTION 'Referenced staging locator mutation unexpectedly succeeded.';
    EXCEPTION WHEN check_violation THEN NULL;
    END;
END;
$test$;

ROLLBACK;

SELECT json_build_object(
    'status', 'passed',
    'typed_bindings', true,
    'acyclic_graphs', true,
    'batch_supersession', true,
    'validation_issue_locator', true,
    'fixtures_rolled_back', true);
