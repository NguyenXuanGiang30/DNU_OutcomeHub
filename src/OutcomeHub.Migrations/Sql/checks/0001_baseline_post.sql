SELECT
    pg_catalog.to_regclass('public."__EFMigrationsHistory"') IS NULL
    AND (
        SELECT count(*)
        FROM pg_catalog.pg_class AS relation
        INNER JOIN pg_catalog.pg_namespace AS namespace
            ON namespace.oid = relation.relnamespace
        WHERE namespace.nspname IN (
            'academic',
            'ai',
            'audit',
            'document',
            'governance',
            'iam',
            'integration',
            'measurement',
            'ops',
            'portfolio',
            'quality',
            'reporting',
            'result',
            'workflow')
          AND relation.relkind IN ('r', 'p')) >= 250
    AND (
        SELECT count(*)
        FROM pg_catalog.pg_constraint AS constraint_row
        INNER JOIN pg_catalog.pg_namespace AS namespace
            ON namespace.oid = constraint_row.connamespace
        WHERE namespace.nspname IN (
            'academic', 'ai', 'audit', 'document', 'governance', 'iam',
            'integration', 'measurement', 'ops', 'portfolio', 'quality',
            'reporting', 'result', 'workflow')
          AND constraint_row.contype = 'f') >= 839
    AND (
        SELECT count(*)
        FROM pg_catalog.pg_constraint AS constraint_row
        INNER JOIN pg_catalog.pg_namespace AS namespace
            ON namespace.oid = constraint_row.connamespace
        WHERE namespace.nspname IN (
            'academic', 'ai', 'audit', 'document', 'governance', 'iam',
            'integration', 'measurement', 'ops', 'portfolio', 'quality',
            'reporting', 'result', 'workflow')
          AND constraint_row.contype = 'c') >= 690
    AND (
        SELECT count(*)
        FROM pg_catalog.pg_constraint AS constraint_row
        INNER JOIN pg_catalog.pg_namespace AS namespace
            ON namespace.oid = constraint_row.connamespace
        WHERE namespace.nspname IN (
            'academic', 'ai', 'audit', 'document', 'governance', 'iam',
            'integration', 'measurement', 'ops', 'portfolio', 'quality',
            'reporting', 'result', 'workflow')
          AND constraint_row.contype = 'x') >= 9
    AND EXISTS (
        SELECT 1
        FROM pg_catalog.pg_extension
        WHERE extname = 'btree_gist')
    AND EXISTS (
        SELECT 1
        FROM pg_catalog.pg_extension
        WHERE extname = 'citext')
    AND EXISTS (
        SELECT 1
        FROM pg_catalog.pg_class AS relation
        INNER JOIN pg_catalog.pg_namespace AS namespace
            ON namespace.oid = relation.relnamespace
        WHERE namespace.nspname = 'academic'
          AND relation.relname = 'course'
          AND relation.relrowsecurity
          AND relation.relforcerowsecurity)
    AND NOT EXISTS (
        SELECT required.policy_name
        FROM (VALUES
            ('course_select_policy'),
            ('course_insert_policy'),
            ('course_update_policy'),
            ('course_delete_policy')) AS required(policy_name)
        WHERE NOT EXISTS (
            SELECT 1
            FROM pg_catalog.pg_policies AS policy
            WHERE policy.schemaname = 'academic'
              AND policy.tablename = 'course'
              AND policy.policyname = required.policy_name))
    AND NOT EXISTS (
        SELECT required.id
        FROM (VALUES
            ('10000000-0000-7000-8000-000000000001'::uuid, 'READ'),
            ('10000000-0000-7000-8000-000000000002'::uuid, 'CREATE'),
            ('10000000-0000-7000-8000-000000000003'::uuid, 'UPDATE'),
            ('10000000-0000-7000-8000-000000000004'::uuid, 'DELETE'))
            AS required(id, action)
        WHERE NOT EXISTS (
            SELECT 1
            FROM iam.permission AS permission
            WHERE permission.id = required.id
              AND permission.resource_type = 'academic.course'
              AND permission.action = required.action
              AND permission.field_scope = '*'))
    AND (
        SELECT pg_catalog.pg_get_userbyid(routine.proowner)
        FROM pg_catalog.pg_proc AS routine
        INNER JOIN pg_catalog.pg_namespace AS namespace
            ON namespace.oid = routine.pronamespace
        WHERE namespace.nspname = 'iam'
          AND routine.proname = 'has_permission') = 'outcomehub_authorizer';
