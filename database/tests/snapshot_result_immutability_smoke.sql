\set ON_ERROR_STOP on

BEGIN;

SET LOCAL session_replication_role = replica;

INSERT INTO measurement.input_snapshot (
    id,
    governed_resource_id,
    measurement_period_id,
    org_unit_id,
    snapshot_no,
    policy_version_id,
    program_policy_binding_id,
    institution_template_version_id,
    program_version_id,
    academic_year_start,
    status,
    schema_version,
    hash_algorithm,
    manifest_checksum,
    population_count,
    score_count,
    parent_snapshot_id,
    created_by,
    created_at,
    sealed_by,
    sealed_at)
VALUES (
    '83000000-0000-7000-8000-000000000001',
    '83000000-0000-7000-8000-000000000002',
    '83000000-0000-7000-8000-000000000003',
    '83000000-0000-7000-8000-000000000004',
    1,
    '83000000-0000-7000-8000-000000000005',
    '83000000-0000-7000-8000-000000000006',
    '83000000-0000-7000-8000-000000000007',
    '83000000-0000-7000-8000-000000000008',
    2026,
    'BUILDING',
    'snapshot-v1',
    'SHA-256',
    NULL,
    1,
    0,
    NULL,
    '83000000-0000-7000-8000-000000000009',
    CURRENT_TIMESTAMP,
    NULL,
    NULL);

INSERT INTO measurement.snapshot_resource (
    input_snapshot_id,
    resource_type,
    resource_id,
    version_id,
    checksum,
    canonical_payload,
    created_at)
VALUES (
    '83000000-0000-7000-8000-000000000001',
    'PROGRAM_VERSION',
    '83000000-0000-7000-8000-000000000010',
    '83000000-0000-7000-8000-000000000011',
    repeat('a', 64),
    '{"state":"initial"}'::jsonb,
    CURRENT_TIMESTAMP);

INSERT INTO result.result_batch (
    id,
    governed_resource_id,
    measurement_period_id,
    input_snapshot_id,
    policy_version_id,
    program_policy_binding_id,
    org_unit_id,
    program_version_id,
    academic_year_start,
    batch_no,
    engine_version,
    source_commit,
    container_digest,
    status,
    idempotency_key,
    request_checksum,
    recalculates_batch_id,
    recalculation_reason,
    workflow_instance_id,
    sod_policy_version_id,
    result_checksum,
    started_at,
    completed_at,
    published_at)
VALUES (
    '83000000-0000-7000-8000-000000000101',
    '83000000-0000-7000-8000-000000000102',
    '83000000-0000-7000-8000-000000000003',
    '83000000-0000-7000-8000-000000000001',
    '83000000-0000-7000-8000-000000000005',
    '83000000-0000-7000-8000-000000000006',
    '83000000-0000-7000-8000-000000000004',
    '83000000-0000-7000-8000-000000000008',
    2026,
    1,
    'immutability-smoke-engine-v1',
    'immutability-smoke-source-v1',
    NULL,
    'RUNNING',
    'immutability-smoke-request-1',
    repeat('b', 64),
    NULL,
    NULL,
    '83000000-0000-7000-8000-000000000103',
    '83000000-0000-7000-8000-000000000104',
    NULL,
    NULL,
    NULL,
    NULL);

INSERT INTO result.cohort_population_decision (
    academic_year_start,
    id,
    batch_id,
    org_unit_id,
    program_id,
    program_version_id,
    measurement_period_id,
    cohort_id,
    curriculum_path_id,
    outcome_level,
    clo_id,
    program_pi_id,
    program_plo_id,
    method,
    student_id,
    decision_bucket,
    reason_code)
VALUES (
    2026,
    '83000000-0000-7000-8000-000000000201',
    '83000000-0000-7000-8000-000000000101',
    '83000000-0000-7000-8000-000000000004',
    '83000000-0000-7000-8000-000000000202',
    '83000000-0000-7000-8000-000000000008',
    '83000000-0000-7000-8000-000000000003',
    '83000000-0000-7000-8000-000000000203',
    '83000000-0000-7000-8000-000000000204',
    'PI',
    NULL,
    '83000000-0000-7000-8000-000000000205',
    NULL,
    'DIRECT',
    '83000000-0000-7000-8000-000000000206',
    'ATTAINED',
    NULL);

SET LOCAL session_replication_role = origin;

-- The fixtures deliberately use synthetic foreign keys. Keep only the guards
-- under test enabled; every ALTER is transactional and is rolled back below.
ALTER TABLE measurement.input_snapshot DISABLE TRIGGER ALL;
ALTER TABLE measurement.input_snapshot
    ENABLE TRIGGER trg_input_snapshot_guard_mutation;
ALTER TABLE measurement.input_snapshot
    ENABLE TRIGGER trg_input_snapshot_reject_truncate;

ALTER TABLE measurement.snapshot_resource DISABLE TRIGGER ALL;
ALTER TABLE measurement.snapshot_resource
    ENABLE TRIGGER trg_snapshot_child_guard_mutation;
ALTER TABLE measurement.snapshot_resource
    ENABLE TRIGGER trg_snapshot_child_reject_truncate;

ALTER TABLE result.result_batch DISABLE TRIGGER ALL;
ALTER TABLE result.result_batch
    ENABLE TRIGGER trg_result_batch_guard_mutation;
ALTER TABLE result.result_batch
    ENABLE TRIGGER trg_result_batch_reject_truncate;

ALTER TABLE result.cohort_population_decision DISABLE TRIGGER ALL;
ALTER TABLE result.cohort_population_decision
    ENABLE TRIGGER trg_final_detail_guard_mutation;
ALTER TABLE result.cohort_population_decision
    ENABLE TRIGGER trg_final_detail_reject_truncate;

DO $test$
DECLARE
    affected_rows integer;
BEGIN
    UPDATE measurement.snapshot_resource
    SET canonical_payload = '{"state":"updated-before-seal"}'::jsonb
    WHERE input_snapshot_id = '83000000-0000-7000-8000-000000000001'
      AND resource_id = '83000000-0000-7000-8000-000000000010';
    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows <> 1 THEN
        RAISE EXCEPTION 'Snapshot child must be mutable while its parent is BUILDING.';
    END IF;
END;
$test$;

UPDATE measurement.input_snapshot
SET status = 'SEALED',
    manifest_checksum = repeat('c', 64),
    sealed_by = '83000000-0000-7000-8000-000000000009',
    sealed_at = CURRENT_TIMESTAMP
WHERE id = '83000000-0000-7000-8000-000000000001';

DO $test$
BEGIN
    BEGIN
        INSERT INTO measurement.snapshot_resource (
            input_snapshot_id,
            resource_type,
            resource_id,
            version_id,
            checksum,
            canonical_payload,
            created_at)
        VALUES (
            '83000000-0000-7000-8000-000000000001',
            'PROGRAM_VERSION',
            '83000000-0000-7000-8000-000000000012',
            '83000000-0000-7000-8000-000000000013',
            repeat('d', 64),
            '{}'::jsonb,
            CURRENT_TIMESTAMP);
        RAISE EXCEPTION 'SEALED snapshot unexpectedly accepted a child INSERT.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;

    BEGIN
        UPDATE measurement.snapshot_resource
        SET canonical_payload = '{"state":"tampered"}'::jsonb
        WHERE input_snapshot_id = '83000000-0000-7000-8000-000000000001';
        RAISE EXCEPTION 'SEALED snapshot unexpectedly accepted a child UPDATE.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;

    BEGIN
        DELETE FROM measurement.snapshot_resource
        WHERE input_snapshot_id = '83000000-0000-7000-8000-000000000001';
        RAISE EXCEPTION 'SEALED snapshot unexpectedly accepted a child DELETE.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;

    BEGIN
        UPDATE measurement.input_snapshot
        SET population_count = population_count + 1
        WHERE id = '83000000-0000-7000-8000-000000000001';
        RAISE EXCEPTION 'SEALED InputSnapshot unexpectedly accepted a mutation.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;

    BEGIN
        UPDATE measurement.input_snapshot
        SET status = 'BUILDING'
        WHERE id = '83000000-0000-7000-8000-000000000001';
        RAISE EXCEPTION 'SEALED InputSnapshot unexpectedly returned to BUILDING.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;

    BEGIN
        TRUNCATE TABLE measurement.snapshot_resource;
        RAISE EXCEPTION 'Snapshot child table unexpectedly allowed TRUNCATE.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;
END;
$test$;

DO $test$
DECLARE
    affected_rows integer;
BEGIN
    UPDATE result.cohort_population_decision
    SET reason_code = 'UPDATED_BEFORE_FINALIZE'
    WHERE academic_year_start = 2026
      AND id = '83000000-0000-7000-8000-000000000201';
    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows <> 1 THEN
        RAISE EXCEPTION 'Final detail must be mutable while ResultBatch is RUNNING.';
    END IF;
END;
$test$;

UPDATE result.result_batch
SET status = 'CALCULATED',
    container_digest = 'sha256:immutability-smoke',
    result_checksum = repeat('e', 64),
    started_at = CURRENT_TIMESTAMP - INTERVAL '1 minute',
    completed_at = CURRENT_TIMESTAMP
WHERE id = '83000000-0000-7000-8000-000000000101';

DO $test$
BEGIN
    BEGIN
        INSERT INTO result.cohort_population_decision (
            academic_year_start,
            id,
            batch_id,
            org_unit_id,
            program_id,
            program_version_id,
            measurement_period_id,
            cohort_id,
            curriculum_path_id,
            outcome_level,
            program_pi_id,
            method,
            student_id,
            decision_bucket)
        VALUES (
            2026,
            '83000000-0000-7000-8000-000000000207',
            '83000000-0000-7000-8000-000000000101',
            '83000000-0000-7000-8000-000000000004',
            '83000000-0000-7000-8000-000000000202',
            '83000000-0000-7000-8000-000000000008',
            '83000000-0000-7000-8000-000000000003',
            '83000000-0000-7000-8000-000000000203',
            '83000000-0000-7000-8000-000000000204',
            'PI',
            '83000000-0000-7000-8000-000000000205',
            'DIRECT',
            '83000000-0000-7000-8000-000000000208',
            'ATTAINED');
        RAISE EXCEPTION 'CALCULATED batch unexpectedly accepted a detail INSERT.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;

    BEGIN
        UPDATE result.cohort_population_decision
        SET reason_code = 'TAMPERED_AFTER_FINALIZE'
        WHERE academic_year_start = 2026
          AND id = '83000000-0000-7000-8000-000000000201';
        RAISE EXCEPTION 'CALCULATED batch unexpectedly accepted a detail UPDATE.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;

    BEGIN
        DELETE FROM result.cohort_population_decision
        WHERE academic_year_start = 2026
          AND id = '83000000-0000-7000-8000-000000000201';
        RAISE EXCEPTION 'CALCULATED batch unexpectedly accepted a detail DELETE.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;

    BEGIN
        UPDATE result.result_batch
        SET status = 'RUNNING'
        WHERE id = '83000000-0000-7000-8000-000000000101';
        RAISE EXCEPTION 'CALCULATED ResultBatch unexpectedly returned to RUNNING.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;

    BEGIN
        UPDATE result.result_batch
        SET source_commit = 'tampered-source'
        WHERE id = '83000000-0000-7000-8000-000000000101';
        RAISE EXCEPTION 'ResultBatch immutable envelope unexpectedly changed.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;

    BEGIN
        UPDATE result.result_batch
        SET result_checksum = repeat('f', 64)
        WHERE id = '83000000-0000-7000-8000-000000000101';
        RAISE EXCEPTION 'ResultBatch checksum unexpectedly changed.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;

    BEGIN
        DELETE FROM result.result_batch
        WHERE id = '83000000-0000-7000-8000-000000000101';
        RAISE EXCEPTION 'ResultBatch unexpectedly allowed DELETE.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;

    BEGIN
        TRUNCATE TABLE result.cohort_population_decision;
        RAISE EXCEPTION 'Final detail table unexpectedly allowed TRUNCATE.';
    EXCEPTION WHEN object_not_in_prerequisite_state THEN NULL;
    END;
END;
$test$;

UPDATE result.result_batch
SET status = 'VALIDATED'
WHERE id = '83000000-0000-7000-8000-000000000101';

DO $test$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM result.result_batch
        WHERE id = '83000000-0000-7000-8000-000000000101'
          AND status = 'VALIDATED'
          AND result_checksum = repeat('e', 64)) THEN
        RAISE EXCEPTION 'Allowed frozen-state ResultBatch transition did not persist.';
    END IF;
END;
$test$;

ROLLBACK;

SELECT json_build_object(
    'status', 'passed',
    'snapshot_children_guarded', 12,
    'final_result_tables_guarded', 11,
    'fixtures_rolled_back', true);
