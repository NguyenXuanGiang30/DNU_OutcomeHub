-- Migration 0011: Table grants for measurement periods, offerings, and enrollments.

GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE
    measurement.measurement_period,
    measurement.measurement_period_cohort,
    measurement.measurement_period_offering,
    measurement.measurement_period_target,
    measurement.program_policy_binding,
    measurement.enrollment,
    measurement.enrollment_revision,
    measurement.score_dataset,
    measurement.score_identity,
    measurement.calculation_policy,
    measurement.calculation_policy_version,
    academic.program_version_cohort
TO outcomehub_app;
