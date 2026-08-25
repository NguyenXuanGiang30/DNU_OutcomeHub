\set ON_ERROR_STOP on

BEGIN;

SET LOCAL session_replication_role = replica;

INSERT INTO iam.principal (id, principal_type, status, display_name, created_at)
VALUES
    ('82000000-0000-7000-8000-000000000101', 'USER', 'ACTIVE', 'Org root manager', CURRENT_TIMESTAMP),
    ('82000000-0000-7000-8000-000000000102', 'USER', 'ACTIVE', 'Program scoped reader', CURRENT_TIMESTAMP),
    ('82000000-0000-7000-8000-000000000103', 'USER', 'ACTIVE', 'Course scoped reader', CURRENT_TIMESTAMP),
    ('82000000-0000-7000-8000-000000000104', 'USER', 'ACTIVE', 'No access principal', CURRENT_TIMESTAMP);

INSERT INTO academic.org_unit (
    id, parent_id, code, name, unit_type, effective_from, effective_to,
    status, created_at, created_by, updated_at, updated_by, row_version)
VALUES
    ('82000000-0000-7000-8000-000000000000', NULL, 'ROOT_RLS_ROOT', 'Root RLS University', 'UNIVERSITY', DATE '2020-01-01', NULL, 'ACTIVE', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101', 1),
    ('82000000-0000-7000-8000-000000000001', '82000000-0000-7000-8000-000000000000', 'ROOT_RLS_FAC_A', 'Root RLS Faculty A', 'FACULTY', DATE '2020-01-01', NULL, 'ACTIVE', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101', 1),
    ('82000000-0000-7000-8000-000000000002', '82000000-0000-7000-8000-000000000001', 'ROOT_RLS_DEP_A', 'Root RLS Department A', 'DEPARTMENT', DATE '2020-01-01', NULL, 'ACTIVE', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101', 1),
    ('82000000-0000-7000-8000-000000000003', '82000000-0000-7000-8000-000000000000', 'ROOT_RLS_FAC_B', 'Root RLS Faculty B', 'FACULTY', DATE '2020-01-01', NULL, 'ACTIVE', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101', 1);

INSERT INTO academic.program (
    id, code, name, degree_level, education_mode, owner_org_unit_id, status,
    created_at, created_by, updated_at, updated_by, row_version)
VALUES
    ('82000000-0000-7000-8000-000000000201', 'ROOT_RLS_PROGRAM_A', 'Root RLS Program A', 'BACHELOR', 'FULL_TIME', '82000000-0000-7000-8000-000000000002', 'ACTIVE', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101', 1),
    ('82000000-0000-7000-8000-000000000202', 'ROOT_RLS_PROGRAM_B', 'Root RLS Program B', 'BACHELOR', 'FULL_TIME', '82000000-0000-7000-8000-000000000003', 'ACTIVE', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101', 1);

INSERT INTO academic.course (id, code, name, owner_org_unit_id, status)
VALUES
    ('82000000-0000-7000-8000-000000000301', 'ROOT_RLS_COURSE_A', 'Root RLS Course A', '82000000-0000-7000-8000-000000000002', 'ACTIVE'),
    ('82000000-0000-7000-8000-000000000302', 'ROOT_RLS_COURSE_B', 'Root RLS Course B', '82000000-0000-7000-8000-000000000003', 'ACTIVE');

INSERT INTO academic.institution_template (
    id, code, name, owner_org_unit_id, description, created_at)
VALUES
    ('82000000-0000-7000-8000-000000000401', 'ROOT_RLS_IT_A', 'Root RLS Institution Template A', '82000000-0000-7000-8000-000000000002', NULL, CURRENT_TIMESTAMP),
    ('82000000-0000-7000-8000-000000000402', 'ROOT_RLS_IT_B', 'Root RLS Institution Template B', '82000000-0000-7000-8000-000000000003', NULL, CURRENT_TIMESTAMP);

INSERT INTO portfolio.syllabus_template (
    id, code, name, owner_org_unit_id, description)
VALUES
    ('82000000-0000-7000-8000-000000000411', 'ROOT_RLS_ST_A', 'Root RLS Syllabus Template A', '82000000-0000-7000-8000-000000000002', NULL),
    ('82000000-0000-7000-8000-000000000412', 'ROOT_RLS_ST_B', 'Root RLS Syllabus Template B', '82000000-0000-7000-8000-000000000003', NULL);

INSERT INTO portfolio.shared_syllabus_core (
    id, course_id, owner_org_unit_id, code)
VALUES
    ('82000000-0000-7000-8000-000000000421', '82000000-0000-7000-8000-000000000301', '82000000-0000-7000-8000-000000000002', 'ROOT_RLS_CORE_A'),
    ('82000000-0000-7000-8000-000000000422', '82000000-0000-7000-8000-000000000302', '82000000-0000-7000-8000-000000000003', 'ROOT_RLS_CORE_B');

INSERT INTO integration.source_system (
    id, code, name, system_type, base_url, owner_org_unit_id,
    service_principal_id, status, data_classification, created_at)
VALUES
    ('82000000-0000-7000-8000-000000000431', 'root-rls-source-a', 'Root RLS Source A', 'LMS', NULL, '82000000-0000-7000-8000-000000000002', '82000000-0000-7000-8000-000000000531', 'ACTIVE', 'INTERNAL', CURRENT_TIMESTAMP),
    ('82000000-0000-7000-8000-000000000432', 'root-rls-source-b', 'Root RLS Source B', 'LMS', NULL, '82000000-0000-7000-8000-000000000003', '82000000-0000-7000-8000-000000000532', 'ACTIVE', 'INTERNAL', CURRENT_TIMESTAMP);

INSERT INTO measurement.calculation_policy (
    id, code, name, owner_org_unit_id, description, created_at)
VALUES
    ('82000000-0000-7000-8000-000000000441', 'ROOT_RLS_POLICY_A', 'Root RLS Policy A', '82000000-0000-7000-8000-000000000002', NULL, CURRENT_TIMESTAMP),
    ('82000000-0000-7000-8000-000000000442', 'ROOT_RLS_POLICY_B', 'Root RLS Policy B', '82000000-0000-7000-8000-000000000003', NULL, CURRENT_TIMESTAMP);

INSERT INTO measurement.indirect_instrument (
    id, code, name, owner_org_unit_id)
VALUES
    ('82000000-0000-7000-8000-000000000451', 'ROOT_RLS_INSTRUMENT_A', 'Root RLS Instrument A', '82000000-0000-7000-8000-000000000002'),
    ('82000000-0000-7000-8000-000000000452', 'ROOT_RLS_INSTRUMENT_B', 'Root RLS Instrument B', '82000000-0000-7000-8000-000000000003');

INSERT INTO iam.access_scope (
    id, scope_type, org_unit_id, program_id, program_version_id, cohort_id,
    curriculum_path_id, course_id, course_offering_id, measurement_period_id,
    subject_principal_id, include_descendants, checksum, created_at)
VALUES
    ('82000000-0000-7000-8000-000000000601', 'ORG_UNIT', '82000000-0000-7000-8000-000000000001', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, true, repeat('5', 64), CURRENT_TIMESTAMP),
    ('82000000-0000-7000-8000-000000000602', 'PROGRAM', NULL, '82000000-0000-7000-8000-000000000201', NULL, NULL, NULL, NULL, NULL, NULL, NULL, false, repeat('6', 64), CURRENT_TIMESTAMP),
    ('82000000-0000-7000-8000-000000000603', 'COURSE', NULL, NULL, NULL, NULL, NULL, '82000000-0000-7000-8000-000000000301', NULL, NULL, NULL, false, repeat('7', 64), CURRENT_TIMESTAMP);

INSERT INTO iam.role (id, code, name, is_system, status, created_at)
VALUES
    ('82000000-0000-7000-8000-000000000701', 'ROOT_RLS_ORG_MANAGER', 'Root RLS organization manager', true, 'ACTIVE', CURRENT_TIMESTAMP),
    ('82000000-0000-7000-8000-000000000702', 'ROOT_RLS_PROGRAM_READER', 'Root RLS program reader', true, 'ACTIVE', CURRENT_TIMESTAMP),
    ('82000000-0000-7000-8000-000000000703', 'ROOT_RLS_COURSE_READER', 'Root RLS Course reader', true, 'ACTIVE', CURRENT_TIMESTAMP);

INSERT INTO iam.role_version (
    id, role_id, version_no, status, effective_from, effective_to,
    workflow_instance_id, decision_id, permission_set_checksum, checksum,
    created_by, created_at)
VALUES
    ('82000000-0000-7000-8000-000000000711', '82000000-0000-7000-8000-000000000701', 1, 'ACTIVE', DATE '2020-01-01', NULL, '82000000-0000-7000-8000-000000000721', NULL, repeat('8', 64), repeat('9', 64), '82000000-0000-7000-8000-000000000101', CURRENT_TIMESTAMP),
    ('82000000-0000-7000-8000-000000000712', '82000000-0000-7000-8000-000000000702', 1, 'ACTIVE', DATE '2020-01-01', NULL, '82000000-0000-7000-8000-000000000722', NULL, repeat('a', 64), repeat('b', 64), '82000000-0000-7000-8000-000000000101', CURRENT_TIMESTAMP),
    ('82000000-0000-7000-8000-000000000713', '82000000-0000-7000-8000-000000000703', 1, 'ACTIVE', DATE '2020-01-01', NULL, '82000000-0000-7000-8000-000000000723', NULL, repeat('c', 64), repeat('d', 64), '82000000-0000-7000-8000-000000000101', CURRENT_TIMESTAMP);

INSERT INTO iam.role_version_permission (
    role_version_id, permission_id, granted_at, granted_by)
SELECT
    '82000000-0000-7000-8000-000000000711'::uuid,
    permission.id,
    CURRENT_TIMESTAMP,
    '82000000-0000-7000-8000-000000000101'::uuid
FROM iam.permission AS permission
WHERE permission.id::text BETWEEN
    '10000000-0000-7000-8000-000000000005'
    AND '10000000-0000-7000-8000-000000000033';

INSERT INTO iam.role_version_permission (
    role_version_id, permission_id, granted_at, granted_by)
VALUES
    ('82000000-0000-7000-8000-000000000712', '10000000-0000-7000-8000-000000000010', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101'),
    ('82000000-0000-7000-8000-000000000713', '10000000-0000-7000-8000-000000000018', CURRENT_TIMESTAMP, '82000000-0000-7000-8000-000000000101');

INSERT INTO iam.role_assignment (
    id, principal_id, role_id, role_version_id, access_scope_id,
    effective_from, effective_to, status, source, source_reference,
    granted_by, approved_by, workflow_instance_id, sod_policy_version_id,
    authorization_snapshot_checksum, requested_by, requested_at, approved_at,
    revoked_at, reason, revoke_reason)
VALUES
    ('82000000-0000-7000-8000-000000000731', '82000000-0000-7000-8000-000000000101', '82000000-0000-7000-8000-000000000701', '82000000-0000-7000-8000-000000000711', '82000000-0000-7000-8000-000000000601', CURRENT_TIMESTAMP - INTERVAL '1 day', CURRENT_TIMESTAMP + INTERVAL '1 day', 'ACTIVE', 'MANUAL', NULL, '82000000-0000-7000-8000-000000000101', '82000000-0000-7000-8000-000000000101', '82000000-0000-7000-8000-000000000741', '82000000-0000-7000-8000-000000000751', repeat('e', 64), '82000000-0000-7000-8000-000000000101', CURRENT_TIMESTAMP - INTERVAL '1 day', CURRENT_TIMESTAMP - INTERVAL '1 day', NULL, 'Org root RLS smoke manager', NULL),
    ('82000000-0000-7000-8000-000000000732', '82000000-0000-7000-8000-000000000102', '82000000-0000-7000-8000-000000000702', '82000000-0000-7000-8000-000000000712', '82000000-0000-7000-8000-000000000602', CURRENT_TIMESTAMP - INTERVAL '1 day', CURRENT_TIMESTAMP + INTERVAL '1 day', 'ACTIVE', 'MANUAL', NULL, '82000000-0000-7000-8000-000000000101', '82000000-0000-7000-8000-000000000101', '82000000-0000-7000-8000-000000000742', '82000000-0000-7000-8000-000000000751', repeat('f', 64), '82000000-0000-7000-8000-000000000102', CURRENT_TIMESTAMP - INTERVAL '1 day', CURRENT_TIMESTAMP - INTERVAL '1 day', NULL, 'Program RLS smoke reader', NULL),
    ('82000000-0000-7000-8000-000000000733', '82000000-0000-7000-8000-000000000103', '82000000-0000-7000-8000-000000000703', '82000000-0000-7000-8000-000000000713', '82000000-0000-7000-8000-000000000603', CURRENT_TIMESTAMP - INTERVAL '1 day', CURRENT_TIMESTAMP + INTERVAL '1 day', 'ACTIVE', 'MANUAL', NULL, '82000000-0000-7000-8000-000000000101', '82000000-0000-7000-8000-000000000101', '82000000-0000-7000-8000-000000000743', '82000000-0000-7000-8000-000000000751', repeat('0', 64), '82000000-0000-7000-8000-000000000103', CURRENT_TIMESTAMP - INTERVAL '1 day', CURRENT_TIMESTAMP - INTERVAL '1 day', NULL, 'Course RLS smoke reader', NULL);

SET LOCAL session_replication_role = origin;
SET LOCAL ROLE outcomehub_app;

DO $test$
BEGIN
    IF (SELECT count(*) FROM academic.org_unit WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM academic.program WHERE code LIKE 'ROOT_RLS_%') <> 0 THEN
        RAISE EXCEPTION 'Missing request context must deny org-owned root rows.';
    END IF;
END;
$test$;

SELECT pg_catalog.set_config('app.principal_id', '82000000-0000-7000-8000-000000000101', true);
SELECT pg_catalog.set_config('app.request_id', '82000000-0000-7000-8000-000000000801', true);
SELECT pg_catalog.set_config('app.purpose', 'ORG_ROOT_RLS_SMOKE', true);

DO $test$
BEGIN
    IF (SELECT count(*) FROM academic.org_unit WHERE code LIKE 'ROOT_RLS_%') <> 2 THEN
        RAISE EXCEPTION 'ORG_UNIT descendant scope must see Faculty A and Department A.';
    END IF;

    IF (SELECT count(*) FROM academic.program WHERE code LIKE 'ROOT_RLS_%') <> 1
       OR (SELECT count(*) FROM academic.institution_template WHERE code LIKE 'ROOT_RLS_%') <> 1
       OR (SELECT count(*) FROM portfolio.syllabus_template WHERE code LIKE 'ROOT_RLS_%') <> 1
       OR (SELECT count(*) FROM portfolio.shared_syllabus_core WHERE code LIKE 'ROOT_RLS_%') <> 1
       OR (SELECT count(*) FROM integration.source_system WHERE code LIKE 'root-rls-%') <> 1
       OR (SELECT count(*) FROM measurement.calculation_policy WHERE code LIKE 'ROOT_RLS_%') <> 1
       OR (SELECT count(*) FROM measurement.indirect_instrument WHERE code LIKE 'ROOT_RLS_%') <> 1 THEN
        RAISE EXCEPTION 'ORG_UNIT scope leaked an org-owned root across Faculty boundaries.';
    END IF;

    BEGIN
        INSERT INTO academic.org_unit (
            id, parent_id, code, name, unit_type, effective_from, status,
            created_at, created_by, updated_at, updated_by, row_version)
        VALUES (
            '82000000-0000-7000-8000-000000000009',
            '82000000-0000-7000-8000-000000000001',
            'ROOT_RLS_DENIED_ORG',
            'Denied Org Unit',
            'DEPARTMENT',
            DATE '2026-01-01',
            'ACTIVE',
            CURRENT_TIMESTAMP,
            '82000000-0000-7000-8000-000000000101',
            CURRENT_TIMESTAMP,
            '82000000-0000-7000-8000-000000000101',
            1);
        RAISE EXCEPTION 'Application role unexpectedly inserted OrgUnit.';
    EXCEPTION WHEN insufficient_privilege THEN NULL;
    END;
END;
$test$;

INSERT INTO academic.program (
    id, code, name, degree_level, education_mode, owner_org_unit_id, status,
    created_at, created_by, updated_at, updated_by, row_version)
VALUES (
    '82000000-0000-7000-8000-000000000203',
    'ROOT_RLS_PROGRAM_NEW',
    'Root RLS Program new',
    'BACHELOR',
    'FULL_TIME',
    '82000000-0000-7000-8000-000000000002',
    'DRAFT',
    CURRENT_TIMESTAMP,
    '82000000-0000-7000-8000-000000000101',
    CURRENT_TIMESTAMP,
    '82000000-0000-7000-8000-000000000101',
    1);

DO $test$
DECLARE
    affected_rows integer;
BEGIN
    UPDATE academic.program
    SET name = 'Root RLS Program new updated'
    WHERE id = '82000000-0000-7000-8000-000000000203';
    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows <> 1
       OR NOT EXISTS (
            SELECT 1
            FROM academic.program
            WHERE id = '82000000-0000-7000-8000-000000000203'
              AND name = 'Root RLS Program new updated') THEN
        RAISE EXCEPTION 'Allowed DRAFT Program UPDATE did not affect exactly one row.';
    END IF;
END;
$test$;

DO $test$
DECLARE
    affected_rows integer;
BEGIN
    BEGIN
        UPDATE academic.program
        SET status = 'ACTIVE'
        WHERE id = '82000000-0000-7000-8000-000000000203';
        RAISE EXCEPTION 'Direct Program activation unexpectedly succeeded.';
    EXCEPTION WHEN insufficient_privilege THEN NULL;
    END;

    UPDATE academic.program
    SET name = 'Active Program update denied'
    WHERE id = '82000000-0000-7000-8000-000000000201';
    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    IF affected_rows <> 0 THEN
        RAISE EXCEPTION 'ACTIVE Program was directly updated.';
    END IF;

    DELETE FROM academic.program
    WHERE id = '82000000-0000-7000-8000-000000000201';
    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    IF affected_rows <> 0 THEN
        RAISE EXCEPTION 'ACTIVE Program was hard-deleted.';
    END IF;

    BEGIN
        INSERT INTO academic.institution_template (
            id, code, name, owner_org_unit_id, created_at)
        VALUES (
            '82000000-0000-7000-8000-000000000409',
            'ROOT_RLS_CROSS_ORG_DENIED',
            'Cross-org denied',
            '82000000-0000-7000-8000-000000000003',
            CURRENT_TIMESTAMP);
        RAISE EXCEPTION 'Cross-org INSERT unexpectedly succeeded.';
    EXCEPTION WHEN insufficient_privilege THEN NULL;
    END;

    BEGIN
        UPDATE portfolio.shared_syllabus_core
        SET course_id = '82000000-0000-7000-8000-000000000302'
        WHERE id = '82000000-0000-7000-8000-000000000421';
        RAISE EXCEPTION 'Direct SharedSyllabusCore scope re-anchor unexpectedly succeeded.';
    EXCEPTION WHEN insufficient_privilege THEN NULL;
    END;
END;
$test$;

DO $test$
DECLARE
    affected_rows integer;
BEGIN
    DELETE FROM academic.program
    WHERE id = '82000000-0000-7000-8000-000000000203';
    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows <> 1 THEN
        RAISE EXCEPTION 'Allowed DRAFT Program DELETE did not affect exactly one row.';
    END IF;
END;
$test$;

SELECT pg_catalog.set_config('app.principal_id', '82000000-0000-7000-8000-000000000102', true);

DO $test$
BEGIN
    IF (SELECT count(*) FROM academic.program WHERE code LIKE 'ROOT_RLS_%') <> 1
       OR NOT EXISTS (
            SELECT 1
            FROM academic.program
            WHERE id = '82000000-0000-7000-8000-000000000201') THEN
        RAISE EXCEPTION 'PROGRAM scope must see exactly its anchored Program.';
    END IF;

    IF (SELECT count(*) FROM academic.org_unit WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM academic.institution_template WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM portfolio.syllabus_template WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM portfolio.shared_syllabus_core WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM integration.source_system WHERE code LIKE 'root-rls-%') <> 0
       OR (SELECT count(*) FROM measurement.calculation_policy WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM measurement.indirect_instrument WHERE code LIKE 'ROOT_RLS_%') <> 0 THEN
        RAISE EXCEPTION 'PROGRAM scope leaked into an unrelated resource permission.';
    END IF;
END;
$test$;

SELECT pg_catalog.set_config('app.principal_id', '82000000-0000-7000-8000-000000000103', true);

DO $test$
BEGIN
    IF (SELECT count(*) FROM portfolio.shared_syllabus_core WHERE code LIKE 'ROOT_RLS_%') <> 1
       OR NOT EXISTS (
            SELECT 1
            FROM portfolio.shared_syllabus_core
            WHERE course_id = '82000000-0000-7000-8000-000000000301') THEN
        RAISE EXCEPTION 'COURSE scope must see exactly its anchored SharedSyllabusCore.';
    END IF;

    IF (SELECT count(*) FROM academic.org_unit WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM academic.institution_template WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM academic.program WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM portfolio.syllabus_template WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM integration.source_system WHERE code LIKE 'root-rls-%') <> 0
       OR (SELECT count(*) FROM measurement.calculation_policy WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM measurement.indirect_instrument WHERE code LIKE 'ROOT_RLS_%') <> 0 THEN
        RAISE EXCEPTION 'COURSE scope leaked into Program permission.';
    END IF;
END;
$test$;

SELECT pg_catalog.set_config('app.principal_id', '82000000-0000-7000-8000-000000000104', true);

DO $test$
BEGIN
    IF (SELECT count(*) FROM academic.org_unit WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM academic.institution_template WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM academic.program WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM portfolio.syllabus_template WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM portfolio.shared_syllabus_core WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM integration.source_system WHERE code LIKE 'root-rls-%') <> 0
       OR (SELECT count(*) FROM measurement.calculation_policy WHERE code LIKE 'ROOT_RLS_%') <> 0
       OR (SELECT count(*) FROM measurement.indirect_instrument WHERE code LIKE 'ROOT_RLS_%') <> 0 THEN
        RAISE EXCEPTION 'Principal without an assignment unexpectedly received access.';
    END IF;
END;
$test$;

SET LOCAL row_security = off;

DO $test$
BEGIN
    BEGIN
        PERFORM 1 FROM academic.org_unit LIMIT 1;
        RAISE EXCEPTION 'Application role bypassed org-unit RLS with row_security=off.';
    EXCEPTION WHEN insufficient_privilege THEN NULL;
    END;
END;
$test$;

SET LOCAL row_security = on;
RESET ROLE;
ROLLBACK;

BEGIN;
SET LOCAL ROLE outcomehub_app;

DO $test$
BEGIN
    IF iam.current_context_uuid('app.principal_id') IS NOT NULL
       OR iam.current_context_uuid('app.request_id') IS NOT NULL THEN
        RAISE EXCEPTION 'Org-owned-root RLS context leaked into the next transaction.';
    END IF;
END;
$test$;

RESET ROLE;
ROLLBACK;

SELECT json_build_object(
    'status', 'passed',
    'resources', 8,
    'program_scope', true,
    'org_descendants', true,
    'draft_only_program_writes', true,
    'fixtures_rolled_back', true);
