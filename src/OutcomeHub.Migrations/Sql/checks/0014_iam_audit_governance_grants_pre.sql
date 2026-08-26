SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name = '0013_cqi_improvement_grants'
          AND status = 'APPLIED'
    ) = 1
    AND pg_catalog.to_regclass('iam.principal') IS NOT NULL
    AND pg_catalog.to_regclass('iam.role') IS NOT NULL
    AND pg_catalog.to_regclass('iam.access_scope') IS NOT NULL
    AND pg_catalog.to_regclass('audit.audit_event') IS NOT NULL
    AND pg_catalog.to_regclass('governance.legal_hold') IS NOT NULL;
