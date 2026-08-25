SELECT
    pg_catalog.has_table_privilege('outcomehub_app', 'academic.student', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'academic.staff', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'academic.course_offering', 'SELECT');
