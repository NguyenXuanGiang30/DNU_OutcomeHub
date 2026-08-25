SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name IN (
            '0001_baseline_20260825',
            '0002_database_hardening',
            '0003_critical_business_invariants',
            '0004_org_owned_roots_rls',
            '0005_snapshot_result_immutability')
          AND status = 'APPLIED'
    ) = 5
    AND pg_catalog.to_regclass('measurement.score_record') IS NOT NULL
    AND NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_policies
        WHERE schemaname = 'measurement'
          AND tablename = 'score_record')
    AND NOT EXISTS (
        SELECT 1
        FROM iam.permission
        WHERE id = '10000000-0000-7000-8000-000000000034'::uuid);
