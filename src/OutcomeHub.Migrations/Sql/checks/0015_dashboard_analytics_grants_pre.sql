SELECT
    (
        SELECT count(*)
        FROM ops.schema_migration
        WHERE migration_name = '0014_iam_audit_governance_grants'
          AND status = 'APPLIED'
    ) = 1
    AND pg_catalog.to_regclass('academic.program_version') IS NOT NULL
    AND pg_catalog.to_regclass('result.cohort_outcome_result') IS NOT NULL
    AND pg_catalog.to_regclass('quality.improvement_plan') IS NOT NULL;
