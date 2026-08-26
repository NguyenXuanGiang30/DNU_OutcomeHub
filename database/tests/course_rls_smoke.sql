\set ON_ERROR_STOP on

BEGIN;

SET LOCAL session_replication_role = replica;

INSERT INTO iam.principal (id, principal_type, status, display_name, created_at)
VALUES
    ('20000000-0000-7000-8000-000000000001', 'USER', 'ACTIVE', 'RLS manager A', CURRENT_TIMESTAMP),
    ('20000000-0000-7000-8000-000000000002', 'USER', 'ACTIVE', 'RLS reader B', CURRENT_TIMESTAMP),
    ('20000000-0000-7000-8000-000000000003', 'USER', 'ACTIVE', 'RLS no access', CURRENT_TIMESTAMP);

INSERT INTO academic.org_unit (
    id,
    parent_id,
    code,
    name,
    unit_type,
    effective_from,
    effective_to,
    status,
    created_at,
    created_by,
    updated_at,
    updated_by,
    row_version)
VALUES
    ('30000000-0000-7000-8000-000000000000', NULL, 'RLS_ROOT', 'RLS University', 'UNIVERSITY', DATE '2020-01-01', NULL, 'ACTIVE', CURRENT_TIMESTAMP, '20000000-0000-7000-8000-000000000001', CURRENT_TIMESTAMP, '20000000-0000-7000-8000-000000000001', 1),
    ('30000000-0000-7000-8000-000000000001', '30000000-0000-7000-8000-000000000000', 'RLS_FAC_A', 'RLS Faculty A', 'FACULTY', DATE '2020-01-01', NULL, 'ACTIVE', CURRENT_TIMESTAMP, '20000000-0000-7000-8000-000000000001', CURRENT_TIMESTAMP, '20000000-0000-7000-8000-000000000001', 1),
    ('30000000-0000-7000-8000-000000000002', '30000000-0000-7000-8000-000000000001', 'RLS_DEP_A', 'RLS Department A', 'DEPARTMENT', DATE '2020-01-01', NULL, 'ACTIVE', CURRENT_TIMESTAMP, '20000000-0000-7000-8000-000000000001', CURRENT_TIMESTAMP, '20000000-0000-7000-8000-000000000001', 1),
    ('30000000-0000-7000-8000-000000000003', '30000000-0000-7000-8000-000000000000', 'RLS_FAC_B', 'RLS Faculty B', 'FACULTY', DATE '2020-01-01', NULL, 'ACTIVE', CURRENT_TIMESTAMP, '20000000-0000-7000-8000-000000000001', CURRENT_TIMESTAMP, '20000000-0000-7000-8000-000000000001', 1);

INSERT INTO academic.course (id, code, name, owner_org_unit_id, status)
VALUES
    ('40000000-0000-7000-8000-000000000001', 'RLS_COURSE_A', 'RLS Course A', '30000000-0000-7000-8000-000000000001', 'ACTIVE'),
    ('40000000-0000-7000-8000-000000000002', 'RLS_COURSE_A_CHILD', 'RLS Course A child', '30000000-0000-7000-8000-000000000002', 'ACTIVE'),
    ('40000000-0000-7000-8000-000000000003', 'RLS_COURSE_B', 'RLS Course B', '30000000-0000-7000-8000-000000000003', 'ACTIVE');

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
VALUES
    ('50000000-0000-7000-8000-000000000001', 'ORG_UNIT', '30000000-0000-7000-8000-000000000001', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, true, repeat('a', 64), CURRENT_TIMESTAMP),
    ('50000000-0000-7000-8000-000000000002', 'ORG_UNIT', '30000000-0000-7000-8000-000000000003', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, false, repeat('b', 64), CURRENT_TIMESTAMP);

INSERT INTO iam.role (id, code, name, is_system, status, created_at)
VALUES
    ('60000000-0000-7000-8000-000000000001', 'RLS_COURSE_MANAGER', 'RLS Course manager', true, 'ACTIVE', CURRENT_TIMESTAMP),
    ('60000000-0000-7000-8000-000000000002', 'RLS_COURSE_READER', 'RLS Course reader', true, 'ACTIVE', CURRENT_TIMESTAMP);

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
VALUES
    ('61000000-0000-7000-8000-000000000001', '60000000-0000-7000-8000-000000000001', 1, 'ACTIVE', DATE '2020-01-01', NULL, '61100000-0000-7000-8000-000000000001', NULL, repeat('c', 64), repeat('d', 64), '20000000-0000-7000-8000-000000000001', CURRENT_TIMESTAMP),
    ('61000000-0000-7000-8000-000000000002', '60000000-0000-7000-8000-000000000002', 1, 'ACTIVE', DATE '2020-01-01', NULL, '61100000-0000-7000-8000-000000000002', NULL, repeat('e', 64), repeat('f', 64), '20000000-0000-7000-8000-000000000001', CURRENT_TIMESTAMP);

INSERT INTO iam.role_version_permission (role_version_id, permission_id, granted_at, granted_by)
VALUES
    ('61000000-0000-7000-8000-000000000001', '10000000-0000-7000-8000-000000000001', CURRENT_TIMESTAMP, '20000000-0000-7000-8000-000000000001'),
    ('61000000-0000-7000-8000-000000000001', '10000000-0000-7000-8000-000000000002', CURRENT_TIMESTAMP, '20000000-0000-7000-8000-000000000001'),
    ('61000000-0000-7000-8000-000000000001', '10000000-0000-7000-8000-000000000003', CURRENT_TIMESTAMP, '20000000-0000-7000-8000-000000000001'),
    ('61000000-0000-7000-8000-000000000001', '10000000-0000-7000-8000-000000000004', CURRENT_TIMESTAMP, '20000000-0000-7000-8000-000000000001'),
    ('61000000-0000-7000-8000-000000000002', '10000000-0000-7000-8000-000000000001', CURRENT_TIMESTAMP, '20000000-0000-7000-8000-000000000001');

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
VALUES
    ('62000000-0000-7000-8000-000000000001', '20000000-0000-7000-8000-000000000001', '60000000-0000-7000-8000-000000000001', '61000000-0000-7000-8000-000000000001', '50000000-0000-7000-8000-000000000001', CURRENT_TIMESTAMP - INTERVAL '1 day', CURRENT_TIMESTAMP + INTERVAL '1 day', 'ACTIVE', 'MANUAL', NULL, '20000000-0000-7000-8000-000000000001', '20000000-0000-7000-8000-000000000001', '62100000-0000-7000-8000-000000000001', '62200000-0000-7000-8000-000000000001', repeat('1', 64), '20000000-0000-7000-8000-000000000001', CURRENT_TIMESTAMP - INTERVAL '1 day', CURRENT_TIMESTAMP - INTERVAL '1 day', NULL, 'RLS smoke test manager assignment', NULL),
    ('62000000-0000-7000-8000-000000000002', '20000000-0000-7000-8000-000000000002', '60000000-0000-7000-8000-000000000002', '61000000-0000-7000-8000-000000000002', '50000000-0000-7000-8000-000000000002', CURRENT_TIMESTAMP - INTERVAL '1 day', CURRENT_TIMESTAMP + INTERVAL '1 day', 'ACTIVE', 'MANUAL', NULL, '20000000-0000-7000-8000-000000000001', '20000000-0000-7000-8000-000000000001', '62100000-0000-7000-8000-000000000002', '62200000-0000-7000-8000-000000000001', repeat('2', 64), '20000000-0000-7000-8000-000000000001', CURRENT_TIMESTAMP - INTERVAL '1 day', CURRENT_TIMESTAMP - INTERVAL '1 day', NULL, 'RLS smoke test reader assignment', NULL);

SET LOCAL session_replication_role = origin;
SET LOCAL ROLE outcomehub_app;

DO $test$
BEGIN
    IF iam.current_context_uuid('app.principal_id') IS NOT NULL THEN
        RAISE EXCEPTION 'Missing principal context must return NULL.';
    END IF;

    IF iam.current_context_uuid('app.unknown') IS NOT NULL THEN
        RAISE EXCEPTION 'Unknown context name must return NULL.';
    END IF;

    IF (SELECT count(*) FROM academic.course WHERE code LIKE 'RLS_%') <> 0 THEN
        RAISE EXCEPTION 'Missing context must deny every Course row.';
    END IF;
END;
$test$;

SELECT pg_catalog.set_config('app.principal_id', '', true);

DO $test$
BEGIN
    IF iam.current_context_uuid('app.principal_id') IS NOT NULL THEN
        RAISE EXCEPTION 'Empty principal context must return NULL.';
    END IF;
END;
$test$;

SELECT pg_catalog.set_config('app.principal_id', 'not-a-uuid', true);

DO $test$
BEGIN
    IF iam.current_context_uuid('app.principal_id') IS NOT NULL THEN
        RAISE EXCEPTION 'Malformed principal context must return NULL.';
    END IF;

    IF (SELECT count(*) FROM academic.course WHERE code LIKE 'RLS_%') <> 0 THEN
        RAISE EXCEPTION 'Malformed principal context must deny every Course row.';
    END IF;
END;
$test$;

SELECT pg_catalog.set_config('app.principal_id', '20000000-0000-7000-8000-000000000001', true);
SELECT pg_catalog.set_config('app.request_id', 'not-a-uuid', true);
SELECT pg_catalog.set_config('app.purpose', 'RLS_SMOKE_TEST', true);

DO $test$
BEGIN
    IF (SELECT count(*) FROM academic.course WHERE code LIKE 'RLS_%') <> 0 THEN
        RAISE EXCEPTION 'Malformed request context must deny every Course row.';
    END IF;
END;
$test$;

SELECT pg_catalog.set_config('app.request_id', '70000000-0000-7000-8000-000000000001', true);
SELECT pg_catalog.set_config('app.purpose', '', true);

DO $test$
BEGIN
    IF (SELECT count(*) FROM academic.course WHERE code LIKE 'RLS_%') <> 0 THEN
        RAISE EXCEPTION 'Empty purpose must deny every Course row.';
    END IF;
END;
$test$;

SELECT pg_catalog.set_config('app.purpose', 'RLS_SMOKE_TEST', true);

DO $test$
BEGIN
    IF (SELECT count(*) FROM academic.course WHERE code LIKE 'RLS_%') <> 2 THEN
        RAISE EXCEPTION 'Manager A must see Faculty A and its descendant only.';
    END IF;

    IF EXISTS (SELECT 1 FROM academic.course WHERE code = 'RLS_COURSE_B') THEN
        RAISE EXCEPTION 'Manager A must not see Faculty B.';
    END IF;
END;
$test$;

DO $test$
DECLARE
    affected_rows integer;
BEGIN
    UPDATE academic.course
    SET name = 'RLS active Course update denied'
    WHERE id = '40000000-0000-7000-8000-000000000001';

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows <> 0 THEN
        RAISE EXCEPTION 'An ACTIVE Course was updated through direct runtime CRUD.';
    END IF;

    DELETE FROM academic.course
    WHERE id = '40000000-0000-7000-8000-000000000001';

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows <> 0 THEN
        RAISE EXCEPTION 'An ACTIVE Course was hard-deleted through direct runtime CRUD.';
    END IF;
END;
$test$;

INSERT INTO academic.course (id, code, name, owner_org_unit_id, status)
VALUES ('40000000-0000-7000-8000-000000000004', 'RLS_NEW_A', 'RLS New Course A', '30000000-0000-7000-8000-000000000002', 'DRAFT');

DO $test$
BEGIN
    BEGIN
        INSERT INTO academic.course (id, code, name, owner_org_unit_id, status)
        VALUES ('40000000-0000-7000-8000-000000000005', 'RLS_NEW_B_DENIED', 'RLS New Course B denied', '30000000-0000-7000-8000-000000000003', 'DRAFT');

        RAISE EXCEPTION 'Cross-scope INSERT unexpectedly succeeded.';
    EXCEPTION
        WHEN insufficient_privilege THEN
            NULL;
    END;
END;
$test$;

UPDATE academic.course
SET name = 'RLS New Course A updated'
WHERE id = '40000000-0000-7000-8000-000000000004';

DO $test$
BEGIN
    BEGIN
        UPDATE academic.course
        SET owner_org_unit_id = '30000000-0000-7000-8000-000000000003'
        WHERE id = '40000000-0000-7000-8000-000000000004';

        RAISE EXCEPTION 'Cross-scope UPDATE unexpectedly succeeded.';
    EXCEPTION
        WHEN insufficient_privilege THEN
            NULL;
    END;
END;
$test$;

DO $test$
DECLARE
    affected_rows integer;
BEGIN
    DELETE FROM academic.course
    WHERE id = '40000000-0000-7000-8000-000000000003';

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows <> 0 THEN
        RAISE EXCEPTION 'Manager A deleted a Course owned by Faculty B.';
    END IF;
END;
$test$;

DELETE FROM academic.course
WHERE id = '40000000-0000-7000-8000-000000000004';

SELECT pg_catalog.set_config('app.principal_id', '20000000-0000-7000-8000-000000000002', true);

DO $test$
DECLARE
    affected_rows integer;
BEGIN
    IF (SELECT count(*) FROM academic.course WHERE code LIKE 'RLS_%') <> 1 THEN
        RAISE EXCEPTION 'Reader B must see exactly its own Faculty Course.';
    END IF;

    UPDATE academic.course
    SET name = 'RLS Course B denied update'
    WHERE id = '40000000-0000-7000-8000-000000000003';

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows <> 0 THEN
        RAISE EXCEPTION 'Reader B unexpectedly updated a Course.';
    END IF;

    BEGIN
        INSERT INTO academic.course (id, code, name, owner_org_unit_id, status)
        VALUES ('40000000-0000-7000-8000-000000000005', 'RLS_READER_INSERT', 'RLS Reader insert', '30000000-0000-7000-8000-000000000003', 'DRAFT');

        RAISE EXCEPTION 'Reader B unexpectedly inserted a Course.';
    EXCEPTION
        WHEN insufficient_privilege THEN
            NULL;
    END;
END;
$test$;

SELECT pg_catalog.set_config('app.principal_id', '20000000-0000-7000-8000-000000000003', true);

DO $test$
BEGIN
    IF (SELECT count(*) FROM academic.course WHERE code LIKE 'RLS_%') <> 0 THEN
        RAISE EXCEPTION 'Principal without assignment must not see Course rows.';
    END IF;

    BEGIN
        PERFORM 1 FROM iam.database_principal_binding LIMIT 1;
        RAISE EXCEPTION 'Application role unexpectedly read IAM tables.';
    EXCEPTION
        WHEN insufficient_privilege THEN
            NULL;
    END;

    IF pg_catalog.pg_has_role(CURRENT_USER, 'outcomehub_authorizer', 'MEMBER') THEN
        RAISE EXCEPTION 'Application role must not inherit the authorizer role.';
    END IF;
END;
$test$;

SET LOCAL row_security = off;

DO $test$
BEGIN
    BEGIN
        PERFORM 1 FROM academic.course LIMIT 1;
        RAISE EXCEPTION 'Application role unexpectedly bypassed RLS with row_security=off.';
    EXCEPTION
        WHEN insufficient_privilege THEN
            NULL;
    END;
END;
$test$;

SET LOCAL row_security = on;
RESET ROLE;

DO $test$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_class AS relation
        INNER JOIN pg_catalog.pg_namespace AS namespace
            ON namespace.oid = relation.relnamespace
        WHERE namespace.nspname = 'academic'
          AND relation.relname = 'course'
          AND relation.relrowsecurity
          AND relation.relforcerowsecurity) THEN
        RAISE EXCEPTION 'Course must have ENABLE and FORCE ROW LEVEL SECURITY.';
    END IF;

    IF (SELECT count(*) FROM pg_catalog.pg_policies WHERE schemaname = 'academic' AND tablename = 'course') <> 4 THEN
        RAISE EXCEPTION 'Course must have exactly four RLS policies.';
    END IF;
END;
$test$;

ROLLBACK;

BEGIN;
SET LOCAL ROLE outcomehub_app;

DO $test$
BEGIN
    IF iam.current_context_uuid('app.principal_id') IS NOT NULL
       OR iam.current_context_uuid('app.request_id') IS NOT NULL THEN
        RAISE EXCEPTION 'Transaction-local RLS context leaked into the next transaction.';
    END IF;
END;
$test$;

RESET ROLE;
ROLLBACK;

SELECT json_build_object(
    'status', 'passed',
    'resource', 'academic.course',
    'policies', 4,
    'fixtures_rolled_back', true,
    'context_leak', false);
