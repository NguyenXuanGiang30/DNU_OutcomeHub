SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name = '0016_curriculum_matrix_advanced_grants'
          AND status = 'APPLIED'
    ) = 1
    AND pg_catalog.to_regclass('portfolio.assessment_question') IS NOT NULL
    AND pg_catalog.to_regclass('portfolio.teaching_session') IS NOT NULL
    AND pg_catalog.to_regclass('document.document') IS NOT NULL;
