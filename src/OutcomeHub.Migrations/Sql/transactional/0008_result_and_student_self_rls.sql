INSERT INTO iam.permission (
    id,
    resource_type,
    action,
    field_scope,
    description)
SELECT seed.id, seed.resource_type, seed.action, '*', seed.description
FROM (VALUES
    ('10000000-0000-7000-8000-000000000040'::uuid, 'result.result_batch', 'READ', 'Read calculation result batches'),
    ('10000000-0000-7000-8000-000000000041'::uuid, 'result.student_clo_result', 'READ', 'Read student CLO attainment results'),
    ('10000000-0000-7000-8000-000000000042'::uuid, 'result.student_pi_result', 'READ', 'Read student PI attainment results'),
    ('10000000-0000-7000-8000-000000000043'::uuid, 'result.student_plo_result', 'READ', 'Read student PLO attainment results'),
    ('10000000-0000-7000-8000-000000000044'::uuid, 'result.cohort_outcome_result', 'READ', 'Read cohort outcome summary statistics'))
    AS seed(id, resource_type, action, description)
ON CONFLICT (id) DO NOTHING;

-- RLS: result.result_batch
ALTER TABLE result.result_batch ENABLE ROW LEVEL SECURITY;
ALTER TABLE result.result_batch FORCE ROW LEVEL SECURITY;

CREATE POLICY result_batch_migrator_policy
ON result.result_batch
FOR ALL
TO outcomehub_migrator
USING (true)
WITH CHECK (true);

CREATE POLICY result_batch_select_policy
ON result.result_batch
FOR SELECT
TO outcomehub_app
USING (
    iam.has_permission(
        'result.result_batch',
        'READ',
        '*',
        org_unit_id,
        NULL::uuid,
        program_version_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        measurement_period_id,
        NULL::uuid,
        NULL::text));

-- RLS: result.student_clo_result
ALTER TABLE result.student_clo_result ENABLE ROW LEVEL SECURITY;
ALTER TABLE result.student_clo_result FORCE ROW LEVEL SECURITY;

CREATE POLICY student_clo_result_migrator_policy
ON result.student_clo_result
FOR ALL
TO outcomehub_migrator
USING (true)
WITH CHECK (true);

CREATE POLICY student_clo_result_select_policy
ON result.student_clo_result
FOR SELECT
TO outcomehub_app
USING (
    iam.has_permission(
        'result.student_clo_result',
        'READ',
        '*',
        org_unit_id,
        program_id,
        program_version_id,
        cohort_id,
        curriculum_path_id,
        course_id,
        course_offering_id,
        measurement_period_id,
        student_id,
        NULL::text));

-- RLS: result.student_pi_result
ALTER TABLE result.student_pi_result ENABLE ROW LEVEL SECURITY;
ALTER TABLE result.student_pi_result FORCE ROW LEVEL SECURITY;

CREATE POLICY student_pi_result_migrator_policy
ON result.student_pi_result
FOR ALL
TO outcomehub_migrator
USING (true)
WITH CHECK (true);

CREATE POLICY student_pi_result_select_policy
ON result.student_pi_result
FOR SELECT
TO outcomehub_app
USING (
    iam.has_permission(
        'result.student_pi_result',
        'READ',
        '*',
        org_unit_id,
        program_id,
        program_version_id,
        cohort_id,
        curriculum_path_id,
        NULL::uuid,
        NULL::uuid,
        measurement_period_id,
        student_id,
        NULL::text));

-- RLS: result.student_plo_result
ALTER TABLE result.student_plo_result ENABLE ROW LEVEL SECURITY;
ALTER TABLE result.student_plo_result FORCE ROW LEVEL SECURITY;

CREATE POLICY student_plo_result_migrator_policy
ON result.student_plo_result
FOR ALL
TO outcomehub_migrator
USING (true)
WITH CHECK (true);

CREATE POLICY student_plo_result_select_policy
ON result.student_plo_result
FOR SELECT
TO outcomehub_app
USING (
    iam.has_permission(
        'result.student_plo_result',
        'READ',
        '*',
        org_unit_id,
        program_id,
        program_version_id,
        cohort_id,
        curriculum_path_id,
        NULL::uuid,
        NULL::uuid,
        measurement_period_id,
        student_id,
        NULL::text));

-- RLS: result.cohort_outcome_result
ALTER TABLE result.cohort_outcome_result ENABLE ROW LEVEL SECURITY;
ALTER TABLE result.cohort_outcome_result FORCE ROW LEVEL SECURITY;

CREATE POLICY cohort_outcome_result_migrator_policy
ON result.cohort_outcome_result
FOR ALL
TO outcomehub_migrator
USING (true)
WITH CHECK (true);

CREATE POLICY cohort_outcome_result_select_policy
ON result.cohort_outcome_result
FOR SELECT
TO outcomehub_app
USING (
    iam.has_permission(
        'result.cohort_outcome_result',
        'READ',
        '*',
        org_unit_id,
        program_id,
        program_version_id,
        cohort_id,
        curriculum_path_id,
        NULL::uuid,
        NULL::uuid,
        measurement_period_id,
        NULL::uuid,
        NULL::text));

-- Privileges
GRANT USAGE ON SCHEMA result TO outcomehub_app;

REVOKE ALL PRIVILEGES ON TABLE
    result.result_batch,
    result.student_clo_result,
    result.student_pi_result,
    result.student_plo_result,
    result.cohort_outcome_result
FROM PUBLIC, outcomehub_app;

GRANT SELECT ON TABLE
    result.result_batch,
    result.student_clo_result,
    result.student_pi_result,
    result.student_plo_result,
    result.cohort_outcome_result
TO outcomehub_app;
