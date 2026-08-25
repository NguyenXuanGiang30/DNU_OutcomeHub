\set ON_ERROR_STOP on

BEGIN;

INSERT INTO integration.outbox_message (
    id,
    aggregate_type,
    aggregate_id,
    aggregate_version,
    event_type,
    event_schema_version,
    payload,
    headers,
    classification,
    correlation_id,
    causation_id,
    trace_id,
    occurred_at,
    available_at,
    published_at,
    attempt_count,
    locked_by,
    locked_until,
    status,
    last_error_code)
VALUES (
    '80000000-0000-7000-8000-000000000001',
    'RLS_SMOKE_AGGREGATE',
    '80000000-0000-7000-8000-000000000002',
    1,
    'RLS_SMOKE_EVENT',
    1,
    '{"value":"original"}'::jsonb,
    NULL,
    'INTERNAL',
    '80000000-0000-7000-8000-000000000003',
    NULL,
    'rls-smoke-trace',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    NULL,
    0,
    NULL,
    NULL,
    'PENDING',
    NULL);

UPDATE integration.outbox_message
SET status = 'PROCESSING',
    attempt_count = 1,
    locked_by = '80000000-0000-7000-8000-000000000004',
    locked_until = CURRENT_TIMESTAMP + INTERVAL '1 minute'
WHERE id = '80000000-0000-7000-8000-000000000001';

DO $test$
BEGIN
    BEGIN
        UPDATE integration.outbox_message
        SET payload = '{"value":"tampered"}'::jsonb
        WHERE id = '80000000-0000-7000-8000-000000000001';

        RAISE EXCEPTION 'Outbox envelope mutation unexpectedly succeeded.';
    EXCEPTION
        WHEN object_not_in_prerequisite_state THEN
            NULL;
    END;

    BEGIN
        UPDATE iam.permission
        SET description = 'Tampered permission description'
        WHERE id = '10000000-0000-7000-8000-000000000001';

        RAISE EXCEPTION 'Append-only permission mutation unexpectedly succeeded.';
    EXCEPTION
        WHEN object_not_in_prerequisite_state THEN
            NULL;
    END;

    BEGIN
        DELETE FROM iam.permission
        WHERE id = '10000000-0000-7000-8000-000000000001';

        RAISE EXCEPTION 'Append-only permission deletion unexpectedly succeeded.';
    EXCEPTION
        WHEN object_not_in_prerequisite_state THEN
            NULL;
    END;

    BEGIN
        TRUNCATE TABLE audit.audit_event;

        RAISE EXCEPTION 'Audit-event truncation unexpectedly succeeded.';
    EXCEPTION
        WHEN object_not_in_prerequisite_state THEN
            NULL;
    END;

    BEGIN
        TRUNCATE TABLE ai.evaluation_result;

        RAISE EXCEPTION 'Append-only evaluation-result truncation unexpectedly succeeded.';
    EXCEPTION
        WHEN object_not_in_prerequisite_state THEN
            NULL;
    END;

    BEGIN
        UPDATE ops.schema_migration
        SET runner_version = 'tampered'
        WHERE status = 'APPLIED';

        RAISE EXCEPTION 'Applied migration-ledger mutation unexpectedly succeeded.';
    EXCEPTION
        WHEN object_not_in_prerequisite_state THEN
            NULL;
    END;

    BEGIN
        DELETE FROM ops.schema_migration
        WHERE status = 'APPLIED';

        RAISE EXCEPTION 'Applied migration-ledger deletion unexpectedly succeeded.';
    EXCEPTION
        WHEN object_not_in_prerequisite_state THEN
            NULL;
    END;

    BEGIN
        TRUNCATE TABLE ops.schema_migration;

        RAISE EXCEPTION 'Migration-ledger truncation unexpectedly succeeded.';
    EXCEPTION
        WHEN object_not_in_prerequisite_state THEN
            NULL;
    END;
END;
$test$;

ROLLBACK;

SELECT json_build_object(
    'status', 'passed',
    'outbox_envelope_immutable', true,
    'audit_truncate_rejected', true,
    'append_only_guards', true,
    'migration_ledger_immutable', true,
    'fixtures_rolled_back', true);
