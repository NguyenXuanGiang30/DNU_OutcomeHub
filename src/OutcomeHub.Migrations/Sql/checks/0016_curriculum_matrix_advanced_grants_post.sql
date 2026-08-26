SELECT
    pg_catalog.has_table_privilege('outcomehub_app', 'academic.direct_measurement_plan', 'INSERT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'academic.direct_measurement_source', 'INSERT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'academic.direct_measurement_plan', 'SELECT');
