SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name = '0006_score_record_read_rls'
          AND status = 'APPLIED'
    ) = 1
    AND pg_catalog.to_regclass('portfolio.syllabus') IS NOT NULL
    AND pg_catalog.to_regclass('portfolio.syllabus_version') IS NOT NULL
    AND pg_catalog.to_regclass('academic.course_offering') IS NOT NULL
    AND pg_catalog.to_regclass('measurement.enrollment') IS NOT NULL
    AND NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_policies
        WHERE (schemaname, tablename) IN (
            ('portfolio', 'syllabus'),
            ('portfolio', 'syllabus_version'),
            ('academic', 'course_offering'),
            ('measurement', 'enrollment')));
