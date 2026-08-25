SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name = '0012_result_calculation_grants'
          AND status = 'APPLIED'
    ) = 1
    AND pg_catalog.to_regclass('quality.improvement_plan') IS NOT NULL
    AND pg_catalog.to_regclass('quality.improvement_action') IS NOT NULL
    AND pg_catalog.to_regclass('quality.improvement_evidence') IS NOT NULL;
