SELECT
    pg_catalog.has_table_privilege('outcomehub_app', 'result.cohort_outcome_result', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'quality.improvement_plan', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'academic.program_plo', 'SELECT');
