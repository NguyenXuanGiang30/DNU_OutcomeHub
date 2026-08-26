-- Migration 0014: Grant IAM, Audit, and Governance permissions to outcomehub_app
GRANT USAGE ON SCHEMA iam, audit, governance TO outcomehub_app;

GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE
    iam.principal,
    iam.user_account,
    iam.role,
    iam.role_version,
    iam.role_version_permission,
    iam.role_assignment,
    iam.access_scope,
    iam.permission,
    iam.sod_policy_version,
    iam.sod_rule,
    iam.sod_exception,
    iam.auth_session,
    iam.identity_provider,
    iam.external_identity,
    iam.idp_group_role_mapping,
    iam.service_account,
    iam.service_credential
TO outcomehub_app;

GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE
    audit.audit_event,
    audit.chain_head,
    governance.legal_hold,
    governance.legal_hold_item,
    governance.retention_binding,
    governance.retention_policy_version,
    governance.disposition_case,
    governance.disposition_item
TO outcomehub_app;

-- RLS mutation policies for outcomehub_app IAM writes
CREATE POLICY principal_insert_policy
ON iam.principal
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY principal_update_policy
ON iam.principal
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);

CREATE POLICY user_account_insert_policy
ON iam.user_account
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY user_account_update_policy
ON iam.user_account
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);

CREATE POLICY role_insert_policy
ON iam.role
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY role_update_policy
ON iam.role
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);

CREATE POLICY role_version_insert_policy
ON iam.role_version
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY role_version_update_policy
ON iam.role_version
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);

CREATE POLICY role_version_permission_insert_policy
ON iam.role_version_permission
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY role_version_permission_delete_policy
ON iam.role_version_permission
FOR DELETE
TO outcomehub_app
USING (true);

CREATE POLICY access_scope_insert_policy
ON iam.access_scope
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY access_scope_update_policy
ON iam.access_scope
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);

CREATE POLICY role_assignment_insert_policy
ON iam.role_assignment
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY role_assignment_update_policy
ON iam.role_assignment
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);

CREATE POLICY audit_event_insert_policy
ON audit.audit_event
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY legal_hold_insert_policy
ON governance.legal_hold
FOR INSERT
TO outcomehub_app
WITH CHECK (true);

CREATE POLICY legal_hold_update_policy
ON governance.legal_hold
FOR UPDATE
TO outcomehub_app
USING (true)
WITH CHECK (true);
