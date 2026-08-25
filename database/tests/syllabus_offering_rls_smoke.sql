\set ON_ERROR_STOP on

BEGIN;

SET LOCAL session_replication_role = replica;

INSERT INTO iam.principal (
    id,
    principal_type,
    status,
    display_name,
    created_at)
VALUES
    ('22000000-0000-7000-8000-000000000001', 'USER', 'ACTIVE', 'Syllabus Manager Khoa CNTT', CURRENT_TIMESTAMP),
    ('22000000-0000-7000-8000-000000000002', 'USER', 'ACTIVE', 'Syllabus Reader Unassigned', CURRENT_TIMESTAMP);

INSERT INTO iam.role (
    id,
    code,
    name,
    is_system,
    status,
    created_at)
VALUES (
    '62100000-0000-7000-8000-000000000001',
    'SYLLABUS_OFFERING_MANAGER',
    'Syllabus & Offering Manager',
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
    '62200000-0000-7000-8000-000000000001',
    '62100000-0000-7000-8000-000000000001',
    1,
    'ACTIVE',
    CURRENT_DATE - 1,
    NULL,
    '62300000-0000-7000-8000-000000000001',
    NULL,
    repeat('a', 64),
    repeat('b', 64),
    '22000000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP);

INSERT INTO iam.role_version_permission (
    role_version_id,
    permission_id,
    granted_at,
    granted_by)
VALUES
    ('62200000-0000-7000-8000-000000000001', '10000000-0000-7000-8000-000000000035', CURRENT_TIMESTAMP, '22000000-0000-7000-8000-000000000001'),
    ('62200000-0000-7000-8000-000000000001', '10000000-0000-7000-8000-00000000003c', CURRENT_TIMESTAMP, '22000000-0000-7000-8000-000000000001');

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
    '62400000-0000-7000-8000-000000000001',
    'ORG_UNIT',
    '30000000-0000-7000-8000-000000000001',
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
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
    '62500000-0000-7000-8000-000000000001',
    '22000000-0000-7000-8000-000000000001',
    '62100000-0000-7000-8000-000000000001',
    '62200000-0000-7000-8000-000000000001',
    '62400000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    CURRENT_TIMESTAMP + INTERVAL '1 day',
    'ACTIVE',
    'MANUAL',
    NULL,
    '22000000-0000-7000-8000-000000000001',
    '22000000-0000-7000-8000-000000000001',
    '62600000-0000-7000-8000-000000000001',
    '62700000-0000-7000-8000-000000000001',
    repeat('d', 64),
    '22000000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    NULL,
    'Syllabus & Offering smoke assignment',
    NULL);

INSERT INTO portfolio.syllabus (
    id,
    program_course_id,
    code,
    owner_org_unit_id,
    created_at)
VALUES (
    '71000000-0000-7000-8000-000000000001',
    '72000000-0000-7000-8000-000000000001',
    'SYLLABUS_IT4101',
    '30000000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP);

INSERT INTO academic.course_offering (
    id,
    code,
    program_course_id,
    course_version_id,
    program_version_id,
    syllabus_version_id,
    academic_year_start,
    term_code,
    org_unit_id,
    status,
    start_date,
    end_date,
    source_system_id,
    source_record_id)
VALUES (
    '73000000-0000-7000-8000-000000000001',
    'IT4101_2026_HK1',
    '72000000-0000-7000-8000-000000000001',
    '74000000-0000-7000-8000-000000000001',
    '75000000-0000-7000-8000-000000000001',
    '76000000-0000-7000-8000-000000000001',
    2026,
    'HK1_2026',
    '30000000-0000-7000-8000-000000000001',
    'ACTIVE',
    CURRENT_DATE,
    CURRENT_DATE + 90,
    NULL,
    NULL);

SET LOCAL session_replication_role = origin;
SET LOCAL ROLE outcomehub_app;

DO $test$
BEGIN
    IF (SELECT count(*) FROM portfolio.syllabus) <> 0 THEN
        RAISE EXCEPTION 'Missing principal context must deny syllabus rows.';
    END IF;
    IF (SELECT count(*) FROM academic.course_offering) <> 0 THEN
        RAISE EXCEPTION 'Missing principal context must deny course offering rows.';
    END IF;
END;
$test$;

SELECT pg_catalog.set_config(
    'app.principal_id',
    '22000000-0000-7000-8000-000000000001',
    true);
SELECT pg_catalog.set_config(
    'app.request_id',
    '77000000-0000-7000-8000-000000000001',
    true);
SELECT pg_catalog.set_config('app.purpose', 'SYLLABUS_RLS_SMOKE_TEST', true);

DO $test$
BEGIN
    IF (SELECT count(*) FROM portfolio.syllabus) <> 1 THEN
        RAISE EXCEPTION 'Scoped manager must see exactly one syllabus row.';
    END IF;
    IF (SELECT count(*) FROM academic.course_offering) <> 1 THEN
        RAISE EXCEPTION 'Scoped manager must see exactly one course offering row.';
    END IF;
END;
$test$;

SELECT pg_catalog.set_config(
    'app.principal_id',
    '22000000-0000-7000-8000-000000000002',
    true);

DO $test$
BEGIN
    IF (SELECT count(*) FROM portfolio.syllabus) <> 0 THEN
        RAISE EXCEPTION 'Unassigned principal must not see syllabus rows.';
    END IF;
    IF (SELECT count(*) FROM academic.course_offering) <> 0 THEN
        RAISE EXCEPTION 'Unassigned principal must not see course offering rows.';
    END IF;
END;
$test$;

ROLLBACK;

SELECT json_build_object(
    'status', 'passed',
    'syllabus_and_offering_rls_verified', true,
    'fixtures_rolled_back', true);
