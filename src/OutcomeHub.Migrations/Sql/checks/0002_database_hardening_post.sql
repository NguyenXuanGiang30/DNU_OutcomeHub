SELECT
    NOT pg_catalog.has_function_privilege(
        'outcomehub_app',
        'audit.reject_mutation()',
        'EXECUTE')
    AND NOT pg_catalog.has_function_privilege(
        'outcomehub_app',
        'integration.guard_outbox_envelope()',
        'EXECUTE')
    AND NOT pg_catalog.has_function_privilege(
        'outcomehub_app',
        'measurement.validate_score_record_scope()',
        'EXECUTE')
    AND EXISTS (
        SELECT 1
        FROM pg_catalog.pg_trigger AS trigger_row
        WHERE trigger_row.tgrelid = 'audit.audit_event'::regclass
          AND trigger_row.tgname = 'trg_audit_event_reject_truncate'
          AND trigger_row.tgenabled = 'O'
          AND NOT trigger_row.tgisinternal)
    AND EXISTS (
        SELECT 1
        FROM pg_catalog.pg_trigger AS trigger_row
        WHERE trigger_row.tgrelid = 'integration.outbox_message'::regclass
          AND trigger_row.tgname = 'trg_outbox_message_envelope_immutable'
          AND trigger_row.tgenabled = 'O'
          AND NOT trigger_row.tgisinternal)
    AND EXISTS (
        SELECT 1
        FROM pg_catalog.pg_trigger AS trigger_row
        WHERE trigger_row.tgrelid = 'measurement.score_record'::regclass
          AND trigger_row.tgname = 'trg_score_record_validate_scope'
          AND trigger_row.tgenabled = 'O'
          AND NOT trigger_row.tgisinternal)
    AND (
        SELECT count(*)
        FROM pg_catalog.pg_trigger AS trigger_row
        WHERE trigger_row.tgname = 'trg_append_only_immutable'
          AND trigger_row.tgenabled = 'O'
          AND NOT trigger_row.tgisinternal) >= 10
    AND (
        SELECT count(*)
        FROM pg_catalog.pg_trigger AS trigger_row
        WHERE trigger_row.tgname = 'trg_append_only_reject_truncate'
          AND trigger_row.tgenabled = 'O'
          AND NOT trigger_row.tgisinternal) >= 10
    AND (
        SELECT pg_catalog.pg_get_expr(policy.polwithcheck, policy.polrelid)
        FROM pg_catalog.pg_policy AS policy
        WHERE policy.polrelid = 'academic.course'::regclass
          AND policy.polname = 'course_insert_policy') LIKE '%status%''DRAFT''%'
    AND (
        SELECT pg_catalog.pg_get_expr(policy.polqual, policy.polrelid)
        FROM pg_catalog.pg_policy AS policy
        WHERE policy.polrelid = 'academic.course'::regclass
          AND policy.polname = 'course_update_policy') LIKE '%status%''DRAFT''%'
    AND (
        SELECT pg_catalog.pg_get_expr(policy.polqual, policy.polrelid)
        FROM pg_catalog.pg_policy AS policy
        WHERE policy.polrelid = 'academic.course'::regclass
          AND policy.polname = 'course_delete_policy') LIKE '%status%''DRAFT''%';
