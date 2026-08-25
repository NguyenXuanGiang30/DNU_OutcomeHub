SELECT
    pg_catalog.has_table_privilege('outcomehub_app', 'measurement.measurement_period', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'measurement.score_record', 'SELECT');
