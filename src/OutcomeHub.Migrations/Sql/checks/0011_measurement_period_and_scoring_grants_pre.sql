SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name = '0010_people_and_offerings_grants'
          AND status = 'APPLIED'
    ) = 1
    AND pg_catalog.to_regclass('measurement.measurement_period') IS NOT NULL
    AND pg_catalog.to_regclass('measurement.score_record') IS NOT NULL;
