SELECT
    pg_catalog.to_regprocedure(
        'academic.validate_program_version_cohort_same_program()') IS NULL
    AND pg_catalog.to_regprocedure(
        'academic.validate_program_course_curriculum_plan_version()') IS NULL
    AND pg_catalog.to_regprocedure(
        'academic.validate_anchor_criterion_typed_binding()') IS NULL
    AND pg_catalog.to_regprocedure(
        'governance.enforce_resource_dependency_acyclic()') IS NULL
    AND pg_catalog.to_regprocedure(
        'result.enforce_batch_supersession()') IS NULL
    AND pg_catalog.to_regprocedure(
        'integration.enforce_validation_issue_locator()') IS NULL
    AND NOT EXISTS (
        SELECT 1 FROM academic.program_version_cohort
        UNION ALL
        SELECT 1 FROM academic.program_course
        UNION ALL
        SELECT 1 FROM academic.anchor_criterion
        UNION ALL
        SELECT 1 FROM academic.curriculum_block
        UNION ALL
        SELECT 1 FROM portfolio.assessment_item
        UNION ALL
        SELECT 1 FROM governance.resource_dependency
        UNION ALL
        SELECT 1 FROM result.batch_supersession
        UNION ALL
        SELECT 1 FROM result.current_publication
        UNION ALL
        SELECT 1 FROM result.publication_revocation
        UNION ALL
        SELECT 1 FROM integration.validation_issue);
