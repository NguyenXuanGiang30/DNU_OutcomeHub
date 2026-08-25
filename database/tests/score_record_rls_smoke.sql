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
    ('21000000-0000-7000-8000-000000000001', 'USER', 'ACTIVE', 'Score RLS manager', CURRENT_TIMESTAMP),
    ('21000000-0000-7000-8000-000000000002', 'USER', 'ACTIVE', 'Score RLS no access', CURRENT_TIMESTAMP);

INSERT INTO iam.role (
    id,
    code,
    name,
    is_system,
    status,
    created_at)
VALUES (
    '61000000-0000-7000-8000-000000000001',
    'SCORE_RLS_READER',
    'Score RLS reader',
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
    '62000000-0000-7000-8000-000000000001',
    '61000000-0000-7000-8000-000000000001',
    1,
    'ACTIVE',
    CURRENT_DATE - 1,
    NULL,
    '63000000-0000-7000-8000-000000000001',
    NULL,
    repeat('a', 64),
    repeat('b', 64),
    '21000000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP);

INSERT INTO iam.role_version_permission (
    role_version_id,
    permission_id,
    granted_at,
    granted_by)
VALUES (
    '62000000-0000-7000-8000-000000000001',
    '10000000-0000-7000-8000-000000000034',
    CURRENT_TIMESTAMP,
    '21000000-0000-7000-8000-000000000001');

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
    '64000000-0000-7000-8000-000000000001',
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
    '65000000-0000-7000-8000-000000000001',
    '21000000-0000-7000-8000-000000000001',
    '61000000-0000-7000-8000-000000000001',
    '62000000-0000-7000-8000-000000000001',
    '64000000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    CURRENT_TIMESTAMP + INTERVAL '1 day',
    'ACTIVE',
    'MANUAL',
    NULL,
    '21000000-0000-7000-8000-000000000001',
    '21000000-0000-7000-8000-000000000001',
    '66000000-0000-7000-8000-000000000001',
    '67000000-0000-7000-8000-000000000001',
    repeat('d', 64),
    '21000000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    NULL,
    'Score RLS smoke assignment',
    NULL);

INSERT INTO measurement.score_record (
    academic_year_start,
    id,
    score_identity_id,
    student_id,
    course_offering_id,
    org_unit_id,
    program_id,
    program_version_id,
    course_id,
    revision_no,
    raw_score,
    max_score,
    score_status,
    source_system_id,
    source_record_id,
    source_revision,
    ingestion_batch_id,
    supersedes_id,
    correction_reason,
    recorded_by,
    recorded_at,
    checksum)
VALUES (
    2026,
    '68000000-0000-7000-8000-000000000001',
    '68100000-0000-7000-8000-000000000001',
    '68200000-0000-7000-8000-000000000001',
    '68300000-0000-7000-8000-000000000001',
    '30000000-0000-7000-8000-000000000001',
    '68400000-0000-7000-8000-000000000001',
    '68500000-0000-7000-8000-000000000001',
    '68600000-0000-7000-8000-000000000001',
    1,
    75,
    100,
    'SCORED',
    '68700000-0000-7000-8000-000000000001',
    'SCORE_RLS_SMOKE',
    '1',
    '68800000-0000-7000-8000-000000000001',
    NULL,
    NULL,
    '21000000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP,
    repeat('e', 64));

SET LOCAL session_replication_role = origin;
SET LOCAL ROLE outcomehub_app;

DO $test$
BEGIN
    IF (SELECT count(*) FROM measurement.score_record) <> 0 THEN
        RAISE EXCEPTION 'Missing principal context must deny every score row.';
    END IF;
END;
$test$;

SELECT pg_catalog.set_config(
    'app.principal_id',
    '21000000-0000-7000-8000-000000000001',
    true);
SELECT pg_catalog.set_config(
    'app.request_id',
    '69000000-0000-7000-8000-000000000001',
    true);
SELECT pg_catalog.set_config('app.purpose', 'SCORE_RLS_SMOKE_TEST', true);

DO $test$
BEGIN
    IF (SELECT count(*) FROM measurement.score_record) <> 1 THEN
        RAISE EXCEPTION 'Scoped reader must see exactly one score row.';
    END IF;
END;
$test$;

SELECT pg_catalog.set_config(
    'app.principal_id',
    '21000000-0000-7000-8000-000000000002',
    true);

DO $test$
BEGIN
    IF (SELECT count(*) FROM measurement.score_record) <> 0 THEN
        RAISE EXCEPTION 'Unassigned principal must not see score rows.';
    END IF;
END;
$test$;

DO $test$
BEGIN
    BEGIN
        INSERT INTO measurement.score_record (
            academic_year_start,
            id,
            score_identity_id,
            student_id,
            course_offering_id,
            org_unit_id,
            program_id,
            program_version_id,
            course_id,
            revision_no,
            raw_score,
            max_score,
            score_status,
            source_system_id,
            source_record_id,
            source_revision,
            ingestion_batch_id,
            supersedes_id,
            correction_reason,
            recorded_by,
            recorded_at,
            checksum)
        SELECT
            academic_year_start,
            '68f00000-0000-7000-8000-000000000001',
            score_identity_id,
            student_id,
            course_offering_id,
            org_unit_id,
            program_id,
            program_version_id,
            course_id,
            revision_no + 1,
            raw_score,
            max_score,
            score_status,
            source_system_id,
            'SCORE_RLS_WRITE_DENIED',
            source_revision,
            ingestion_batch_id,
            id,
            'Direct runtime write must be denied.',
            recorded_by,
            recorded_at,
            repeat('f', 64)
        FROM measurement.score_record
        WHERE id = '68000000-0000-7000-8000-000000000001';

        RAISE EXCEPTION 'Direct score INSERT unexpectedly succeeded.';
    EXCEPTION
        WHEN insufficient_privilege THEN
            NULL;
    END;
END;
$test$;

ROLLBACK;

SELECT json_build_object(
    'status', 'passed',
    'score_record_scope_read', true,
    'score_record_runtime_write_denied', true,
    'fixtures_rolled_back', true);
