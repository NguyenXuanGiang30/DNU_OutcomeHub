SELECT
    pg_catalog.to_regclass('public."__EFMigrationsHistory"') IS NULL
    AND NOT EXISTS (
        SELECT 1
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
          AND relation.relkind IN ('r', 'p')
          AND NOT (
              namespace.nspname = 'ops'
              AND relation.relname = 'schema_migration'));
