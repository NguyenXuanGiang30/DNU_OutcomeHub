-- ==============================================================================
-- OutcomeHub: Migration 0009 - Academic Structure & Portfolio Grants
-- ==============================================================================

GRANT USAGE ON SCHEMA academic, portfolio, measurement, workflow, iam, document TO outcomehub_app;

GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE
    academic.program_version,
    academic.program_plo,
    academic.program_pi,
    academic.decision_record,
    academic.institution_template_version,
    academic.template_plo,
    academic.template_pi,
    academic.program_course,
    academic.course_version,
    academic.course_pi_mapping,
    academic.cohort,
    academic.program_version_cohort,
    academic.curriculum_plan,
    academic.curriculum_block,
    portfolio.clo,
    portfolio.course_objective,
    portfolio.assessment_item,
    portfolio.rubric,
    portfolio.rubric_criterion,
    portfolio.rubric_level,
    portfolio.syllabus_section_content,
    portfolio.syllabus_template_version,
    portfolio.syllabus_template_rubric_scale,
    portfolio.syllabus_template_rubric_scale_level,
    workflow.instance,
    workflow.definition,
    iam.sod_policy_version
TO outcomehub_app;
