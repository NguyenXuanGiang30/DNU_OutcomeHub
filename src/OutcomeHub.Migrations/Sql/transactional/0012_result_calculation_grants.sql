-- Migration 0012: Grant execution and calculation permissions on results, snapshots, and governance
GRANT USAGE ON SCHEMA measurement, result, governance TO outcomehub_app;

GRANT SELECT, INSERT, UPDATE ON TABLE
    measurement.input_snapshot,
    result.result_batch,
    result.student_clo_result,
    result.student_pi_result,
    result.student_plo_result,
    result.cohort_outcome_result
TO outcomehub_app;

GRANT SELECT, INSERT ON TABLE
    governance.governed_resource
TO outcomehub_app;

GRANT SELECT ON TABLE
    academic.curriculum_path,
    academic.student_path,
    iam.sod_policy_version
TO outcomehub_app;

-- RLS mutation policies for outcomehub_app calculation writes
CREATE POLICY result_batch_insert_policy
ON result.result_batch
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY result_batch_update_policy
ON result.result_batch
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);

CREATE POLICY student_clo_result_insert_policy
ON result.student_clo_result
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY student_clo_result_update_policy
ON result.student_clo_result
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);

CREATE POLICY student_pi_result_insert_policy
ON result.student_pi_result
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY student_pi_result_update_policy
ON result.student_pi_result
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);

CREATE POLICY student_plo_result_insert_policy
ON result.student_plo_result
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY student_plo_result_update_policy
ON result.student_plo_result
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);

CREATE POLICY cohort_outcome_result_insert_policy
ON result.cohort_outcome_result
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY cohort_outcome_result_update_policy
ON result.cohort_outcome_result
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);
