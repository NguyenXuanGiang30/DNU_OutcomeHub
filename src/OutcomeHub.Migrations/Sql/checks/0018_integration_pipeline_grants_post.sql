SELECT
    pg_catalog.has_table_privilege('outcomehub_app', 'integration.ingestion_batch', 'INSERT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'integration.webhook_subscription', 'INSERT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'integration.webhook_delivery', 'SELECT');
