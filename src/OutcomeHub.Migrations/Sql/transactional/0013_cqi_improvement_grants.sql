-- Migration 0013: Grant CQI improvement plan permissions on quality schema
GRANT USAGE ON SCHEMA quality, document, workflow TO outcomehub_app;

GRANT SELECT, INSERT, UPDATE ON TABLE
    quality.improvement_plan,
    quality.improvement_action,
    quality.improvement_finding,
    quality.improvement_evidence,
    quality.improvement_document,
    quality.remeasurement_evaluation
TO outcomehub_app;

GRANT SELECT ON TABLE
    quality.plan_waiver,
    document.document_version,
    document.evidence_version,
    document.evidence,
    workflow.definition,
    workflow.instance
TO outcomehub_app;

GRANT INSERT, UPDATE ON TABLE
    workflow.instance
TO outcomehub_app;

-- RLS mutation policies for outcomehub_app CQI writes
CREATE POLICY improvement_plan_insert_policy
ON quality.improvement_plan
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY improvement_plan_update_policy
ON quality.improvement_plan
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);

CREATE POLICY improvement_action_insert_policy
ON quality.improvement_action
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY improvement_action_update_policy
ON quality.improvement_action
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);

CREATE POLICY improvement_finding_insert_policy
ON quality.improvement_finding
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY improvement_evidence_insert_policy
ON quality.improvement_evidence
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY improvement_evidence_update_policy
ON quality.improvement_evidence
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);

CREATE POLICY improvement_document_insert_policy
ON quality.improvement_document
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY remeasurement_evaluation_insert_policy
ON quality.remeasurement_evaluation
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY workflow_instance_insert_policy
ON workflow.instance
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY workflow_instance_update_policy
ON workflow.instance
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);
