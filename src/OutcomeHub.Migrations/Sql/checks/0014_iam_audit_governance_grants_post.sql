SELECT
    pg_catalog.has_table_privilege('outcomehub_app', 'iam.principal', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'iam.role', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'iam.access_scope', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'audit.audit_event', 'SELECT')
    AND pg_catalog.has_table_privilege('outcomehub_app', 'governance.legal_hold', 'SELECT');
