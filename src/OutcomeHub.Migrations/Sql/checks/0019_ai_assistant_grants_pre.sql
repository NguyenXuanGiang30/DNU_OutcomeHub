SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name = '0018_integration_pipeline_grants'
          AND status = 'APPLIED'
    ) = 1
    AND pg_catalog.to_regclass('academic.program_version') IS NOT NULL
    AND pg_catalog.to_regclass('academic.program_plo') IS NOT NULL;
