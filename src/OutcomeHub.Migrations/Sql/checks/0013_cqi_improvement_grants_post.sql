SELECT
    pg_catalog.has_table_privilege('outcomehub_app', 'quality.improvement_plan', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'quality.improvement_action', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'quality.improvement_evidence', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'quality.remeasurement_evaluation', 'SELECT');
