SELECT
    EXISTS (
        SELECT 1
        FROM iam.permission
        WHERE id = '10000000-0000-7000-8000-000000000034'::uuid
          AND resource_type = 'measurement.score_record'
          AND action = 'READ'
          AND field_scope = '*')
    AND EXISTS (
        SELECT 1
        FROM pg_catalog.pg_class AS relation
        INNER JOIN pg_catalog.pg_namespace AS namespace
            ON namespace.oid = relation.relnamespace
        WHERE namespace.nspname = 'measurement'
          AND relation.relname = 'score_record'
          AND relation.relrowsecurity
          AND relation.relforcerowsecurity)
    AND (
        SELECT count(*)
        FROM pg_catalog.pg_policies AS policy
        WHERE policy.schemaname = 'measurement'
          AND policy.tablename = 'score_record'
          AND policy.policyname = 'score_record_select_policy'
          AND policy.permissive = 'PERMISSIVE'
          AND policy.roles = ARRAY['outcomehub_app']::name[]
          AND policy.cmd = 'SELECT'
          AND policy.qual IS NOT NULL
          AND policy.with_check IS NULL) = 1
    AND (
        SELECT count(*)
        FROM pg_catalog.pg_policies AS policy
        WHERE policy.schemaname = 'measurement'
          AND policy.tablename = 'score_record') = 1
    AND pg_catalog.has_table_privilege(
        'outcomehub_app',
        'measurement.score_record',
        'SELECT')
    AND NOT pg_catalog.has_table_privilege(
        'outcomehub_app',
        'measurement.score_record',
        'INSERT')
    AND NOT pg_catalog.has_table_privilege(
        'outcomehub_app',
        'measurement.score_record',
        'UPDATE')
    AND NOT pg_catalog.has_table_privilege(
        'outcomehub_app',
        'measurement.score_record',
        'DELETE')
    AND NOT pg_catalog.has_table_privilege(
        'outcomehub_app',
        'measurement.score_record',
        'TRUNCATE')
    AND NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_class AS relation
        CROSS JOIN LATERAL pg_catalog.aclexplode(
            COALESCE(
                relation.relacl,
                pg_catalog.acldefault('r'::"char", relation.relowner)))
            AS table_acl
        WHERE relation.oid = 'measurement.score_record'::regclass
          AND table_acl.grantee = 0);
