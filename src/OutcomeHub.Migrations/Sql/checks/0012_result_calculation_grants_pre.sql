SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name = '0011_measurement_period_and_scoring_grants'
          AND status = 'APPLIED'
    ) = 1
    AND pg_catalog.to_regclass('measurement.input_snapshot') IS NOT NULL
    AND pg_catalog.to_regclass('result.result_batch') IS NOT NULL
    AND pg_catalog.to_regclass('result.student_clo_result') IS NOT NULL;
