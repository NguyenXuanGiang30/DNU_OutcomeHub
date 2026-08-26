SELECT
    pg_catalog.has_table_privilege('outcomehub_app', 'portfolio.teaching_session', 'INSERT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'portfolio.assessment_question', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'document.document', 'SELECT');
