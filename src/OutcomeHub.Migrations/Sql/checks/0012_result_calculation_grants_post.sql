SELECT
    pg_catalog.has_table_privilege('outcomehub_app', 'result.result_batch', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'result.student_clo_result', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'measurement.input_snapshot', 'SELECT');
