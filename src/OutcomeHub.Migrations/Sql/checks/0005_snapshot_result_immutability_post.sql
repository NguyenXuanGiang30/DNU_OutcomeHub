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
expected_functions(function_signature) AS (
    VALUES
        ('measurement.guard_input_snapshot_mutation()'),
        ('measurement.guard_snapshot_child_mutation()'),
        ('result.guard_result_batch_mutation()'),
        ('result.guard_final_detail_mutation()')
),
expected_triggers(
    table_name,
    row_trigger_name,
    truncate_trigger_name,
    function_signature) AS (
    SELECT
        'measurement.input_snapshot',
        'trg_input_snapshot_guard_mutation',
        'trg_input_snapshot_reject_truncate',
        'measurement.guard_input_snapshot_mutation()'
    UNION ALL
    SELECT
        snapshot_children.table_name,
        'trg_snapshot_child_guard_mutation',
        'trg_snapshot_child_reject_truncate',
        'measurement.guard_snapshot_child_mutation()'
    FROM snapshot_children
    UNION ALL
    SELECT
        'result.result_batch',
        'trg_result_batch_guard_mutation',
        'trg_result_batch_reject_truncate',
        'result.guard_result_batch_mutation()'
    UNION ALL
    SELECT
        final_details.table_name,
        'trg_final_detail_guard_mutation',
        'trg_final_detail_reject_truncate',
        'result.guard_final_detail_mutation()'
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
        SELECT expected_functions.function_signature
        FROM expected_functions
        WHERE pg_catalog.to_regprocedure(
                expected_functions.function_signature) IS NULL
           OR NOT EXISTS (
                SELECT 1
                FROM pg_catalog.pg_proc AS function_metadata
                INNER JOIN pg_catalog.pg_roles AS owner_role
                    ON owner_role.oid = function_metadata.proowner
                WHERE function_metadata.oid = pg_catalog.to_regprocedure(
                        expected_functions.function_signature)
                  AND function_metadata.prosecdef
                  AND function_metadata.proconfig =
                        ARRAY['search_path=pg_catalog']::text[]
                  AND owner_role.rolname <> 'outcomehub_app'
                  AND pg_catalog.has_table_privilege(
                        owner_role.rolname,
                        CASE
                            WHEN expected_functions.function_signature LIKE
                                    'measurement.%'
                                THEN 'measurement.input_snapshot'
                            ELSE 'result.result_batch'
                        END,
                        'SELECT')
                  AND pg_catalog.has_table_privilege(
                        owner_role.rolname,
                        CASE
                            WHEN expected_functions.function_signature LIKE
                                    'measurement.%'
                                THEN 'measurement.input_snapshot'
                            ELSE 'result.result_batch'
                        END,
                        'UPDATE')
                  AND NOT EXISTS (
                        SELECT 1
                        FROM pg_catalog.aclexplode(
                            COALESCE(
                                function_metadata.proacl,
                                pg_catalog.acldefault(
                                    'f'::"char",
                                    function_metadata.proowner)))
                            AS function_acl
                        WHERE function_acl.grantee = 0
                          AND function_acl.privilege_type = 'EXECUTE')))
    AND pg_catalog.pg_get_functiondef(
            'measurement.guard_snapshot_child_mutation()'::pg_catalog.regprocedure)
            LIKE '%FOR SHARE%'
    AND pg_catalog.pg_get_functiondef(
            'result.guard_result_batch_mutation()'::pg_catalog.regprocedure)
            LIKE '%FOR SHARE%'
    AND pg_catalog.pg_get_functiondef(
            'result.guard_final_detail_mutation()'::pg_catalog.regprocedure)
            LIKE '%FOR SHARE%'
    AND NOT EXISTS (
        SELECT expected_triggers.table_name,
               expected_triggers.row_trigger_name
        FROM expected_triggers
        WHERE NOT EXISTS (
            SELECT 1
            FROM pg_catalog.pg_trigger AS database_trigger
            WHERE database_trigger.tgrelid = pg_catalog.to_regclass(
                    expected_triggers.table_name)
              AND database_trigger.tgname =
                    expected_triggers.row_trigger_name
              AND database_trigger.tgfoid = pg_catalog.to_regprocedure(
                    expected_triggers.function_signature)
              AND database_trigger.tgtype::integer = 31
              AND database_trigger.tgenabled = 'O'
              AND NOT database_trigger.tgisinternal))
    AND NOT EXISTS (
        SELECT expected_triggers.table_name,
               expected_triggers.truncate_trigger_name
        FROM expected_triggers
        WHERE NOT EXISTS (
            SELECT 1
            FROM pg_catalog.pg_trigger AS database_trigger
            WHERE database_trigger.tgrelid = pg_catalog.to_regclass(
                    expected_triggers.table_name)
              AND database_trigger.tgname =
                    expected_triggers.truncate_trigger_name
              AND database_trigger.tgfoid = pg_catalog.to_regprocedure(
                    expected_triggers.function_signature)
              AND database_trigger.tgtype::integer = 34
              AND database_trigger.tgenabled = 'O'
              AND NOT database_trigger.tgisinternal))
    AND (
        SELECT pg_catalog.count(*)
        FROM pg_catalog.pg_trigger AS database_trigger
        WHERE database_trigger.tgfoid IN (
                'measurement.guard_input_snapshot_mutation()'::pg_catalog.regprocedure,
                'measurement.guard_snapshot_child_mutation()'::pg_catalog.regprocedure,
                'result.guard_result_batch_mutation()'::pg_catalog.regprocedure,
                'result.guard_final_detail_mutation()'::pg_catalog.regprocedure)
          AND NOT database_trigger.tgisinternal) = 50
    AND NOT EXISTS (
        SELECT expected_tables.table_name
        FROM expected_tables
        WHERE pg_catalog.has_table_privilege(
            'outcomehub_app',
            expected_tables.table_name,
            'TRUNCATE'))
    AND NOT EXISTS (
        SELECT 1
        FROM result.result_batch AS result_batch
        LEFT JOIN measurement.input_snapshot AS input_snapshot
            ON input_snapshot.id = result_batch.input_snapshot_id
        WHERE input_snapshot.id IS NULL
           OR input_snapshot.status <> 'SEALED')
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
