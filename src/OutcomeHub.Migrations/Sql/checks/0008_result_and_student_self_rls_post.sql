SELECT
    (
        SELECT count(*)
        FROM pg_catalog.pg_class AS relation
        INNER JOIN pg_catalog.pg_namespace AS namespace
            ON namespace.oid = relation.relnamespace
        WHERE (namespace.nspname, relation.relname) IN (
            ('result', 'result_batch'),
            ('result', 'student_clo_result'),
            ('result', 'student_pi_result'),
            ('result', 'student_plo_result'),
            ('result', 'cohort_outcome_result'))
          AND relation.relrowsecurity
          AND relation.relforcerowsecurity
    ) = 5
    AND (
        SELECT count(*)
        FROM pg_catalog.pg_policies
        WHERE (schemaname, tablename) IN (
            ('result', 'result_batch'),
            ('result', 'student_clo_result'),
            ('result', 'student_pi_result'),
            ('result', 'student_plo_result'),
            ('result', 'cohort_outcome_result'))
    ) >= 5
    AND (
        SELECT count(*)
        FROM iam.permission
        WHERE id BETWEEN
            '10000000-0000-7000-8000-000000000040'
            AND '10000000-0000-7000-8000-000000000044'
    ) = 5
    AND pg_catalog.has_table_privilege('outcomehub_app', 'result.result_batch', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'result.student_pi_result', 'SELECT');
