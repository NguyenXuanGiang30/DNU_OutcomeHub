SELECT
    pg_catalog.has_table_privilege('outcomehub_app', 'academic.program_plo', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'academic.program_version', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'portfolio.syllabus_version', 'SELECT');
