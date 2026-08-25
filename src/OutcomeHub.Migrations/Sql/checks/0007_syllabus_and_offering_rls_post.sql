SELECT
    (
        SELECT count(*)
        FROM pg_catalog.pg_class AS relation
        INNER JOIN pg_catalog.pg_namespace AS namespace
            ON namespace.oid = relation.relnamespace
        WHERE (namespace.nspname, relation.relname) IN (
            ('portfolio', 'syllabus'),
            ('portfolio', 'syllabus_version'),
            ('academic', 'course_offering'),
            ('measurement', 'enrollment'))
          AND relation.relrowsecurity
          AND relation.relforcerowsecurity
    ) = 4
    AND (
        SELECT count(*)
        FROM pg_catalog.pg_policies
        WHERE (schemaname, tablename) IN (
            ('portfolio', 'syllabus'),
            ('portfolio', 'syllabus_version'),
            ('academic', 'course_offering'),
            ('measurement', 'enrollment'))
    ) >= 11
    AND (
        SELECT count(*)
        FROM iam.permission
        WHERE id BETWEEN
            '10000000-0000-7000-8000-000000000035'
            AND '10000000-0000-7000-8000-00000000003f'
    ) = 11
    AND pg_catalog.has_table_privilege('outcomehub_app', 'portfolio.syllabus', 'SELECT, INSERT, UPDATE, DELETE')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'academic.course_offering', 'SELECT, INSERT, UPDATE')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'measurement.enrollment', 'SELECT');
