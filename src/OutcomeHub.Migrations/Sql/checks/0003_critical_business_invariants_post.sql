SELECT
    NOT EXISTS (
        SELECT required.function_name
        FROM (VALUES
            ('academic.validate_program_version_cohort_same_program()'),
            ('academic.validate_program_course_curriculum_plan_version()'),
            ('academic.validate_anchor_criterion_typed_binding()'),
            ('academic.enforce_curriculum_block_acyclic()'),
            ('portfolio.enforce_assessment_item_acyclic()'),
            ('governance.enforce_resource_dependency_acyclic()'),
            ('result.enforce_batch_supersession()'),
            ('result.validate_batch_supersession_same_period()'),
            ('result.validate_current_publication_not_revoked()'),
            ('integration.enforce_validation_issue_locator()'),
            ('integration.guard_referenced_staging_locator()'))
            AS required(function_name)
        WHERE pg_catalog.to_regprocedure(required.function_name) IS NULL)
    AND NOT EXISTS (
        SELECT required.trigger_name
        FROM (VALUES
            ('ctrg_program_version_cohort_same_program'),
            ('ctrg_program_version_same_program_cohort'),
            ('ctrg_cohort_same_program_version'),
            ('ctrg_program_course_curriculum_plan_version'),
            ('ctrg_curriculum_block_program_course_version'),
            ('ctrg_curriculum_plan_program_course_version'),
            ('ctrg_anchor_criterion_typed_binding'),
            ('ctrg_anchor_assessment_typed_binding'),
            ('ctrg_direct_measurement_source_typed_binding'),
            ('ctrg_syllabus_traceability_anchor_binding'),
            ('ctrg_rubric_criterion_anchor_binding'),
            ('trg_curriculum_block_acyclic'),
            ('trg_assessment_item_acyclic'),
            ('trg_resource_dependency_acyclic'),
            ('trg_batch_supersession_guard'),
            ('ctrg_result_batch_supersession_same_period'),
            ('ctrg_current_publication_not_revoked'),
            ('ctrg_publication_revocation_not_current'),
            ('trg_validation_issue_typed_locator'),
            ('trg_staging_student_validation_issue_locator'),
            ('trg_staging_course_offering_validation_issue_locator'),
            ('trg_staging_enrollment_validation_issue_locator'),
            ('trg_staging_score_validation_issue_locator'),
            ('trg_staging_course_pi_mapping_validation_issue_locator'),
            ('trg_staging_direct_measurement_plan_validation_issue_locator'),
            ('trg_staging_rubric_criterion_validation_issue_locator'))
            AS required(trigger_name)
        WHERE NOT EXISTS (
            SELECT 1
            FROM pg_catalog.pg_trigger AS trigger_row
            WHERE trigger_row.tgname = required.trigger_name
              AND trigger_row.tgenabled = 'O'
              AND NOT trigger_row.tgisinternal));
