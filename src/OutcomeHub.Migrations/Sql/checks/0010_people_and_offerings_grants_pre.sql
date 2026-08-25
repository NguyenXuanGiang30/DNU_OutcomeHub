SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name = '0009_academic_structure_grants'
          AND status = 'APPLIED'
    ) = 1
    AND pg_catalog.to_regclass('academic.student') IS NOT NULL
    AND pg_catalog.to_regclass('academic.staff') IS NOT NULL
    AND pg_catalog.to_regclass('academic.course_offering') IS NOT NULL;
