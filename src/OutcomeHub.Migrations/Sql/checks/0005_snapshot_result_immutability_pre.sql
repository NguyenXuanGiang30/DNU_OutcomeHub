WITH snapshot_children(table_name) AS (
    VALUES
        ('measurement.snapshot_resource'),
        ('measurement.snapshot_offering'),
        ('measurement.snapshot_population_member'),
        ('measurement.snapshot_enrollment'),
        ('measurement.snapshot_score'),
        ('measurement.snapshot_direct_pi_weight'),
        ('measurement.snapshot_question_criterion_weight'),
        ('measurement.snapshot_pi_source_weight'),
        ('measurement.snapshot_pi_plo_weight'),
        ('measurement.snapshot_threshold'),
        ('measurement.snapshot_indirect_observation'),
        ('measurement.snapshot_manifest_chunk')
),
final_details(table_name) AS (
    VALUES
        ('result.student_criterion_result'),
        ('result.student_criterion_score_lineage'),
        ('result.criterion_pi_contribution'),
        ('result.student_clo_result'),
        ('result.course_pi_result'),
        ('result.student_pi_result'),
        ('result.student_pi_source_contribution'),
        ('result.student_plo_result'),
        ('result.student_plo_pi_contribution'),
        ('result.cohort_outcome_result'),
        ('result.cohort_population_decision')
),
expected_tables(table_name) AS (
    SELECT 'measurement.input_snapshot'
    UNION ALL
    SELECT snapshot_children.table_name
    FROM snapshot_children
    UNION ALL
    SELECT 'result.result_batch'
    UNION ALL
    SELECT final_details.table_name
    FROM final_details
),
expected_triggers(table_name, row_trigger_name, truncate_trigger_name) AS (
    SELECT
        'measurement.input_snapshot',
        'trg_input_snapshot_guard_mutation',
        'trg_input_snapshot_reject_truncate'
    UNION ALL
    SELECT
        snapshot_children.table_name,
        'trg_snapshot_child_guard_mutation',
        'trg_snapshot_child_reject_truncate'
    FROM snapshot_children
    UNION ALL
    SELECT
        'result.result_batch',
        'trg_result_batch_guard_mutation',
        'trg_result_batch_reject_truncate'
    UNION ALL
    SELECT
        final_details.table_name,
        'trg_final_detail_guard_mutation',
        'trg_final_detail_reject_truncate'
    FROM final_details
)
SELECT
    EXISTS (
        SELECT 1
        FROM pg_catalog.pg_roles AS application_role
        WHERE application_role.rolname = 'outcomehub_app'
          AND NOT application_role.rolsuper
          AND NOT application_role.rolreplication)
    AND NOT EXISTS (
        SELECT expected_tables.table_name
        FROM expected_tables
        WHERE pg_catalog.to_regclass(expected_tables.table_name) IS NULL)
    AND NOT EXISTS (
        SELECT snapshot_children.table_name
        FROM snapshot_children
        WHERE NOT EXISTS (
            SELECT 1
            FROM pg_catalog.pg_attribute AS parent_column
            WHERE parent_column.attrelid =
                    pg_catalog.to_regclass(snapshot_children.table_name)
              AND parent_column.attname = 'input_snapshot_id'
              AND parent_column.atttypid = 'uuid'::pg_catalog.regtype
              AND parent_column.attnotnull
              AND parent_column.attnum > 0
              AND NOT parent_column.attisdropped)
           OR NOT EXISTS (
            SELECT 1
            FROM pg_catalog.pg_constraint AS foreign_key
            INNER JOIN pg_catalog.pg_attribute AS parent_column
                ON parent_column.attrelid = foreign_key.conrelid
                AND parent_column.attname = 'input_snapshot_id'
                AND parent_column.attnum = ANY(foreign_key.conkey)
            WHERE foreign_key.contype = 'f'
              AND foreign_key.conrelid =
                    pg_catalog.to_regclass(snapshot_children.table_name)
              AND foreign_key.confrelid =
                    'measurement.input_snapshot'::pg_catalog.regclass
              AND foreign_key.convalidated))
    AND NOT EXISTS (
        SELECT final_details.table_name
        FROM final_details
        WHERE NOT EXISTS (
            SELECT 1
            FROM pg_catalog.pg_attribute AS parent_column
            WHERE parent_column.attrelid =
                    pg_catalog.to_regclass(final_details.table_name)
              AND parent_column.attname = 'batch_id'
              AND parent_column.atttypid = 'uuid'::pg_catalog.regtype
              AND parent_column.attnotnull
              AND parent_column.attnum > 0
              AND NOT parent_column.attisdropped)
           OR NOT EXISTS (
            SELECT 1
            FROM pg_catalog.pg_constraint AS foreign_key
            INNER JOIN pg_catalog.pg_attribute AS parent_column
                ON parent_column.attrelid = foreign_key.conrelid
                AND parent_column.attname = 'batch_id'
                AND parent_column.attnum = ANY(foreign_key.conkey)
            WHERE foreign_key.contype = 'f'
              AND foreign_key.conrelid =
                    pg_catalog.to_regclass(final_details.table_name)
              AND foreign_key.confrelid =
                    'result.result_batch'::pg_catalog.regclass
              AND foreign_key.convalidated))
    AND EXISTS (
        SELECT 1
        FROM pg_catalog.pg_constraint AS status_constraint
        WHERE status_constraint.conrelid =
                'measurement.input_snapshot'::pg_catalog.regclass
          AND status_constraint.conname = 'ck_input_snapshot_status'
          AND status_constraint.contype = 'c'
          AND status_constraint.convalidated
          AND pg_catalog.pg_get_constraintdef(status_constraint.oid)
                LIKE '%BUILDING%'
          AND pg_catalog.pg_get_constraintdef(status_constraint.oid)
                LIKE '%SEALED%'
          AND pg_catalog.pg_get_constraintdef(status_constraint.oid)
                LIKE '%VOID%')
    AND EXISTS (
        SELECT 1
        FROM pg_catalog.pg_constraint AS status_constraint
        WHERE status_constraint.conrelid =
                'result.result_batch'::pg_catalog.regclass
          AND status_constraint.conname = 'ck_result_batch_status'
          AND status_constraint.contype = 'c'
          AND status_constraint.convalidated
          AND pg_catalog.pg_get_constraintdef(status_constraint.oid)
                LIKE '%QUEUED%'
          AND pg_catalog.pg_get_constraintdef(status_constraint.oid)
                LIKE '%RUNNING%'
          AND pg_catalog.pg_get_constraintdef(status_constraint.oid)
                LIKE '%CALCULATED%'
          AND pg_catalog.pg_get_constraintdef(status_constraint.oid)
                LIKE '%VALIDATED%'
          AND pg_catalog.pg_get_constraintdef(status_constraint.oid)
                LIKE '%IN_REVIEW%'
          AND pg_catalog.pg_get_constraintdef(status_constraint.oid)
                LIKE '%APPROVED%'
          AND pg_catalog.pg_get_constraintdef(status_constraint.oid)
                LIKE '%PUBLISHED%'
          AND pg_catalog.pg_get_constraintdef(status_constraint.oid)
                LIKE '%FAILED%'
          AND pg_catalog.pg_get_constraintdef(status_constraint.oid)
                LIKE '%CANCELLED%')
    AND pg_catalog.to_regprocedure(
        'measurement.guard_input_snapshot_mutation()') IS NULL
    AND pg_catalog.to_regprocedure(
        'measurement.guard_snapshot_child_mutation()') IS NULL
    AND pg_catalog.to_regprocedure(
        'result.guard_result_batch_mutation()') IS NULL
    AND pg_catalog.to_regprocedure(
        'result.guard_final_detail_mutation()') IS NULL
    AND NOT EXISTS (
        SELECT 1
        FROM expected_triggers
        INNER JOIN pg_catalog.pg_trigger AS database_trigger
            ON database_trigger.tgrelid =
                pg_catalog.to_regclass(expected_triggers.table_name)
            AND database_trigger.tgname IN (
                expected_triggers.row_trigger_name,
                expected_triggers.truncate_trigger_name)
        WHERE NOT database_trigger.tgisinternal)
    AND NOT EXISTS (
        SELECT 1
        FROM result.result_batch AS result_batch
        INNER JOIN measurement.input_snapshot AS input_snapshot
            ON input_snapshot.id = result_batch.input_snapshot_id
        WHERE input_snapshot.status <> 'SEALED')
    AND NOT EXISTS (
        SELECT 1
        FROM result.result_batch AS result_batch
        WHERE result_batch.status IN (
                'CALCULATED',
                'VALIDATED',
                'IN_REVIEW',
                'APPROVED',
                'PUBLISHED')
          AND (
                result_batch.result_checksum IS NULL
                OR result_batch.completed_at IS NULL));
