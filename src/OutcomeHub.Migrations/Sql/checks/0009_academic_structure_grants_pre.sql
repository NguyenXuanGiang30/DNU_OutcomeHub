SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name = '0008_result_and_student_self_rls'
          AND status = 'APPLIED'
    ) = 1
    AND pg_catalog.to_regclass('academic.program_version') IS NOT NULL
    AND pg_catalog.to_regclass('academic.program_plo') IS NOT NULL
    AND pg_catalog.to_regclass('academic.program_pi') IS NOT NULL;
