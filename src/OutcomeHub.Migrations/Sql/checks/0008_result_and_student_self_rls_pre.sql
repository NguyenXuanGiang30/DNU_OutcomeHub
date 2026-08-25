SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name = '0007_syllabus_and_offering_rls'
          AND status = 'APPLIED'
    ) = 1
    AND pg_catalog.to_regclass('result.result_batch') IS NOT NULL
    AND pg_catalog.to_regclass('result.student_clo_result') IS NOT NULL
    AND pg_catalog.to_regclass('result.student_pi_result') IS NOT NULL
    AND pg_catalog.to_regclass('result.student_plo_result') IS NOT NULL
    AND pg_catalog.to_regclass('result.cohort_outcome_result') IS NOT NULL
    AND NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_policies
        WHERE (schemaname, tablename) IN (
            ('result', 'result_batch'),
            ('result', 'student_clo_result'),
            ('result', 'student_pi_result'),
            ('result', 'student_plo_result'),
            ('result', 'cohort_outcome_result')));
