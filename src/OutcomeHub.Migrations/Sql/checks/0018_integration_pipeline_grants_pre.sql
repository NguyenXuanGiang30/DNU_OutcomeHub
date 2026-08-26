SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name = '0017_exam_blueprint_and_portfolio_grants'
          AND status = 'APPLIED'
    ) = 1
    AND pg_catalog.to_regclass('integration.ingestion_batch') IS NOT NULL
    AND pg_catalog.to_regclass('integration.webhook_subscription') IS NOT NULL
    AND pg_catalog.to_regclass('integration.webhook_delivery') IS NOT NULL;
