SELECT
    pg_catalog.to_regprocedure('audit.reject_mutation()') IS NOT NULL
    AND pg_catalog.to_regclass('integration.outbox_message') IS NOT NULL
    AND pg_catalog.to_regclass('measurement.score_record') IS NOT NULL
    AND pg_catalog.to_regprocedure('integration.guard_outbox_envelope()') IS NULL
    AND pg_catalog.to_regprocedure('measurement.validate_score_record_scope()') IS NULL
    AND NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_trigger
        WHERE tgname IN (
            'trg_audit_event_reject_truncate',
            'trg_outbox_message_envelope_immutable',
            'trg_score_record_validate_scope',
            'trg_score_record_update_immutable',
            'trg_raw_record_update_immutable',
            'trg_append_only_immutable',
            'trg_append_only_reject_truncate')
          AND NOT tgisinternal);
