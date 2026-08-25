-- ==============================================================================
-- OutcomeHub: Migration 0010 - People, Offerings & Teaching Assignment Grants
-- ==============================================================================

GRANT USAGE ON SCHEMA academic, portfolio, measurement, workflow, iam, document TO outcomehub_app;

GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE
    academic.person,
    academic.student,
    academic.student_path,
    academic.staff,
    academic.course_offering,
    academic.course_offering_instructor,
    academic.curriculum_path,
    academic.curriculum_path_course,
    measurement.enrollment,
    iam.user_account
TO outcomehub_app;
