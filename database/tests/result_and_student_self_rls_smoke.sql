\set ON_ERROR_STOP on

BEGIN;

SET LOCAL session_replication_role = replica;

-- 1. Create 2 Persons & Students
INSERT INTO academic.person (
    id,
    full_name,
    status,
    effective_from)
VALUES
    ('81000000-0000-7000-8000-000000000001', 'Nguyen Van An', 'ACTIVE', DATE '2026-01-01'),
    ('81000000-0000-7000-8000-000000000002', 'Tran Van Binh', 'ACTIVE', DATE '2026-01-01');

INSERT INTO academic.student (
    person_id,
    student_code,
    admission_cohort_id,
    current_status)
VALUES
    ('81000000-0000-7000-8000-000000000001', 'SV001', '82000000-0000-7000-8000-000000000001', 'ACTIVE'),
    ('81000000-0000-7000-8000-000000000002', 'SV002', '82000000-0000-7000-8000-000000000001', 'ACTIVE');

-- 2. Principals for Student 1 and Student 2
INSERT INTO iam.principal (
    id,
    principal_type,
    status,
    display_name,
    created_at)
VALUES
    ('83000000-0000-7000-8000-000000000001', 'USER', 'ACTIVE', 'Sinh vien 1', CURRENT_TIMESTAMP),
    ('83000000-0000-7000-8000-000000000002', 'USER', 'ACTIVE', 'Sinh vien 2', CURRENT_TIMESTAMP);

-- Link Principal to UserAccount with Person
INSERT INTO iam.user_account (
    principal_id,
    person_id,
    username)
VALUES
    ('83000000-0000-7000-8000-000000000001', '81000000-0000-7000-8000-000000000001', 'sv001'),
    ('83000000-0000-7000-8000-000000000002', '81000000-0000-7000-8000-000000000002', 'sv002');

-- Role: STUDENT_RESULT_READER
INSERT INTO iam.role (
    id,
    code,
    name,
    is_system,
    status,
    created_at)
VALUES (
    '84000000-0000-7000-8000-000000000001',
    'STUDENT_SELF_READER',
    'Student Self Result Reader',
    true,
    'ACTIVE',
    CURRENT_TIMESTAMP);

INSERT INTO iam.role_version (
    id,
    role_id,
    version_no,
    status,
    effective_from,
    effective_to,
    workflow_instance_id,
    decision_id,
    permission_set_checksum,
    checksum,
    created_by,
    created_at)
VALUES (
    '84100000-0000-7000-8000-000000000001',
    '84000000-0000-7000-8000-000000000001',
    1,
    'ACTIVE',
    CURRENT_DATE - 1,
    NULL,
    '84200000-0000-7000-8000-000000000001',
    NULL,
    repeat('a', 64),
    repeat('b', 64),
    '83000000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP);

INSERT INTO iam.role_version_permission (
    role_version_id,
    permission_id,
    granted_at,
    granted_by)
VALUES
    ('84100000-0000-7000-8000-000000000001', '10000000-0000-7000-8000-000000000041', CURRENT_TIMESTAMP, '83000000-0000-7000-8000-000000000001'),
    ('84100000-0000-7000-8000-000000000001', '10000000-0000-7000-8000-000000000042', CURRENT_TIMESTAMP, '83000000-0000-7000-8000-000000000001'),
    ('84100000-0000-7000-8000-000000000001', '10000000-0000-7000-8000-000000000043', CURRENT_TIMESTAMP, '83000000-0000-7000-8000-000000000001');

-- Scopes: SELF for Student 1
INSERT INTO iam.access_scope (
    id,
    scope_type,
    org_unit_id,
    program_id,
    program_version_id,
    cohort_id,
    curriculum_path_id,
    course_id,
    course_offering_id,
    measurement_period_id,
    subject_principal_id,
    include_descendants,
    checksum,
    created_at)
VALUES (
    '84300000-0000-7000-8000-000000000001',
    'SELF',
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    '83000000-0000-7000-8000-000000000001',
    false,
    repeat('c', 64),
    CURRENT_TIMESTAMP);

INSERT INTO iam.role_assignment (
    id,
    principal_id,
    role_id,
    role_version_id,
    access_scope_id,
    effective_from,
    effective_to,
    status,
    source,
    source_reference,
    granted_by,
    approved_by,
    workflow_instance_id,
    sod_policy_version_id,
    authorization_snapshot_checksum,
    requested_by,
    requested_at,
    approved_at,
    revoked_at,
    reason,
    revoke_reason)
VALUES (
    '84400000-0000-7000-8000-000000000001',
    '83000000-0000-7000-8000-000000000001',
    '84000000-0000-7000-8000-000000000001',
    '84100000-0000-7000-8000-000000000001',
    '84300000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    CURRENT_TIMESTAMP + INTERVAL '1 day',
    'ACTIVE',
    'MANUAL',
    NULL,
    '83000000-0000-7000-8000-000000000001',
    '83000000-0000-7000-8000-000000000001',
    '84500000-0000-7000-8000-000000000001',
    '84600000-0000-7000-8000-000000000001',
    repeat('d', 64),
    '83000000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    NULL,
    'Student 1 self scope assignment',
    NULL);

-- Insert Student PI Results for Student 1 & Student 2
INSERT INTO result.student_pi_result (
    academic_year_start,
    id,
    batch_id,
    org_unit_id,
    program_id,
    program_version_id,
    measurement_period_id,
    cohort_id,
    curriculum_path_id,
    student_id,
    student_path_id,
    program_pi_id,
    method,
    score,
    theta_ind,
    attainment_status,
    core_gate_status,
    data_status,
    alpha)
VALUES
    (
        2026,
        '85000000-0000-7000-8000-000000000001',
        '86000000-0000-7000-8000-000000000001',
        '30000000-0000-7000-8000-000000000001',
        '87000000-0000-7000-8000-000000000001',
        '88000000-0000-7000-8000-000000000001',
        '89000000-0000-7000-8000-000000000001',
        '82000000-0000-7000-8000-000000000001',
        '89100000-0000-7000-8000-000000000001',
        '81000000-0000-7000-8000-000000000001', -- Student 1
        '89200000-0000-7000-8000-000000000001',
        '89300000-0000-7000-8000-000000000001',
        'DIRECT',
        85.0000000000,
        85.0000000000,
        'ATTAINED',
        'PASSED',
        'VALID',
        NULL
    ),
    (
        2026,
        '85000000-0000-7000-8000-000000000002',
        '86000000-0000-7000-8000-000000000001',
        '30000000-0000-7000-8000-000000000001',
        '87000000-0000-7000-8000-000000000001',
        '88000000-0000-7000-8000-000000000001',
        '89000000-0000-7000-8000-000000000001',
        '82000000-0000-7000-8000-000000000001',
        '89100000-0000-7000-8000-000000000001',
        '81000000-0000-7000-8000-000000000002', -- Student 2
        '89200000-0000-7000-8000-000000000002',
        '89300000-0000-7000-8000-000000000001',
        'DIRECT',
        65.0000000000,
        65.0000000000,
        'NOT_ATTAINED',
        'FAILED',
        'VALID',
        NULL
    );

SET LOCAL session_replication_role = origin;
SET LOCAL ROLE outcomehub_app;

-- Test 1: No context -> 0 rows
DO $test$
BEGIN
    IF (SELECT count(*) FROM result.student_pi_result) <> 0 THEN
        RAISE EXCEPTION 'Missing context must deny all result rows.';
    END IF;
END;
$test$;

-- Test 2: Student 1 context -> sees only Student 1 row (1 row)
SELECT pg_catalog.set_config(
    'app.principal_id',
    '83000000-0000-7000-8000-000000000001',
    true);
SELECT pg_catalog.set_config(
    'app.request_id',
    '89900000-0000-7000-8000-000000000001',
    true);
SELECT pg_catalog.set_config('app.purpose', 'STUDENT_SELF_RLS_SMOKE_TEST', true);

DO $test$
DECLARE
    seen_student uuid;
BEGIN
    IF (SELECT count(*) FROM result.student_pi_result) <> 1 THEN
        RAISE EXCEPTION 'Student 1 must see exactly 1 PI result row.';
    END IF;

    SELECT student_id INTO seen_student FROM result.student_pi_result;
    IF seen_student <> '81000000-0000-7000-8000-000000000001'::uuid THEN
        RAISE EXCEPTION 'Student 1 must only see their own result.';
    END IF;
END;
$test$;

-- Test 3: Student 2 without role assignment -> sees 0 rows
SELECT pg_catalog.set_config(
    'app.principal_id',
    '83000000-0000-7000-8000-000000000002',
    true);

DO $test$
BEGIN
    IF (SELECT count(*) FROM result.student_pi_result) <> 0 THEN
        RAISE EXCEPTION 'Student 2 without assignment must see 0 rows.';
    END IF;
END;
$test$;

ROLLBACK;

SELECT json_build_object(
    'status', 'passed',
    'student_self_rls_verified', true,
    'student_isolation_verified', true,
    'fixtures_rolled_back', true);
