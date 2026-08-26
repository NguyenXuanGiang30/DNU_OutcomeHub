SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name = '0015_dashboard_analytics_grants'
          AND status = 'APPLIED'
    ) = 1
    AND pg_catalog.to_regclass('academic.direct_measurement_plan') IS NOT NULL
    AND pg_catalog.to_regclass('academic.direct_measurement_source') IS NOT NULL;
