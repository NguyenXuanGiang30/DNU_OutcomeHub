
-- OutcomeHub canonical SQL baseline.
-- Curated from EF Core migrations through 20260824173957_AddCourseRlsFoundation.
-- Transaction ownership and migration history are managed by OutcomeHub.Migrations.

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'iam') THEN
        CREATE SCHEMA iam;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'ai') THEN
        CREATE SCHEMA ai;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'academic') THEN
        CREATE SCHEMA academic;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'audit') THEN
        CREATE SCHEMA audit;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'portfolio') THEN
        CREATE SCHEMA portfolio;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'result') THEN
        CREATE SCHEMA result;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'measurement') THEN
        CREATE SCHEMA measurement;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'workflow') THEN
        CREATE SCHEMA workflow;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'ops') THEN
        CREATE SCHEMA ops;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'governance') THEN
        CREATE SCHEMA governance;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'document') THEN
        CREATE SCHEMA document;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'integration') THEN
        CREATE SCHEMA integration;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'quality') THEN
        CREATE SCHEMA quality;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'reporting') THEN
        CREATE SCHEMA reporting;
    END IF;
END $EF$;

CREATE EXTENSION IF NOT EXISTS citext;

CREATE TABLE audit.chain_head (
    partition_start date NOT NULL,
    chain_id uuid NOT NULL,
    last_sequence bigint NOT NULL,
    last_hash char(64) NOT NULL,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_chain_head PRIMARY KEY (partition_start, chain_id),
    CONSTRAINT ck_chain_head_last_hash CHECK (last_hash ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_chain_head_last_sequence CHECK (last_sequence >= 0),
    CONSTRAINT ck_chain_head_row_version CHECK (row_version > 0)
);

CREATE TABLE workflow.definition (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    version_no integer NOT NULL,
    subject_type varchar(64) NOT NULL,
    configuration jsonb NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    status varchar(32) NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_definition PRIMARY KEY (id),
    CONSTRAINT ck_definition_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_definition_code CHECK (code = btrim(code) AND char_length(code) > 0),
    CONSTRAINT ck_definition_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_definition_version_no CHECK (version_no > 0)
);

CREATE TABLE ops.deployment_event (
    id uuid NOT NULL,
    application_release varchar(128) NOT NULL,
    migration_version_from varchar(255),
    migration_version_to varchar(255),
    started_at timestamptz NOT NULL,
    completed_at timestamptz,
    actor varchar(255) NOT NULL,
    status varchar(20) NOT NULL,
    duration_ms bigint,
    log_reference varchar(1024),
    CONSTRAINT pk_deployment_event PRIMARY KEY (id),
    CONSTRAINT ck_deployment_event_completion CHECK (completed_at IS NULL OR completed_at >= started_at),
    CONSTRAINT ck_deployment_event_duration CHECK (duration_ms IS NULL OR duration_ms >= 0)
);

CREATE TABLE governance.governed_resource (
    id uuid NOT NULL,
    resource_type varchar(64) NOT NULL,
    classification varchar(16) NOT NULL,
    disposition_status varchar(32) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_governed_resource PRIMARY KEY (id),
    CONSTRAINT ck_governed_resource_classification CHECK (classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')),
    CONSTRAINT ck_governed_resource_disposition_status CHECK (disposition_status IN ('ACTIVE','ON_HOLD','ELIGIBLE','PENDING','DISPOSED','FAILED'))
);

CREATE TABLE ai.ground_truth_suite (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    CONSTRAINT pk_ground_truth_suite PRIMARY KEY (id),
    CONSTRAINT ck_ground_truth_suite_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0)
);

CREATE TABLE iam.identity_provider (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    protocol varchar(20) NOT NULL,
    issuer_or_entity_id varchar(512) NOT NULL,
    client_id varchar(255),
    metadata_url varchar(2048),
    claims_mapping jsonb NOT NULL,
    claims_mapping_version integer NOT NULL,
    secret_reference varchar(512),
    status varchar(20) NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    CONSTRAINT pk_identity_provider PRIMARY KEY (id),
    CONSTRAINT ck_identity_provider_code CHECK (code = btrim(code) AND char_length(code) > 0),
    CONSTRAINT ck_identity_provider_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_identity_provider_mapping_version CHECK (claims_mapping_version > 0),
    CONSTRAINT ck_identity_provider_protocol CHECK (protocol IN ('OIDC', 'SAML')),
    CONSTRAINT ck_identity_provider_status CHECK (status IN ('ACTIVE', 'DISABLED'))
);

CREATE TABLE integration.outbox_message (
    id uuid NOT NULL,
    aggregate_type varchar(64) NOT NULL,
    aggregate_id uuid NOT NULL,
    aggregate_version bigint NOT NULL,
    event_type varchar(128) NOT NULL,
    event_schema_version integer NOT NULL,
    payload jsonb NOT NULL,
    headers jsonb,
    classification varchar(20) NOT NULL,
    correlation_id uuid NOT NULL,
    causation_id uuid,
    trace_id varchar(64),
    occurred_at timestamptz NOT NULL,
    available_at timestamptz NOT NULL,
    published_at timestamptz,
    attempt_count integer NOT NULL,
    locked_by uuid,
    locked_until timestamptz,
    status varchar(32) NOT NULL,
    last_error_code varchar(64),
    CONSTRAINT pk_outbox_message PRIMARY KEY (id),
    CONSTRAINT ck_outbox_message_attempt_count CHECK (attempt_count >= 0),
    CONSTRAINT ck_outbox_message_classification CHECK (classification IN ('PUBLIC', 'INTERNAL', 'CONFIDENTIAL', 'RESTRICTED')),
    CONSTRAINT ck_outbox_message_lock CHECK (num_nonnulls(locked_by, locked_until) IN (0, 2)),
    CONSTRAINT ck_outbox_message_published_at CHECK (published_at IS NULL OR published_at >= occurred_at),
    CONSTRAINT ck_outbox_message_versions CHECK (aggregate_version >= 0 AND event_schema_version > 0)
);

CREATE TABLE iam.permission (
    id uuid NOT NULL,
    resource_type varchar(64) NOT NULL,
    action varchar(64) NOT NULL,
    field_scope varchar(128) NOT NULL,
    description text,
    CONSTRAINT pk_permission PRIMARY KEY (id),
    CONSTRAINT ck_permission_action CHECK (action = btrim(action) AND char_length(action) > 0),
    CONSTRAINT ck_permission_field_scope CHECK (field_scope = btrim(field_scope) AND char_length(field_scope) > 0),
    CONSTRAINT ck_permission_resource_type CHECK (resource_type = btrim(resource_type) AND char_length(resource_type) > 0)
);

CREATE TABLE iam.principal (
    id uuid NOT NULL,
    principal_type varchar(20) NOT NULL,
    status varchar(20) NOT NULL,
    display_name varchar(255) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_principal PRIMARY KEY (id),
    CONSTRAINT ck_principal_display_name CHECK (display_name = btrim(display_name) AND char_length(display_name) > 0),
    CONSTRAINT ck_principal_principal_type CHECK (principal_type IN ('USER', 'SERVICE_ACCOUNT', 'SYSTEM')),
    CONSTRAINT ck_principal_status CHECK (status IN ('ACTIVE', 'LOCKED', 'DISABLED', 'EXPIRED'))
);

CREATE TABLE reporting.refresh_registry (
    view_name varchar(128) NOT NULL,
    last_started_at timestamptz,
    last_completed_at timestamptz,
    status varchar(20) NOT NULL,
    source_watermark varchar(255),
    row_count bigint,
    duration_ms bigint,
    error text,
    CONSTRAINT pk_refresh_registry PRIMARY KEY (view_name),
    CONSTRAINT ck_refresh_registry_counts CHECK ((row_count IS NULL OR row_count >= 0) AND (duration_ms IS NULL OR duration_ms >= 0)),
    CONSTRAINT ck_refresh_registry_error CHECK (status <> 'FAILED' OR error IS NOT NULL),
    CONSTRAINT ck_refresh_registry_state_time CHECK ((status = 'PENDING') OR (status = 'RUNNING' AND last_started_at IS NOT NULL) OR (status IN ('SUCCEEDED','FAILED') AND last_started_at IS NOT NULL AND last_completed_at IS NOT NULL)),
    CONSTRAINT ck_refresh_registry_status CHECK (status IN ('PENDING','RUNNING','SUCCEEDED','FAILED')),
    CONSTRAINT ck_refresh_registry_time CHECK (last_completed_at IS NULL OR (last_started_at IS NOT NULL AND last_completed_at >= last_started_at)),
    CONSTRAINT ck_refresh_registry_view_name CHECK (view_name = lower(btrim(view_name)) AND char_length(view_name) > 0 AND view_name ~ '^[a-z][a-z0-9_]*(\.[a-z][a-z0-9_]*)?$')
);

CREATE TABLE iam.role (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    is_system boolean NOT NULL,
    status varchar(20) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_role PRIMARY KEY (id),
    CONSTRAINT ck_role_code CHECK (code = btrim(code) AND char_length(code) > 0),
    CONSTRAINT ck_role_name CHECK (name = btrim(name) AND char_length(name) > 0),
    CONSTRAINT ck_role_status CHECK (status IN ('ACTIVE', 'DISABLED'))
);


CREATE TABLE audit.archive_manifest (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    period_from timestamptz NOT NULL,
    period_to timestamptz NOT NULL,
    first_event_id uuid NOT NULL,
    last_event_id uuid NOT NULL,
    event_count bigint NOT NULL,
    root_hash char(64) NOT NULL,
    signature bytea NOT NULL,
    object_uri varchar(2048) NOT NULL,
    object_checksum char(64) NOT NULL,
    archived_at timestamptz NOT NULL,
    verified_at timestamptz,
    CONSTRAINT pk_archive_manifest PRIMARY KEY (id),
    CONSTRAINT ck_archive_manifest_event_count CHECK (event_count > 0),
    CONSTRAINT ck_archive_manifest_object_checksum CHECK (object_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_archive_manifest_period CHECK (period_to > period_from),
    CONSTRAINT ck_archive_manifest_root_hash CHECK (root_hash ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_archive_manifest_verified_at CHECK (verified_at IS NULL OR verified_at >= archived_at),
    CONSTRAINT fk_archive_manifest_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT
);

CREATE TABLE governance.resource_dependency (
    parent_governed_resource_id uuid NOT NULL,
    child_governed_resource_id uuid NOT NULL,
    dependency_role varchar(32) NOT NULL,
    CONSTRAINT pk_resource_dependency PRIMARY KEY (parent_governed_resource_id, child_governed_resource_id, dependency_role),
    CONSTRAINT ck_resource_dependency_not_self CHECK (parent_governed_resource_id <> child_governed_resource_id),
    CONSTRAINT fk_resource_dependency_child FOREIGN KEY (child_governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_resource_dependency_parent FOREIGN KEY (parent_governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT
);

CREATE TABLE iam.auth_session (
    id uuid NOT NULL,
    principal_id uuid NOT NULL,
    session_token_hash char(64) NOT NULL,
    idp_session_hash char(64),
    issued_at timestamptz NOT NULL,
    last_seen_at timestamptz NOT NULL,
    expires_at timestamptz NOT NULL,
    revoked_at timestamptz,
    ip_address inet,
    user_agent_hash char(64),
    auth_strength varchar(32) NOT NULL,
    mfa_used boolean NOT NULL,
    CONSTRAINT pk_auth_session PRIMARY KEY (id),
    CONSTRAINT ck_auth_session_auth_strength CHECK (auth_strength = btrim(auth_strength) AND char_length(auth_strength) > 0),
    CONSTRAINT ck_auth_session_idp_hash CHECK (idp_session_hash IS NULL OR idp_session_hash ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_auth_session_times CHECK (last_seen_at >= issued_at AND expires_at > issued_at AND (revoked_at IS NULL OR revoked_at >= issued_at)),
    CONSTRAINT ck_auth_session_token_hash CHECK (session_token_hash ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_auth_session_user_agent_hash CHECK (user_agent_hash IS NULL OR user_agent_hash ~ '^[0-9a-f]{64}$'),
    CONSTRAINT fk_auth_session_principal FOREIGN KEY (principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE governance.disposition_case (
    id uuid NOT NULL,
    case_code varchar(64) NOT NULL,
    status varchar(32) NOT NULL,
    requested_action varchar(32) NOT NULL,
    approved_by uuid,
    approved_at timestamptz,
    disposal_certificate_checksum char(64),
    created_at timestamptz NOT NULL,
    created_by uuid NOT NULL,
    CONSTRAINT pk_disposition_case PRIMARY KEY (id),
    CONSTRAINT ck_disposition_case_approval CHECK ((approved_by IS NULL) = (approved_at IS NULL)),
    CONSTRAINT ck_disposition_case_certificate CHECK (disposal_certificate_checksum IS NULL OR disposal_certificate_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_disposition_case_code CHECK (case_code = upper(btrim(case_code)) AND char_length(case_code) > 0),
    CONSTRAINT ck_disposition_case_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','RUNNING','COMPLETED','FAILED','CANCELLED')),
    CONSTRAINT fk_disposition_case_approver FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_disposition_case_creator FOREIGN KEY (created_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE document.file_object (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    storage_provider varchar(64) NOT NULL,
    bucket varchar(255) NOT NULL,
    object_key varchar(1024) NOT NULL,
    storage_version varchar(255) NOT NULL,
    original_filename varchar(255) NOT NULL,
    declared_media_type varchar(127) NOT NULL,
    detected_media_type varchar(127),
    size_bytes bigint NOT NULL,
    sha256 char(64) NOT NULL,
    classification varchar(16) NOT NULL,
    malware_scan_status varchar(32) NOT NULL,
    malware_scan_engine varchar(127),
    malware_scan_version varchar(64),
    malware_scan_at timestamptz,
    encryption_key_reference varchar(255),
    purged_at timestamptz,
    created_by uuid NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_file_object PRIMARY KEY (id),
    CONSTRAINT ck_file_object_classification CHECK (classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')),
    CONSTRAINT ck_file_object_malware_scan_status CHECK (malware_scan_status IN ('PENDING','SCANNING','CLEAN','INFECTED','ERROR')),
    CONSTRAINT ck_file_object_scan_metadata CHECK (malware_scan_at IS NULL OR malware_scan_engine IS NOT NULL),
    CONSTRAINT ck_file_object_sha256 CHECK (sha256 ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_file_object_size CHECK (size_bytes >= 0),
    CONSTRAINT fk_file_object_creator FOREIGN KEY (created_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_file_object_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT
);

CREATE TABLE integration.idempotency_record (
    id uuid NOT NULL,
    principal_id uuid NOT NULL,
    operation_code varchar(64) NOT NULL,
    idempotency_key varchar(255) NOT NULL,
    request_hash char(64) NOT NULL,
    status varchar(20) NOT NULL,
    locked_by uuid,
    locked_until timestamptz,
    response_status integer,
    response_headers jsonb,
    response_body jsonb,
    resource_id uuid,
    created_at timestamptz NOT NULL,
    completed_at timestamptz,
    expires_at timestamptz NOT NULL,
    CONSTRAINT pk_idempotency_record PRIMARY KEY (id),
    CONSTRAINT ck_idempotency_record_lock CHECK (num_nonnulls(locked_by, locked_until) IN (0, 2)),
    CONSTRAINT ck_idempotency_record_request_hash CHECK (request_hash ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_idempotency_record_response_status CHECK (response_status IS NULL OR response_status BETWEEN 100 AND 599),
    CONSTRAINT ck_idempotency_record_status CHECK (status IN ('IN_PROGRESS', 'SUCCEEDED', 'FAILED_FINAL')),
    CONSTRAINT ck_idempotency_record_times CHECK (expires_at > created_at AND (completed_at IS NULL OR completed_at >= created_at)),
    CONSTRAINT fk_idempotency_record_locked_by FOREIGN KEY (locked_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_idempotency_record_principal FOREIGN KEY (principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE workflow.instance (
    id uuid NOT NULL,
    definition_id uuid NOT NULL,
    current_state varchar(64) NOT NULL,
    started_by uuid NOT NULL,
    started_at timestamptz NOT NULL,
    completed_at timestamptz,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_instance PRIMARY KEY (id),
    CONSTRAINT ck_instance_completion CHECK (completed_at IS NULL OR completed_at >= started_at),
    CONSTRAINT ck_instance_current_state CHECK (current_state = btrim(current_state) AND char_length(current_state) > 0),
    CONSTRAINT ck_instance_row_version CHECK (row_version > 0),
    CONSTRAINT fk_instance_definition FOREIGN KEY (definition_id) REFERENCES workflow.definition (id) ON DELETE RESTRICT,
    CONSTRAINT fk_instance_started_by FOREIGN KEY (started_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE governance.legal_hold (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    title varchar(255) NOT NULL,
    reason text NOT NULL,
    status varchar(32) NOT NULL,
    effective_from timestamptz NOT NULL,
    released_at timestamptz,
    created_by uuid NOT NULL,
    approved_by uuid,
    CONSTRAINT pk_legal_hold PRIMARY KEY (id),
    CONSTRAINT ck_legal_hold_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_legal_hold_release CHECK (released_at IS NULL OR released_at >= effective_from),
    CONSTRAINT ck_legal_hold_status CHECK (status IN ('DRAFT','ACTIVE','RELEASED','CANCELLED')),
    CONSTRAINT fk_legal_hold_approver FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_legal_hold_creator FOREIGN KEY (created_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE academic.org_unit (
    id uuid NOT NULL,
    parent_id uuid,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    unit_type varchar(20) NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    status varchar(20) NOT NULL,
    created_at timestamptz NOT NULL,
    created_by uuid NOT NULL,
    updated_at timestamptz NOT NULL,
    updated_by uuid NOT NULL,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_org_unit PRIMARY KEY (id),
    CONSTRAINT ck_org_unit_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_org_unit_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_org_unit_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT ck_org_unit_unit_type CHECK (unit_type IN ('UNIVERSITY','CAMPUS','FACULTY','INSTITUTE','DEPARTMENT','CENTER')),
    CONSTRAINT fk_org_unit_created_by FOREIGN KEY (created_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_org_unit_parent FOREIGN KEY (parent_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_org_unit_updated_by FOREIGN KEY (updated_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE governance.retention_policy_version (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    version_no integer NOT NULL,
    name varchar(255) NOT NULL,
    resource_type varchar(64) NOT NULL,
    trigger_event varchar(64) NOT NULL,
    retention_days integer NOT NULL,
    disposition_action varchar(32) NOT NULL,
    legal_basis text NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    status varchar(20) NOT NULL,
    approved_by uuid,
    approved_at timestamptz,
    CONSTRAINT pk_retention_policy_version PRIMARY KEY (id),
    CONSTRAINT ck_retention_policy_version_approval CHECK ((approved_by IS NULL) = (approved_at IS NULL)),
    CONSTRAINT ck_retention_policy_version_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_retention_policy_version_days CHECK (retention_days >= 0),
    CONSTRAINT ck_retention_policy_version_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_retention_policy_version_no CHECK (version_no > 0),
    CONSTRAINT ck_retention_policy_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_retention_policy_version_approver FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE academic.validation_run (
    id uuid NOT NULL,
    aggregate_type varchar(64) NOT NULL,
    aggregate_id uuid NOT NULL,
    ruleset_version varchar(64) NOT NULL,
    content_hash char(64) NOT NULL,
    passed boolean NOT NULL,
    run_at timestamptz NOT NULL,
    requested_by uuid NOT NULL,
    CONSTRAINT pk_validation_run PRIMARY KEY (id),
    CONSTRAINT ck_validation_run_aggregate_type CHECK (aggregate_type = upper(btrim(aggregate_type)) AND char_length(aggregate_type) > 0),
    CONSTRAINT ck_validation_run_content_hash CHECK (content_hash ~ '^[0-9a-f]{64}$'),
    CONSTRAINT fk_validation_run_requested_by FOREIGN KEY (requested_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE governance.object_reference (
    governed_resource_id uuid NOT NULL,
    file_object_id uuid NOT NULL,
    reference_role varchar(32) NOT NULL,
    effective_from timestamptz NOT NULL,
    effective_to timestamptz,
    CONSTRAINT pk_object_reference PRIMARY KEY (governed_resource_id, file_object_id, reference_role, effective_from),
    CONSTRAINT ck_object_reference_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT fk_object_reference_file_object FOREIGN KEY (file_object_id) REFERENCES document.file_object (id) ON DELETE RESTRICT,
    CONSTRAINT fk_object_reference_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT
);

CREATE TABLE workflow.comment (
    id uuid NOT NULL,
    instance_id uuid NOT NULL,
    author_principal_id uuid NOT NULL,
    target_locator jsonb,
    body text NOT NULL,
    created_at timestamptz NOT NULL,
    resolved_at timestamptz,
    CONSTRAINT pk_comment PRIMARY KEY (id),
    CONSTRAINT ck_comment_body CHECK (char_length(btrim(body)) > 0),
    CONSTRAINT ck_comment_resolved_at CHECK (resolved_at IS NULL OR resolved_at >= created_at),
    CONSTRAINT fk_comment_author_principal FOREIGN KEY (author_principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_comment_instance FOREIGN KEY (instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE iam.sod_policy_version (
    id uuid NOT NULL,
    version_no integer NOT NULL,
    status varchar(32) NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    workflow_instance_id uuid NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_sod_policy_version PRIMARY KEY (id),
    CONSTRAINT ck_sod_policy_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_sod_policy_version_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_sod_policy_version_number CHECK (version_no > 0),
    CONSTRAINT ck_sod_policy_version_status CHECK (status IN ('DRAFT', 'IN_REVIEW', 'APPROVED', 'ACTIVE', 'EXPIRED', 'REJECTED')),
    CONSTRAINT fk_sod_policy_version_workflow_instance FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE workflow.task (
    id uuid NOT NULL,
    instance_id uuid NOT NULL,
    step_code varchar(64) NOT NULL,
    assignee_principal_id uuid,
    assignee_role_id uuid,
    status varchar(32) NOT NULL,
    due_at timestamptz,
    decision varchar(32),
    decision_reason text,
    completed_at timestamptz,
    CONSTRAINT pk_task PRIMARY KEY (id),
    CONSTRAINT ck_task_assignee CHECK (num_nonnulls(assignee_principal_id, assignee_role_id) >= 1),
    CONSTRAINT ck_task_step_code CHECK (step_code = btrim(step_code) AND char_length(step_code) > 0),
    CONSTRAINT fk_task_assignee_principal FOREIGN KEY (assignee_principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_task_assignee_role FOREIGN KEY (assignee_role_id) REFERENCES iam.role (id) ON DELETE RESTRICT,
    CONSTRAINT fk_task_instance FOREIGN KEY (instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE workflow.transition (
    id uuid NOT NULL,
    instance_id uuid NOT NULL,
    from_state varchar(64) NOT NULL,
    to_state varchar(64) NOT NULL,
    event_code varchar(64) NOT NULL,
    actor_principal_id uuid NOT NULL,
    reason text,
    occurred_at timestamptz NOT NULL,
    request_id uuid NOT NULL,
    CONSTRAINT pk_transition PRIMARY KEY (id),
    CONSTRAINT ck_transition_event_code CHECK (event_code = btrim(event_code) AND char_length(event_code) > 0),
    CONSTRAINT ck_transition_states CHECK (char_length(btrim(from_state)) > 0 AND char_length(btrim(to_state)) > 0),
    CONSTRAINT fk_transition_actor_principal FOREIGN KEY (actor_principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_transition_instance FOREIGN KEY (instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE governance.legal_hold_item (
    legal_hold_id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    added_at timestamptz NOT NULL,
    added_by uuid NOT NULL,
    CONSTRAINT pk_legal_hold_item PRIMARY KEY (legal_hold_id, governed_resource_id),
    CONSTRAINT fk_legal_hold_item_added_by FOREIGN KEY (added_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_legal_hold_item_hold FOREIGN KEY (legal_hold_id) REFERENCES governance.legal_hold (id) ON DELETE RESTRICT,
    CONSTRAINT fk_legal_hold_item_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.calculation_policy (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    description text,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_calculation_policy PRIMARY KEY (id),
    CONSTRAINT ck_calculation_policy_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT fk_calculation_policy_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT
);

CREATE TABLE academic.course (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    status varchar(20) NOT NULL,
    CONSTRAINT pk_course PRIMARY KEY (id),
    CONSTRAINT ck_course_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_course_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_course_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT
);

CREATE TABLE document.document (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    document_type varchar(64) NOT NULL,
    title varchar(255) NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    classification varchar(16) NOT NULL,
    status varchar(20) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_document PRIMARY KEY (id),
    CONSTRAINT ck_document_classification CHECK (classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')),
    CONSTRAINT ck_document_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','SUPERSEDED','ARCHIVED')),
    CONSTRAINT fk_document_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_document_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT
);

CREATE TABLE document.evidence (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    evidence_type varchar(64) NOT NULL,
    title varchar(255) NOT NULL,
    owner_principal_id uuid NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    classification varchar(16) NOT NULL,
    status varchar(20) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_evidence PRIMARY KEY (id),
    CONSTRAINT ck_evidence_classification CHECK (classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')),
    CONSTRAINT ck_evidence_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_evidence_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','SUPERSEDED','ARCHIVED')),
    CONSTRAINT fk_evidence_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_evidence_owner_principal FOREIGN KEY (owner_principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.indirect_instrument (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    CONSTRAINT pk_indirect_instrument PRIMARY KEY (id),
    CONSTRAINT ck_indirect_instrument_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT fk_indirect_instrument_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT
);

CREATE TABLE academic.institution_template (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    description text,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_institution_template PRIMARY KEY (id),
    CONSTRAINT ck_institution_template_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT fk_institution_template_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT
);

CREATE TABLE ai.model_deployment (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    CONSTRAINT pk_model_deployment PRIMARY KEY (id),
    CONSTRAINT ck_model_deployment_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT fk_model_deployment_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT
);

CREATE TABLE academic.program (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    degree_level varchar(32) NOT NULL,
    education_mode varchar(32) NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    status varchar(20) NOT NULL,
    created_at timestamptz NOT NULL,
    created_by uuid NOT NULL,
    updated_at timestamptz NOT NULL,
    updated_by uuid NOT NULL,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_program PRIMARY KEY (id),
    CONSTRAINT ck_program_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_program_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_program_created_by FOREIGN KEY (created_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_updated_by FOREIGN KEY (updated_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE ai.prompt (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    purpose text NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    CONSTRAINT pk_prompt PRIMARY KEY (id),
    CONSTRAINT ck_prompt_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT fk_prompt_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT
);

CREATE TABLE iam.service_account (
    principal_id uuid NOT NULL,
    client_id varchar(128) NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    purpose text NOT NULL,
    expires_at timestamptz,
    technical_contact varchar(255) NOT NULL,
    CONSTRAINT pk_service_account PRIMARY KEY (principal_id),
    CONSTRAINT ck_service_account_client_id CHECK (client_id = btrim(client_id) AND char_length(client_id) > 0),
    CONSTRAINT ck_service_account_purpose CHECK (char_length(btrim(purpose)) > 0),
    CONSTRAINT ck_service_account_technical_contact CHECK (technical_contact = btrim(technical_contact) AND char_length(technical_contact) > 0),
    CONSTRAINT fk_service_account_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_service_account_principal FOREIGN KEY (principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.syllabus_template (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    description text,
    CONSTRAINT pk_syllabus_template PRIMARY KEY (id),
    CONSTRAINT ck_syllabus_template_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT fk_syllabus_template_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT
);

CREATE TABLE governance.retention_binding (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    retention_policy_version_id uuid NOT NULL,
    trigger_event_at timestamptz NOT NULL,
    calculated_until timestamptz NOT NULL,
    status varchar(32) NOT NULL,
    source_reason text NOT NULL,
    CONSTRAINT pk_retention_binding PRIMARY KEY (id),
    CONSTRAINT ck_retention_binding_range CHECK (calculated_until >= trigger_event_at),
    CONSTRAINT ck_retention_binding_status CHECK (status IN ('ACTIVE','SUPERSEDED','ON_HOLD','ELIGIBLE','DISPOSED')),
    CONSTRAINT fk_retention_binding_policy_version FOREIGN KEY (retention_policy_version_id) REFERENCES governance.retention_policy_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_retention_binding_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT
);

CREATE TABLE academic.validation_issue (
    id uuid NOT NULL,
    validation_run_id uuid NOT NULL,
    rule_code varchar(64) NOT NULL,
    severity varchar(16) NOT NULL,
    entity_type varchar(64) NOT NULL,
    entity_id uuid,
    field_path varchar(512),
    message text NOT NULL,
    details jsonb,
    CONSTRAINT pk_validation_issue PRIMARY KEY (id),
    CONSTRAINT ck_validation_issue_rule_code CHECK (rule_code = upper(btrim(rule_code)) AND char_length(rule_code) > 0),
    CONSTRAINT ck_validation_issue_severity CHECK (severity IN ('INFO','WARNING','ERROR','BLOCKING')),
    CONSTRAINT fk_validation_issue_run FOREIGN KEY (validation_run_id) REFERENCES academic.validation_run (id) ON DELETE RESTRICT
);

CREATE TABLE iam.sod_rule (
    id uuid NOT NULL,
    policy_version_id uuid NOT NULL,
    resource_type varchar(64) NOT NULL,
    permission_a_id uuid NOT NULL,
    permission_b_id uuid NOT NULL,
    conflict_mode varchar(32) NOT NULL,
    severity varchar(20) NOT NULL,
    CONSTRAINT pk_sod_rule PRIMARY KEY (id),
    CONSTRAINT ck_sod_rule_conflict_mode CHECK (conflict_mode IN ('SAME_RESOURCE', 'SAME_WORKFLOW_INSTANCE')),
    CONSTRAINT ck_sod_rule_permissions CHECK (permission_a_id <> permission_b_id),
    CONSTRAINT ck_sod_rule_resource_type CHECK (resource_type = btrim(resource_type) AND char_length(resource_type) > 0),
    CONSTRAINT ck_sod_rule_severity CHECK (severity IN ('LOW', 'MEDIUM', 'HIGH', 'CRITICAL')),
    CONSTRAINT fk_sod_rule_permission_a FOREIGN KEY (permission_a_id) REFERENCES iam.permission (id) ON DELETE RESTRICT,
    CONSTRAINT fk_sod_rule_permission_b FOREIGN KEY (permission_b_id) REFERENCES iam.permission (id) ON DELETE RESTRICT,
    CONSTRAINT fk_sod_rule_policy_version FOREIGN KEY (policy_version_id) REFERENCES iam.sod_policy_version (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.calculation_policy_version (
    id uuid NOT NULL,
    policy_id uuid NOT NULL,
    version_no integer NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    status varchar(20) NOT NULL,
    formula_family varchar(64) NOT NULL,
    engine_contract_version varchar(32) NOT NULL,
    direct_source_min integer NOT NULL,
    direct_source_max integer NOT NULL,
    missing_data_rule varchar(64) NOT NULL,
    repeat_attempt_rule varchar(64) NOT NULL,
    withdrawal_rule varchar(64) NOT NULL,
    recognition_rule varchar(64) NOT NULL,
    direct_indirect_mode varchar(20) NOT NULL,
    alpha numeric(12,10),
    core_gate_mode varchar(32) NOT NULL,
    default_min_sample_size integer NOT NULL,
    definition jsonb NOT NULL,
    schema_version varchar(32) NOT NULL,
    workflow_instance_id uuid NOT NULL,
    checksum char(64) NOT NULL,
    supersedes_id uuid,
    CONSTRAINT pk_calculation_policy_version PRIMARY KEY (id),
    CONSTRAINT ck_calculation_policy_version_alpha CHECK ((direct_indirect_mode = 'COMBINED' AND alpha IS NOT NULL AND alpha NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND alpha >= 0 AND alpha <= 1) OR (direct_indirect_mode <> 'COMBINED' AND alpha IS NULL)),
    CONSTRAINT ck_calculation_policy_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_calculation_policy_version_direct_indirect_mode CHECK (direct_indirect_mode IN ('DIRECT','INDIRECT','COMBINED')),
    CONSTRAINT ck_calculation_policy_version_direct_sources CHECK (direct_source_min >= 0 AND direct_source_max >= direct_source_min),
    CONSTRAINT ck_calculation_policy_version_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_calculation_policy_version_no CHECK (version_no > 0),
    CONSTRAINT ck_calculation_policy_version_sample_size CHECK (default_min_sample_size > 0),
    CONSTRAINT ck_calculation_policy_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_calculation_policy_version_policy FOREIGN KEY (policy_id) REFERENCES measurement.calculation_policy (id) ON DELETE RESTRICT,
    CONSTRAINT fk_calculation_policy_version_supersedes FOREIGN KEY (supersedes_id) REFERENCES measurement.calculation_policy_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_calculation_policy_version_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.shared_syllabus_core (
    id uuid NOT NULL,
    course_id uuid NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    CONSTRAINT pk_shared_syllabus_core PRIMARY KEY (id),
    CONSTRAINT ck_shared_syllabus_core_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT fk_shared_syllabus_core_course FOREIGN KEY (course_id) REFERENCES academic.course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_shared_syllabus_core_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT
);

CREATE TABLE document.document_version (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    document_id uuid NOT NULL,
    version_no integer NOT NULL,
    file_object_id uuid NOT NULL,
    source_document_version_id uuid,
    generation_provenance jsonb,
    structured_content jsonb,
    content_schema_version varchar(64),
    metadata jsonb,
    checksum char(64) NOT NULL,
    status varchar(20) NOT NULL,
    workflow_instance_id uuid,
    supersedes_id uuid,
    approved_by uuid,
    approved_at timestamptz,
    created_by uuid NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_document_version PRIMARY KEY (id),
    CONSTRAINT ck_document_version_approval CHECK ((approved_by IS NULL) = (approved_at IS NULL)),
    CONSTRAINT ck_document_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_document_version_no CHECK (version_no > 0),
    CONSTRAINT ck_document_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','SUPERSEDED','ARCHIVED')),
    CONSTRAINT fk_document_version_approver FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_document_version_creator FOREIGN KEY (created_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_document_version_document FOREIGN KEY (document_id) REFERENCES document.document (id) ON DELETE RESTRICT,
    CONSTRAINT fk_document_version_file_object FOREIGN KEY (file_object_id) REFERENCES document.file_object (id) ON DELETE RESTRICT,
    CONSTRAINT fk_document_version_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_document_version_source FOREIGN KEY (source_document_version_id) REFERENCES document.document_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_document_version_supersedes FOREIGN KEY (supersedes_id) REFERENCES document.document_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_document_version_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.indirect_instrument_version (
    id uuid NOT NULL,
    instrument_id uuid NOT NULL,
    version_no integer NOT NULL,
    scale_min numeric(20,10) NOT NULL,
    scale_max numeric(20,10) NOT NULL,
    workflow_instance_id uuid NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_indirect_instrument_version PRIMARY KEY (id),
    CONSTRAINT ck_indirect_instrument_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_indirect_instrument_version_no CHECK (version_no > 0),
    CONSTRAINT ck_indirect_instrument_version_scale CHECK (scale_min NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND scale_max NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND scale_min < scale_max),
    CONSTRAINT fk_indirect_instrument_version_instrument FOREIGN KEY (instrument_id) REFERENCES measurement.indirect_instrument (id) ON DELETE RESTRICT,
    CONSTRAINT fk_indirect_instrument_version_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE academic.cohort (
    id uuid NOT NULL,
    program_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    admission_year integer NOT NULL,
    start_date date NOT NULL,
    end_date date,
    CONSTRAINT pk_cohort PRIMARY KEY (id),
    CONSTRAINT uq_cohort_id_program UNIQUE (id, program_id),
    CONSTRAINT ck_cohort_admission_year CHECK (admission_year BETWEEN 1900 AND 9999),
    CONSTRAINT ck_cohort_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_cohort_date_range CHECK (end_date IS NULL OR end_date >= start_date),
    CONSTRAINT fk_cohort_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT
);

CREATE TABLE iam.service_credential (
    id uuid NOT NULL,
    service_principal_id uuid NOT NULL,
    credential_type varchar(32) NOT NULL,
    key_prefix varchar(32),
    secret_hash varchar(255),
    secret_reference varchar(512),
    certificate_thumbprint varchar(128),
    public_jwk jsonb,
    effective_from date NOT NULL,
    effective_to date,
    revoked_at timestamptz,
    revoked_by uuid,
    revoke_reason text,
    last_used_at timestamptz,
    CONSTRAINT pk_service_credential PRIMARY KEY (id),
    CONSTRAINT ck_service_credential_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_service_credential_material CHECK ((credential_type = 'CLIENT_SECRET' AND num_nonnulls(secret_hash, secret_reference) = 1 AND certificate_thumbprint IS NULL AND public_jwk IS NULL) OR (credential_type = 'API_KEY' AND key_prefix IS NOT NULL AND secret_hash IS NOT NULL AND secret_reference IS NULL AND certificate_thumbprint IS NULL AND public_jwk IS NULL) OR (credential_type = 'MTLS' AND certificate_thumbprint IS NOT NULL AND secret_hash IS NULL AND secret_reference IS NULL AND public_jwk IS NULL) OR (credential_type = 'JWK' AND public_jwk IS NOT NULL AND secret_hash IS NULL AND secret_reference IS NULL AND certificate_thumbprint IS NULL)),
    CONSTRAINT ck_service_credential_revocation CHECK ((revoked_at IS NULL AND revoked_by IS NULL AND revoke_reason IS NULL) OR (revoked_at IS NOT NULL AND revoked_by IS NOT NULL AND char_length(btrim(revoke_reason)) > 0)),
    CONSTRAINT ck_service_credential_type CHECK (credential_type IN ('CLIENT_SECRET', 'API_KEY', 'MTLS', 'JWK')),
    CONSTRAINT fk_service_credential_revoked_by FOREIGN KEY (revoked_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_service_credential_service_account FOREIGN KEY (service_principal_id) REFERENCES iam.service_account (principal_id) ON DELETE RESTRICT
);

CREATE TABLE integration.source_system (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    system_type varchar(32) NOT NULL,
    base_url varchar(2048),
    owner_org_unit_id uuid NOT NULL,
    service_principal_id uuid NOT NULL,
    status varchar(20) NOT NULL,
    data_classification varchar(20) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_source_system PRIMARY KEY (id),
    CONSTRAINT ck_source_system_classification CHECK (data_classification IN ('PUBLIC', 'INTERNAL', 'CONFIDENTIAL', 'RESTRICTED')),
    CONSTRAINT ck_source_system_code CHECK (code = btrim(code) AND char_length(code) > 0),
    CONSTRAINT ck_source_system_status CHECK (status IN ('ACTIVE', 'DISABLED')),
    CONSTRAINT fk_source_system_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_source_system_service_account FOREIGN KEY (service_principal_id) REFERENCES iam.service_account (principal_id) ON DELETE RESTRICT
);

CREATE TABLE governance.disposition_item (
    id uuid NOT NULL,
    disposition_case_id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    retention_binding_id uuid NOT NULL,
    planned_action varchar(32) NOT NULL,
    status varchar(32) NOT NULL,
    object_deleted boolean NOT NULL,
    database_anonymized boolean NOT NULL,
    error text,
    completed_at timestamptz,
    CONSTRAINT pk_disposition_item PRIMARY KEY (id),
    CONSTRAINT ck_disposition_item_status CHECK (status IN ('PENDING','RUNNING','COMPLETED','FAILED','SKIPPED')),
    CONSTRAINT fk_disposition_item_case FOREIGN KEY (disposition_case_id) REFERENCES governance.disposition_case (id) ON DELETE RESTRICT,
    CONSTRAINT fk_disposition_item_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_disposition_item_retention_binding FOREIGN KEY (retention_binding_id) REFERENCES governance.retention_binding (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.policy_course_limit (
    policy_version_id uuid NOT NULL,
    course_type varchar(32) NOT NULL,
    max_m_count integer,
    max_direct_pi_count integer,
    exception_required boolean NOT NULL,
    CONSTRAINT pk_policy_course_limit PRIMARY KEY (policy_version_id, course_type),
    CONSTRAINT ck_policy_course_limit_counts CHECK ((max_m_count IS NULL OR max_m_count >= 0) AND (max_direct_pi_count IS NULL OR max_direct_pi_count >= 0)),
    CONSTRAINT fk_policy_course_limit_policy_version FOREIGN KEY (policy_version_id) REFERENCES measurement.calculation_policy_version (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.policy_population_rule (
    policy_version_id uuid NOT NULL,
    enrollment_status varchar(20) NOT NULL,
    denominator_action varchar(40) NOT NULL,
    CONSTRAINT pk_policy_population_rule PRIMARY KEY (policy_version_id, enrollment_status),
    CONSTRAINT ck_policy_population_rule_status CHECK (enrollment_status IN ('ENROLLED','COMPLETED','ABSENT','DEFERRED','WITHDRAWN','CANCELLED','RECOGNIZED')),
    CONSTRAINT fk_policy_population_rule_policy_version FOREIGN KEY (policy_version_id) REFERENCES measurement.calculation_policy_version (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.policy_rounding_rule (
    policy_version_id uuid NOT NULL,
    result_level varchar(20) NOT NULL,
    scale integer NOT NULL,
    rounding_mode varchar(32) NOT NULL,
    CONSTRAINT pk_policy_rounding_rule PRIMARY KEY (policy_version_id, result_level),
    CONSTRAINT ck_policy_rounding_rule_scale CHECK (scale BETWEEN 0 AND 10),
    CONSTRAINT fk_policy_rounding_rule_policy_version FOREIGN KEY (policy_version_id) REFERENCES measurement.calculation_policy_version (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.policy_threshold (
    policy_version_id uuid NOT NULL,
    outcome_level varchar(20) NOT NULL,
    theta_ind numeric(20,10) NOT NULL,
    theta_coh numeric(20,10) NOT NULL,
    near_threshold numeric(20,10),
    min_sample_size integer NOT NULL,
    CONSTRAINT pk_policy_threshold PRIMARY KEY (policy_version_id, outcome_level),
    CONSTRAINT ck_policy_threshold_level CHECK (outcome_level IN ('CLO','PI','PLO')),
    CONSTRAINT ck_policy_threshold_values CHECK (theta_ind NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_coh NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_ind BETWEEN 0 AND 100 AND theta_coh BETWEEN 0 AND 100 AND (near_threshold IS NULL OR near_threshold NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND near_threshold BETWEEN 0 AND 100) AND min_sample_size > 0),
    CONSTRAINT fk_policy_threshold_policy_version FOREIGN KEY (policy_version_id) REFERENCES measurement.calculation_policy_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.decision_record (
    id uuid NOT NULL,
    decision_number varchar(64) NOT NULL,
    issued_on date NOT NULL,
    issuer_org_unit_id uuid NOT NULL,
    title varchar(255) NOT NULL,
    document_version_id uuid,
    status varchar(20) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_decision_record PRIMARY KEY (id),
    CONSTRAINT ck_decision_record_number CHECK (decision_number = btrim(decision_number) AND char_length(decision_number) > 0),
    CONSTRAINT ck_decision_record_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_decision_record_document_version FOREIGN KEY (document_version_id) REFERENCES document.document_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_decision_record_issuer_org_unit FOREIGN KEY (issuer_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT
);

CREATE TABLE document.document_rendition (
    id uuid NOT NULL,
    document_version_id uuid NOT NULL,
    rendition_type varchar(16) NOT NULL,
    file_object_id uuid NOT NULL,
    renderer_name varchar(127) NOT NULL,
    renderer_version varchar(64) NOT NULL,
    template_checksum char(64),
    checksum char(64) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_document_rendition PRIMARY KEY (id),
    CONSTRAINT ck_document_rendition_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_document_rendition_template_checksum CHECK (template_checksum IS NULL OR template_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_document_rendition_type CHECK (rendition_type IN ('SOURCE','DOCX','PDF','XLSX','PREVIEW')),
    CONSTRAINT fk_document_rendition_document_version FOREIGN KEY (document_version_id) REFERENCES document.document_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_document_rendition_file_object FOREIGN KEY (file_object_id) REFERENCES document.file_object (id) ON DELETE RESTRICT
);

CREATE TABLE document.evidence_version (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    evidence_id uuid NOT NULL,
    version_no integer NOT NULL,
    document_version_id uuid,
    external_url varchar(2048),
    url_snapshot_file_object_id uuid,
    system_record_reference jsonb,
    description text,
    collected_at timestamptz NOT NULL,
    checksum char(64) NOT NULL,
    metadata jsonb,
    approved_by uuid,
    approved_at timestamptz,
    created_by uuid NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_evidence_version PRIMARY KEY (id),
    CONSTRAINT ck_evidence_version_approval CHECK ((approved_by IS NULL) = (approved_at IS NULL)),
    CONSTRAINT ck_evidence_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_evidence_version_no CHECK (version_no > 0),
    CONSTRAINT ck_evidence_version_source CHECK (num_nonnulls(document_version_id, external_url, system_record_reference) = 1),
    CONSTRAINT ck_evidence_version_url_snapshot CHECK (url_snapshot_file_object_id IS NULL OR external_url IS NOT NULL),
    CONSTRAINT fk_evidence_version_approver FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_evidence_version_creator FOREIGN KEY (created_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_evidence_version_document_version FOREIGN KEY (document_version_id) REFERENCES document.document_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_evidence_version_evidence FOREIGN KEY (evidence_id) REFERENCES document.evidence (id) ON DELETE RESTRICT,
    CONSTRAINT fk_evidence_version_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_evidence_version_url_snapshot_file FOREIGN KEY (url_snapshot_file_object_id) REFERENCES document.file_object (id) ON DELETE RESTRICT
);

CREATE TABLE integration.inbox_message (
    id uuid NOT NULL,
    source_system_id uuid NOT NULL,
    message_id varchar(255) NOT NULL,
    message_type varchar(128) NOT NULL,
    event_schema_version integer NOT NULL,
    payload jsonb NOT NULL,
    payload_checksum char(64) NOT NULL,
    classification varchar(20) NOT NULL,
    signature_key_version integer NOT NULL,
    signature_valid boolean NOT NULL,
    nonce varchar(255) NOT NULL,
    source_timestamp timestamptz NOT NULL,
    received_at timestamptz NOT NULL,
    processed_at timestamptz,
    status varchar(32) NOT NULL,
    attempt_count integer NOT NULL,
    locked_by uuid,
    locked_until timestamptz,
    error_code varchar(64),
    CONSTRAINT pk_inbox_message PRIMARY KEY (id),
    CONSTRAINT ck_inbox_message_attempt_count CHECK (attempt_count >= 0),
    CONSTRAINT ck_inbox_message_checksum CHECK (payload_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_inbox_message_classification CHECK (classification IN ('PUBLIC', 'INTERNAL', 'CONFIDENTIAL', 'RESTRICTED')),
    CONSTRAINT ck_inbox_message_lock CHECK (num_nonnulls(locked_by, locked_until) IN (0, 2)),
    CONSTRAINT ck_inbox_message_processed_at CHECK (processed_at IS NULL OR processed_at >= received_at),
    CONSTRAINT ck_inbox_message_schema_version CHECK (event_schema_version > 0),
    CONSTRAINT fk_inbox_message_source_system FOREIGN KEY (source_system_id) REFERENCES integration.source_system (id) ON DELETE RESTRICT
);

CREATE TABLE integration.ingestion_batch (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    source_system_id uuid NOT NULL,
    data_type varchar(64) NOT NULL,
    source_batch_id varchar(255),
    idempotency_key varchar(255) NOT NULL,
    schema_version integer NOT NULL,
    payload_checksum char(64) NOT NULL,
    file_object_id uuid,
    classification varchar(20) NOT NULL,
    status varchar(32) NOT NULL,
    received_at timestamptz NOT NULL,
    completed_at timestamptz,
    total_count bigint NOT NULL,
    accepted_count bigint NOT NULL,
    rejected_count bigint NOT NULL,
    CONSTRAINT pk_ingestion_batch PRIMARY KEY (id),
    CONSTRAINT ck_ingestion_batch_classification CHECK (classification IN ('PUBLIC', 'INTERNAL', 'CONFIDENTIAL', 'RESTRICTED')),
    CONSTRAINT ck_ingestion_batch_completion CHECK (completed_at IS NULL OR completed_at >= received_at),
    CONSTRAINT ck_ingestion_batch_counts CHECK (total_count >= 0 AND accepted_count >= 0 AND rejected_count >= 0 AND accepted_count + rejected_count <= total_count),
    CONSTRAINT ck_ingestion_batch_payload_checksum CHECK (payload_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_ingestion_batch_schema_version CHECK (schema_version > 0),
    CONSTRAINT fk_ingestion_batch_file_object FOREIGN KEY (file_object_id) REFERENCES document.file_object (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ingestion_batch_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ingestion_batch_source_system FOREIGN KEY (source_system_id) REFERENCES integration.source_system (id) ON DELETE RESTRICT
);

CREATE TABLE academic.person (
    id uuid NOT NULL,
    source_system_id uuid,
    source_person_id varchar(128),
    full_name varchar(255) NOT NULL,
    contact_ciphertext bytea,
    contact_lookup_hash char(64),
    status varchar(20) NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    CONSTRAINT pk_person PRIMARY KEY (id),
    CONSTRAINT ck_person_contact_hash CHECK (contact_lookup_hash IS NULL OR contact_lookup_hash ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_person_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_person_source_identity CHECK ((source_system_id IS NULL) = (source_person_id IS NULL)),
    CONSTRAINT ck_person_source_person_id CHECK (source_person_id IS NULL OR (source_person_id = btrim(source_person_id) AND char_length(source_person_id) > 0)),
    CONSTRAINT ck_person_status CHECK (status IN ('ACTIVE','INACTIVE','SUSPENDED','EXPIRED')),
    CONSTRAINT fk_person_source_system FOREIGN KEY (source_system_id) REFERENCES integration.source_system (id) ON DELETE RESTRICT
);

CREATE TABLE integration.source_record_map (
    source_system_id uuid NOT NULL,
    entity_type varchar(64) NOT NULL,
    source_record_id varchar(255) NOT NULL,
    target_id uuid NOT NULL,
    source_updated_at timestamptz,
    last_payload_checksum char(64) NOT NULL,
    status varchar(20) NOT NULL,
    updated_at timestamptz NOT NULL,
    CONSTRAINT pk_source_record_map PRIMARY KEY (source_system_id, entity_type, source_record_id),
    CONSTRAINT ck_source_record_map_checksum CHECK (last_payload_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT fk_source_record_map_source_system FOREIGN KEY (source_system_id) REFERENCES integration.source_system (id) ON DELETE RESTRICT
);

CREATE TABLE integration.sync_job (
    id uuid NOT NULL,
    source_system_id uuid NOT NULL,
    data_type varchar(64) NOT NULL,
    mode varchar(32) NOT NULL,
    cursor_from text,
    cursor_to text,
    updated_since timestamptz,
    status varchar(32) NOT NULL,
    started_at timestamptz NOT NULL,
    completed_at timestamptz,
    read_count bigint NOT NULL,
    accepted_count bigint NOT NULL,
    rejected_count bigint NOT NULL,
    error_summary text,
    request_id uuid NOT NULL,
    CONSTRAINT pk_sync_job PRIMARY KEY (id),
    CONSTRAINT ck_sync_job_completion CHECK (completed_at IS NULL OR completed_at >= started_at),
    CONSTRAINT ck_sync_job_counts CHECK (read_count >= 0 AND accepted_count >= 0 AND rejected_count >= 0 AND accepted_count + rejected_count <= read_count),
    CONSTRAINT fk_sync_job_source_system FOREIGN KEY (source_system_id) REFERENCES integration.source_system (id) ON DELETE RESTRICT
);

CREATE TABLE academic.course_version (
    id uuid NOT NULL,
    course_id uuid NOT NULL,
    version_no integer NOT NULL,
    name varchar(255) NOT NULL,
    credit_value numeric(10,2) NOT NULL,
    course_type varchar(20) NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    shared_core_flag boolean NOT NULL,
    status varchar(20) NOT NULL,
    decision_id uuid NOT NULL,
    workflow_instance_id uuid NOT NULL,
    supersedes_id uuid,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_course_version PRIMARY KEY (id),
    CONSTRAINT uq_course_version_id_course UNIQUE (id, course_id),
    CONSTRAINT ck_course_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_course_version_credit CHECK (credit_value > 0 AND credit_value <> 'NaN'::numeric AND credit_value NOT IN ('Infinity'::numeric, '-Infinity'::numeric)),
    CONSTRAINT ck_course_version_no CHECK (version_no > 0),
    CONSTRAINT ck_course_version_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_course_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT ck_course_version_type CHECK (course_type IN ('STANDARD','PRACTICE','INTERNSHIP','PROJECT','THESIS','CLINICAL')),
    CONSTRAINT fk_course_version_course FOREIGN KEY (course_id) REFERENCES academic.course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_version_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_version_supersedes FOREIGN KEY (supersedes_id) REFERENCES academic.course_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_version_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE academic.decision_document (
    decision_record_id uuid NOT NULL,
    document_version_id uuid NOT NULL,
    document_role varchar(32) NOT NULL,
    CONSTRAINT pk_decision_document PRIMARY KEY (decision_record_id, document_version_id, document_role),
    CONSTRAINT fk_decision_document_decision FOREIGN KEY (decision_record_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_decision_document_document_version FOREIGN KEY (document_version_id) REFERENCES document.document_version (id) ON DELETE RESTRICT
);

CREATE TABLE ai.evaluation_policy_version (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    version_no integer NOT NULL,
    metric_definition jsonb NOT NULL,
    threshold_definition jsonb NOT NULL,
    aggregation_rule jsonb NOT NULL,
    sampling_rule jsonb NOT NULL,
    classification varchar(20) NOT NULL,
    status varchar(20) NOT NULL,
    workflow_instance_id uuid NOT NULL,
    decision_id uuid NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_evaluation_policy_version PRIMARY KEY (id),
    CONSTRAINT uq_evaluation_policy_version_id_checksum UNIQUE (id, checksum),
    CONSTRAINT ck_evaluation_policy_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_evaluation_policy_version_classification CHECK (classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')),
    CONSTRAINT ck_evaluation_policy_version_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_evaluation_policy_version_no CHECK (version_no > 0),
    CONSTRAINT ck_evaluation_policy_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_evaluation_policy_version_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_evaluation_policy_version_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_evaluation_policy_version_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE ai.ground_truth_suite_version (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    suite_id uuid NOT NULL,
    version_no integer NOT NULL,
    job_type varchar(24) NOT NULL,
    classification varchar(20) NOT NULL,
    status varchar(20) NOT NULL,
    workflow_instance_id uuid NOT NULL,
    decision_id uuid NOT NULL,
    checksum char(64) NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    CONSTRAINT pk_ground_truth_suite_version PRIMARY KEY (id),
    CONSTRAINT uq_ground_truth_suite_version_id_checksum UNIQUE (id, checksum),
    CONSTRAINT ck_ground_truth_suite_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_ground_truth_suite_version_classification CHECK (classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')),
    CONSTRAINT ck_ground_truth_suite_version_job_type CHECK (job_type IN ('EXTRACT','GENERATE','CHAT','DETECT_ANOMALY')),
    CONSTRAINT ck_ground_truth_suite_version_no CHECK (version_no > 0),
    CONSTRAINT ck_ground_truth_suite_version_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_ground_truth_suite_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_ground_truth_suite_version_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ground_truth_suite_version_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ground_truth_suite_version_suite FOREIGN KEY (suite_id) REFERENCES ai.ground_truth_suite (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ground_truth_suite_version_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE academic.institution_template_version (
    id uuid NOT NULL,
    institution_template_id uuid NOT NULL,
    version_no integer NOT NULL,
    decision_id uuid NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    status varchar(20) NOT NULL,
    layout_configuration jsonb NOT NULL,
    policy_configuration jsonb NOT NULL,
    workflow_instance_id uuid NOT NULL,
    checksum char(64) NOT NULL,
    supersedes_id uuid,
    CONSTRAINT pk_institution_template_version PRIMARY KEY (id),
    CONSTRAINT ck_institution_template_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_institution_template_version_no CHECK (version_no > 0),
    CONSTRAINT ck_institution_template_version_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_institution_template_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_institution_template_version_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_institution_template_version_supersedes FOREIGN KEY (supersedes_id) REFERENCES academic.institution_template_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_institution_template_version_template FOREIGN KEY (institution_template_id) REFERENCES academic.institution_template (id) ON DELETE RESTRICT,
    CONSTRAINT fk_institution_template_version_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE iam.role_version (
    id uuid NOT NULL,
    role_id uuid NOT NULL,
    version_no integer NOT NULL,
    status varchar(32) NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    workflow_instance_id uuid NOT NULL,
    decision_id uuid,
    permission_set_checksum char(64) NOT NULL,
    checksum char(64) NOT NULL,
    created_by uuid NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_role_version PRIMARY KEY (id),
    CONSTRAINT uq_role_version_id_role_id UNIQUE (id, role_id),
    CONSTRAINT ck_role_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_role_version_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_role_version_permission_set_checksum CHECK (permission_set_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_role_version_status CHECK (status IN ('DRAFT', 'IN_REVIEW', 'APPROVED', 'ACTIVE', 'EXPIRED', 'REJECTED')),
    CONSTRAINT ck_role_version_version_no CHECK (version_no > 0),
    CONSTRAINT fk_role_version_created_by FOREIGN KEY (created_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_role_version_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_role_version_role FOREIGN KEY (role_id) REFERENCES iam.role (id) ON DELETE RESTRICT,
    CONSTRAINT fk_role_version_workflow_instance FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE document.evidence_link (
    evidence_version_id uuid NOT NULL,
    resource_type varchar(64) NOT NULL,
    resource_id uuid NOT NULL,
    link_role varchar(32) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_evidence_link PRIMARY KEY (evidence_version_id, resource_type, resource_id, link_role),
    CONSTRAINT fk_evidence_link_evidence_version FOREIGN KEY (evidence_version_id) REFERENCES document.evidence_version (id) ON DELETE RESTRICT
);

CREATE TABLE integration.raw_record (
    id bigint GENERATED BY DEFAULT AS IDENTITY,
    ingestion_batch_id uuid NOT NULL,
    row_no integer NOT NULL,
    source_record_id varchar(255),
    source_updated_at timestamptz,
    payload jsonb NOT NULL,
    payload_checksum char(64) NOT NULL,
    received_at timestamptz NOT NULL,
    CONSTRAINT pk_raw_record PRIMARY KEY (id),
    CONSTRAINT ck_raw_record_payload_checksum CHECK (payload_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_raw_record_row_no CHECK (row_no > 0),
    CONSTRAINT fk_raw_record_ingestion_batch FOREIGN KEY (ingestion_batch_id) REFERENCES integration.ingestion_batch (id) ON DELETE RESTRICT
);

CREATE TABLE governance.privacy_request (
    id uuid NOT NULL,
    subject_person_id uuid NOT NULL,
    request_type varchar(32) NOT NULL,
    legal_basis text NOT NULL,
    status varchar(32) NOT NULL,
    requested_at timestamptz NOT NULL,
    verified_at timestamptz,
    completed_at timestamptz,
    approved_by uuid,
    disposition_case_id uuid,
    CONSTRAINT pk_privacy_request PRIMARY KEY (id),
    CONSTRAINT ck_privacy_request_status CHECK (status IN ('RECEIVED','VERIFYING','VERIFIED','IN_REVIEW','APPROVED','REJECTED','PROCESSING','COMPLETED','CANCELLED')),
    CONSTRAINT ck_privacy_request_timeline CHECK ((verified_at IS NULL OR verified_at >= requested_at) AND (completed_at IS NULL OR completed_at >= requested_at)),
    CONSTRAINT fk_privacy_request_approver FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_privacy_request_disposition_case FOREIGN KEY (disposition_case_id) REFERENCES governance.disposition_case (id) ON DELETE RESTRICT,
    CONSTRAINT fk_privacy_request_subject_person FOREIGN KEY (subject_person_id) REFERENCES academic.person (id) ON DELETE RESTRICT
);

CREATE TABLE academic.staff (
    person_id uuid NOT NULL,
    staff_code varchar(64) NOT NULL,
    home_org_unit_id uuid NOT NULL,
    staff_type varchar(32) NOT NULL,
    current_status varchar(20) NOT NULL,
    CONSTRAINT pk_staff PRIMARY KEY (person_id),
    CONSTRAINT ck_staff_code CHECK (staff_code = upper(btrim(staff_code)) AND char_length(staff_code) > 0),
    CONSTRAINT ck_staff_status CHECK (current_status IN ('ACTIVE','INACTIVE','SUSPENDED','RETIRED','EXPIRED')),
    CONSTRAINT fk_staff_home_org_unit FOREIGN KEY (home_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staff_person FOREIGN KEY (person_id) REFERENCES academic.person (id) ON DELETE RESTRICT
);

CREATE TABLE academic.student (
    person_id uuid NOT NULL,
    student_code varchar(64) NOT NULL,
    admission_cohort_id uuid NOT NULL,
    current_status varchar(20) NOT NULL,
    CONSTRAINT pk_student PRIMARY KEY (person_id),
    CONSTRAINT uq_student_person_cohort UNIQUE (person_id, admission_cohort_id),
    CONSTRAINT ck_student_code CHECK (student_code = upper(btrim(student_code)) AND char_length(student_code) > 0),
    CONSTRAINT ck_student_status CHECK (current_status IN ('ACTIVE','SUSPENDED','GRADUATED','WITHDRAWN','EXPIRED')),
    CONSTRAINT fk_student_admission_cohort FOREIGN KEY (admission_cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_person FOREIGN KEY (person_id) REFERENCES academic.person (id) ON DELETE RESTRICT
);

CREATE TABLE iam.user_account (
    principal_id uuid NOT NULL,
    person_id uuid,
    username citext,
    email_ciphertext bytea,
    email_lookup_hash char(64),
    last_login_at timestamptz,
    CONSTRAINT pk_user_account PRIMARY KEY (principal_id),
    CONSTRAINT ck_user_account_email_lookup_hash CHECK (email_lookup_hash IS NULL OR email_lookup_hash ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_user_account_username CHECK (username IS NULL OR (username = btrim(username::text) AND char_length(username::text) > 0)),
    CONSTRAINT fk_user_account_person FOREIGN KEY (person_id) REFERENCES academic.person (id) ON DELETE RESTRICT,
    CONSTRAINT fk_user_account_principal FOREIGN KEY (principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE integration.sync_cursor (
    source_system_id uuid NOT NULL,
    resource_type varchar(64) NOT NULL,
    cursor_value_ciphertext bytea NOT NULL,
    last_source_updated_at timestamptz,
    last_successful_job_id uuid,
    updated_at timestamptz NOT NULL,
    CONSTRAINT pk_sync_cursor PRIMARY KEY (source_system_id, resource_type),
    CONSTRAINT ck_sync_cursor_resource_type CHECK (resource_type = btrim(resource_type) AND char_length(resource_type) > 0),
    CONSTRAINT fk_sync_cursor_last_job FOREIGN KEY (last_successful_job_id) REFERENCES integration.sync_job (id) ON DELETE RESTRICT,
    CONSTRAINT fk_sync_cursor_source_system FOREIGN KEY (source_system_id) REFERENCES integration.source_system (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.shared_syllabus_core_version (
    id uuid NOT NULL,
    shared_syllabus_core_id uuid NOT NULL,
    course_version_id uuid NOT NULL,
    version_no integer NOT NULL,
    status varchar(20) NOT NULL,
    decision_id uuid,
    workflow_instance_id uuid,
    supersedes_id uuid,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_shared_syllabus_core_version PRIMARY KEY (id),
    CONSTRAINT uq_shared_syllabus_core_version_id_course_version UNIQUE (id, course_version_id),
    CONSTRAINT ck_shared_syllabus_core_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_shared_syllabus_core_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT ck_shared_syllabus_core_version_version_no CHECK (version_no > 0),
    CONSTRAINT fk_shared_syllabus_core_version_core FOREIGN KEY (shared_syllabus_core_id) REFERENCES portfolio.shared_syllabus_core (id) ON DELETE RESTRICT,
    CONSTRAINT fk_shared_syllabus_core_version_course_version FOREIGN KEY (course_version_id) REFERENCES academic.course_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_shared_syllabus_core_version_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_shared_syllabus_core_version_supersedes FOREIGN KEY (supersedes_id) REFERENCES portfolio.shared_syllabus_core_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_shared_syllabus_core_version_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE academic.program_template_section (
    id uuid NOT NULL,
    institution_template_version_id uuid NOT NULL,
    section_code varchar(64) NOT NULL,
    title varchar(255) NOT NULL,
    sort_order integer NOT NULL,
    required boolean NOT NULL,
    lock_mode varchar(16) NOT NULL,
    CONSTRAINT pk_program_template_section PRIMARY KEY (id),
    CONSTRAINT ck_program_template_section_code CHECK (section_code = upper(btrim(section_code)) AND char_length(section_code) > 0),
    CONSTRAINT ck_program_template_section_lock_mode CHECK (lock_mode IN ('LOCKED','OVERRIDABLE','OPEN')),
    CONSTRAINT ck_program_template_section_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_program_template_section_version FOREIGN KEY (institution_template_version_id) REFERENCES academic.institution_template_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.program_version (
    id uuid NOT NULL,
    program_id uuid NOT NULL,
    institution_template_version_id uuid NOT NULL,
    version_no integer NOT NULL,
    code varchar(64) NOT NULL,
    decision_id uuid NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    status varchar(20) NOT NULL,
    total_credits numeric(10,2) NOT NULL,
    workflow_instance_id uuid NOT NULL,
    supersedes_id uuid,
    checksum char(64) NOT NULL,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_program_version PRIMARY KEY (id),
    CONSTRAINT uq_program_version_id_program UNIQUE (id, program_id),
    CONSTRAINT uq_program_version_id_template UNIQUE (id, institution_template_version_id),
    CONSTRAINT ck_program_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_program_version_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_program_version_credits CHECK (total_credits > 0 AND total_credits <> 'NaN'::numeric AND total_credits NOT IN ('Infinity'::numeric, '-Infinity'::numeric)),
    CONSTRAINT ck_program_version_no CHECK (version_no > 0),
    CONSTRAINT ck_program_version_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_program_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_program_version_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_version_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_version_supersedes FOREIGN KEY (supersedes_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_version_template_version FOREIGN KEY (institution_template_version_id) REFERENCES academic.institution_template_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_version_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.syllabus_template_version (
    id uuid NOT NULL,
    syllabus_template_id uuid NOT NULL,
    institution_template_version_id uuid NOT NULL,
    version_no integer NOT NULL,
    decision_id uuid,
    effective_from date NOT NULL,
    effective_to date,
    status varchar(20) NOT NULL,
    workflow_instance_id uuid,
    supersedes_id uuid,
    checksum char(64) NOT NULL,
    created_at timestamptz NOT NULL,
    created_by uuid NOT NULL,
    updated_at timestamptz NOT NULL,
    updated_by uuid NOT NULL,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_syllabus_template_version PRIMARY KEY (id),
    CONSTRAINT uq_syllabus_template_version_id_institution UNIQUE (id, institution_template_version_id),
    CONSTRAINT ck_syllabus_template_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_syllabus_template_version_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_syllabus_template_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT ck_syllabus_template_version_version_no CHECK (version_no > 0),
    CONSTRAINT fk_syllabus_template_version_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_template_version_institution_template_version FOREIGN KEY (institution_template_version_id) REFERENCES academic.institution_template_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_template_version_supersedes FOREIGN KEY (supersedes_id) REFERENCES portfolio.syllabus_template_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_template_version_template FOREIGN KEY (syllabus_template_id) REFERENCES portfolio.syllabus_template (id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_template_version_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE academic.template_plo (
    id uuid NOT NULL,
    institution_template_version_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    description text NOT NULL,
    domain varchar(64) NOT NULL,
    bloom_level varchar(32),
    sort_order integer NOT NULL,
    is_locked boolean NOT NULL,
    CONSTRAINT pk_template_plo PRIMARY KEY (id),
    CONSTRAINT uq_template_plo_id_version UNIQUE (id, institution_template_version_id),
    CONSTRAINT ck_template_plo_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_template_plo_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_template_plo_version FOREIGN KEY (institution_template_version_id) REFERENCES academic.institution_template_version (id) ON DELETE RESTRICT
);

CREATE TABLE iam.role_version_permission (
    role_version_id uuid NOT NULL,
    permission_id uuid NOT NULL,
    granted_at timestamptz NOT NULL,
    granted_by uuid NOT NULL,
    CONSTRAINT pk_role_version_permission PRIMARY KEY (role_version_id, permission_id),
    CONSTRAINT fk_role_version_permission_granted_by FOREIGN KEY (granted_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_role_version_permission_permission FOREIGN KEY (permission_id) REFERENCES iam.permission (id) ON DELETE RESTRICT,
    CONSTRAINT fk_role_version_permission_role_version FOREIGN KEY (role_version_id) REFERENCES iam.role_version (id) ON DELETE RESTRICT
);

CREATE TABLE integration.validation_issue (
    id uuid NOT NULL,
    ingestion_batch_id uuid NOT NULL,
    raw_record_id bigint,
    staging_table varchar(64),
    staging_row_id uuid,
    field_name varchar(128),
    error_code varchar(64) NOT NULL,
    severity varchar(20) NOT NULL,
    message text NOT NULL,
    suggested_action text,
    status varchar(20) NOT NULL,
    resolved_by uuid,
    resolved_at timestamptz,
    CONSTRAINT pk_validation_issue PRIMARY KEY (id),
    CONSTRAINT ck_validation_issue_resolution CHECK ((resolved_by IS NULL AND resolved_at IS NULL) OR (resolved_by IS NOT NULL AND resolved_at IS NOT NULL)),
    CONSTRAINT ck_validation_issue_severity CHECK (severity IN ('INFO', 'WARNING', 'ERROR', 'BLOCKING')),
    CONSTRAINT ck_validation_issue_staging_locator CHECK (num_nonnulls(staging_table, staging_row_id) IN (0, 2)),
    CONSTRAINT fk_validation_issue_ingestion_batch FOREIGN KEY (ingestion_batch_id) REFERENCES integration.ingestion_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_validation_issue_raw_record FOREIGN KEY (raw_record_id) REFERENCES integration.raw_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_validation_issue_resolved_by FOREIGN KEY (resolved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE integration.staging_student (
    id uuid NOT NULL,
    ingestion_batch_id uuid NOT NULL,
    row_no integer NOT NULL,
    raw_record_id bigint NOT NULL,
    student_code varchar(64) NOT NULL,
    full_name varchar(255),
    email varchar(320),
    resolved_student_id uuid,
    validation_status varchar(20) NOT NULL,
    row_checksum char(64) NOT NULL,
    CONSTRAINT pk_staging_student PRIMARY KEY (id),
    CONSTRAINT ck_staging_student_checksum CHECK (row_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_staging_student_code CHECK (student_code = btrim(student_code) AND char_length(student_code) > 0),
    CONSTRAINT ck_staging_student_row_no CHECK (row_no > 0),
    CONSTRAINT ck_staging_student_validation_status CHECK (validation_status IN ('PENDING', 'VALID', 'INVALID', 'PROMOTED')),
    CONSTRAINT fk_staging_student_batch FOREIGN KEY (ingestion_batch_id) REFERENCES integration.ingestion_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_student_raw_record FOREIGN KEY (raw_record_id) REFERENCES integration.raw_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_student_resolved_student FOREIGN KEY (resolved_student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT
);

CREATE TABLE iam.external_identity (
    id uuid NOT NULL,
    user_principal_id uuid NOT NULL,
    identity_provider_id uuid NOT NULL,
    subject varchar(255) NOT NULL,
    claims_snapshot jsonb,
    claims_hash char(64) NOT NULL,
    first_seen_at timestamptz NOT NULL,
    last_seen_at timestamptz NOT NULL,
    CONSTRAINT pk_external_identity PRIMARY KEY (id),
    CONSTRAINT ck_external_identity_claims_hash CHECK (claims_hash ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_external_identity_seen_range CHECK (last_seen_at >= first_seen_at),
    CONSTRAINT ck_external_identity_subject CHECK (subject = btrim(subject) AND char_length(subject) > 0),
    CONSTRAINT fk_external_identity_identity_provider FOREIGN KEY (identity_provider_id) REFERENCES iam.identity_provider (id) ON DELETE RESTRICT,
    CONSTRAINT fk_external_identity_user_account FOREIGN KEY (user_principal_id) REFERENCES iam.user_account (principal_id) ON DELETE RESTRICT
);

CREATE TABLE academic.program_template_field (
    id uuid NOT NULL,
    program_template_section_id uuid NOT NULL,
    field_code varchar(64) NOT NULL,
    label varchar(255) NOT NULL,
    data_type varchar(32) NOT NULL,
    required boolean NOT NULL,
    lock_mode varchar(16) NOT NULL,
    default_value jsonb,
    validation_schema jsonb,
    sort_order integer NOT NULL,
    CONSTRAINT pk_program_template_field PRIMARY KEY (id),
    CONSTRAINT ck_program_template_field_code CHECK (field_code = upper(btrim(field_code)) AND char_length(field_code) > 0),
    CONSTRAINT ck_program_template_field_lock_mode CHECK (lock_mode IN ('LOCKED','OVERRIDABLE','OPEN')),
    CONSTRAINT ck_program_template_field_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_program_template_field_section FOREIGN KEY (program_template_section_id) REFERENCES academic.program_template_section (id) ON DELETE RESTRICT
);

CREATE TABLE academic.competency (
    id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    parent_id uuid,
    level_no integer NOT NULL,
    code varchar(64) NOT NULL,
    description text NOT NULL,
    sort_order integer NOT NULL,
    CONSTRAINT pk_competency PRIMARY KEY (id),
    CONSTRAINT uq_competency_id_version UNIQUE (id, program_version_id),
    CONSTRAINT ck_competency_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_competency_level CHECK (level_no BETWEEN 1 AND 3),
    CONSTRAINT ck_competency_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_competency_parent_version FOREIGN KEY (parent_id, program_version_id) REFERENCES academic.competency (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_competency_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.course_version_relation (
    id uuid NOT NULL,
    from_course_version_id uuid NOT NULL,
    to_course_version_id uuid NOT NULL,
    program_version_id uuid,
    relation_type varchar(20) NOT NULL,
    decision_id uuid NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    status varchar(20) NOT NULL,
    rationale text,
    CONSTRAINT pk_course_version_relation PRIMARY KEY (id),
    CONSTRAINT ck_course_version_relation_distinct CHECK (from_course_version_id <> to_course_version_id),
    CONSTRAINT ck_course_version_relation_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_course_version_relation_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT ck_course_version_relation_type CHECK (relation_type IN ('EQUIVALENT','SUBSTITUTE','REPLACES','RECOGNIZED_AS')),
    CONSTRAINT fk_course_version_relation_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_version_relation_from FOREIGN KEY (from_course_version_id) REFERENCES academic.course_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_version_relation_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_version_relation_to FOREIGN KEY (to_course_version_id) REFERENCES academic.course_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.curriculum_path (
    id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    path_type varchar(24) NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    is_default boolean NOT NULL,
    workflow_instance_id uuid NOT NULL,
    CONSTRAINT pk_curriculum_path PRIMARY KEY (id),
    CONSTRAINT uq_curriculum_path_id_version UNIQUE (id, program_version_id),
    CONSTRAINT ck_curriculum_path_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_curriculum_path_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_curriculum_path_type CHECK (path_type IN ('COMMON','MAJOR','SPECIALIZATION','ELECTIVE_ROUTE','GRADUATION_OPTION')),
    CONSTRAINT fk_curriculum_path_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_curriculum_path_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE academic.curriculum_plan (
    id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    declared_total_credits numeric(10,2) NOT NULL,
    status varchar(20) NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_curriculum_plan PRIMARY KEY (id),
    CONSTRAINT uq_curriculum_plan_id_version UNIQUE (id, program_version_id),
    CONSTRAINT ck_curriculum_plan_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_curriculum_plan_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_curriculum_plan_credits CHECK (declared_total_credits > 0 AND declared_total_credits <> 'NaN'::numeric AND declared_total_credits NOT IN ('Infinity'::numeric, '-Infinity'::numeric)),
    CONSTRAINT ck_curriculum_plan_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_curriculum_plan_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE quality.improvement_plan (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    org_unit_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    title varchar(255) NOT NULL,
    problem_statement text NOT NULL,
    root_cause_summary text,
    baseline_value numeric(20,10),
    target_value numeric(20,10),
    kpi_definition text NOT NULL,
    owner_principal_id uuid NOT NULL,
    due_date date NOT NULL,
    workflow_instance_id uuid NOT NULL,
    status varchar(20) NOT NULL,
    created_by uuid NOT NULL,
    created_at timestamptz NOT NULL,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_improvement_plan PRIMARY KEY (id),
    CONSTRAINT ck_improvement_plan_code CHECK (code = btrim(code) AND char_length(code) > 0),
    CONSTRAINT ck_improvement_plan_row_version CHECK (row_version > 0),
    CONSTRAINT ck_improvement_plan_values CHECK ((baseline_value IS NULL OR baseline_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)) AND (target_value IS NULL OR target_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric))),
    CONSTRAINT fk_improvement_plan_created_by FOREIGN KEY (created_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_improvement_plan_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_improvement_plan_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_improvement_plan_owner FOREIGN KEY (owner_principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_improvement_plan_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_improvement_plan_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE academic.program_objective (
    id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    description text NOT NULL,
    sort_order integer NOT NULL,
    CONSTRAINT pk_program_objective PRIMARY KEY (id),
    CONSTRAINT uq_program_objective_id_version UNIQUE (id, program_version_id),
    CONSTRAINT ck_program_objective_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_program_objective_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_program_objective_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.program_policy_binding (
    id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    policy_version_id uuid NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    status varchar(20) NOT NULL,
    decision_id uuid NOT NULL,
    workflow_instance_id uuid NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_program_policy_binding PRIMARY KEY (id),
    CONSTRAINT "AK_program_policy_binding_id_program_version_id" UNIQUE (id, program_version_id),
    CONSTRAINT "AK_program_policy_binding_id_program_version_id_policy_version~" UNIQUE (id, program_version_id, policy_version_id),
    CONSTRAINT ck_program_policy_binding_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_program_policy_binding_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_program_policy_binding_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_program_policy_binding_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_policy_binding_policy_version FOREIGN KEY (policy_version_id) REFERENCES measurement.calculation_policy_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_policy_binding_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_policy_binding_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE academic.program_version_cohort (
    program_version_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    is_default boolean NOT NULL,
    CONSTRAINT pk_program_version_cohort PRIMARY KEY (program_version_id, cohort_id),
    CONSTRAINT ck_program_version_cohort_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT fk_program_version_cohort_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_version_cohort_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.program_version_crosswalk (
    id uuid NOT NULL,
    from_program_version_id uuid NOT NULL,
    to_program_version_id uuid NOT NULL,
    status varchar(20) NOT NULL,
    decision_id uuid NOT NULL,
    rationale text,
    CONSTRAINT pk_program_version_crosswalk PRIMARY KEY (id),
    CONSTRAINT ck_program_version_crosswalk_distinct CHECK (from_program_version_id <> to_program_version_id),
    CONSTRAINT ck_program_version_crosswalk_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_program_version_crosswalk_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_version_crosswalk_from FOREIGN KEY (from_program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_version_crosswalk_to FOREIGN KEY (to_program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.syllabus_template_rubric_scale (
    id uuid NOT NULL,
    syllabus_template_version_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    CONSTRAINT pk_syllabus_template_rubric_scale PRIMARY KEY (id),
    CONSTRAINT uq_syllabus_template_rubric_scale_id_version UNIQUE (id, syllabus_template_version_id),
    CONSTRAINT ck_syllabus_template_rubric_scale_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT fk_syllabus_template_rubric_scale_version FOREIGN KEY (syllabus_template_version_id) REFERENCES portfolio.syllabus_template_version (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.syllabus_template_section (
    id uuid NOT NULL,
    syllabus_template_version_id uuid NOT NULL,
    section_code varchar(64) NOT NULL,
    title varchar(255) NOT NULL,
    sort_order integer NOT NULL,
    required boolean NOT NULL,
    content_type varchar(32) NOT NULL,
    locked boolean NOT NULL,
    CONSTRAINT pk_syllabus_template_section PRIMARY KEY (id),
    CONSTRAINT uq_syllabus_template_section_id_version UNIQUE (id, syllabus_template_version_id),
    CONSTRAINT ck_syllabus_template_section_code CHECK (section_code = upper(btrim(section_code)) AND char_length(section_code) > 0),
    CONSTRAINT ck_syllabus_template_section_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_syllabus_template_section_version FOREIGN KEY (syllabus_template_version_id) REFERENCES portfolio.syllabus_template_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.program_plo (
    id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    description text NOT NULL,
    domain varchar(64) NOT NULL,
    bloom_level varchar(32),
    source_template_plo_id uuid,
    is_locked boolean NOT NULL,
    sort_order integer NOT NULL,
    CONSTRAINT pk_program_plo PRIMARY KEY (id),
    CONSTRAINT uq_program_plo_id_version UNIQUE (id, program_version_id),
    CONSTRAINT ck_program_plo_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_program_plo_sort_order CHECK (sort_order >= 0),
    CONSTRAINT ck_program_plo_source_lock CHECK (source_template_plo_id IS NULL OR is_locked),
    CONSTRAINT fk_program_plo_source_template FOREIGN KEY (source_template_plo_id) REFERENCES academic.template_plo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_plo_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.template_pi (
    id uuid NOT NULL,
    institution_template_version_id uuid NOT NULL,
    template_plo_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    description text NOT NULL,
    sort_order integer NOT NULL,
    is_locked boolean NOT NULL,
    is_core boolean NOT NULL,
    CONSTRAINT pk_template_pi PRIMARY KEY (id),
    CONSTRAINT uq_template_pi_id_version UNIQUE (id, institution_template_version_id),
    CONSTRAINT ck_template_pi_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_template_pi_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_template_pi_plo_version FOREIGN KEY (template_plo_id, institution_template_version_id) REFERENCES academic.template_plo (id, institution_template_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_template_pi_version FOREIGN KEY (institution_template_version_id) REFERENCES academic.institution_template_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.student_path (
    id uuid NOT NULL,
    student_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    path_status varchar(20) NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    decision_id uuid,
    is_primary boolean NOT NULL,
    CONSTRAINT pk_student_path PRIMARY KEY (id),
    CONSTRAINT uq_student_path_id_student UNIQUE (id, student_id),
    CONSTRAINT uq_student_path_id_student_curriculum_path UNIQUE (id, student_id, curriculum_path_id),
    CONSTRAINT uq_student_path_population_binding UNIQUE (id, student_id, program_version_id, curriculum_path_id),
    CONSTRAINT ck_student_path_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_student_path_status CHECK (path_status IN ('ACTIVE','SUSPENDED','COMPLETED','TRANSFERRED','WITHDRAWN','EXPIRED')),
    CONSTRAINT fk_student_path_curriculum_path_version FOREIGN KEY (curriculum_path_id, program_version_id) REFERENCES academic.curriculum_path (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_path_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_path_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_path_program_version_program FOREIGN KEY (program_version_id, program_id) REFERENCES academic.program_version (id, program_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_path_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT
);

CREATE TABLE academic.curriculum_block (
    id uuid NOT NULL,
    curriculum_plan_id uuid NOT NULL,
    parent_id uuid,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    block_type varchar(32) NOT NULL,
    required_credits numeric(10,2) NOT NULL,
    maximum_credits numeric(10,2),
    sort_order integer NOT NULL,
    CONSTRAINT pk_curriculum_block PRIMARY KEY (id),
    CONSTRAINT uq_curriculum_block_id_plan UNIQUE (id, curriculum_plan_id),
    CONSTRAINT ck_curriculum_block_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_curriculum_block_credits CHECK (required_credits >= 0 AND (maximum_credits IS NULL OR maximum_credits >= required_credits)),
    CONSTRAINT ck_curriculum_block_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_curriculum_block_parent_plan FOREIGN KEY (parent_id, curriculum_plan_id) REFERENCES academic.curriculum_block (id, curriculum_plan_id) ON DELETE RESTRICT,
    CONSTRAINT fk_curriculum_block_plan FOREIGN KEY (curriculum_plan_id) REFERENCES academic.curriculum_plan (id) ON DELETE RESTRICT
);

CREATE TABLE quality.improvement_action (
    id uuid NOT NULL,
    improvement_plan_id uuid NOT NULL,
    action_no integer NOT NULL,
    description text NOT NULL,
    owner_principal_id uuid NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    start_date date NOT NULL,
    due_date date NOT NULL,
    status varchar(20) NOT NULL,
    completion_ratio numeric(12,10) NOT NULL,
    completed_at timestamptz,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_improvement_action PRIMARY KEY (id),
    CONSTRAINT uq_improvement_action_id_plan UNIQUE (id, improvement_plan_id),
    CONSTRAINT ck_improvement_action_completion CHECK (completion_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND completion_ratio >= 0 AND completion_ratio <= 1),
    CONSTRAINT ck_improvement_action_dates CHECK (due_date >= start_date),
    CONSTRAINT ck_improvement_action_number CHECK (action_no > 0),
    CONSTRAINT ck_improvement_action_row_version CHECK (row_version > 0),
    CONSTRAINT fk_improvement_action_owner FOREIGN KEY (owner_principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_improvement_action_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_improvement_action_plan FOREIGN KEY (improvement_plan_id) REFERENCES quality.improvement_plan (id) ON DELETE RESTRICT
);

CREATE TABLE quality.improvement_document (
    improvement_plan_id uuid NOT NULL,
    document_version_id uuid NOT NULL,
    document_role varchar(32) NOT NULL,
    CONSTRAINT pk_improvement_document PRIMARY KEY (improvement_plan_id, document_version_id, document_role),
    CONSTRAINT fk_improvement_document_document_version FOREIGN KEY (document_version_id) REFERENCES document.document_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_improvement_document_plan FOREIGN KEY (improvement_plan_id) REFERENCES quality.improvement_plan (id) ON DELETE RESTRICT
);

CREATE TABLE academic.po_competency_mapping (
    program_objective_id uuid NOT NULL,
    competency_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    mapping_level char(1) NOT NULL,
    rationale text,
    CONSTRAINT pk_po_competency_mapping PRIMARY KEY (program_objective_id, competency_id),
    CONSTRAINT ck_po_competency_mapping_level CHECK (mapping_level IN ('L','M','H')),
    CONSTRAINT fk_po_competency_mapping_competency_version FOREIGN KEY (competency_id, program_version_id) REFERENCES academic.competency (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_po_competency_mapping_objective_version FOREIGN KEY (program_objective_id, program_version_id) REFERENCES academic.program_objective (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_po_competency_mapping_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.measurement_period (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    org_unit_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    academic_year_start smallint NOT NULL,
    term_code varchar(32) NOT NULL,
    status varchar(20) NOT NULL,
    program_policy_binding_id uuid NOT NULL,
    workflow_instance_id uuid NOT NULL,
    collection_open_at timestamptz,
    collection_close_at timestamptz,
    data_cutoff_at timestamptz,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_measurement_period PRIMARY KEY (id),
    CONSTRAINT "AK_measurement_period_id_org_unit_id_program_version_id_academ~" UNIQUE (id, org_unit_id, program_version_id, academic_year_start),
    CONSTRAINT "AK_measurement_period_id_program_version_id" UNIQUE (id, program_version_id),
    CONSTRAINT "AK_measurement_period_id_program_version_id_academic_year_start" UNIQUE (id, program_version_id, academic_year_start),
    CONSTRAINT ck_measurement_period_academic_year CHECK (academic_year_start BETWEEN 1900 AND 9999),
    CONSTRAINT ck_measurement_period_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_measurement_period_collection_window CHECK (collection_close_at IS NULL OR collection_open_at IS NOT NULL AND collection_close_at > collection_open_at),
    CONSTRAINT ck_measurement_period_status CHECK (status IN ('DRAFT','OPEN','COLLECTING','RECONCILING','CALCULATED','APPROVED','PUBLISHED','CLOSED','REOPENED')),
    CONSTRAINT fk_measurement_period_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_period_policy_binding FOREIGN KEY (program_policy_binding_id, program_version_id) REFERENCES measurement.program_policy_binding (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_period_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_period_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.syllabus_template_rubric_scale_level (
    id uuid NOT NULL,
    rubric_scale_id uuid NOT NULL,
    level_code varchar(64) NOT NULL,
    label varchar(255) NOT NULL,
    level_order integer NOT NULL,
    score_from numeric(20,10) NOT NULL,
    score_to numeric(20,10) NOT NULL,
    numeric_value numeric(20,10),
    CONSTRAINT pk_syllabus_template_rubric_scale_level PRIMARY KEY (id),
    CONSTRAINT ck_syllabus_template_rubric_scale_level_order CHECK (level_order >= 0),
    CONSTRAINT ck_syllabus_template_rubric_scale_level_range CHECK (score_from < score_to),
    CONSTRAINT fk_syllabus_template_rubric_scale_level_scale FOREIGN KEY (rubric_scale_id) REFERENCES portfolio.syllabus_template_rubric_scale (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.syllabus_template_field (
    id uuid NOT NULL,
    syllabus_template_section_id uuid NOT NULL,
    syllabus_template_version_id uuid NOT NULL,
    field_code varchar(64) NOT NULL,
    label varchar(255) NOT NULL,
    data_type varchar(32) NOT NULL,
    required boolean NOT NULL,
    lock_mode varchar(16) NOT NULL,
    default_value jsonb,
    validation_schema jsonb,
    sort_order integer NOT NULL,
    CONSTRAINT pk_syllabus_template_field PRIMARY KEY (id),
    CONSTRAINT uq_syllabus_template_field_id_version UNIQUE (id, syllabus_template_version_id),
    CONSTRAINT ck_syllabus_template_field_code CHECK (field_code = upper(btrim(field_code)) AND char_length(field_code) > 0),
    CONSTRAINT ck_syllabus_template_field_lock_mode CHECK (lock_mode IN ('LOCKED','OVERRIDABLE','OPEN')),
    CONSTRAINT ck_syllabus_template_field_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_syllabus_template_field_section_version FOREIGN KEY (syllabus_template_section_id, syllabus_template_version_id) REFERENCES portfolio.syllabus_template_section (id, syllabus_template_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_template_field_version FOREIGN KEY (syllabus_template_version_id) REFERENCES portfolio.syllabus_template_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.competency_plo_mapping (
    competency_id uuid NOT NULL,
    program_plo_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    mapping_level char(1) NOT NULL,
    rationale text,
    CONSTRAINT pk_competency_plo_mapping PRIMARY KEY (competency_id, program_plo_id),
    CONSTRAINT ck_competency_plo_mapping_level CHECK (mapping_level IN ('L','M','H')),
    CONSTRAINT fk_competency_plo_mapping_competency_version FOREIGN KEY (competency_id, program_version_id) REFERENCES academic.competency (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_competency_plo_mapping_plo_version FOREIGN KEY (program_plo_id, program_version_id) REFERENCES academic.program_plo (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_competency_plo_mapping_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.plo_crosswalk (
    id uuid NOT NULL,
    program_version_crosswalk_id uuid NOT NULL,
    from_program_plo_id uuid NOT NULL,
    to_program_plo_id uuid,
    relation_type varchar(20) NOT NULL,
    allocation_ratio numeric(12,10),
    rationale text,
    CONSTRAINT pk_plo_crosswalk PRIMARY KEY (id),
    CONSTRAINT ck_plo_crosswalk_ratio CHECK (allocation_ratio IS NULL OR (allocation_ratio >= 0 AND allocation_ratio <= 1 AND allocation_ratio <> 'NaN'::numeric AND allocation_ratio NOT IN ('Infinity'::numeric, '-Infinity'::numeric))),
    CONSTRAINT ck_plo_crosswalk_relation CHECK (relation_type IN ('EQUIVALENT','REPLACED_BY','SPLIT_TO','MERGED_INTO','NO_EQUIVALENT')),
    CONSTRAINT ck_plo_crosswalk_target CHECK ((relation_type = 'NO_EQUIVALENT' AND to_program_plo_id IS NULL) OR (relation_type <> 'NO_EQUIVALENT' AND to_program_plo_id IS NOT NULL)),
    CONSTRAINT fk_plo_crosswalk_from FOREIGN KEY (from_program_plo_id) REFERENCES academic.program_plo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_plo_crosswalk_header FOREIGN KEY (program_version_crosswalk_id) REFERENCES academic.program_version_crosswalk (id) ON DELETE RESTRICT,
    CONSTRAINT fk_plo_crosswalk_to FOREIGN KEY (to_program_plo_id) REFERENCES academic.program_plo (id) ON DELETE RESTRICT
);

CREATE TABLE academic.po_plo_mapping (
    program_objective_id uuid NOT NULL,
    program_plo_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    mapping_level char(1) NOT NULL,
    rationale text,
    CONSTRAINT pk_po_plo_mapping PRIMARY KEY (program_objective_id, program_plo_id),
    CONSTRAINT ck_po_plo_mapping_level CHECK (mapping_level IN ('L','M','H')),
    CONSTRAINT fk_po_plo_mapping_objective_version FOREIGN KEY (program_objective_id, program_version_id) REFERENCES academic.program_objective (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_po_plo_mapping_plo_version FOREIGN KEY (program_plo_id, program_version_id) REFERENCES academic.program_plo (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_po_plo_mapping_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.program_pi (
    id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    program_plo_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    description text NOT NULL,
    source_template_pi_id uuid,
    is_locked boolean NOT NULL,
    is_core boolean NOT NULL,
    weight_ratio numeric(12,10),
    sort_order integer NOT NULL,
    CONSTRAINT pk_program_pi PRIMARY KEY (id),
    CONSTRAINT uq_program_pi_id_version UNIQUE (id, program_version_id),
    CONSTRAINT ck_program_pi_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_program_pi_sort_order CHECK (sort_order >= 0),
    CONSTRAINT ck_program_pi_source_lock CHECK (source_template_pi_id IS NULL OR is_locked),
    CONSTRAINT ck_program_pi_weight CHECK (weight_ratio IS NULL OR (weight_ratio >= 0 AND weight_ratio <= 1 AND weight_ratio <> 'NaN'::numeric AND weight_ratio NOT IN ('Infinity'::numeric, '-Infinity'::numeric))),
    CONSTRAINT fk_program_pi_plo_version FOREIGN KEY (program_plo_id, program_version_id) REFERENCES academic.program_plo (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_pi_source_template FOREIGN KEY (source_template_pi_id) REFERENCES academic.template_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_pi_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.shared_course_pi_mapping (
    id uuid NOT NULL,
    course_version_id uuid NOT NULL,
    institution_template_version_id uuid NOT NULL,
    template_pi_id uuid NOT NULL,
    version_no integer NOT NULL,
    contribution_level char(1) NOT NULL,
    is_direct_assessment boolean NOT NULL,
    status varchar(20) NOT NULL,
    decision_id uuid NOT NULL,
    workflow_instance_id uuid NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_shared_course_pi_mapping PRIMARY KEY (id),
    CONSTRAINT ck_shared_course_pi_mapping_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_shared_course_pi_mapping_contribution CHECK (contribution_level IN ('I','R','M')),
    CONSTRAINT ck_shared_course_pi_mapping_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT ck_shared_course_pi_mapping_version CHECK (version_no > 0),
    CONSTRAINT fk_shared_course_pi_mapping_course_version FOREIGN KEY (course_version_id) REFERENCES academic.course_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_shared_course_pi_mapping_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_shared_course_pi_mapping_pi_version FOREIGN KEY (template_pi_id, institution_template_version_id) REFERENCES academic.template_pi (id, institution_template_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_shared_course_pi_mapping_template_version FOREIGN KEY (institution_template_version_id) REFERENCES academic.institution_template_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_shared_course_pi_mapping_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE academic.curriculum_elective_group (
    id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    curriculum_block_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    minimum_course_count integer NOT NULL,
    maximum_course_count integer,
    minimum_credits numeric(10,2) NOT NULL,
    maximum_credits numeric(10,2),
    CONSTRAINT pk_curriculum_elective_group PRIMARY KEY (id),
    CONSTRAINT ck_curriculum_elective_group_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_curriculum_elective_group_count CHECK (minimum_course_count >= 0 AND (maximum_course_count IS NULL OR maximum_course_count >= minimum_course_count)),
    CONSTRAINT ck_curriculum_elective_group_credits CHECK (minimum_credits >= 0 AND (maximum_credits IS NULL OR maximum_credits >= minimum_credits)),
    CONSTRAINT fk_curriculum_elective_group_block FOREIGN KEY (curriculum_block_id) REFERENCES academic.curriculum_block (id) ON DELETE RESTRICT,
    CONSTRAINT fk_curriculum_elective_group_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT
);

CREATE TABLE academic.program_course (
    id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    course_version_id uuid NOT NULL,
    curriculum_block_id uuid NOT NULL,
    catalog_role varchar(20) NOT NULL,
    credit_override numeric(10,2),
    is_locked boolean NOT NULL,
    status varchar(20) NOT NULL,
    CONSTRAINT pk_program_course PRIMARY KEY (id),
    CONSTRAINT uq_program_course_id_version UNIQUE (id, program_version_id),
    CONSTRAINT uq_program_course_id_version_course_version UNIQUE (id, program_version_id, course_version_id),
    CONSTRAINT ck_program_course_catalog_role CHECK (catalog_role IN ('REQUIRED','ELECTIVE','ORIENTATION','GRADUATION')),
    CONSTRAINT ck_program_course_credit_override CHECK (credit_override IS NULL OR (credit_override > 0 AND credit_override <> 'NaN'::numeric AND credit_override NOT IN ('Infinity'::numeric, '-Infinity'::numeric))),
    CONSTRAINT ck_program_course_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_program_course_course_version FOREIGN KEY (course_version_id) REFERENCES academic.course_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_course_curriculum_block FOREIGN KEY (curriculum_block_id) REFERENCES academic.curriculum_block (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_course_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE quality.improvement_evidence (
    id uuid NOT NULL,
    improvement_plan_id uuid NOT NULL,
    improvement_action_id uuid,
    evidence_version_id uuid NOT NULL,
    link_role varchar(32) NOT NULL,
    verified_by uuid,
    verified_at timestamptz,
    CONSTRAINT pk_improvement_evidence PRIMARY KEY (id),
    CONSTRAINT ck_improvement_evidence_verification CHECK (num_nonnulls(verified_by, verified_at) IN (0, 2)),
    CONSTRAINT fk_improvement_evidence_action_plan FOREIGN KEY (improvement_action_id, improvement_plan_id) REFERENCES quality.improvement_action (id, improvement_plan_id) ON DELETE RESTRICT,
    CONSTRAINT fk_improvement_evidence_evidence_version FOREIGN KEY (evidence_version_id) REFERENCES document.evidence_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_improvement_evidence_plan FOREIGN KEY (improvement_plan_id) REFERENCES quality.improvement_plan (id) ON DELETE RESTRICT,
    CONSTRAINT fk_improvement_evidence_verified_by FOREIGN KEY (verified_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.indirect_response_batch (
    id uuid NOT NULL,
    instrument_version_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    status varchar(20) NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_indirect_response_batch PRIMARY KEY (id),
    CONSTRAINT "AK_indirect_response_batch_id_instrument_version_id_program_ve~" UNIQUE (id, instrument_version_id, program_version_id),
    CONSTRAINT ck_indirect_response_batch_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT fk_indirect_response_batch_instrument_version FOREIGN KEY (instrument_version_id) REFERENCES measurement.indirect_instrument_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_indirect_response_batch_period_program FOREIGN KEY (measurement_period_id, program_version_id) REFERENCES measurement.measurement_period (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_indirect_response_batch_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.input_snapshot (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    snapshot_no integer NOT NULL,
    policy_version_id uuid NOT NULL,
    program_policy_binding_id uuid NOT NULL,
    institution_template_version_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    academic_year_start smallint NOT NULL,
    status varchar(20) NOT NULL,
    schema_version varchar(32) NOT NULL,
    hash_algorithm varchar(16) NOT NULL DEFAULT 'SHA-256',
    manifest_checksum char(64),
    population_count bigint NOT NULL,
    score_count bigint NOT NULL,
    parent_snapshot_id uuid,
    created_by uuid NOT NULL,
    created_at timestamptz NOT NULL,
    sealed_by uuid,
    sealed_at timestamptz,
    CONSTRAINT pk_input_snapshot PRIMARY KEY (id),
    CONSTRAINT uq_input_snapshot_3 UNIQUE (id, measurement_period_id),
    CONSTRAINT uq_input_snapshot_4 UNIQUE (id, measurement_period_id, policy_version_id, program_policy_binding_id, org_unit_id, program_version_id, academic_year_start),
    CONSTRAINT ck_input_snapshot_counts CHECK (population_count >= 0 AND score_count >= 0),
    CONSTRAINT ck_input_snapshot_hash_algorithm CHECK (hash_algorithm = 'SHA-256'),
    CONSTRAINT ck_input_snapshot_manifest CHECK ((status <> 'SEALED' OR (manifest_checksum IS NOT NULL AND manifest_checksum ~ '^[0-9a-f]{64}$' AND sealed_by IS NOT NULL AND sealed_at IS NOT NULL)) AND (manifest_checksum IS NULL OR manifest_checksum ~ '^[0-9a-f]{64}$') AND ((sealed_by IS NULL) = (sealed_at IS NULL))),
    CONSTRAINT ck_input_snapshot_no CHECK (snapshot_no > 0),
    CONSTRAINT ck_input_snapshot_status CHECK (status IN ('BUILDING','SEALED','VOID')),
    CONSTRAINT fk_input_snapshot_creator FOREIGN KEY (created_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_input_snapshot_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_input_snapshot_institution_template FOREIGN KEY (institution_template_version_id) REFERENCES academic.institution_template_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_input_snapshot_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_input_snapshot_parent_period FOREIGN KEY (parent_snapshot_id, measurement_period_id) REFERENCES measurement.input_snapshot (id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_input_snapshot_period_scope FOREIGN KEY (measurement_period_id, org_unit_id, program_version_id, academic_year_start) REFERENCES measurement.measurement_period (id, org_unit_id, program_version_id, academic_year_start) ON DELETE RESTRICT,
    CONSTRAINT fk_input_snapshot_policy_binding FOREIGN KEY (program_policy_binding_id, program_version_id, policy_version_id) REFERENCES measurement.program_policy_binding (id, program_version_id, policy_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_input_snapshot_policy_version FOREIGN KEY (policy_version_id) REFERENCES measurement.calculation_policy_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_input_snapshot_program_template FOREIGN KEY (program_version_id, institution_template_version_id) REFERENCES academic.program_version (id, institution_template_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_input_snapshot_sealer FOREIGN KEY (sealed_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.measurement_period_cohort (
    measurement_period_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    CONSTRAINT pk_measurement_period_cohort PRIMARY KEY (measurement_period_id, cohort_id),
    CONSTRAINT "AK_measurement_period_cohort_measurement_period_id_program_ver~" UNIQUE (measurement_period_id, program_version_id, cohort_id),
    CONSTRAINT fk_measurement_period_cohort_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_period_cohort_period_program FOREIGN KEY (measurement_period_id, program_version_id) REFERENCES measurement.measurement_period (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_period_cohort_program_cohort FOREIGN KEY (program_version_id, cohort_id) REFERENCES academic.program_version_cohort (program_version_id, cohort_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_period_cohort_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.direct_measurement_plan (
    id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    program_pi_id uuid NOT NULL,
    version_no integer NOT NULL,
    status varchar(20) NOT NULL,
    workflow_instance_id uuid NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    supersedes_id uuid,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_direct_measurement_plan PRIMARY KEY (id),
    CONSTRAINT uq_direct_measurement_plan_binding UNIQUE (id, program_version_id, curriculum_path_id, program_pi_id),
    CONSTRAINT ck_direct_measurement_plan_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_direct_measurement_plan_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_direct_measurement_plan_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT ck_direct_measurement_plan_version CHECK (version_no > 0),
    CONSTRAINT fk_direct_measurement_plan_path_version FOREIGN KEY (curriculum_path_id, program_version_id) REFERENCES academic.curriculum_path (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_direct_measurement_plan_pi_version FOREIGN KEY (program_pi_id, program_version_id) REFERENCES academic.program_pi (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_direct_measurement_plan_supersedes FOREIGN KEY (supersedes_id) REFERENCES academic.direct_measurement_plan (id) ON DELETE RESTRICT,
    CONSTRAINT fk_direct_measurement_plan_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_direct_measurement_plan_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.indirect_item (
    id uuid NOT NULL,
    instrument_version_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    prompt text NOT NULL,
    program_pi_id uuid,
    program_plo_id uuid,
    weight_ratio numeric(12,10) NOT NULL,
    CONSTRAINT pk_indirect_item PRIMARY KEY (id),
    CONSTRAINT "AK_indirect_item_id_instrument_version_id_program_version_id" UNIQUE (id, instrument_version_id, program_version_id),
    CONSTRAINT ck_indirect_item_outcome CHECK (num_nonnulls(program_pi_id, program_plo_id) = 1),
    CONSTRAINT ck_indirect_item_outcome_level_binding CHECK ((program_pi_id IS NOT NULL AND program_plo_id IS NULL) OR (program_pi_id IS NULL AND program_plo_id IS NOT NULL)),
    CONSTRAINT ck_indirect_item_weight CHECK (weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND weight_ratio > 0 AND weight_ratio <= 1),
    CONSTRAINT fk_indirect_item_instrument_version FOREIGN KEY (instrument_version_id) REFERENCES measurement.indirect_instrument_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_indirect_item_program_pi FOREIGN KEY (program_pi_id, program_version_id) REFERENCES academic.program_pi (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_indirect_item_program_plo FOREIGN KEY (program_plo_id, program_version_id) REFERENCES academic.program_plo (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_indirect_item_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.pi_crosswalk (
    id uuid NOT NULL,
    program_version_crosswalk_id uuid NOT NULL,
    from_program_pi_id uuid NOT NULL,
    to_program_pi_id uuid,
    relation_type varchar(20) NOT NULL,
    allocation_ratio numeric(12,10),
    rationale text,
    CONSTRAINT pk_pi_crosswalk PRIMARY KEY (id),
    CONSTRAINT ck_pi_crosswalk_ratio CHECK (allocation_ratio IS NULL OR (allocation_ratio >= 0 AND allocation_ratio <= 1 AND allocation_ratio <> 'NaN'::numeric AND allocation_ratio NOT IN ('Infinity'::numeric, '-Infinity'::numeric))),
    CONSTRAINT ck_pi_crosswalk_relation CHECK (relation_type IN ('EQUIVALENT','REPLACED_BY','SPLIT_TO','MERGED_INTO','NO_EQUIVALENT')),
    CONSTRAINT ck_pi_crosswalk_target CHECK ((relation_type = 'NO_EQUIVALENT' AND to_program_pi_id IS NULL) OR (relation_type <> 'NO_EQUIVALENT' AND to_program_pi_id IS NOT NULL)),
    CONSTRAINT fk_pi_crosswalk_from FOREIGN KEY (from_program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_pi_crosswalk_header FOREIGN KEY (program_version_crosswalk_id) REFERENCES academic.program_version_crosswalk (id) ON DELETE RESTRICT,
    CONSTRAINT fk_pi_crosswalk_to FOREIGN KEY (to_program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT
);

CREATE TABLE academic.course_crosswalk (
    id uuid NOT NULL,
    program_version_crosswalk_id uuid NOT NULL,
    from_program_course_id uuid NOT NULL,
    to_program_course_id uuid,
    relation_type varchar(20) NOT NULL,
    allocation_ratio numeric(12,10),
    rationale text,
    CONSTRAINT pk_course_crosswalk PRIMARY KEY (id),
    CONSTRAINT ck_course_crosswalk_ratio CHECK (allocation_ratio IS NULL OR (allocation_ratio >= 0 AND allocation_ratio <= 1 AND allocation_ratio <> 'NaN'::numeric AND allocation_ratio NOT IN ('Infinity'::numeric, '-Infinity'::numeric))),
    CONSTRAINT ck_course_crosswalk_relation CHECK (relation_type IN ('EQUIVALENT','REPLACED_BY','SPLIT_TO','MERGED_INTO','NO_EQUIVALENT')),
    CONSTRAINT ck_course_crosswalk_target CHECK ((relation_type = 'NO_EQUIVALENT' AND to_program_course_id IS NULL) OR (relation_type <> 'NO_EQUIVALENT' AND to_program_course_id IS NOT NULL)),
    CONSTRAINT fk_course_crosswalk_from FOREIGN KEY (from_program_course_id) REFERENCES academic.program_course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_crosswalk_header FOREIGN KEY (program_version_crosswalk_id) REFERENCES academic.program_version_crosswalk (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_crosswalk_to FOREIGN KEY (to_program_course_id) REFERENCES academic.program_course (id) ON DELETE RESTRICT
);

CREATE TABLE academic.course_pi_mapping (
    id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    program_course_id uuid NOT NULL,
    program_pi_id uuid NOT NULL,
    contribution_level char(1) NOT NULL,
    is_direct_assessment boolean NOT NULL,
    rationale text,
    source_type varchar(20) NOT NULL,
    source_shared_mapping_id uuid,
    is_locked boolean NOT NULL,
    exception_decision_id uuid,
    CONSTRAINT pk_course_pi_mapping PRIMARY KEY (id),
    CONSTRAINT uq_course_pi_mapping_id_course_version UNIQUE (id, program_course_id, program_version_id),
    CONSTRAINT uq_course_pi_mapping_id_version UNIQUE (id, program_version_id),
    CONSTRAINT uq_course_pi_mapping_id_version_pi UNIQUE (id, program_version_id, program_pi_id),
    CONSTRAINT ck_course_pi_mapping_contribution CHECK (contribution_level IN ('I','R','M')),
    CONSTRAINT ck_course_pi_mapping_exception CHECK (source_type <> 'APPENDIX' OR exception_decision_id IS NOT NULL),
    CONSTRAINT ck_course_pi_mapping_source CHECK (source_type IN ('TEMPLATE','PROGRAM','APPENDIX')),
    CONSTRAINT ck_course_pi_mapping_source_lock CHECK (source_shared_mapping_id IS NULL OR is_locked),
    CONSTRAINT fk_course_pi_mapping_course_version FOREIGN KEY (program_course_id, program_version_id) REFERENCES academic.program_course (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_mapping_exception_decision FOREIGN KEY (exception_decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_mapping_pi_version FOREIGN KEY (program_pi_id, program_version_id) REFERENCES academic.program_pi (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_mapping_shared_source FOREIGN KEY (source_shared_mapping_id) REFERENCES academic.shared_course_pi_mapping (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_mapping_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.course_prerequisite_group (
    id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    target_program_course_id uuid NOT NULL,
    group_no integer NOT NULL,
    minimum_items_satisfied integer NOT NULL,
    relation_type varchar(16) NOT NULL,
    CONSTRAINT pk_course_prerequisite_group PRIMARY KEY (id),
    CONSTRAINT uq_course_prerequisite_group_id_version UNIQUE (id, program_version_id),
    CONSTRAINT ck_course_prerequisite_group_minimum CHECK (minimum_items_satisfied > 0),
    CONSTRAINT ck_course_prerequisite_group_no CHECK (group_no > 0),
    CONSTRAINT ck_course_prerequisite_group_relation CHECK (relation_type IN ('ALL','ANY','AT_LEAST')),
    CONSTRAINT fk_course_prerequisite_group_target_version FOREIGN KEY (target_program_course_id, program_version_id) REFERENCES academic.program_course (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_prerequisite_group_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.curriculum_path_course (
    id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    program_course_id uuid NOT NULL,
    planned_term integer,
    requirement_type varchar(16) NOT NULL,
    elective_group_id uuid,
    sort_order integer NOT NULL,
    CONSTRAINT pk_curriculum_path_course PRIMARY KEY (id),
    CONSTRAINT ck_curriculum_path_course_elective_group CHECK (requirement_type = 'ELECTIVE' OR elective_group_id IS NULL),
    CONSTRAINT ck_curriculum_path_course_requirement CHECK (requirement_type IN ('REQUIRED','ELECTIVE','OPTIONAL','SUBSTITUTE')),
    CONSTRAINT ck_curriculum_path_course_sort_order CHECK (sort_order >= 0),
    CONSTRAINT ck_curriculum_path_course_term CHECK (planned_term IS NULL OR planned_term > 0),
    CONSTRAINT fk_curriculum_path_course_elective_group FOREIGN KEY (elective_group_id) REFERENCES academic.curriculum_elective_group (id) ON DELETE RESTRICT,
    CONSTRAINT fk_curriculum_path_course_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_curriculum_path_course_program_course FOREIGN KEY (program_course_id) REFERENCES academic.program_course (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.syllabus (
    id uuid NOT NULL,
    program_course_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    owner_org_unit_id uuid NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_syllabus PRIMARY KEY (id),
    CONSTRAINT uq_syllabus_id_program_course UNIQUE (id, program_course_id),
    CONSTRAINT ck_syllabus_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT fk_syllabus_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_program_course FOREIGN KEY (program_course_id) REFERENCES academic.program_course (id) ON DELETE RESTRICT
);

CREATE TABLE result.result_batch (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    input_snapshot_id uuid NOT NULL,
    policy_version_id uuid NOT NULL,
    program_policy_binding_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    academic_year_start smallint NOT NULL,
    batch_no integer NOT NULL,
    engine_version varchar(64) NOT NULL,
    source_commit varchar(64) NOT NULL,
    container_digest varchar(255),
    status varchar(24) NOT NULL,
    idempotency_key varchar(128) NOT NULL,
    request_checksum char(64) NOT NULL,
    recalculates_batch_id uuid,
    recalculation_reason text,
    workflow_instance_id uuid NOT NULL,
    sod_policy_version_id uuid NOT NULL,
    result_checksum char(64),
    started_at timestamptz,
    completed_at timestamptz,
    published_at timestamptz,
    CONSTRAINT pk_result_batch PRIMARY KEY (id),
    CONSTRAINT "AK_result_batch_id_measurement_period_id" UNIQUE (id, measurement_period_id),
    CONSTRAINT uq_result_batch_scope_covering UNIQUE (id, academic_year_start, org_unit_id, program_version_id, measurement_period_id),
    CONSTRAINT uq_result_batch_snapshot_scope_covering UNIQUE (id, input_snapshot_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id),
    CONSTRAINT ck_result_batch_batch_no CHECK (batch_no > 0),
    CONSTRAINT ck_result_batch_no_self_recalculation CHECK (recalculates_batch_id IS NULL OR recalculates_batch_id <> id),
    CONSTRAINT ck_result_batch_recalculation CHECK (num_nonnulls(recalculates_batch_id, recalculation_reason) IN (0, 2)),
    CONSTRAINT ck_result_batch_request_checksum CHECK (request_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_result_batch_result_checksum CHECK (result_checksum IS NULL OR result_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_result_batch_status CHECK (status IN ('QUEUED', 'RUNNING', 'CALCULATED', 'VALIDATED', 'IN_REVIEW', 'APPROVED', 'PUBLISHED', 'FAILED', 'CANCELLED')),
    CONSTRAINT ck_result_batch_times CHECK ((completed_at IS NULL OR (started_at IS NOT NULL AND completed_at >= started_at)) AND (published_at IS NULL OR (completed_at IS NOT NULL AND published_at >= completed_at))),
    CONSTRAINT fk_result_batch_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_batch_measurement_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_batch_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_batch_policy_binding FOREIGN KEY (program_policy_binding_id) REFERENCES measurement.program_policy_binding (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_batch_policy_version FOREIGN KEY (policy_version_id) REFERENCES measurement.calculation_policy_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_batch_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_batch_recalculates_batch FOREIGN KEY (recalculates_batch_id) REFERENCES result.result_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_batch_snapshot_scope_policy FOREIGN KEY (input_snapshot_id, measurement_period_id, policy_version_id, program_policy_binding_id, org_unit_id, program_version_id, academic_year_start) REFERENCES measurement.input_snapshot (id, measurement_period_id, policy_version_id, program_policy_binding_id, org_unit_id, program_version_id, academic_year_start) ON DELETE RESTRICT,
    CONSTRAINT fk_result_batch_sod_policy_version FOREIGN KEY (sod_policy_version_id) REFERENCES iam.sod_policy_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_batch_workflow_instance FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.snapshot_manifest_chunk (
    input_snapshot_id uuid NOT NULL,
    entity_type varchar(64) NOT NULL,
    chunk_no integer NOT NULL,
    row_count bigint NOT NULL,
    first_key varchar(255) NOT NULL,
    last_key varchar(255) NOT NULL,
    checksum char(64) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_snapshot_manifest_chunk PRIMARY KEY (input_snapshot_id, entity_type, chunk_no),
    CONSTRAINT ck_snapshot_manifest_chunk_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_snapshot_manifest_chunk_count CHECK (row_count >= 0),
    CONSTRAINT ck_snapshot_manifest_chunk_no CHECK (chunk_no >= 0),
    CONSTRAINT fk_snapshot_manifest_chunk_input_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.snapshot_pi_plo_weight (
    input_snapshot_id uuid NOT NULL,
    program_pi_id uuid NOT NULL,
    program_plo_id uuid NOT NULL,
    pi_weight_ratio numeric(12,10) NOT NULL,
    is_core boolean NOT NULL,
    source_program_pi_id uuid NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_snapshot_pi_plo_weight PRIMARY KEY (input_snapshot_id, program_pi_id, program_plo_id),
    CONSTRAINT ck_snapshot_pi_plo_weight CHECK (pi_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND pi_weight_ratio > 0 AND pi_weight_ratio <= 1),
    CONSTRAINT fk_snapshot_pi_plo_weight_input_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_pi_plo_weight_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_pi_plo_weight_program_plo FOREIGN KEY (program_plo_id) REFERENCES academic.program_plo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_pi_plo_weight_source_pi FOREIGN KEY (source_program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.snapshot_population_member (
    input_snapshot_id uuid NOT NULL,
    student_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    student_path_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    decision varchar(20) NOT NULL,
    exclusion_reason_code varchar(64),
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_snapshot_population_member PRIMARY KEY (input_snapshot_id, student_id),
    CONSTRAINT ck_snapshot_population_member_decision CHECK (decision IN ('PENDING','INCLUDED','EXCLUDED')),
    CONSTRAINT ck_snapshot_population_member_exclusion CHECK ((decision = 'EXCLUDED' AND exclusion_reason_code IS NOT NULL) OR (decision <> 'EXCLUDED' AND exclusion_reason_code IS NULL)),
    CONSTRAINT fk_snapshot_population_member_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_population_member_curriculum_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_population_member_input_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_population_member_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_population_member_student_path FOREIGN KEY (student_path_id, student_id, curriculum_path_id) REFERENCES academic.student_path (id, student_id, curriculum_path_id) ON DELETE RESTRICT
);

CREATE TABLE measurement.snapshot_resource (
    input_snapshot_id uuid NOT NULL,
    resource_type varchar(64) NOT NULL,
    resource_id uuid NOT NULL,
    version_id uuid NOT NULL,
    checksum char(64) NOT NULL,
    canonical_payload jsonb NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_snapshot_resource PRIMARY KEY (input_snapshot_id, resource_type, resource_id, version_id),
    CONSTRAINT ck_snapshot_resource_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT fk_snapshot_resource_input_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.period_population_member (
    measurement_period_id uuid NOT NULL,
    student_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    student_path_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    decision varchar(20) NOT NULL,
    exclusion_reason_code varchar(64),
    decision_source varchar(32) NOT NULL,
    decided_by uuid NOT NULL,
    decided_at timestamptz NOT NULL,
    CONSTRAINT pk_period_population_member PRIMARY KEY (measurement_period_id, student_id),
    CONSTRAINT ck_period_population_member_decision CHECK (decision IN ('PENDING','INCLUDED','EXCLUDED')),
    CONSTRAINT ck_period_population_member_exclusion CHECK ((decision = 'EXCLUDED' AND exclusion_reason_code IS NOT NULL) OR (decision <> 'EXCLUDED' AND exclusion_reason_code IS NULL)),
    CONSTRAINT fk_period_population_member_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_period_population_member_curriculum_path FOREIGN KEY (curriculum_path_id, program_version_id) REFERENCES academic.curriculum_path (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_period_population_member_decider FOREIGN KEY (decided_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_period_population_member_period_cohort FOREIGN KEY (measurement_period_id, program_version_id, cohort_id) REFERENCES measurement.measurement_period_cohort (measurement_period_id, program_version_id, cohort_id) ON DELETE RESTRICT,
    CONSTRAINT fk_period_population_member_period_program FOREIGN KEY (measurement_period_id, program_version_id) REFERENCES measurement.measurement_period (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_period_population_member_student_cohort FOREIGN KEY (student_id, cohort_id) REFERENCES academic.student (person_id, admission_cohort_id) ON DELETE RESTRICT,
    CONSTRAINT fk_period_population_member_student_path FOREIGN KEY (student_path_id, student_id, program_version_id, curriculum_path_id) REFERENCES academic.student_path (id, student_id, program_version_id, curriculum_path_id) ON DELETE RESTRICT
);

CREATE TABLE integration.staging_direct_measurement_plan (
    id uuid NOT NULL,
    ingestion_batch_id uuid NOT NULL,
    row_no integer NOT NULL,
    raw_record_id bigint NOT NULL,
    course_code varchar(64) NOT NULL,
    pi_code varchar(64) NOT NULL,
    assessment_code varchar(64) NOT NULL,
    criterion_code varchar(64),
    weight numeric(12,10),
    resolved_direct_measurement_plan_id uuid,
    validation_status varchar(20) NOT NULL,
    row_checksum char(64) NOT NULL,
    CONSTRAINT pk_staging_direct_measurement_plan PRIMARY KEY (id),
    CONSTRAINT ck_staging_direct_measurement_plan_checksum CHECK (row_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_staging_direct_measurement_plan_row_no CHECK (row_no > 0),
    CONSTRAINT ck_staging_direct_measurement_plan_validation_status CHECK (validation_status IN ('PENDING', 'VALID', 'INVALID', 'PROMOTED')),
    CONSTRAINT ck_staging_direct_measurement_plan_weight CHECK (weight IS NULL OR (weight NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND weight >= 0 AND weight <= 1)),
    CONSTRAINT fk_staging_direct_measurement_plan_batch FOREIGN KEY (ingestion_batch_id) REFERENCES integration.ingestion_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_direct_measurement_plan_raw_record FOREIGN KEY (raw_record_id) REFERENCES integration.raw_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_direct_measurement_plan_resolved_plan FOREIGN KEY (resolved_direct_measurement_plan_id) REFERENCES academic.direct_measurement_plan (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.indirect_observation (
    id uuid NOT NULL,
    response_batch_id uuid NOT NULL,
    instrument_version_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    item_id uuid NOT NULL,
    respondent_key varchar(128) NOT NULL,
    student_id uuid,
    raw_value numeric(20,10) NOT NULL,
    max_value numeric(20,10) NOT NULL,
    group_dimension jsonb,
    recorded_at timestamptz NOT NULL,
    CONSTRAINT pk_indirect_observation PRIMARY KEY (id),
    CONSTRAINT ck_indirect_observation_respondent CHECK (char_length(btrim(respondent_key)) > 0),
    CONSTRAINT ck_indirect_observation_value CHECK (raw_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_value > 0 AND raw_value >= 0 AND raw_value <= max_value),
    CONSTRAINT fk_indirect_observation_batch_binding FOREIGN KEY (response_batch_id, instrument_version_id, program_version_id) REFERENCES measurement.indirect_response_batch (id, instrument_version_id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_indirect_observation_instrument_version FOREIGN KEY (instrument_version_id) REFERENCES measurement.indirect_instrument_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_indirect_observation_item_binding FOREIGN KEY (item_id, instrument_version_id, program_version_id) REFERENCES measurement.indirect_item (id, instrument_version_id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_indirect_observation_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT
);

CREATE TABLE academic.course_pi_path_override (
    id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    course_pi_mapping_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    contribution_level char(1) NOT NULL,
    direct_assessment_enabled boolean NOT NULL,
    exception_decision_id uuid NOT NULL,
    rationale text NOT NULL,
    CONSTRAINT pk_course_pi_path_override PRIMARY KEY (id),
    CONSTRAINT ck_course_pi_path_override_contribution CHECK (contribution_level IN ('I','R','M')),
    CONSTRAINT fk_course_pi_path_override_decision FOREIGN KEY (exception_decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_path_override_mapping_version FOREIGN KEY (course_pi_mapping_id, program_version_id) REFERENCES academic.course_pi_mapping (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_path_override_path_version FOREIGN KEY (curriculum_path_id, program_version_id) REFERENCES academic.curriculum_path (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_path_override_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.direct_measurement_source (
    id uuid NOT NULL,
    direct_measurement_plan_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    program_pi_id uuid NOT NULL,
    course_pi_mapping_id uuid NOT NULL,
    planned_term integer,
    owner_org_unit_id uuid NOT NULL,
    source_weight_ratio numeric(12,10) NOT NULL,
    source_role varchar(16) NOT NULL,
    sort_order integer NOT NULL,
    CONSTRAINT pk_direct_measurement_source PRIMARY KEY (id),
    CONSTRAINT ck_direct_measurement_source_role CHECK (source_role IN ('OFFICIAL','COMPARISON')),
    CONSTRAINT ck_direct_measurement_source_sort_order CHECK (sort_order >= 0),
    CONSTRAINT ck_direct_measurement_source_term CHECK (planned_term IS NULL OR planned_term > 0),
    CONSTRAINT ck_direct_measurement_source_weight CHECK (source_weight_ratio > 0 AND source_weight_ratio <= 1 AND source_weight_ratio <> 'NaN'::numeric AND source_weight_ratio NOT IN ('Infinity'::numeric, '-Infinity'::numeric)),
    CONSTRAINT fk_direct_measurement_source_mapping_binding FOREIGN KEY (course_pi_mapping_id, program_version_id, program_pi_id) REFERENCES academic.course_pi_mapping (id, program_version_id, program_pi_id) ON DELETE RESTRICT,
    CONSTRAINT fk_direct_measurement_source_owner_org_unit FOREIGN KEY (owner_org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_direct_measurement_source_path_version FOREIGN KEY (curriculum_path_id, program_version_id) REFERENCES academic.curriculum_path (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_direct_measurement_source_pi_version FOREIGN KEY (program_pi_id, program_version_id) REFERENCES academic.program_pi (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_direct_measurement_source_plan_binding FOREIGN KEY (direct_measurement_plan_id, program_version_id, curriculum_path_id, program_pi_id) REFERENCES academic.direct_measurement_plan (id, program_version_id, curriculum_path_id, program_pi_id) ON DELETE RESTRICT,
    CONSTRAINT fk_direct_measurement_source_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT
);

CREATE TABLE integration.staging_course_pi_mapping (
    id uuid NOT NULL,
    ingestion_batch_id uuid NOT NULL,
    row_no integer NOT NULL,
    raw_record_id bigint NOT NULL,
    course_code varchar(64) NOT NULL,
    pi_code varchar(64) NOT NULL,
    contribution_weight numeric(12,10),
    resolved_course_pi_mapping_id uuid,
    validation_status varchar(20) NOT NULL,
    row_checksum char(64) NOT NULL,
    CONSTRAINT pk_staging_course_pi_mapping PRIMARY KEY (id),
    CONSTRAINT ck_staging_course_pi_mapping_checksum CHECK (row_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_staging_course_pi_mapping_row_no CHECK (row_no > 0),
    CONSTRAINT ck_staging_course_pi_mapping_validation_status CHECK (validation_status IN ('PENDING', 'VALID', 'INVALID', 'PROMOTED')),
    CONSTRAINT ck_staging_course_pi_mapping_weight CHECK (contribution_weight IS NULL OR (contribution_weight NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND contribution_weight >= 0 AND contribution_weight <= 1)),
    CONSTRAINT fk_staging_course_pi_mapping_batch FOREIGN KEY (ingestion_batch_id) REFERENCES integration.ingestion_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_course_pi_mapping_raw_record FOREIGN KEY (raw_record_id) REFERENCES integration.raw_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_course_pi_mapping_resolved_mapping FOREIGN KEY (resolved_course_pi_mapping_id) REFERENCES academic.course_pi_mapping (id) ON DELETE RESTRICT
);

CREATE TABLE academic.course_prerequisite_item (
    group_id uuid NOT NULL,
    required_program_course_id uuid NOT NULL,
    minimum_grade numeric(20,10),
    allow_concurrent boolean NOT NULL,
    rationale text,
    CONSTRAINT pk_course_prerequisite_item PRIMARY KEY (group_id, required_program_course_id),
    CONSTRAINT ck_course_prerequisite_item_grade CHECK (minimum_grade IS NULL OR (minimum_grade >= 0 AND minimum_grade <= 100 AND minimum_grade <> 'NaN'::numeric AND minimum_grade NOT IN ('Infinity'::numeric, '-Infinity'::numeric))),
    CONSTRAINT fk_course_prerequisite_item_group FOREIGN KEY (group_id) REFERENCES academic.course_prerequisite_group (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_prerequisite_item_required_course FOREIGN KEY (required_program_course_id) REFERENCES academic.program_course (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.syllabus_version (
    id uuid NOT NULL,
    syllabus_id uuid NOT NULL,
    program_course_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    institution_template_version_id uuid NOT NULL,
    course_version_id uuid NOT NULL,
    syllabus_template_version_id uuid NOT NULL,
    version_no integer NOT NULL,
    applicable_from date NOT NULL,
    applicable_to date,
    status varchar(20) NOT NULL,
    shared_syllabus_core_version_id uuid,
    workflow_instance_id uuid,
    supersedes_id uuid,
    content_checksum char(64) NOT NULL,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_syllabus_version PRIMARY KEY (id),
    CONSTRAINT uq_syllabus_version_full_binding UNIQUE (id, program_course_id, program_version_id, course_version_id),
    CONSTRAINT uq_syllabus_version_id_course_program UNIQUE (id, program_course_id, program_version_id),
    CONSTRAINT uq_syllabus_version_id_program_course_course_version UNIQUE (id, program_course_id, course_version_id),
    CONSTRAINT uq_syllabus_version_id_program_version UNIQUE (id, program_version_id),
    CONSTRAINT uq_syllabus_version_id_template UNIQUE (id, syllabus_template_version_id),
    CONSTRAINT ck_syllabus_version_applicable_range CHECK (applicable_to IS NULL OR applicable_to > applicable_from),
    CONSTRAINT ck_syllabus_version_content_checksum CHECK (content_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_syllabus_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT ck_syllabus_version_version_no CHECK (version_no > 0),
    CONSTRAINT fk_syllabus_version_course_version FOREIGN KEY (course_version_id) REFERENCES academic.course_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_version_institution_template_version FOREIGN KEY (institution_template_version_id) REFERENCES academic.institution_template_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_version_program_course_binding FOREIGN KEY (program_course_id, program_version_id, course_version_id) REFERENCES academic.program_course (id, program_version_id, course_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_version_program_template FOREIGN KEY (program_version_id, institution_template_version_id) REFERENCES academic.program_version (id, institution_template_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_version_shared_core FOREIGN KEY (shared_syllabus_core_version_id, course_version_id) REFERENCES portfolio.shared_syllabus_core_version (id, course_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_version_supersedes FOREIGN KEY (supersedes_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_version_syllabus_program_course FOREIGN KEY (syllabus_id, program_course_id) REFERENCES portfolio.syllabus (id, program_course_id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_version_syllabus_template FOREIGN KEY (syllabus_template_version_id, institution_template_version_id) REFERENCES portfolio.syllabus_template_version (id, institution_template_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_version_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE result.batch_delta (
    id uuid NOT NULL,
    old_batch_id uuid NOT NULL,
    new_batch_id uuid NOT NULL,
    entity_type varchar(64) NOT NULL,
    entity_key jsonb NOT NULL,
    old_value numeric(20,10),
    new_value numeric(20,10),
    delta numeric(20,10),
    reason text,
    CONSTRAINT pk_batch_delta PRIMARY KEY (id),
    CONSTRAINT ck_batch_delta_no_self CHECK (old_batch_id <> new_batch_id),
    CONSTRAINT fk_batch_delta_new_batch FOREIGN KEY (new_batch_id) REFERENCES result.result_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_batch_delta_old_batch FOREIGN KEY (old_batch_id) REFERENCES result.result_batch (id) ON DELETE RESTRICT
);

CREATE TABLE result.batch_supersession (
    old_batch_id uuid NOT NULL,
    new_batch_id uuid NOT NULL,
    reason text NOT NULL,
    created_by uuid NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_batch_supersession PRIMARY KEY (old_batch_id),
    CONSTRAINT ck_batch_supersession_no_self CHECK (old_batch_id <> new_batch_id),
    CONSTRAINT fk_batch_supersession_created_by FOREIGN KEY (created_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_batch_supersession_new_batch FOREIGN KEY (new_batch_id) REFERENCES result.result_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_batch_supersession_old_batch FOREIGN KEY (old_batch_id) REFERENCES result.result_batch (id) ON DELETE RESTRICT
);

CREATE TABLE result.calculation_run (
    id uuid NOT NULL,
    batch_id uuid NOT NULL,
    attempt_no integer NOT NULL,
    worker_id varchar(128) NOT NULL,
    status varchar(20) NOT NULL,
    started_at timestamptz NOT NULL,
    heartbeat_at timestamptz,
    completed_at timestamptz,
    progress_ratio numeric(12,10) NOT NULL,
    error_code varchar(64),
    error_detail text,
    log_reference varchar(512),
    CONSTRAINT pk_calculation_run PRIMARY KEY (id),
    CONSTRAINT ck_calculation_run_attempt_no CHECK (attempt_no > 0),
    CONSTRAINT ck_calculation_run_progress CHECK (progress_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND progress_ratio >= 0 AND progress_ratio <= 1),
    CONSTRAINT ck_calculation_run_times CHECK ((heartbeat_at IS NULL OR heartbeat_at >= started_at) AND (completed_at IS NULL OR completed_at >= started_at)),
    CONSTRAINT fk_calculation_run_batch FOREIGN KEY (batch_id) REFERENCES result.result_batch (id) ON DELETE RESTRICT
);

CREATE TABLE result.publication (
    id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    batch_id uuid NOT NULL,
    publication_type varchar(32) NOT NULL,
    published_by uuid NOT NULL,
    published_at timestamptz NOT NULL,
    watermark_template text,
    document_version_id uuid,
    CONSTRAINT pk_publication PRIMARY KEY (id),
    CONSTRAINT "AK_publication_id_batch_id_measurement_period_id" UNIQUE (id, batch_id, measurement_period_id),
    CONSTRAINT ck_publication_type CHECK (publication_type = btrim(publication_type) AND char_length(publication_type) > 0),
    CONSTRAINT fk_publication_batch_period FOREIGN KEY (batch_id, measurement_period_id) REFERENCES result.result_batch (id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_publication_document_version FOREIGN KEY (document_version_id) REFERENCES document.document_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_publication_measurement_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_publication_published_by FOREIGN KEY (published_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE quality.remeasurement_evaluation (
    id uuid NOT NULL,
    improvement_plan_id uuid NOT NULL,
    before_batch_id uuid NOT NULL,
    after_batch_id uuid NOT NULL,
    comparability_status varchar(24) NOT NULL,
    baseline_value numeric(20,10),
    after_value numeric(20,10),
    delta_value numeric(20,10),
    conclusion text NOT NULL,
    verified_by uuid NOT NULL,
    verified_at timestamptz NOT NULL,
    CONSTRAINT pk_remeasurement_evaluation PRIMARY KEY (id),
    CONSTRAINT ck_remeasurement_evaluation_batches CHECK (before_batch_id <> after_batch_id),
    CONSTRAINT ck_remeasurement_evaluation_delta CHECK (num_nonnulls(baseline_value, after_value, delta_value) IN (0, 3) AND (delta_value IS NULL OR delta_value = after_value - baseline_value)),
    CONSTRAINT ck_remeasurement_evaluation_values CHECK ((baseline_value IS NULL OR baseline_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)) AND (after_value IS NULL OR after_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)) AND (delta_value IS NULL OR delta_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric))),
    CONSTRAINT fk_remeasurement_evaluation_after_batch FOREIGN KEY (after_batch_id) REFERENCES result.result_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_remeasurement_evaluation_before_batch FOREIGN KEY (before_batch_id) REFERENCES result.result_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_remeasurement_evaluation_plan FOREIGN KEY (improvement_plan_id) REFERENCES quality.improvement_plan (id) ON DELETE RESTRICT,
    CONSTRAINT fk_remeasurement_evaluation_verified_by FOREIGN KEY (verified_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE result.result_batch_evidence (
    batch_id uuid NOT NULL,
    evidence_version_id uuid NOT NULL,
    link_role varchar(32) NOT NULL,
    CONSTRAINT pk_result_batch_evidence PRIMARY KEY (batch_id, evidence_version_id, link_role),
    CONSTRAINT fk_result_batch_evidence_batch FOREIGN KEY (batch_id) REFERENCES result.result_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_batch_evidence_evidence_version FOREIGN KEY (evidence_version_id) REFERENCES document.evidence_version (id) ON DELETE RESTRICT
);

CREATE TABLE result.result_report_document (
    batch_id uuid NOT NULL,
    document_version_id uuid NOT NULL,
    report_type varchar(32) NOT NULL,
    filter_checksum char(64) NOT NULL,
    CONSTRAINT pk_result_report_document PRIMARY KEY (batch_id, document_version_id, report_type),
    CONSTRAINT ck_result_report_document_filter_checksum CHECK (filter_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT fk_result_report_document_batch FOREIGN KEY (batch_id) REFERENCES result.result_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_report_document_document_version FOREIGN KEY (document_version_id) REFERENCES document.document_version (id) ON DELETE RESTRICT
);

CREATE TABLE result.student_pi_result (
    academic_year_start smallint NOT NULL,
    id uuid NOT NULL,
    batch_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    student_id uuid NOT NULL,
    student_path_id uuid NOT NULL,
    program_pi_id uuid NOT NULL,
    method varchar(16) NOT NULL,
    score numeric(20,10),
    theta_ind numeric(20,10) NOT NULL,
    attainment_status varchar(24) NOT NULL,
    core_gate_status varchar(24) NOT NULL,
    data_status varchar(24) NOT NULL,
    alpha numeric(12,10),
    CONSTRAINT pk_student_pi_result PRIMARY KEY (academic_year_start, id),
    CONSTRAINT uq_student_pi_result_covering UNIQUE (academic_year_start, id, batch_id, student_id, student_path_id, program_pi_id, method),
    CONSTRAINT ck_student_pi_result_alpha CHECK ((method = 'COMBINED' AND alpha IS NOT NULL AND alpha NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND alpha >= 0 AND alpha <= 1) OR (method <> 'COMBINED' AND alpha IS NULL)),
    CONSTRAINT ck_student_pi_result_method CHECK (method IN ('DIRECT', 'INDIRECT', 'COMBINED')),
    CONSTRAINT ck_student_pi_result_score CHECK (score IS NULL OR (score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND score >= 0 AND score <= 100)),
    CONSTRAINT ck_student_pi_result_theta CHECK (theta_ind NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_ind >= 0 AND theta_ind <= 100),
    CONSTRAINT fk_student_pi_result_batch_scope FOREIGN KEY (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) REFERENCES result.result_batch (id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_result_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_result_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_result_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_result_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_result_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_result_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_result_program_version FOREIGN KEY (program_version_id, program_id) REFERENCES academic.program_version (id, program_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_result_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_result_student_path FOREIGN KEY (student_path_id) REFERENCES academic.student_path (id) ON DELETE RESTRICT
);

CREATE TABLE result.student_plo_result (
    academic_year_start smallint NOT NULL,
    id uuid NOT NULL,
    batch_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    student_id uuid NOT NULL,
    student_path_id uuid NOT NULL,
    program_plo_id uuid NOT NULL,
    method varchar(16) NOT NULL,
    score numeric(20,10),
    theta_ind numeric(20,10) NOT NULL,
    attainment_status varchar(24) NOT NULL,
    core_gate_status varchar(24) NOT NULL,
    data_status varchar(24) NOT NULL,
    alpha numeric(12,10),
    CONSTRAINT pk_student_plo_result PRIMARY KEY (academic_year_start, id),
    CONSTRAINT uq_student_plo_result_covering UNIQUE (academic_year_start, id, batch_id, student_id, student_path_id, program_plo_id, method),
    CONSTRAINT ck_student_plo_result_alpha CHECK ((method = 'COMBINED' AND alpha IS NOT NULL AND alpha NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND alpha >= 0 AND alpha <= 1) OR (method <> 'COMBINED' AND alpha IS NULL)),
    CONSTRAINT ck_student_plo_result_method CHECK (method IN ('DIRECT', 'INDIRECT', 'COMBINED')),
    CONSTRAINT ck_student_plo_result_score CHECK (score IS NULL OR (score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND score >= 0 AND score <= 100)),
    CONSTRAINT ck_student_plo_result_theta CHECK (theta_ind NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_ind >= 0 AND theta_ind <= 100),
    CONSTRAINT fk_student_plo_result_batch_scope FOREIGN KEY (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) REFERENCES result.result_batch (id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_result_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_result_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_result_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_result_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_result_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_result_program_plo FOREIGN KEY (program_plo_id) REFERENCES academic.program_plo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_result_program_version FOREIGN KEY (program_version_id, program_id) REFERENCES academic.program_version (id, program_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_result_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_result_student_path FOREIGN KEY (student_path_id) REFERENCES academic.student_path (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.snapshot_indirect_observation (
    input_snapshot_id uuid NOT NULL,
    indirect_observation_id uuid NOT NULL,
    item_id uuid NOT NULL,
    program_pi_id uuid,
    program_plo_id uuid,
    raw_value numeric(20,10) NOT NULL,
    max_value numeric(20,10) NOT NULL,
    normalized_value numeric(20,10) NOT NULL,
    source_checksum char(64) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_snapshot_indirect_observation PRIMARY KEY (input_snapshot_id, indirect_observation_id),
    CONSTRAINT ck_snapshot_indirect_observation_checksum CHECK (source_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_snapshot_indirect_observation_outcome CHECK (num_nonnulls(program_pi_id, program_plo_id) = 1),
    CONSTRAINT ck_snapshot_indirect_observation_values CHECK (raw_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND normalized_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_value > 0 AND raw_value >= 0 AND raw_value <= max_value AND normalized_value BETWEEN 0 AND 100),
    CONSTRAINT fk_snapshot_indirect_observation_input_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_indirect_observation_item FOREIGN KEY (item_id) REFERENCES measurement.indirect_item (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_indirect_observation_observation FOREIGN KEY (indirect_observation_id) REFERENCES measurement.indirect_observation (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_indirect_observation_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_indirect_observation_program_plo FOREIGN KEY (program_plo_id) REFERENCES academic.program_plo (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.assessment_item (
    id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    parent_id uuid,
    assessment_code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    assessment_type varchar(32) NOT NULL,
    course_weight_ratio numeric(12,10) NOT NULL,
    individual_component_ratio numeric(12,10),
    is_group_assessment boolean NOT NULL,
    counts_toward_course_grade boolean NOT NULL,
    max_score numeric(20,10) NOT NULL,
    sort_order integer NOT NULL,
    CONSTRAINT pk_assessment_item PRIMARY KEY (id),
    CONSTRAINT uq_assessment_item_id_version UNIQUE (id, syllabus_version_id),
    CONSTRAINT ck_assessment_item_code CHECK (assessment_code = upper(btrim(assessment_code)) AND char_length(assessment_code) > 0),
    CONSTRAINT ck_assessment_item_course_weight CHECK (course_weight_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND course_weight_ratio >= 0 AND course_weight_ratio <= 1),
    CONSTRAINT ck_assessment_item_individual_ratio CHECK (individual_component_ratio IS NULL OR (individual_component_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND individual_component_ratio >= 0 AND individual_component_ratio <= 1)),
    CONSTRAINT ck_assessment_item_max_score CHECK (max_score NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND max_score > 0),
    CONSTRAINT ck_assessment_item_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_assessment_item_parent_version FOREIGN KEY (parent_id, syllabus_version_id) REFERENCES portfolio.assessment_item (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_assessment_item_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.clo (
    id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    description text NOT NULL,
    domain varchar(32) NOT NULL,
    bloom_level varchar(32) NOT NULL,
    is_core boolean NOT NULL,
    sort_order integer NOT NULL,
    CONSTRAINT pk_clo PRIMARY KEY (id),
    CONSTRAINT uq_clo_id_version UNIQUE (id, syllabus_version_id),
    CONSTRAINT ck_clo_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_clo_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_clo_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.course_objective (
    id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    description text NOT NULL,
    sort_order integer NOT NULL,
    CONSTRAINT pk_course_objective PRIMARY KEY (id),
    CONSTRAINT uq_course_objective_id_version UNIQUE (id, syllabus_version_id),
    CONSTRAINT ck_course_objective_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_course_objective_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_course_objective_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE academic.course_offering (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    program_course_id uuid NOT NULL,
    course_version_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    academic_year_start smallint NOT NULL,
    term_code varchar(32) NOT NULL,
    org_unit_id uuid NOT NULL,
    status varchar(20) NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    source_system_id uuid,
    source_record_id varchar(128),
    CONSTRAINT pk_course_offering PRIMARY KEY (id),
    CONSTRAINT uq_course_offering_id_version UNIQUE (id, program_version_id),
    CONSTRAINT uq_course_offering_id_version_year UNIQUE (id, program_version_id, academic_year_start),
    CONSTRAINT uq_course_offering_id_year UNIQUE (id, academic_year_start),
    CONSTRAINT uq_course_offering_result_binding UNIQUE (id, program_version_id, syllabus_version_id, academic_year_start),
    CONSTRAINT ck_course_offering_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_course_offering_dates CHECK (end_date >= start_date),
    CONSTRAINT ck_course_offering_source CHECK ((source_system_id IS NULL) = (source_record_id IS NULL)),
    CONSTRAINT ck_course_offering_status CHECK (status IN ('PLANNED','OPEN','ACTIVE','COMPLETED','CANCELLED','ARCHIVED')),
    CONSTRAINT ck_course_offering_year CHECK (academic_year_start BETWEEN 1900 AND 9999),
    CONSTRAINT fk_course_offering_course_version FOREIGN KEY (course_version_id) REFERENCES academic.course_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_offering_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_offering_program_course_binding FOREIGN KEY (program_course_id, program_version_id, course_version_id) REFERENCES academic.program_course (id, program_version_id, course_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_offering_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_offering_source_system FOREIGN KEY (source_system_id) REFERENCES integration.source_system (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_offering_syllabus_binding FOREIGN KEY (syllabus_version_id, program_course_id, program_version_id, course_version_id) REFERENCES portfolio.syllabus_version (id, program_course_id, program_version_id, course_version_id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.learning_material (
    id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    material_type varchar(32) NOT NULL,
    citation text NOT NULL,
    url varchar(2048),
    required boolean NOT NULL,
    sort_order integer NOT NULL,
    CONSTRAINT pk_learning_material PRIMARY KEY (id),
    CONSTRAINT uq_learning_material_id_version UNIQUE (id, syllabus_version_id),
    CONSTRAINT ck_learning_material_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_learning_material_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.llo (
    id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    description text NOT NULL,
    sort_order integer NOT NULL,
    CONSTRAINT pk_llo PRIMARY KEY (id),
    CONSTRAINT uq_llo_id_version UNIQUE (id, syllabus_version_id),
    CONSTRAINT ck_llo_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_llo_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_llo_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.syllabus_document (
    syllabus_version_id uuid NOT NULL,
    document_version_id uuid NOT NULL,
    document_role varchar(32) NOT NULL,
    CONSTRAINT pk_syllabus_document PRIMARY KEY (syllabus_version_id, document_version_id, document_role),
    CONSTRAINT fk_syllabus_document_document_version FOREIGN KEY (document_version_id) REFERENCES document.document_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_document_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.syllabus_evidence (
    syllabus_version_id uuid NOT NULL,
    evidence_version_id uuid NOT NULL,
    link_role varchar(32) NOT NULL,
    CONSTRAINT pk_syllabus_evidence PRIMARY KEY (syllabus_version_id, evidence_version_id, link_role),
    CONSTRAINT fk_syllabus_evidence_evidence_version FOREIGN KEY (evidence_version_id) REFERENCES document.evidence_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_evidence_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.syllabus_section_content (
    id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    syllabus_template_version_id uuid NOT NULL,
    template_field_id uuid NOT NULL,
    content_text text,
    content_jsonb jsonb,
    source_kind varchar(32) NOT NULL,
    is_inherited boolean NOT NULL,
    last_edited_by uuid NOT NULL,
    created_at timestamptz NOT NULL,
    created_by uuid NOT NULL,
    updated_at timestamptz NOT NULL,
    updated_by uuid NOT NULL,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_syllabus_section_content PRIMARY KEY (id),
    CONSTRAINT ck_syllabus_section_content_value CHECK (num_nonnulls(content_text, content_jsonb) = 1),
    CONSTRAINT fk_syllabus_section_content_last_editor FOREIGN KEY (last_edited_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_section_content_syllabus_template FOREIGN KEY (syllabus_version_id, syllabus_template_version_id) REFERENCES portfolio.syllabus_version (id, syllabus_template_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_section_content_template_field FOREIGN KEY (template_field_id, syllabus_template_version_id) REFERENCES portfolio.syllabus_template_field (id, syllabus_template_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_section_content_template_version FOREIGN KEY (syllabus_template_version_id) REFERENCES portfolio.syllabus_template_version (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.teaching_session (
    id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    session_no integer NOT NULL,
    title varchar(255) NOT NULL,
    planned_hours numeric(10,2) NOT NULL,
    teaching_method text NOT NULL,
    assessment_method text,
    self_study_task text,
    sort_order integer NOT NULL,
    CONSTRAINT pk_teaching_session PRIMARY KEY (id),
    CONSTRAINT uq_teaching_session_id_version UNIQUE (id, syllabus_version_id),
    CONSTRAINT ck_teaching_session_planned_hours CHECK (planned_hours NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND planned_hours > 0),
    CONSTRAINT ck_teaching_session_session_no CHECK (session_no > 0),
    CONSTRAINT ck_teaching_session_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_teaching_session_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE result.current_publication (
    measurement_period_id uuid NOT NULL,
    publication_id uuid NOT NULL,
    batch_id uuid NOT NULL,
    updated_by uuid NOT NULL,
    updated_at timestamptz NOT NULL,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_current_publication PRIMARY KEY (measurement_period_id),
    CONSTRAINT ck_current_publication_row_version CHECK (row_version > 0),
    CONSTRAINT fk_current_publication_batch_period FOREIGN KEY (batch_id, measurement_period_id) REFERENCES result.result_batch (id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_current_publication_measurement_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_current_publication_publication_batch_period FOREIGN KEY (publication_id, batch_id, measurement_period_id) REFERENCES result.publication (id, batch_id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_current_publication_updated_by FOREIGN KEY (updated_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE result.publication_revocation (
    id uuid NOT NULL,
    publication_id uuid NOT NULL,
    reason text NOT NULL,
    revoked_by uuid NOT NULL,
    revoked_at timestamptz NOT NULL,
    decision_id uuid NOT NULL,
    CONSTRAINT pk_publication_revocation PRIMARY KEY (id),
    CONSTRAINT ck_publication_revocation_reason CHECK (char_length(btrim(reason)) > 0),
    CONSTRAINT fk_publication_revocation_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_publication_revocation_publication FOREIGN KEY (publication_id) REFERENCES result.publication (id) ON DELETE RESTRICT,
    CONSTRAINT fk_publication_revocation_revoked_by FOREIGN KEY (revoked_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE result.student_plo_pi_contribution (
    academic_year_start smallint NOT NULL,
    student_plo_result_id uuid NOT NULL,
    student_pi_result_id uuid NOT NULL,
    batch_id uuid NOT NULL,
    input_snapshot_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    student_id uuid NOT NULL,
    student_path_id uuid NOT NULL,
    method varchar(16) NOT NULL,
    program_plo_id uuid NOT NULL,
    program_pi_id uuid NOT NULL,
    pi_weight_ratio numeric(12,10) NOT NULL,
    weighted_contribution numeric(20,10) NOT NULL,
    is_core boolean NOT NULL,
    gate_failure_reason text,
    CONSTRAINT pk_student_plo_pi_contribution PRIMARY KEY (academic_year_start, student_plo_result_id, student_pi_result_id),
    CONSTRAINT ck_student_plo_pi_contribution_gate_reason CHECK (is_core OR gate_failure_reason IS NULL),
    CONSTRAINT ck_student_plo_pi_contribution_method CHECK (method IN ('DIRECT', 'INDIRECT', 'COMBINED')),
    CONSTRAINT ck_student_plo_pi_contribution_weight CHECK (pi_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND pi_weight_ratio > 0 AND pi_weight_ratio <= 1 AND weighted_contribution NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)),
    CONSTRAINT fk_student_plo_pi_contribution_batch_snapshot_scope FOREIGN KEY (batch_id, input_snapshot_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) REFERENCES result.result_batch (id, input_snapshot_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_pi_result FOREIGN KEY (academic_year_start, student_pi_result_id, batch_id, student_id, student_path_id, program_pi_id, method) REFERENCES result.student_pi_result (academic_year_start, id, batch_id, student_id, student_path_id, program_pi_id, method) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_plo_result FOREIGN KEY (academic_year_start, student_plo_result_id, batch_id, student_id, student_path_id, program_plo_id, method) REFERENCES result.student_plo_result (academic_year_start, id, batch_id, student_id, student_path_id, program_plo_id, method) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_program_plo FOREIGN KEY (program_plo_id) REFERENCES academic.program_plo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_program_version FOREIGN KEY (program_version_id, program_id) REFERENCES academic.program_version (id, program_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_snapshot_weight FOREIGN KEY (input_snapshot_id, program_pi_id, program_plo_id) REFERENCES measurement.snapshot_pi_plo_weight (input_snapshot_id, program_pi_id, program_plo_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_plo_pi_contribution_student_path FOREIGN KEY (student_path_id) REFERENCES academic.student_path (id) ON DELETE RESTRICT
);

CREATE TABLE academic.anchor_assessment (
    id uuid NOT NULL,
    direct_measurement_source_id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    assessment_item_id uuid NOT NULL,
    anchor_role varchar(16) NOT NULL,
    evidence_requirement text NOT NULL,
    approved_at timestamptz,
    CONSTRAINT pk_anchor_assessment PRIMARY KEY (id),
    CONSTRAINT uq_anchor_assessment_id_version_item UNIQUE (id, syllabus_version_id, assessment_item_id),
    CONSTRAINT ck_anchor_assessment_role CHECK (anchor_role IN ('PRIMARY','SECONDARY','COMPARISON')),
    CONSTRAINT fk_anchor_assessment_item_version FOREIGN KEY (assessment_item_id, syllabus_version_id) REFERENCES portfolio.assessment_item (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_anchor_assessment_source FOREIGN KEY (direct_measurement_source_id) REFERENCES academic.direct_measurement_source (id) ON DELETE RESTRICT,
    CONSTRAINT fk_anchor_assessment_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.assessment_question (
    id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    assessment_item_id uuid NOT NULL,
    question_code varchar(64) NOT NULL,
    max_score numeric(20,10) NOT NULL,
    sort_order integer NOT NULL,
    CONSTRAINT pk_assessment_question PRIMARY KEY (id),
    CONSTRAINT uq_assessment_question_full_binding UNIQUE (id, assessment_item_id, syllabus_version_id),
    CONSTRAINT uq_assessment_question_id_version UNIQUE (id, syllabus_version_id),
    CONSTRAINT ck_assessment_question_code CHECK (question_code = upper(btrim(question_code)) AND char_length(question_code) > 0),
    CONSTRAINT ck_assessment_question_max_score CHECK (max_score NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND max_score > 0),
    CONSTRAINT ck_assessment_question_sort_order CHECK (sort_order >= 0),
    CONSTRAINT fk_assessment_question_item_version FOREIGN KEY (assessment_item_id, syllabus_version_id) REFERENCES portfolio.assessment_item (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_assessment_question_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.rubric (
    id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    syllabus_template_version_id uuid NOT NULL,
    assessment_item_id uuid NOT NULL,
    code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    max_score numeric(20,10) NOT NULL,
    rubric_scale_id uuid NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_rubric PRIMARY KEY (id),
    CONSTRAINT uq_rubric_full_binding UNIQUE (id, assessment_item_id, syllabus_version_id),
    CONSTRAINT ck_rubric_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_rubric_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_rubric_max_score CHECK (max_score NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND max_score > 0),
    CONSTRAINT fk_rubric_assessment_item_version FOREIGN KEY (assessment_item_id, syllabus_version_id) REFERENCES portfolio.assessment_item (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_rubric_scale_template_version FOREIGN KEY (rubric_scale_id, syllabus_template_version_id) REFERENCES portfolio.syllabus_template_rubric_scale (id, syllabus_template_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_rubric_syllabus_template FOREIGN KEY (syllabus_version_id, syllabus_template_version_id) REFERENCES portfolio.syllabus_version (id, syllabus_template_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_rubric_template_version FOREIGN KEY (syllabus_template_version_id) REFERENCES portfolio.syllabus_template_version (id) ON DELETE RESTRICT
);

CREATE TABLE result.cohort_outcome_result (
    academic_year_start smallint NOT NULL,
    id uuid NOT NULL,
    batch_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    outcome_level varchar(20) NOT NULL,
    clo_id uuid,
    program_pi_id uuid,
    program_plo_id uuid,
    method varchar(16) NOT NULL,
    population_count bigint NOT NULL,
    denominator_count bigint NOT NULL,
    attained_count bigint NOT NULL,
    not_attained_observed_count bigint NOT NULL,
    missing_in_denominator_count bigint NOT NULL,
    not_attained_count bigint NOT NULL,
    missing_excluded_count bigint NOT NULL,
    policy_excluded_count bigint NOT NULL,
    attainment_rate numeric(20,10),
    theta_coh numeric(20,10) NOT NULL,
    outcome_status varchar(24) NOT NULL,
    privacy_suppressed boolean NOT NULL,
    CONSTRAINT pk_cohort_outcome_result PRIMARY KEY (academic_year_start, id),
    CONSTRAINT ck_cohort_outcome_result_counts CHECK (population_count >= 0 AND denominator_count >= 0 AND attained_count >= 0 AND not_attained_observed_count >= 0 AND missing_in_denominator_count >= 0 AND not_attained_count >= 0 AND missing_excluded_count >= 0 AND policy_excluded_count >= 0 AND not_attained_count = not_attained_observed_count + missing_in_denominator_count AND denominator_count = attained_count + not_attained_observed_count + missing_in_denominator_count AND population_count = denominator_count + missing_excluded_count + policy_excluded_count),
    CONSTRAINT ck_cohort_outcome_result_outcome CHECK ((outcome_level = 'CLO' AND clo_id IS NOT NULL AND program_pi_id IS NULL AND program_plo_id IS NULL) OR (outcome_level = 'PI' AND clo_id IS NULL AND program_pi_id IS NOT NULL AND program_plo_id IS NULL) OR (outcome_level = 'PLO' AND clo_id IS NULL AND program_pi_id IS NULL AND program_plo_id IS NOT NULL)),
    CONSTRAINT ck_cohort_outcome_result_rate CHECK ((denominator_count = 0 AND attainment_rate IS NULL AND outcome_status = 'INSUFFICIENT_DATA') OR (denominator_count > 0 AND attainment_rate IS NOT NULL AND attainment_rate NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND attainment_rate >= 0 AND attainment_rate <= 100 AND attainment_rate = round((100::numeric * attained_count::numeric / denominator_count::numeric), 10))),
    CONSTRAINT ck_cohort_outcome_result_theta CHECK (theta_coh NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_coh >= 0 AND theta_coh <= 100),
    CONSTRAINT fk_cohort_outcome_result_batch_scope FOREIGN KEY (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) REFERENCES result.result_batch (id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_outcome_result_clo FOREIGN KEY (clo_id) REFERENCES portfolio.clo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_outcome_result_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_outcome_result_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_outcome_result_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_outcome_result_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_outcome_result_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_outcome_result_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_outcome_result_program_plo FOREIGN KEY (program_plo_id) REFERENCES academic.program_plo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_outcome_result_program_version FOREIGN KEY (program_version_id, program_id) REFERENCES academic.program_version (id, program_id) ON DELETE RESTRICT
);

CREATE TABLE result.cohort_population_decision (
    academic_year_start smallint NOT NULL,
    id uuid NOT NULL,
    batch_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    outcome_level varchar(20) NOT NULL,
    clo_id uuid,
    program_pi_id uuid,
    program_plo_id uuid,
    method varchar(16) NOT NULL,
    student_id uuid NOT NULL,
    decision_bucket varchar(32) NOT NULL,
    reason_code varchar(64),
    CONSTRAINT pk_cohort_population_decision PRIMARY KEY (academic_year_start, id),
    CONSTRAINT ck_cohort_population_decision_bucket CHECK (decision_bucket IN ('ATTAINED', 'NOT_ATTAINED_OBSERVED', 'MISSING_IN_DENOMINATOR', 'MISSING_EXCLUDED', 'POLICY_EXCLUDED')),
    CONSTRAINT ck_cohort_population_decision_outcome CHECK ((outcome_level = 'CLO' AND clo_id IS NOT NULL AND program_pi_id IS NULL AND program_plo_id IS NULL) OR (outcome_level = 'PI' AND clo_id IS NULL AND program_pi_id IS NOT NULL AND program_plo_id IS NULL) OR (outcome_level = 'PLO' AND clo_id IS NULL AND program_pi_id IS NULL AND program_plo_id IS NOT NULL)),
    CONSTRAINT fk_cohort_population_decision_batch_scope FOREIGN KEY (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) REFERENCES result.result_batch (id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_population_decision_clo FOREIGN KEY (clo_id) REFERENCES portfolio.clo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_population_decision_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_population_decision_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_population_decision_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_population_decision_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_population_decision_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_population_decision_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_population_decision_program_plo FOREIGN KEY (program_plo_id) REFERENCES academic.program_plo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_population_decision_program_version FOREIGN KEY (program_version_id, program_id) REFERENCES academic.program_version (id, program_id) ON DELETE RESTRICT,
    CONSTRAINT fk_cohort_population_decision_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT
);

CREATE TABLE measurement.program_policy_threshold (
    id uuid NOT NULL,
    program_policy_binding_id uuid NOT NULL,
    outcome_level varchar(20) NOT NULL,
    syllabus_version_id uuid,
    clo_id uuid,
    program_pi_id uuid,
    program_plo_id uuid,
    theta_ind numeric(20,10) NOT NULL,
    theta_coh numeric(20,10) NOT NULL,
    near_threshold numeric(20,10),
    min_sample_size integer NOT NULL,
    CONSTRAINT pk_program_policy_threshold PRIMARY KEY (id),
    CONSTRAINT ck_program_policy_threshold_level CHECK (outcome_level IN ('CLO','PI','PLO')),
    CONSTRAINT ck_program_policy_threshold_outcome CHECK (num_nonnulls(clo_id, program_pi_id, program_plo_id) = 1),
    CONSTRAINT ck_program_policy_threshold_shape CHECK ((outcome_level = 'CLO' AND clo_id IS NOT NULL AND syllabus_version_id IS NOT NULL AND program_pi_id IS NULL AND program_plo_id IS NULL) OR (outcome_level = 'PI' AND clo_id IS NULL AND syllabus_version_id IS NULL AND program_pi_id IS NOT NULL AND program_plo_id IS NULL) OR (outcome_level = 'PLO' AND clo_id IS NULL AND syllabus_version_id IS NULL AND program_pi_id IS NULL AND program_plo_id IS NOT NULL)),
    CONSTRAINT ck_program_policy_threshold_values CHECK (theta_ind NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_coh NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_ind BETWEEN 0 AND 100 AND theta_coh BETWEEN 0 AND 100 AND (near_threshold IS NULL OR near_threshold NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND near_threshold BETWEEN 0 AND 100) AND min_sample_size > 0),
    CONSTRAINT fk_program_policy_threshold_binding FOREIGN KEY (program_policy_binding_id) REFERENCES measurement.program_policy_binding (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_policy_threshold_clo_syllabus FOREIGN KEY (clo_id, syllabus_version_id) REFERENCES portfolio.clo (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_policy_threshold_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_policy_threshold_program_plo FOREIGN KEY (program_plo_id) REFERENCES academic.program_plo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_program_policy_threshold_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE result.result_alert (
    academic_year_start smallint NOT NULL,
    id uuid NOT NULL,
    batch_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    outcome_level varchar(20) NOT NULL,
    clo_id uuid,
    program_pi_id uuid,
    program_plo_id uuid,
    student_id uuid,
    severity varchar(20) NOT NULL,
    reason_code varchar(64) NOT NULL,
    gap_value numeric(20,10),
    status varchar(20) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_result_alert PRIMARY KEY (academic_year_start, id),
    CONSTRAINT ck_result_alert_gap CHECK (gap_value IS NULL OR gap_value NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)),
    CONSTRAINT ck_result_alert_outcome CHECK ((outcome_level = 'CLO' AND clo_id IS NOT NULL AND program_pi_id IS NULL AND program_plo_id IS NULL) OR (outcome_level = 'PI' AND clo_id IS NULL AND program_pi_id IS NOT NULL AND program_plo_id IS NULL) OR (outcome_level = 'PLO' AND clo_id IS NULL AND program_pi_id IS NULL AND program_plo_id IS NOT NULL)),
    CONSTRAINT fk_result_alert_batch_scope FOREIGN KEY (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) REFERENCES result.result_batch (id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_alert_clo FOREIGN KEY (clo_id) REFERENCES portfolio.clo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_alert_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_alert_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_alert_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_alert_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_alert_program_plo FOREIGN KEY (program_plo_id) REFERENCES academic.program_plo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_alert_program_version FOREIGN KEY (program_version_id, program_id) REFERENCES academic.program_version (id, program_id) ON DELETE RESTRICT,
    CONSTRAINT fk_result_alert_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT
);

CREATE TABLE measurement.snapshot_threshold (
    input_snapshot_id uuid NOT NULL,
    outcome_level varchar(20) NOT NULL,
    outcome_key uuid NOT NULL,
    clo_id uuid,
    program_pi_id uuid,
    program_plo_id uuid,
    theta_ind numeric(20,10) NOT NULL,
    theta_coh numeric(20,10) NOT NULL,
    near_threshold numeric(20,10),
    min_sample_size integer NOT NULL,
    threshold_source varchar(32) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_snapshot_threshold PRIMARY KEY (input_snapshot_id, outcome_level, outcome_key),
    CONSTRAINT ck_snapshot_threshold_level CHECK (outcome_level IN ('CLO','PI','PLO')),
    CONSTRAINT ck_snapshot_threshold_outcome CHECK (num_nonnulls(clo_id, program_pi_id, program_plo_id) = 1),
    CONSTRAINT ck_snapshot_threshold_shape CHECK ((outcome_level = 'CLO' AND outcome_key = clo_id AND clo_id IS NOT NULL AND program_pi_id IS NULL AND program_plo_id IS NULL) OR (outcome_level = 'PI' AND outcome_key = program_pi_id AND clo_id IS NULL AND program_pi_id IS NOT NULL AND program_plo_id IS NULL) OR (outcome_level = 'PLO' AND outcome_key = program_plo_id AND clo_id IS NULL AND program_pi_id IS NULL AND program_plo_id IS NOT NULL)),
    CONSTRAINT ck_snapshot_threshold_values CHECK (theta_ind NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_coh NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_ind BETWEEN 0 AND 100 AND theta_coh BETWEEN 0 AND 100 AND (near_threshold IS NULL OR near_threshold NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND near_threshold BETWEEN 0 AND 100) AND min_sample_size > 0),
    CONSTRAINT fk_snapshot_threshold_clo FOREIGN KEY (clo_id) REFERENCES portfolio.clo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_threshold_input_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_threshold_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_threshold_program_plo FOREIGN KEY (program_plo_id) REFERENCES academic.program_plo (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.course_objective_clo (
    course_objective_id uuid NOT NULL,
    clo_id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    CONSTRAINT pk_course_objective_clo PRIMARY KEY (course_objective_id, clo_id),
    CONSTRAINT fk_course_objective_clo_clo_version FOREIGN KEY (clo_id, syllabus_version_id) REFERENCES portfolio.clo (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_objective_clo_objective_version FOREIGN KEY (course_objective_id, syllabus_version_id) REFERENCES portfolio.course_objective (id, syllabus_version_id) ON DELETE RESTRICT
);

CREATE TABLE iam.access_scope (
    id uuid NOT NULL,
    scope_type varchar(32) NOT NULL,
    org_unit_id uuid,
    program_id uuid,
    program_version_id uuid,
    cohort_id uuid,
    curriculum_path_id uuid,
    course_id uuid,
    course_offering_id uuid,
    measurement_period_id uuid,
    subject_principal_id uuid,
    include_descendants boolean NOT NULL,
    checksum char(64) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_access_scope PRIMARY KEY (id),
    CONSTRAINT ck_access_scope_anchor CHECK ((scope_type = 'SYSTEM' AND num_nonnulls(org_unit_id, program_id, program_version_id, cohort_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR (scope_type = 'ORG_UNIT' AND org_unit_id IS NOT NULL AND num_nonnulls(program_id, program_version_id, cohort_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR (scope_type = 'PROGRAM' AND program_id IS NOT NULL AND num_nonnulls(org_unit_id, program_version_id, cohort_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR (scope_type = 'PROGRAM_VERSION' AND program_version_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, cohort_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR (scope_type = 'COHORT' AND cohort_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, program_version_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR (scope_type = 'CURRICULUM_PATH' AND curriculum_path_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, program_version_id, cohort_id, course_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR (scope_type = 'COURSE' AND course_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, program_version_id, cohort_id, curriculum_path_id, course_offering_id, measurement_period_id, subject_principal_id) = 0) OR (scope_type = 'OFFERING' AND course_offering_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, program_version_id, cohort_id, curriculum_path_id, course_id, measurement_period_id, subject_principal_id) = 0) OR (scope_type = 'MEASUREMENT_PERIOD' AND measurement_period_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, program_version_id, cohort_id, curriculum_path_id, course_id, course_offering_id, subject_principal_id) = 0) OR (scope_type = 'SELF' AND subject_principal_id IS NOT NULL AND num_nonnulls(org_unit_id, program_id, program_version_id, cohort_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id) = 0)),
    CONSTRAINT ck_access_scope_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_access_scope_type CHECK (scope_type IN ('SYSTEM', 'ORG_UNIT', 'PROGRAM', 'PROGRAM_VERSION', 'COHORT', 'CURRICULUM_PATH', 'COURSE', 'OFFERING', 'MEASUREMENT_PERIOD', 'SELF')),
    CONSTRAINT fk_access_scope_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_access_scope_course FOREIGN KEY (course_id) REFERENCES academic.course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_access_scope_course_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_access_scope_curriculum_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_access_scope_measurement_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_access_scope_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_access_scope_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_access_scope_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_access_scope_subject_principal FOREIGN KEY (subject_principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE audit.audit_event (
    occurred_at timestamptz NOT NULL,
    id uuid NOT NULL,
    request_id uuid,
    correlation_id uuid,
    trace_id varchar(64),
    actor_principal_id uuid,
    actor_kind varchar(32) NOT NULL,
    impersonator_principal_id uuid,
    action varchar(128) NOT NULL,
    category varchar(64) NOT NULL,
    outcome varchar(20) NOT NULL,
    resource_type varchar(64) NOT NULL,
    resource_id uuid,
    resource_version bigint,
    org_unit_id uuid,
    program_id uuid,
    program_version_id uuid,
    cohort_id uuid,
    curriculum_path_id uuid,
    course_id uuid,
    course_offering_id uuid,
    measurement_period_id uuid,
    student_id uuid,
    purpose varchar(255),
    reason text,
    classification varchar(20) NOT NULL,
    ip_address inet,
    user_agent_hash char(64),
    auth_method varchar(64),
    before_data jsonb,
    after_data jsonb,
    metadata jsonb,
    chain_id uuid NOT NULL,
    chain_sequence bigint NOT NULL,
    previous_hash char(64),
    event_hash char(64) NOT NULL,
    hash_algorithm varchar(32) NOT NULL,
    canonicalization_version integer NOT NULL,
    CONSTRAINT pk_audit_event PRIMARY KEY (occurred_at, id),
    CONSTRAINT ck_audit_event_canonicalization_version CHECK (canonicalization_version > 0),
    CONSTRAINT ck_audit_event_chain_sequence CHECK (chain_sequence > 0),
    CONSTRAINT ck_audit_event_classification CHECK (classification IN ('PUBLIC', 'INTERNAL', 'CONFIDENTIAL', 'RESTRICTED')),
    CONSTRAINT ck_audit_event_event_hash CHECK (event_hash ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_audit_event_outcome CHECK (outcome IN ('SUCCESS', 'DENIED', 'FAILED')),
    CONSTRAINT ck_audit_event_previous_hash CHECK (previous_hash IS NULL OR previous_hash ~ '^[0-9a-f]{64}$'),
    CONSTRAINT fk_audit_event_actor_principal FOREIGN KEY (actor_principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_audit_event_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_audit_event_course FOREIGN KEY (course_id) REFERENCES academic.course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_audit_event_course_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_audit_event_curriculum_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_audit_event_impersonator_principal FOREIGN KEY (impersonator_principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_audit_event_measurement_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_audit_event_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_audit_event_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_audit_event_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_audit_event_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT
);

CREATE TABLE academic.course_offering_instructor (
    id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    staff_id uuid NOT NULL,
    assignment_role varchar(32) NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    is_primary boolean NOT NULL,
    CONSTRAINT pk_course_offering_instructor PRIMARY KEY (id),
    CONSTRAINT ck_course_offering_instructor_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT fk_course_offering_instructor_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_offering_instructor_staff FOREIGN KEY (staff_id) REFERENCES academic.staff (person_id) ON DELETE RESTRICT
);

CREATE TABLE result.course_pi_result (
    academic_year_start smallint NOT NULL,
    id uuid NOT NULL,
    batch_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    student_id uuid NOT NULL,
    student_path_id uuid NOT NULL,
    course_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    program_pi_id uuid NOT NULL,
    course_pi_score numeric(20,10),
    theta_ind numeric(20,10) NOT NULL,
    attainment_status varchar(24) NOT NULL,
    core_gate_status varchar(24) NOT NULL,
    data_status varchar(24) NOT NULL,
    numerator numeric(20,10),
    denominator numeric(20,10),
    CONSTRAINT pk_course_pi_result PRIMARY KEY (academic_year_start, id),
    CONSTRAINT uq_course_pi_result_covering UNIQUE (academic_year_start, id, batch_id, student_id, student_path_id, program_pi_id, course_offering_id),
    CONSTRAINT ck_course_pi_result_fraction CHECK (num_nonnulls(numerator, denominator) IN (0, 2) AND (denominator IS NULL OR denominator > 0)),
    CONSTRAINT ck_course_pi_result_score CHECK (course_pi_score IS NULL OR (course_pi_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND course_pi_score >= 0 AND course_pi_score <= 100)),
    CONSTRAINT ck_course_pi_result_theta CHECK (theta_ind NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_ind >= 0 AND theta_ind <= 100),
    CONSTRAINT fk_course_pi_result_batch_scope FOREIGN KEY (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) REFERENCES result.result_batch (id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_result_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_result_course FOREIGN KEY (course_id) REFERENCES academic.course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_result_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_result_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_result_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_result_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_result_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_result_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_result_program_version FOREIGN KEY (program_version_id, program_id) REFERENCES academic.program_version (id, program_id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_result_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT,
    CONSTRAINT fk_course_pi_result_student_path FOREIGN KEY (student_path_id) REFERENCES academic.student_path (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.enrollment (
    id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    student_id uuid NOT NULL,
    attempt_no smallint NOT NULL,
    source_system_id uuid NOT NULL,
    source_record_id varchar(255) NOT NULL,
    CONSTRAINT pk_enrollment PRIMARY KEY (id),
    CONSTRAINT "AK_enrollment_id_student_id_course_offering_id_attempt_no" UNIQUE (id, student_id, course_offering_id, attempt_no),
    CONSTRAINT ck_enrollment_attempt_no CHECK (attempt_no > 0),
    CONSTRAINT fk_enrollment_course_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_enrollment_source_system FOREIGN KEY (source_system_id) REFERENCES integration.source_system (id) ON DELETE RESTRICT,
    CONSTRAINT fk_enrollment_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT
);

CREATE TABLE measurement.measurement_period_offering (
    measurement_period_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    academic_year_start smallint NOT NULL,
    planned_source_role varchar(32) NOT NULL,
    collection_status varchar(20) NOT NULL,
    due_at timestamptz,
    CONSTRAINT pk_measurement_period_offering PRIMARY KEY (measurement_period_id, course_offering_id),
    CONSTRAINT fk_measurement_period_offering_course_offering FOREIGN KEY (course_offering_id, program_version_id, academic_year_start) REFERENCES academic.course_offering (id, program_version_id, academic_year_start) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_period_offering_period_binding FOREIGN KEY (measurement_period_id, program_version_id, academic_year_start) REFERENCES measurement.measurement_period (id, program_version_id, academic_year_start) ON DELETE RESTRICT
);

CREATE TABLE governance.resource_security_scope (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    org_unit_id uuid,
    program_id uuid,
    program_version_id uuid,
    cohort_id uuid,
    curriculum_path_id uuid,
    course_id uuid,
    course_offering_id uuid,
    measurement_period_id uuid,
    student_id uuid,
    classification varchar(16) NOT NULL,
    derivation_checksum char(64) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_resource_security_scope PRIMARY KEY (id),
    CONSTRAINT ck_resource_security_scope_checksum CHECK (derivation_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_resource_security_scope_classification CHECK (classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')),
    CONSTRAINT fk_resource_security_scope_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_resource_security_scope_course FOREIGN KEY (course_id) REFERENCES academic.course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_resource_security_scope_course_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_resource_security_scope_curriculum_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_resource_security_scope_measurement_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_resource_security_scope_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_resource_security_scope_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_resource_security_scope_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_resource_security_scope_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_resource_security_scope_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT
);

CREATE TABLE measurement.score_dataset (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    source_system_id uuid NOT NULL,
    academic_year_start smallint NOT NULL,
    course_offering_id uuid NOT NULL,
    classification varchar(20) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_score_dataset PRIMARY KEY (id),
    CONSTRAINT "AK_score_dataset_id_course_offering_id_academic_year_start" UNIQUE (id, course_offering_id, academic_year_start),
    CONSTRAINT ck_score_dataset_academic_year CHECK (academic_year_start BETWEEN 1900 AND 9999),
    CONSTRAINT ck_score_dataset_classification CHECK (classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')),
    CONSTRAINT fk_score_dataset_course_offering_year FOREIGN KEY (course_offering_id, academic_year_start) REFERENCES academic.course_offering (id, academic_year_start) ON DELETE RESTRICT,
    CONSTRAINT fk_score_dataset_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_dataset_source_system FOREIGN KEY (source_system_id) REFERENCES integration.source_system (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.snapshot_offering (
    input_snapshot_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    program_course_id uuid NOT NULL,
    course_version_id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    curriculum_path_id uuid,
    source_role varchar(32) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_snapshot_offering PRIMARY KEY (input_snapshot_id, course_offering_id),
    CONSTRAINT fk_snapshot_offering_course_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_offering_course_version FOREIGN KEY (course_version_id) REFERENCES academic.course_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_offering_curriculum_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_offering_input_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_offering_program_course FOREIGN KEY (program_course_id) REFERENCES academic.program_course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_offering_syllabus_binding FOREIGN KEY (syllabus_version_id, program_course_id, course_version_id) REFERENCES portfolio.syllabus_version (id, program_course_id, course_version_id) ON DELETE RESTRICT
);

CREATE TABLE integration.staging_course_offering (
    id uuid NOT NULL,
    ingestion_batch_id uuid NOT NULL,
    row_no integer NOT NULL,
    raw_record_id bigint NOT NULL,
    offering_code varchar(64) NOT NULL,
    course_code varchar(64) NOT NULL,
    academic_year varchar(16) NOT NULL,
    term_code varchar(32) NOT NULL,
    section_code varchar(64),
    resolved_course_offering_id uuid,
    validation_status varchar(20) NOT NULL,
    row_checksum char(64) NOT NULL,
    CONSTRAINT pk_staging_course_offering PRIMARY KEY (id),
    CONSTRAINT ck_staging_course_offering_checksum CHECK (row_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_staging_course_offering_row_no CHECK (row_no > 0),
    CONSTRAINT ck_staging_course_offering_validation_status CHECK (validation_status IN ('PENDING', 'VALID', 'INVALID', 'PROMOTED')),
    CONSTRAINT fk_staging_course_offering_batch FOREIGN KEY (ingestion_batch_id) REFERENCES integration.ingestion_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_course_offering_raw_record FOREIGN KEY (raw_record_id) REFERENCES integration.raw_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_course_offering_resolved_offering FOREIGN KEY (resolved_course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT
);

CREATE TABLE result.student_clo_result (
    academic_year_start smallint NOT NULL,
    id uuid NOT NULL,
    batch_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    student_id uuid NOT NULL,
    course_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    clo_id uuid NOT NULL,
    score numeric(20,10),
    theta_ind numeric(20,10) NOT NULL,
    attainment_status varchar(24) NOT NULL,
    data_status varchar(24) NOT NULL,
    numerator numeric(20,10),
    denominator numeric(20,10),
    CONSTRAINT pk_student_clo_result PRIMARY KEY (academic_year_start, id),
    CONSTRAINT ck_student_clo_result_fraction CHECK (num_nonnulls(numerator, denominator) IN (0, 2) AND (denominator IS NULL OR denominator > 0)),
    CONSTRAINT ck_student_clo_result_score CHECK (score IS NULL OR (score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND score >= 0 AND score <= 100)),
    CONSTRAINT ck_student_clo_result_theta CHECK (theta_ind NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_ind >= 0 AND theta_ind <= 100),
    CONSTRAINT fk_student_clo_result_batch_scope FOREIGN KEY (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) REFERENCES result.result_batch (id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_clo_result_clo FOREIGN KEY (clo_id) REFERENCES portfolio.clo (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_clo_result_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_clo_result_course FOREIGN KEY (course_id) REFERENCES academic.course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_clo_result_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_clo_result_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_clo_result_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_clo_result_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_clo_result_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_clo_result_program_version FOREIGN KEY (program_version_id, program_id) REFERENCES academic.program_version (id, program_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_clo_result_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.llo_clo_mapping (
    llo_id uuid NOT NULL,
    clo_id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    contribution_ratio numeric(12,10) NOT NULL,
    rationale text,
    CONSTRAINT pk_llo_clo_mapping PRIMARY KEY (llo_id, clo_id),
    CONSTRAINT ck_llo_clo_mapping_contribution CHECK (contribution_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND contribution_ratio > 0 AND contribution_ratio <= 1),
    CONSTRAINT fk_llo_clo_mapping_clo_version FOREIGN KEY (clo_id, syllabus_version_id) REFERENCES portfolio.clo (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_llo_clo_mapping_llo_version FOREIGN KEY (llo_id, syllabus_version_id) REFERENCES portfolio.llo (id, syllabus_version_id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.teaching_session_assessment (
    teaching_session_id uuid NOT NULL,
    assessment_item_id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    CONSTRAINT pk_teaching_session_assessment PRIMARY KEY (teaching_session_id, assessment_item_id),
    CONSTRAINT fk_teaching_session_assessment_item_version FOREIGN KEY (assessment_item_id, syllabus_version_id) REFERENCES portfolio.assessment_item (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_teaching_session_assessment_session_version FOREIGN KEY (teaching_session_id, syllabus_version_id) REFERENCES portfolio.teaching_session (id, syllabus_version_id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.teaching_session_clo (
    teaching_session_id uuid NOT NULL,
    clo_id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    CONSTRAINT pk_teaching_session_clo PRIMARY KEY (teaching_session_id, clo_id),
    CONSTRAINT fk_teaching_session_clo_clo_version FOREIGN KEY (clo_id, syllabus_version_id) REFERENCES portfolio.clo (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_teaching_session_clo_session_version FOREIGN KEY (teaching_session_id, syllabus_version_id) REFERENCES portfolio.teaching_session (id, syllabus_version_id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.teaching_session_llo (
    teaching_session_id uuid NOT NULL,
    llo_id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    CONSTRAINT pk_teaching_session_llo PRIMARY KEY (teaching_session_id, llo_id),
    CONSTRAINT fk_teaching_session_llo_llo_version FOREIGN KEY (llo_id, syllabus_version_id) REFERENCES portfolio.llo (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_teaching_session_llo_session_version FOREIGN KEY (teaching_session_id, syllabus_version_id) REFERENCES portfolio.teaching_session (id, syllabus_version_id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.teaching_session_material (
    teaching_session_id uuid NOT NULL,
    learning_material_id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    CONSTRAINT pk_teaching_session_material PRIMARY KEY (teaching_session_id, learning_material_id),
    CONSTRAINT fk_teaching_session_material_material_version FOREIGN KEY (learning_material_id, syllabus_version_id) REFERENCES portfolio.learning_material (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_teaching_session_material_session_version FOREIGN KEY (teaching_session_id, syllabus_version_id) REFERENCES portfolio.teaching_session (id, syllabus_version_id) ON DELETE RESTRICT
);

CREATE TABLE measurement.snapshot_pi_source_weight (
    input_snapshot_id uuid NOT NULL,
    student_path_id uuid NOT NULL,
    program_pi_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    source_weight_ratio numeric(12,10) NOT NULL,
    source_role varchar(32) NOT NULL,
    anchor_assessment_id uuid,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_snapshot_pi_source_weight PRIMARY KEY (input_snapshot_id, student_path_id, program_pi_id, course_offering_id),
    CONSTRAINT ck_snapshot_pi_source_weight CHECK (source_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND source_weight_ratio > 0 AND source_weight_ratio <= 1),
    CONSTRAINT fk_snapshot_pi_source_weight_anchor FOREIGN KEY (anchor_assessment_id) REFERENCES academic.anchor_assessment (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_pi_source_weight_input_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_pi_source_weight_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_pi_source_weight_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_pi_source_weight_student_path FOREIGN KEY (student_path_id) REFERENCES academic.student_path (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.rubric_criterion (
    id uuid NOT NULL,
    rubric_id uuid NOT NULL,
    assessment_item_id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    criterion_code varchar(64) NOT NULL,
    description text NOT NULL,
    max_score numeric(20,10) NOT NULL,
    rubric_weight_ratio numeric(12,10) NOT NULL,
    score_source_mode varchar(16) NOT NULL,
    is_core boolean NOT NULL,
    individual_evidence boolean NOT NULL,
    sort_order integer NOT NULL,
    CONSTRAINT pk_rubric_criterion PRIMARY KEY (id),
    CONSTRAINT uq_rubric_criterion_full_binding UNIQUE (id, assessment_item_id, syllabus_version_id),
    CONSTRAINT uq_rubric_criterion_id_version UNIQUE (id, syllabus_version_id),
    CONSTRAINT ck_rubric_criterion_code CHECK (criterion_code = upper(btrim(criterion_code)) AND char_length(criterion_code) > 0),
    CONSTRAINT ck_rubric_criterion_max_score CHECK (max_score NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND max_score > 0),
    CONSTRAINT ck_rubric_criterion_score_source_mode CHECK (score_source_mode IN ('CRITERION','QUESTION')),
    CONSTRAINT ck_rubric_criterion_sort_order CHECK (sort_order >= 0),
    CONSTRAINT ck_rubric_criterion_weight CHECK (rubric_weight_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND rubric_weight_ratio > 0 AND rubric_weight_ratio <= 1),
    CONSTRAINT fk_rubric_criterion_assessment_version FOREIGN KEY (assessment_item_id, syllabus_version_id) REFERENCES portfolio.assessment_item (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_rubric_criterion_rubric_binding FOREIGN KEY (rubric_id, assessment_item_id, syllabus_version_id) REFERENCES portfolio.rubric (id, assessment_item_id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_rubric_criterion_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE quality.improvement_finding (
    id uuid NOT NULL,
    improvement_plan_id uuid NOT NULL,
    finding_type varchar(32) NOT NULL,
    academic_year_start smallint,
    cohort_outcome_result_id uuid,
    result_alert_id uuid,
    description text,
    source_checksum char(64),
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_improvement_finding PRIMARY KEY (id),
    CONSTRAINT ck_improvement_finding_checksum CHECK (source_checksum IS NULL OR source_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_improvement_finding_source CHECK (cohort_outcome_result_id IS NOT NULL OR result_alert_id IS NOT NULL OR char_length(btrim(description)) > 0),
    CONSTRAINT ck_improvement_finding_source_year CHECK ((cohort_outcome_result_id IS NULL AND result_alert_id IS NULL) OR academic_year_start IS NOT NULL),
    CONSTRAINT fk_improvement_finding_alert_year FOREIGN KEY (academic_year_start, result_alert_id) REFERENCES result.result_alert (academic_year_start, id) ON DELETE RESTRICT,
    CONSTRAINT fk_improvement_finding_cohort_result_year FOREIGN KEY (academic_year_start, cohort_outcome_result_id) REFERENCES result.cohort_outcome_result (academic_year_start, id) ON DELETE RESTRICT,
    CONSTRAINT fk_improvement_finding_plan FOREIGN KEY (improvement_plan_id) REFERENCES quality.improvement_plan (id) ON DELETE RESTRICT
);

CREATE TABLE ai.chat_session (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    owner_principal_id uuid NOT NULL,
    access_scope_id uuid NOT NULL,
    title varchar(255) NOT NULL,
    status varchar(20) NOT NULL,
    created_at timestamptz NOT NULL,
    last_activity_at timestamptz NOT NULL,
    CONSTRAINT pk_chat_session PRIMARY KEY (id),
    CONSTRAINT ck_chat_session_activity CHECK (last_activity_at >= created_at),
    CONSTRAINT ck_chat_session_status CHECK (status IN ('ACTIVE','CLOSED','ARCHIVED')),
    CONSTRAINT fk_chat_session_access_scope FOREIGN KEY (access_scope_id) REFERENCES iam.access_scope (id) ON DELETE RESTRICT,
    CONSTRAINT fk_chat_session_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_chat_session_owner_principal FOREIGN KEY (owner_principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE iam.database_principal_binding (
    database_role_name varchar(63) NOT NULL,
    effective_from date NOT NULL,
    service_principal_id uuid NOT NULL,
    access_scope_id uuid NOT NULL,
    effective_to date,
    status varchar(20) NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_database_principal_binding PRIMARY KEY (database_role_name, effective_from),
    CONSTRAINT ck_database_principal_binding_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_database_principal_binding_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_database_principal_binding_role_name CHECK (database_role_name = btrim(database_role_name) AND char_length(database_role_name) > 0),
    CONSTRAINT ck_database_principal_binding_status CHECK (status IN ('ACTIVE', 'EXPIRED', 'REVOKED')),
    CONSTRAINT fk_database_principal_binding_access_scope FOREIGN KEY (access_scope_id) REFERENCES iam.access_scope (id) ON DELETE RESTRICT,
    CONSTRAINT fk_database_principal_binding_service_account FOREIGN KEY (service_principal_id) REFERENCES iam.service_account (principal_id) ON DELETE RESTRICT
);

CREATE TABLE audit.export_manifest (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    requested_by uuid NOT NULL,
    purpose text NOT NULL,
    canonical_filter jsonb NOT NULL,
    filter_checksum char(64) NOT NULL,
    report_definition_version varchar(64) NOT NULL,
    access_scope_id uuid NOT NULL,
    permission_snapshot_checksum char(64) NOT NULL,
    data_as_of timestamptz NOT NULL,
    row_count bigint NOT NULL,
    file_object_id uuid NOT NULL,
    watermark varchar(255),
    generator_version varchar(64) NOT NULL,
    checksum char(64) NOT NULL,
    classification varchar(20) NOT NULL,
    expires_at timestamptz NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_export_manifest PRIMARY KEY (id),
    CONSTRAINT ck_export_manifest_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_export_manifest_classification CHECK (classification IN ('PUBLIC', 'INTERNAL', 'CONFIDENTIAL', 'RESTRICTED')),
    CONSTRAINT ck_export_manifest_expiry CHECK (expires_at > created_at),
    CONSTRAINT ck_export_manifest_filter_checksum CHECK (filter_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_export_manifest_permission_checksum CHECK (permission_snapshot_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_export_manifest_purpose CHECK (char_length(btrim(purpose)) > 0),
    CONSTRAINT ck_export_manifest_row_count CHECK (row_count >= 0),
    CONSTRAINT fk_export_manifest_access_scope FOREIGN KEY (access_scope_id) REFERENCES iam.access_scope (id) ON DELETE RESTRICT,
    CONSTRAINT fk_export_manifest_file_object FOREIGN KEY (file_object_id) REFERENCES document.file_object (id) ON DELETE RESTRICT,
    CONSTRAINT fk_export_manifest_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_export_manifest_requested_by FOREIGN KEY (requested_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE iam.idp_group_role_mapping (
    id uuid NOT NULL,
    identity_provider_id uuid NOT NULL,
    external_group_id varchar(255) NOT NULL,
    role_id uuid NOT NULL,
    role_version_id uuid NOT NULL,
    access_scope_id uuid NOT NULL,
    version_no integer NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    status varchar(32) NOT NULL,
    workflow_instance_id uuid NOT NULL,
    supersedes_id uuid,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_idp_group_role_mapping PRIMARY KEY (id),
    CONSTRAINT ck_idp_group_role_mapping_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_idp_group_role_mapping_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_idp_group_role_mapping_group CHECK (external_group_id = btrim(external_group_id) AND char_length(external_group_id) > 0),
    CONSTRAINT ck_idp_group_role_mapping_status CHECK (status IN ('DRAFT', 'IN_REVIEW', 'APPROVED', 'ACTIVE', 'EXPIRED', 'REJECTED')),
    CONSTRAINT ck_idp_group_role_mapping_version CHECK (version_no > 0),
    CONSTRAINT fk_idp_group_role_mapping_access_scope FOREIGN KEY (access_scope_id) REFERENCES iam.access_scope (id) ON DELETE RESTRICT,
    CONSTRAINT fk_idp_group_role_mapping_identity_provider FOREIGN KEY (identity_provider_id) REFERENCES iam.identity_provider (id) ON DELETE RESTRICT,
    CONSTRAINT fk_idp_group_role_mapping_role FOREIGN KEY (role_id) REFERENCES iam.role (id) ON DELETE RESTRICT,
    CONSTRAINT fk_idp_group_role_mapping_role_version FOREIGN KEY (role_version_id, role_id) REFERENCES iam.role_version (id, role_id) ON DELETE RESTRICT,
    CONSTRAINT fk_idp_group_role_mapping_supersedes FOREIGN KEY (supersedes_id) REFERENCES iam.idp_group_role_mapping (id) ON DELETE RESTRICT,
    CONSTRAINT fk_idp_group_role_mapping_workflow_instance FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE ops.operation_job (
    id uuid NOT NULL,
    job_type varchar(32) NOT NULL,
    subject_type varchar(64) NOT NULL,
    subject_id uuid NOT NULL,
    status varchar(32) NOT NULL,
    progress_current bigint NOT NULL,
    progress_total bigint,
    queue_name varchar(64) NOT NULL,
    transport_message_id varchar(255),
    available_at timestamptz NOT NULL,
    priority integer NOT NULL DEFAULT 0,
    attempt_count integer NOT NULL DEFAULT 0,
    max_attempts integer NOT NULL,
    requested_by uuid NOT NULL,
    access_scope_id uuid NOT NULL,
    leased_by_principal_id uuid,
    lease_until timestamptz,
    request_id uuid NOT NULL,
    correlation_id uuid NOT NULL,
    cancel_requested_by uuid,
    cancel_requested_at timestamptz,
    created_at timestamptz NOT NULL,
    started_at timestamptz,
    heartbeat_at timestamptz,
    completed_at timestamptz,
    error_code varchar(64),
    error_detail_redacted text,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_operation_job PRIMARY KEY (id),
    CONSTRAINT ck_operation_job_attempts CHECK (max_attempts > 0 AND attempt_count >= 0 AND attempt_count <= max_attempts),
    CONSTRAINT ck_operation_job_cancel CHECK (num_nonnulls(cancel_requested_by, cancel_requested_at) IN (0, 2)),
    CONSTRAINT ck_operation_job_progress CHECK (progress_current >= 0 AND (progress_total IS NULL OR (progress_total >= 0 AND progress_current <= progress_total))),
    CONSTRAINT ck_operation_job_retryable CHECK (status NOT IN ('QUEUED', 'RETRY_WAIT') OR attempt_count < max_attempts),
    CONSTRAINT ck_operation_job_row_version CHECK (row_version > 0),
    CONSTRAINT ck_operation_job_running_lease CHECK (status <> 'RUNNING' OR (leased_by_principal_id IS NOT NULL AND lease_until IS NOT NULL)),
    CONSTRAINT ck_operation_job_status CHECK (status IN ('QUEUED', 'RETRY_WAIT', 'RUNNING', 'SUCCEEDED', 'FAILED', 'CANCEL_REQUESTED', 'CANCELLED')),
    CONSTRAINT ck_operation_job_terminal CHECK (status NOT IN ('SUCCEEDED', 'FAILED', 'CANCELLED') OR completed_at IS NOT NULL),
    CONSTRAINT ck_operation_job_type CHECK (job_type IN ('IMPORT', 'EXPORT', 'CALCULATION', 'OCR', 'AI', 'WEBHOOK', 'REPORT_REFRESH')),
    CONSTRAINT fk_operation_job_access_scope FOREIGN KEY (access_scope_id) REFERENCES iam.access_scope (id) ON DELETE RESTRICT,
    CONSTRAINT fk_operation_job_cancel_requested_by FOREIGN KEY (cancel_requested_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_operation_job_leased_by FOREIGN KEY (leased_by_principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_operation_job_requested_by FOREIGN KEY (requested_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE result.publication_audience (
    publication_id uuid NOT NULL,
    access_scope_id uuid NOT NULL,
    audience_role varchar(32) NOT NULL,
    allow_student_detail boolean NOT NULL,
    CONSTRAINT pk_publication_audience PRIMARY KEY (publication_id, access_scope_id, audience_role),
    CONSTRAINT fk_publication_audience_access_scope FOREIGN KEY (access_scope_id) REFERENCES iam.access_scope (id) ON DELETE RESTRICT,
    CONSTRAINT fk_publication_audience_publication FOREIGN KEY (publication_id) REFERENCES result.publication (id) ON DELETE RESTRICT
);

CREATE TABLE iam.role_assignment (
    id uuid NOT NULL,
    principal_id uuid NOT NULL,
    role_id uuid NOT NULL,
    role_version_id uuid NOT NULL,
    access_scope_id uuid NOT NULL,
    effective_from timestamptz NOT NULL,
    effective_to timestamptz NOT NULL,
    status varchar(20) NOT NULL,
    source varchar(20) NOT NULL,
    source_reference varchar(255),
    granted_by uuid NOT NULL,
    approved_by uuid,
    workflow_instance_id uuid NOT NULL,
    sod_policy_version_id uuid NOT NULL,
    authorization_snapshot_checksum char(64) NOT NULL,
    requested_by uuid NOT NULL,
    requested_at timestamptz NOT NULL,
    approved_at timestamptz,
    revoked_at timestamptz,
    reason text NOT NULL,
    revoke_reason text,
    CONSTRAINT pk_role_assignment PRIMARY KEY (id),
    CONSTRAINT ck_role_assignment_approval_time CHECK (approved_at IS NULL OR approved_at >= requested_at),
    CONSTRAINT ck_role_assignment_authorization_checksum CHECK (authorization_snapshot_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_role_assignment_effective_range CHECK (effective_to > effective_from),
    CONSTRAINT ck_role_assignment_reason CHECK (char_length(btrim(reason)) > 0),
    CONSTRAINT ck_role_assignment_revocation_time CHECK (revoked_at IS NULL OR revoked_at >= requested_at),
    CONSTRAINT ck_role_assignment_revoke_reason CHECK (revoked_at IS NULL OR char_length(btrim(revoke_reason)) > 0),
    CONSTRAINT ck_role_assignment_source CHECK (source IN ('MANUAL', 'IDP_GROUP', 'IMPORT')),
    CONSTRAINT ck_role_assignment_status CHECK (status IN ('PENDING', 'ACTIVE', 'SUSPENDED', 'REVOKED')),
    CONSTRAINT fk_role_assignment_access_scope FOREIGN KEY (access_scope_id) REFERENCES iam.access_scope (id) ON DELETE RESTRICT,
    CONSTRAINT fk_role_assignment_approved_by FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_role_assignment_granted_by FOREIGN KEY (granted_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_role_assignment_principal FOREIGN KEY (principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_role_assignment_requested_by FOREIGN KEY (requested_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_role_assignment_role FOREIGN KEY (role_id) REFERENCES iam.role (id) ON DELETE RESTRICT,
    CONSTRAINT fk_role_assignment_role_version FOREIGN KEY (role_version_id, role_id) REFERENCES iam.role_version (id, role_id) ON DELETE RESTRICT,
    CONSTRAINT fk_role_assignment_sod_policy_version FOREIGN KEY (sod_policy_version_id) REFERENCES iam.sod_policy_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_role_assignment_workflow_instance FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE iam.sod_exception (
    id uuid NOT NULL,
    rule_id uuid NOT NULL,
    principal_id uuid NOT NULL,
    access_scope_id uuid NOT NULL,
    reason text NOT NULL,
    effective_from date NOT NULL,
    effective_to date NOT NULL,
    decision_id uuid NOT NULL,
    approved_by uuid NOT NULL,
    CONSTRAINT pk_sod_exception PRIMARY KEY (id),
    CONSTRAINT ck_sod_exception_effective_range CHECK (effective_to > effective_from),
    CONSTRAINT ck_sod_exception_reason CHECK (char_length(btrim(reason)) > 0),
    CONSTRAINT fk_sod_exception_access_scope FOREIGN KEY (access_scope_id) REFERENCES iam.access_scope (id) ON DELETE RESTRICT,
    CONSTRAINT fk_sod_exception_approved_by FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_sod_exception_decision FOREIGN KEY (decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_sod_exception_principal FOREIGN KEY (principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_sod_exception_rule FOREIGN KEY (rule_id) REFERENCES iam.sod_rule (id) ON DELETE RESTRICT
);

CREATE TABLE integration.webhook_subscription (
    id uuid NOT NULL,
    principal_id uuid NOT NULL,
    access_scope_id uuid NOT NULL,
    endpoint_url varchar(2048) NOT NULL,
    secret_reference varchar(512) NOT NULL,
    signing_algorithm varchar(32) NOT NULL,
    key_version integer NOT NULL,
    status varchar(20) NOT NULL,
    verified_at timestamptz,
    created_at timestamptz NOT NULL,
    expires_at timestamptz,
    CONSTRAINT pk_webhook_subscription PRIMARY KEY (id),
    CONSTRAINT ck_webhook_subscription_endpoint CHECK (endpoint_url ~ '^https://' AND char_length(endpoint_url) <= 2048),
    CONSTRAINT ck_webhook_subscription_expiry CHECK (expires_at IS NULL OR expires_at > created_at),
    CONSTRAINT ck_webhook_subscription_key_version CHECK (key_version > 0),
    CONSTRAINT fk_webhook_subscription_access_scope FOREIGN KEY (access_scope_id) REFERENCES iam.access_scope (id) ON DELETE RESTRICT,
    CONSTRAINT fk_webhook_subscription_principal FOREIGN KEY (principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.enrollment_revision (
    id uuid NOT NULL,
    enrollment_id uuid NOT NULL,
    revision_no integer NOT NULL,
    enrollment_status varchar(20) NOT NULL,
    repeat_flag boolean NOT NULL,
    improvement_flag boolean NOT NULL,
    effective_from timestamptz NOT NULL,
    effective_to timestamptz,
    source_updated_at timestamptz,
    ingestion_batch_id uuid NOT NULL,
    supersedes_id uuid,
    recorded_at timestamptz NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_enrollment_revision PRIMARY KEY (id),
    CONSTRAINT "AK_enrollment_revision_enrollment_id_id" UNIQUE (enrollment_id, id),
    CONSTRAINT ck_enrollment_revision_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_enrollment_revision_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_enrollment_revision_no CHECK (revision_no > 0),
    CONSTRAINT ck_enrollment_revision_status CHECK (enrollment_status IN ('ENROLLED','COMPLETED','ABSENT','DEFERRED','WITHDRAWN','CANCELLED','RECOGNIZED')),
    CONSTRAINT fk_enrollment_revision_enrollment FOREIGN KEY (enrollment_id) REFERENCES measurement.enrollment (id) ON DELETE RESTRICT,
    CONSTRAINT fk_enrollment_revision_ingestion_batch FOREIGN KEY (ingestion_batch_id) REFERENCES integration.ingestion_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_enrollment_revision_supersedes FOREIGN KEY (enrollment_id, supersedes_id) REFERENCES measurement.enrollment_revision (enrollment_id, id) ON DELETE RESTRICT
);

CREATE TABLE integration.staging_enrollment (
    id uuid NOT NULL,
    ingestion_batch_id uuid NOT NULL,
    row_no integer NOT NULL,
    raw_record_id bigint NOT NULL,
    student_code varchar(64) NOT NULL,
    offering_code varchar(64) NOT NULL,
    enrollment_status varchar(32) NOT NULL,
    resolved_enrollment_id uuid,
    validation_status varchar(20) NOT NULL,
    row_checksum char(64) NOT NULL,
    CONSTRAINT pk_staging_enrollment PRIMARY KEY (id),
    CONSTRAINT ck_staging_enrollment_checksum CHECK (row_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_staging_enrollment_row_no CHECK (row_no > 0),
    CONSTRAINT ck_staging_enrollment_validation_status CHECK (validation_status IN ('PENDING', 'VALID', 'INVALID', 'PROMOTED')),
    CONSTRAINT fk_staging_enrollment_batch FOREIGN KEY (ingestion_batch_id) REFERENCES integration.ingestion_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_enrollment_raw_record FOREIGN KEY (raw_record_id) REFERENCES integration.raw_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_enrollment_resolved_enrollment FOREIGN KEY (resolved_enrollment_id) REFERENCES measurement.enrollment (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.measurement_period_target (
    id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    outcome_level varchar(20) NOT NULL,
    course_offering_id uuid,
    syllabus_version_id uuid,
    clo_id uuid,
    program_pi_id uuid,
    program_plo_id uuid,
    target_role varchar(32) NOT NULL,
    CONSTRAINT pk_measurement_period_target PRIMARY KEY (id),
    CONSTRAINT ck_measurement_period_target_level CHECK (outcome_level IN ('CLO','PI','PLO')),
    CONSTRAINT ck_measurement_period_target_outcome CHECK (num_nonnulls(clo_id, program_pi_id, program_plo_id) = 1),
    CONSTRAINT ck_measurement_period_target_shape CHECK ((outcome_level = 'CLO' AND clo_id IS NOT NULL AND program_pi_id IS NULL AND program_plo_id IS NULL AND course_offering_id IS NOT NULL AND syllabus_version_id IS NOT NULL) OR (outcome_level = 'PI' AND clo_id IS NULL AND program_pi_id IS NOT NULL AND program_plo_id IS NULL AND course_offering_id IS NULL AND syllabus_version_id IS NULL) OR (outcome_level = 'PLO' AND clo_id IS NULL AND program_pi_id IS NULL AND program_plo_id IS NOT NULL AND course_offering_id IS NULL AND syllabus_version_id IS NULL)),
    CONSTRAINT fk_measurement_period_target_clo_syllabus FOREIGN KEY (clo_id, syllabus_version_id) REFERENCES portfolio.clo (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_period_target_course_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_period_target_period_offering FOREIGN KEY (measurement_period_id, course_offering_id) REFERENCES measurement.measurement_period_offering (measurement_period_id, course_offering_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_period_target_period_program FOREIGN KEY (measurement_period_id, program_version_id) REFERENCES measurement.measurement_period (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_period_target_program_pi FOREIGN KEY (program_pi_id, program_version_id) REFERENCES academic.program_pi (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_period_target_program_plo FOREIGN KEY (program_plo_id, program_version_id) REFERENCES academic.program_plo (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_period_target_syllabus_program FOREIGN KEY (syllabus_version_id, program_version_id) REFERENCES portfolio.syllabus_version (id, program_version_id) ON DELETE RESTRICT
);

CREATE TABLE measurement.measurement_threshold_override (
    id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    outcome_level varchar(20) NOT NULL,
    course_offering_id uuid,
    syllabus_version_id uuid,
    clo_id uuid,
    program_pi_id uuid,
    program_plo_id uuid,
    theta_ind numeric(20,10) NOT NULL,
    theta_coh numeric(20,10) NOT NULL,
    near_threshold numeric(20,10),
    min_sample_size integer,
    reason text NOT NULL,
    workflow_instance_id uuid NOT NULL,
    CONSTRAINT pk_measurement_threshold_override PRIMARY KEY (id),
    CONSTRAINT ck_measurement_threshold_override_level CHECK (outcome_level IN ('CLO','PI','PLO')),
    CONSTRAINT ck_measurement_threshold_override_outcome CHECK (num_nonnulls(clo_id, program_pi_id, program_plo_id) = 1),
    CONSTRAINT ck_measurement_threshold_override_shape CHECK ((outcome_level = 'CLO' AND clo_id IS NOT NULL AND program_pi_id IS NULL AND program_plo_id IS NULL AND course_offering_id IS NOT NULL AND syllabus_version_id IS NOT NULL) OR (outcome_level = 'PI' AND clo_id IS NULL AND program_pi_id IS NOT NULL AND program_plo_id IS NULL AND course_offering_id IS NULL AND syllabus_version_id IS NULL) OR (outcome_level = 'PLO' AND clo_id IS NULL AND program_pi_id IS NULL AND program_plo_id IS NOT NULL AND course_offering_id IS NULL AND syllabus_version_id IS NULL)),
    CONSTRAINT ck_measurement_threshold_override_values CHECK (theta_ind NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_coh NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND theta_ind BETWEEN 0 AND 100 AND theta_coh BETWEEN 0 AND 100 AND (near_threshold IS NULL OR near_threshold NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND near_threshold BETWEEN 0 AND 100) AND (min_sample_size IS NULL OR min_sample_size > 0)),
    CONSTRAINT fk_measurement_threshold_override_clo_syllabus FOREIGN KEY (clo_id, syllabus_version_id) REFERENCES portfolio.clo (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_threshold_override_course_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_threshold_override_period_offering FOREIGN KEY (measurement_period_id, course_offering_id) REFERENCES measurement.measurement_period_offering (measurement_period_id, course_offering_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_threshold_override_period_program FOREIGN KEY (measurement_period_id, program_version_id) REFERENCES measurement.measurement_period (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_threshold_override_program_pi FOREIGN KEY (program_pi_id, program_version_id) REFERENCES academic.program_pi (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_threshold_override_program_plo FOREIGN KEY (program_plo_id, program_version_id) REFERENCES academic.program_plo (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_threshold_override_syllabus_program FOREIGN KEY (syllabus_version_id, program_version_id) REFERENCES portfolio.syllabus_version (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_measurement_threshold_override_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE result.student_pi_source_contribution (
    academic_year_start smallint NOT NULL,
    student_pi_result_id uuid NOT NULL,
    course_pi_result_id uuid NOT NULL,
    batch_id uuid NOT NULL,
    input_snapshot_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    course_id uuid NOT NULL,
    student_id uuid NOT NULL,
    student_path_id uuid NOT NULL,
    program_pi_id uuid NOT NULL,
    method varchar(16) NOT NULL,
    course_offering_id uuid NOT NULL,
    source_weight_ratio numeric(12,10) NOT NULL,
    weighted_contribution numeric(20,10) NOT NULL,
    source_role varchar(32) NOT NULL,
    anchor_assessment_id uuid,
    CONSTRAINT pk_student_pi_source_contribution PRIMARY KEY (academic_year_start, student_pi_result_id, course_pi_result_id),
    CONSTRAINT ck_student_pi_source_contribution_method CHECK (method IN ('DIRECT', 'INDIRECT', 'COMBINED')),
    CONSTRAINT ck_student_pi_source_contribution_weight CHECK (source_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND source_weight_ratio > 0 AND source_weight_ratio <= 1 AND weighted_contribution NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)),
    CONSTRAINT fk_student_pi_source_contribution_anchor FOREIGN KEY (anchor_assessment_id) REFERENCES academic.anchor_assessment (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_batch_snapshot_scope FOREIGN KEY (batch_id, input_snapshot_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) REFERENCES result.result_batch (id, input_snapshot_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_course FOREIGN KEY (course_id) REFERENCES academic.course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_course_result FOREIGN KEY (academic_year_start, course_pi_result_id, batch_id, student_id, student_path_id, program_pi_id, course_offering_id) REFERENCES result.course_pi_result (academic_year_start, id, batch_id, student_id, student_path_id, program_pi_id, course_offering_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_pi_result FOREIGN KEY (academic_year_start, student_pi_result_id, batch_id, student_id, student_path_id, program_pi_id, method) REFERENCES result.student_pi_result (academic_year_start, id, batch_id, student_id, student_path_id, program_pi_id, method) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_program_version FOREIGN KEY (program_version_id, program_id) REFERENCES academic.program_version (id, program_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_snapshot_weight FOREIGN KEY (input_snapshot_id, student_path_id, program_pi_id, course_offering_id) REFERENCES measurement.snapshot_pi_source_weight (input_snapshot_id, student_path_id, program_pi_id, course_offering_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_pi_source_contribution_student_path FOREIGN KEY (student_path_id) REFERENCES academic.student_path (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.grader_assignment (
    id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    assessment_item_id uuid NOT NULL,
    rubric_criterion_id uuid NOT NULL,
    principal_id uuid NOT NULL,
    assignment_role varchar(20) NOT NULL,
    effective_from timestamptz NOT NULL,
    effective_to timestamptz,
    assigned_by uuid NOT NULL,
    CONSTRAINT pk_grader_assignment PRIMARY KEY (id),
    CONSTRAINT ck_grader_assignment_assignment_role CHECK (assignment_role IN ('SCORER', 'CHECKER', 'APPROVER')),
    CONSTRAINT ck_grader_assignment_effective_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT fk_grader_assignment_assessment FOREIGN KEY (assessment_item_id, syllabus_version_id) REFERENCES portfolio.assessment_item (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_grader_assignment_assigned_by FOREIGN KEY (assigned_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_grader_assignment_course_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_grader_assignment_criterion FOREIGN KEY (rubric_criterion_id, assessment_item_id, syllabus_version_id) REFERENCES portfolio.rubric_criterion (id, assessment_item_id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_grader_assignment_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_grader_assignment_period_offering FOREIGN KEY (measurement_period_id, course_offering_id) REFERENCES measurement.measurement_period_offering (measurement_period_id, course_offering_id) ON DELETE RESTRICT,
    CONSTRAINT fk_grader_assignment_principal FOREIGN KEY (principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_grader_assignment_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.question_criterion_mapping (
    question_id uuid NOT NULL,
    rubric_criterion_id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    criterion_weight_ratio numeric(12,10) NOT NULL,
    CONSTRAINT pk_question_criterion_mapping PRIMARY KEY (question_id, rubric_criterion_id),
    CONSTRAINT ck_question_criterion_mapping_weight CHECK (criterion_weight_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND criterion_weight_ratio > 0 AND criterion_weight_ratio <= 1),
    CONSTRAINT fk_question_criterion_mapping_criterion_version FOREIGN KEY (rubric_criterion_id, syllabus_version_id) REFERENCES portfolio.rubric_criterion (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_question_criterion_mapping_question_version FOREIGN KEY (question_id, syllabus_version_id) REFERENCES portfolio.assessment_question (id, syllabus_version_id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.rubric_level (
    id uuid NOT NULL,
    rubric_criterion_id uuid NOT NULL,
    level_code varchar(64) NOT NULL,
    level_order integer NOT NULL,
    label varchar(255) NOT NULL,
    description text,
    score_from numeric(20,10) NOT NULL,
    score_to numeric(20,10) NOT NULL,
    numeric_value numeric(20,10),
    score_range numrange GENERATED ALWAYS AS (numrange(score_from, score_to, '[)')) STORED NOT NULL,
    CONSTRAINT pk_rubric_level PRIMARY KEY (id),
    CONSTRAINT ck_rubric_level_order CHECK (level_order >= 0),
    CONSTRAINT ck_rubric_level_range CHECK (score_from NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND score_to NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND score_from < score_to),
    CONSTRAINT fk_rubric_level_criterion FOREIGN KEY (rubric_criterion_id) REFERENCES portfolio.rubric_criterion (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.score_identity (
    id uuid NOT NULL,
    score_dataset_id uuid NOT NULL,
    academic_year_start smallint NOT NULL,
    student_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    attempt_no smallint NOT NULL,
    enrollment_id uuid NOT NULL,
    assessment_item_id uuid NOT NULL,
    rubric_criterion_id uuid,
    assessment_question_id uuid,
    score_level varchar(20) NOT NULL,
    CONSTRAINT pk_score_identity PRIMARY KEY (id),
    CONSTRAINT uq_score_identity_scope UNIQUE (academic_year_start, id, student_id, course_offering_id),
    CONSTRAINT ck_score_identity_attempt_no CHECK (attempt_no > 0),
    CONSTRAINT ck_score_identity_level CHECK (score_level IN ('ASSESSMENT','CRITERION','QUESTION')),
    CONSTRAINT ck_score_identity_shape CHECK ((score_level = 'ASSESSMENT' AND rubric_criterion_id IS NULL AND assessment_question_id IS NULL) OR (score_level = 'CRITERION' AND rubric_criterion_id IS NOT NULL AND assessment_question_id IS NULL) OR (score_level = 'QUESTION' AND rubric_criterion_id IS NULL AND assessment_question_id IS NOT NULL)),
    CONSTRAINT fk_score_identity_assessment FOREIGN KEY (assessment_item_id, syllabus_version_id) REFERENCES portfolio.assessment_item (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_identity_criterion FOREIGN KEY (rubric_criterion_id, assessment_item_id, syllabus_version_id) REFERENCES portfolio.rubric_criterion (id, assessment_item_id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_identity_dataset_scope FOREIGN KEY (score_dataset_id, course_offering_id, academic_year_start) REFERENCES measurement.score_dataset (id, course_offering_id, academic_year_start) ON DELETE RESTRICT,
    CONSTRAINT fk_score_identity_enrollment FOREIGN KEY (enrollment_id, student_id, course_offering_id, attempt_no) REFERENCES measurement.enrollment (id, student_id, course_offering_id, attempt_no) ON DELETE RESTRICT,
    CONSTRAINT fk_score_identity_offering_binding FOREIGN KEY (course_offering_id, program_version_id, syllabus_version_id, academic_year_start) REFERENCES academic.course_offering (id, program_version_id, syllabus_version_id, academic_year_start) ON DELETE RESTRICT,
    CONSTRAINT fk_score_identity_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_identity_question FOREIGN KEY (assessment_question_id, assessment_item_id, syllabus_version_id) REFERENCES portfolio.assessment_question (id, assessment_item_id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_identity_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_identity_syllabus_version FOREIGN KEY (syllabus_version_id) REFERENCES portfolio.syllabus_version (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.snapshot_question_criterion_weight (
    input_snapshot_id uuid NOT NULL,
    assessment_question_id uuid NOT NULL,
    rubric_criterion_id uuid NOT NULL,
    source_mode varchar(16) NOT NULL,
    criterion_weight_ratio numeric(12,10) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_snapshot_question_criterion_weight PRIMARY KEY (input_snapshot_id, assessment_question_id, rubric_criterion_id),
    CONSTRAINT ck_snapshot_question_criterion_source_mode CHECK (source_mode IN ('QUESTION','CRITERION')),
    CONSTRAINT ck_snapshot_question_criterion_weight CHECK (criterion_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND criterion_weight_ratio > 0 AND criterion_weight_ratio <= 1),
    CONSTRAINT fk_snapshot_question_criterion_criterion FOREIGN KEY (rubric_criterion_id) REFERENCES portfolio.rubric_criterion (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_question_criterion_input_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_question_criterion_question FOREIGN KEY (assessment_question_id) REFERENCES portfolio.assessment_question (id) ON DELETE RESTRICT
);

CREATE TABLE integration.staging_rubric_criterion (
    id uuid NOT NULL,
    ingestion_batch_id uuid NOT NULL,
    row_no integer NOT NULL,
    raw_record_id bigint NOT NULL,
    rubric_code varchar(64) NOT NULL,
    criterion_code varchar(64) NOT NULL,
    name varchar(255) NOT NULL,
    max_score numeric(20,10),
    resolved_rubric_criterion_id uuid,
    validation_status varchar(20) NOT NULL,
    row_checksum char(64) NOT NULL,
    CONSTRAINT pk_staging_rubric_criterion PRIMARY KEY (id),
    CONSTRAINT ck_staging_rubric_criterion_checksum CHECK (row_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_staging_rubric_criterion_max_score CHECK (max_score IS NULL OR (max_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_score > 0)),
    CONSTRAINT ck_staging_rubric_criterion_row_no CHECK (row_no > 0),
    CONSTRAINT ck_staging_rubric_criterion_validation_status CHECK (validation_status IN ('PENDING', 'VALID', 'INVALID', 'PROMOTED')),
    CONSTRAINT fk_staging_rubric_criterion_batch FOREIGN KEY (ingestion_batch_id) REFERENCES integration.ingestion_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_rubric_criterion_raw_record FOREIGN KEY (raw_record_id) REFERENCES integration.raw_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_rubric_criterion_resolved_criterion FOREIGN KEY (resolved_rubric_criterion_id) REFERENCES portfolio.rubric_criterion (id) ON DELETE RESTRICT
);

CREATE TABLE result.student_criterion_result (
    academic_year_start smallint NOT NULL,
    id uuid NOT NULL,
    batch_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    student_id uuid NOT NULL,
    student_path_id uuid NOT NULL,
    course_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    assessment_item_id uuid NOT NULL,
    rubric_criterion_id uuid NOT NULL,
    source_mode varchar(16) NOT NULL,
    score numeric(20,10),
    max_score numeric(20,10) NOT NULL,
    normalized_score numeric(20,10),
    data_status varchar(24) NOT NULL,
    numerator numeric(20,10),
    denominator numeric(20,10),
    CONSTRAINT pk_student_criterion_result PRIMARY KEY (academic_year_start, id),
    CONSTRAINT uq_student_criterion_result_covering UNIQUE (academic_year_start, id, batch_id, student_id, course_offering_id, rubric_criterion_id),
    CONSTRAINT ck_student_criterion_result_fraction CHECK (num_nonnulls(numerator, denominator) IN (0, 2) AND (denominator IS NULL OR denominator > 0)),
    CONSTRAINT ck_student_criterion_result_scores CHECK (max_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_score > 0 AND (score IS NULL OR score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)) AND (normalized_score IS NULL OR (normalized_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND normalized_score >= 0 AND normalized_score <= 100))),
    CONSTRAINT ck_student_criterion_result_source_mode CHECK (source_mode IN ('CRITERION', 'QUESTION')),
    CONSTRAINT fk_student_criterion_result_assessment FOREIGN KEY (assessment_item_id) REFERENCES portfolio.assessment_item (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_result_batch_scope FOREIGN KEY (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) REFERENCES result.result_batch (id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_result_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_result_course FOREIGN KEY (course_id) REFERENCES academic.course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_result_criterion FOREIGN KEY (rubric_criterion_id) REFERENCES portfolio.rubric_criterion (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_result_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_result_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_result_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_result_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_result_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_result_program_version FOREIGN KEY (program_version_id, program_id) REFERENCES academic.program_version (id, program_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_result_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_result_student_path FOREIGN KEY (student_path_id) REFERENCES academic.student_path (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.syllabus_traceability (
    id uuid NOT NULL,
    syllabus_version_id uuid NOT NULL,
    program_course_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    clo_id uuid NOT NULL,
    course_pi_mapping_id uuid,
    rubric_criterion_id uuid NOT NULL,
    data_role varchar(16) NOT NULL,
    evidence_requirement text,
    allocation_ratio numeric(12,10),
    exception_decision_id uuid,
    rationale text,
    CONSTRAINT pk_syllabus_traceability PRIMARY KEY (id),
    CONSTRAINT ck_syllabus_traceability_allocation CHECK (allocation_ratio IS NULL OR (allocation_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND allocation_ratio > 0 AND allocation_ratio <= 1)),
    CONSTRAINT ck_syllabus_traceability_data_role CHECK (data_role IN ('DIRECT_PI','SUPPORT_PI','CLO_ONLY')),
    CONSTRAINT ck_syllabus_traceability_exception CHECK (exception_decision_id IS NULL OR (allocation_ratio IS NOT NULL AND rationale IS NOT NULL AND char_length(btrim(rationale)) > 0)),
    CONSTRAINT ck_syllabus_traceability_pi_binding CHECK ((data_role = 'CLO_ONLY' AND course_pi_mapping_id IS NULL) OR (data_role IN ('DIRECT_PI','SUPPORT_PI') AND course_pi_mapping_id IS NOT NULL)),
    CONSTRAINT fk_syllabus_traceability_clo_version FOREIGN KEY (clo_id, syllabus_version_id) REFERENCES portfolio.clo (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_traceability_course_pi_mapping FOREIGN KEY (course_pi_mapping_id, program_course_id, program_version_id) REFERENCES academic.course_pi_mapping (id, program_course_id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_traceability_criterion_version FOREIGN KEY (rubric_criterion_id, syllabus_version_id) REFERENCES portfolio.rubric_criterion (id, syllabus_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_traceability_exception_decision FOREIGN KEY (exception_decision_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_traceability_program_course FOREIGN KEY (program_course_id, program_version_id) REFERENCES academic.program_course (id, program_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_traceability_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_syllabus_traceability_syllabus_binding FOREIGN KEY (syllabus_version_id, program_course_id, program_version_id) REFERENCES portfolio.syllabus_version (id, program_course_id, program_version_id) ON DELETE RESTRICT
);

CREATE TABLE quality.plan_waiver (
    id uuid NOT NULL,
    finding_id uuid NOT NULL,
    reason text NOT NULL,
    requested_by uuid NOT NULL,
    workflow_instance_id uuid NOT NULL,
    expires_at timestamptz NOT NULL,
    CONSTRAINT pk_plan_waiver PRIMARY KEY (id),
    CONSTRAINT ck_plan_waiver_reason CHECK (char_length(btrim(reason)) > 0),
    CONSTRAINT fk_plan_waiver_finding FOREIGN KEY (finding_id) REFERENCES quality.improvement_finding (id) ON DELETE RESTRICT,
    CONSTRAINT fk_plan_waiver_requested_by FOREIGN KEY (requested_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_plan_waiver_workflow FOREIGN KEY (workflow_instance_id) REFERENCES workflow.instance (id) ON DELETE RESTRICT
);

CREATE TABLE ai.ai_source_snapshot (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    source_kind varchar(32) NOT NULL,
    source_governed_resource_id uuid NOT NULL,
    document_version_id uuid,
    result_batch_id uuid,
    export_manifest_id uuid,
    improvement_plan_id uuid,
    source_checksum char(64) NOT NULL,
    data_as_of timestamptz NOT NULL,
    scope_snapshot_checksum char(64) NOT NULL,
    permission_snapshot_checksum char(64) NOT NULL,
    snapshot_payload_reference varchar(512) NOT NULL,
    CONSTRAINT pk_ai_source_snapshot PRIMARY KEY (id),
    CONSTRAINT uq_ai_source_snapshot_id_checksum UNIQUE (id, source_checksum),
    CONSTRAINT ck_ai_source_snapshot_checksums CHECK (source_checksum ~ '^[0-9a-f]{64}$' AND scope_snapshot_checksum ~ '^[0-9a-f]{64}$' AND permission_snapshot_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_ai_source_snapshot_distinct_resource CHECK (governed_resource_id <> source_governed_resource_id),
    CONSTRAINT ck_ai_source_snapshot_source CHECK ((source_kind = 'DOCUMENT_VERSION' AND document_version_id IS NOT NULL AND num_nonnulls(result_batch_id, export_manifest_id, improvement_plan_id) = 0) OR (source_kind = 'RESULT_BATCH' AND result_batch_id IS NOT NULL AND num_nonnulls(document_version_id, export_manifest_id, improvement_plan_id) = 0) OR (source_kind = 'EXPORT_MANIFEST' AND export_manifest_id IS NOT NULL AND num_nonnulls(document_version_id, result_batch_id, improvement_plan_id) = 0) OR (source_kind = 'IMPROVEMENT_PLAN' AND improvement_plan_id IS NOT NULL AND num_nonnulls(document_version_id, result_batch_id, export_manifest_id) = 0)),
    CONSTRAINT fk_ai_source_snapshot_document_version FOREIGN KEY (document_version_id) REFERENCES document.document_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_source_snapshot_export_manifest FOREIGN KEY (export_manifest_id) REFERENCES audit.export_manifest (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_source_snapshot_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_source_snapshot_improvement_plan FOREIGN KEY (improvement_plan_id) REFERENCES quality.improvement_plan (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_source_snapshot_result_batch FOREIGN KEY (result_batch_id) REFERENCES result.result_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_source_snapshot_source_governed_resource FOREIGN KEY (source_governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT
);

CREATE TABLE audit.export_manifest_batch (
    export_manifest_id uuid NOT NULL,
    result_batch_id uuid NOT NULL,
    CONSTRAINT pk_export_manifest_batch PRIMARY KEY (export_manifest_id, result_batch_id),
    CONSTRAINT fk_export_manifest_batch_manifest FOREIGN KEY (export_manifest_id) REFERENCES audit.export_manifest (id) ON DELETE RESTRICT,
    CONSTRAINT fk_export_manifest_batch_result_batch FOREIGN KEY (result_batch_id) REFERENCES result.result_batch (id) ON DELETE RESTRICT
);

CREATE TABLE ops.job_attempt (
    operation_job_id uuid NOT NULL,
    attempt_no integer NOT NULL,
    worker_id varchar(128) NOT NULL,
    started_at timestamptz NOT NULL,
    heartbeat_at timestamptz,
    finished_at timestamptz,
    outcome varchar(32),
    error_code varchar(64),
    log_reference varchar(1024),
    CONSTRAINT pk_job_attempt PRIMARY KEY (operation_job_id, attempt_no),
    CONSTRAINT ck_job_attempt_number CHECK (attempt_no > 0),
    CONSTRAINT ck_job_attempt_times CHECK ((heartbeat_at IS NULL OR heartbeat_at >= started_at) AND (finished_at IS NULL OR finished_at >= started_at)),
    CONSTRAINT fk_job_attempt_operation_job FOREIGN KEY (operation_job_id) REFERENCES ops.operation_job (id) ON DELETE RESTRICT
);

CREATE TABLE integration.webhook_delivery (
    id uuid NOT NULL,
    subscription_id uuid NOT NULL,
    outbox_message_id uuid NOT NULL,
    payload_checksum char(64) NOT NULL,
    status varchar(32) NOT NULL,
    attempt_count integer NOT NULL,
    next_retry_at timestamptz,
    delivered_at timestamptz,
    CONSTRAINT pk_webhook_delivery PRIMARY KEY (id),
    CONSTRAINT ck_webhook_delivery_attempt_count CHECK (attempt_count >= 0),
    CONSTRAINT ck_webhook_delivery_checksum CHECK (payload_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT fk_webhook_delivery_outbox_message FOREIGN KEY (outbox_message_id) REFERENCES integration.outbox_message (id) ON DELETE RESTRICT,
    CONSTRAINT fk_webhook_delivery_subscription FOREIGN KEY (subscription_id) REFERENCES integration.webhook_subscription (id) ON DELETE RESTRICT
);

CREATE TABLE integration.webhook_subscription_event (
    subscription_id uuid NOT NULL,
    event_type varchar(128) NOT NULL,
    CONSTRAINT pk_webhook_subscription_event PRIMARY KEY (subscription_id, event_type),
    CONSTRAINT ck_webhook_subscription_event_type CHECK (event_type = btrim(event_type) AND char_length(event_type) > 0),
    CONSTRAINT fk_webhook_subscription_event_subscription FOREIGN KEY (subscription_id) REFERENCES integration.webhook_subscription (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.period_population_enrollment (
    measurement_period_id uuid NOT NULL,
    student_id uuid NOT NULL,
    enrollment_revision_id uuid NOT NULL,
    selection_role varchar(32) NOT NULL,
    CONSTRAINT pk_period_population_enrollment PRIMARY KEY (measurement_period_id, student_id, enrollment_revision_id),
    CONSTRAINT fk_period_population_enrollment_member FOREIGN KEY (measurement_period_id, student_id) REFERENCES measurement.period_population_member (measurement_period_id, student_id) ON DELETE RESTRICT,
    CONSTRAINT fk_period_population_enrollment_revision FOREIGN KEY (enrollment_revision_id) REFERENCES measurement.enrollment_revision (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.snapshot_enrollment (
    input_snapshot_id uuid NOT NULL,
    enrollment_revision_id uuid NOT NULL,
    student_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    attempt_no smallint NOT NULL,
    revision_no integer NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_snapshot_enrollment PRIMARY KEY (input_snapshot_id, enrollment_revision_id),
    CONSTRAINT ck_snapshot_enrollment_attempt CHECK (attempt_no > 0),
    CONSTRAINT ck_snapshot_enrollment_revision CHECK (revision_no > 0),
    CONSTRAINT fk_snapshot_enrollment_course_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_enrollment_input_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_enrollment_revision FOREIGN KEY (enrollment_revision_id) REFERENCES measurement.enrollment_revision (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_enrollment_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT
);

CREATE TABLE measurement.score_record (
    academic_year_start smallint NOT NULL,
    id uuid NOT NULL,
    score_identity_id uuid NOT NULL,
    student_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    course_id uuid NOT NULL,
    revision_no integer NOT NULL,
    raw_score numeric(20,10),
    max_score numeric(20,10) NOT NULL,
    score_status varchar(20) NOT NULL,
    source_system_id uuid NOT NULL,
    source_record_id varchar(255) NOT NULL,
    source_revision varchar(128) NOT NULL,
    ingestion_batch_id uuid NOT NULL,
    supersedes_id uuid,
    correction_reason text,
    recorded_by uuid NOT NULL,
    recorded_at timestamptz NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_score_record PRIMARY KEY (academic_year_start, id),
    CONSTRAINT "AK_score_record_academic_year_start_id_student_id_course_offer~" UNIQUE (academic_year_start, id, student_id, course_offering_id),
    CONSTRAINT "AK_score_record_academic_year_start_score_identity_id_id" UNIQUE (academic_year_start, score_identity_id, id),
    CONSTRAINT ck_score_record_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_score_record_correction CHECK (supersedes_id IS NULL OR (correction_reason IS NOT NULL AND char_length(btrim(correction_reason)) > 0)),
    CONSTRAINT ck_score_record_max_score CHECK (max_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_score > 0),
    CONSTRAINT ck_score_record_revision_no CHECK (revision_no > 0),
    CONSTRAINT ck_score_record_status CHECK (score_status IN ('SCORED','ABSENT','EXCUSED','NOT_SUBMITTED','DEFERRED','WITHDRAWN','MISSING')),
    CONSTRAINT ck_score_record_value_shape CHECK ((score_status = 'SCORED' AND raw_score IS NOT NULL AND raw_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND raw_score >= 0 AND raw_score <= max_score) OR (score_status <> 'SCORED' AND raw_score IS NULL)),
    CONSTRAINT fk_score_record_course FOREIGN KEY (course_id) REFERENCES academic.course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_record_course_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_record_identity_scope FOREIGN KEY (academic_year_start, score_identity_id, student_id, course_offering_id) REFERENCES measurement.score_identity (academic_year_start, id, student_id, course_offering_id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_record_ingestion_batch FOREIGN KEY (ingestion_batch_id) REFERENCES integration.ingestion_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_record_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_record_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_record_program_version FOREIGN KEY (program_version_id) REFERENCES academic.program_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_record_recorder FOREIGN KEY (recorded_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_record_source_system FOREIGN KEY (source_system_id) REFERENCES integration.source_system (id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_record_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_record_supersedes FOREIGN KEY (academic_year_start, score_identity_id, supersedes_id) REFERENCES measurement.score_record (academic_year_start, score_identity_id, id) ON DELETE RESTRICT
);

CREATE TABLE academic.anchor_criterion (
    anchor_assessment_id uuid NOT NULL,
    syllabus_traceability_id uuid NOT NULL,
    CONSTRAINT pk_anchor_criterion PRIMARY KEY (anchor_assessment_id, syllabus_traceability_id),
    CONSTRAINT fk_anchor_criterion_anchor_assessment FOREIGN KEY (anchor_assessment_id) REFERENCES academic.anchor_assessment (id) ON DELETE RESTRICT,
    CONSTRAINT fk_anchor_criterion_traceability FOREIGN KEY (syllabus_traceability_id) REFERENCES portfolio.syllabus_traceability (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.direct_pi_criterion_weight (
    id uuid NOT NULL,
    syllabus_traceability_id uuid NOT NULL,
    direct_weight_ratio numeric(12,10) NOT NULL,
    is_core_gate boolean NOT NULL,
    approved_at timestamptz,
    CONSTRAINT pk_direct_pi_criterion_weight PRIMARY KEY (id),
    CONSTRAINT ck_direct_pi_criterion_weight_ratio CHECK (direct_weight_ratio NOT IN ('NaN'::numeric,'Infinity'::numeric,'-Infinity'::numeric) AND direct_weight_ratio > 0 AND direct_weight_ratio <= 1),
    CONSTRAINT fk_direct_pi_criterion_weight_traceability FOREIGN KEY (syllabus_traceability_id) REFERENCES portfolio.syllabus_traceability (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.snapshot_direct_pi_weight (
    input_snapshot_id uuid NOT NULL,
    syllabus_traceability_id uuid NOT NULL,
    program_pi_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    rubric_criterion_id uuid NOT NULL,
    direct_weight_ratio numeric(12,10) NOT NULL,
    allocation_ratio numeric(12,10),
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_snapshot_direct_pi_weight PRIMARY KEY (input_snapshot_id, syllabus_traceability_id),
    CONSTRAINT ck_snapshot_direct_pi_allocation CHECK (allocation_ratio IS NULL OR allocation_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND allocation_ratio > 0 AND allocation_ratio <= 1),
    CONSTRAINT ck_snapshot_direct_pi_weight CHECK (direct_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND direct_weight_ratio > 0 AND direct_weight_ratio <= 1),
    CONSTRAINT fk_snapshot_direct_pi_weight_criterion FOREIGN KEY (rubric_criterion_id) REFERENCES portfolio.rubric_criterion (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_direct_pi_weight_input_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_direct_pi_weight_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_direct_pi_weight_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_direct_pi_weight_traceability FOREIGN KEY (syllabus_traceability_id) REFERENCES portfolio.syllabus_traceability (id) ON DELETE RESTRICT
);

CREATE TABLE portfolio.traceability_evidence (
    syllabus_traceability_id uuid NOT NULL,
    evidence_version_id uuid NOT NULL,
    link_role varchar(32) NOT NULL,
    CONSTRAINT pk_traceability_evidence PRIMARY KEY (syllabus_traceability_id, evidence_version_id, link_role),
    CONSTRAINT fk_traceability_evidence_evidence_version FOREIGN KEY (evidence_version_id) REFERENCES document.evidence_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_traceability_evidence_traceability FOREIGN KEY (syllabus_traceability_id) REFERENCES portfolio.syllabus_traceability (id) ON DELETE RESTRICT
);

CREATE TABLE ai.ai_source_scope (
    ai_source_snapshot_id uuid NOT NULL,
    resource_security_scope_id uuid NOT NULL,
    scope_checksum char(64) NOT NULL,
    CONSTRAINT pk_ai_source_scope PRIMARY KEY (ai_source_snapshot_id, resource_security_scope_id),
    CONSTRAINT ck_ai_source_scope_checksum CHECK (scope_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT fk_ai_source_scope_security_scope FOREIGN KEY (resource_security_scope_id) REFERENCES governance.resource_security_scope (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_source_scope_snapshot FOREIGN KEY (ai_source_snapshot_id) REFERENCES ai.ai_source_snapshot (id) ON DELETE RESTRICT
);

CREATE TABLE ai.ground_truth_case (
    id uuid NOT NULL,
    suite_version_id uuid NOT NULL,
    case_code varchar(64) NOT NULL,
    input_source_snapshot_id uuid NOT NULL,
    expected_output jsonb NOT NULL,
    acceptance_rule jsonb NOT NULL,
    classification varchar(20) NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_ground_truth_case PRIMARY KEY (id),
    CONSTRAINT ck_ground_truth_case_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_ground_truth_case_classification CHECK (classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')),
    CONSTRAINT ck_ground_truth_case_code CHECK (case_code = upper(btrim(case_code)) AND char_length(case_code) > 0),
    CONSTRAINT fk_ground_truth_case_input_snapshot FOREIGN KEY (input_source_snapshot_id) REFERENCES ai.ai_source_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ground_truth_case_suite_version FOREIGN KEY (suite_version_id) REFERENCES ai.ground_truth_suite_version (id) ON DELETE RESTRICT
);

CREATE TABLE integration.webhook_attempt (
    delivery_id uuid NOT NULL,
    attempt_no integer NOT NULL,
    nonce varchar(255) NOT NULL,
    signature varchar(512) NOT NULL,
    requested_at timestamptz NOT NULL,
    response_status integer,
    response_at timestamptz,
    error_code varchar(64),
    response_excerpt varchar(2048),
    CONSTRAINT pk_webhook_attempt PRIMARY KEY (delivery_id, attempt_no),
    CONSTRAINT ck_webhook_attempt_number CHECK (attempt_no > 0),
    CONSTRAINT ck_webhook_attempt_response_status CHECK (response_status IS NULL OR response_status BETWEEN 100 AND 599),
    CONSTRAINT ck_webhook_attempt_response_time CHECK (response_at IS NULL OR response_at >= requested_at),
    CONSTRAINT fk_webhook_attempt_delivery FOREIGN KEY (delivery_id) REFERENCES integration.webhook_delivery (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.score_source_map (
    source_system_id uuid NOT NULL,
    source_record_id varchar(255) NOT NULL,
    source_revision varchar(128) NOT NULL,
    academic_year_start smallint NOT NULL,
    score_record_id uuid NOT NULL,
    payload_checksum char(64) NOT NULL,
    CONSTRAINT pk_score_source_map PRIMARY KEY (source_system_id, source_record_id, source_revision),
    CONSTRAINT ck_score_source_map_checksum CHECK (payload_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT fk_score_source_map_score_record FOREIGN KEY (academic_year_start, score_record_id) REFERENCES measurement.score_record (academic_year_start, id) ON DELETE RESTRICT,
    CONSTRAINT fk_score_source_map_source_system FOREIGN KEY (source_system_id) REFERENCES integration.source_system (id) ON DELETE RESTRICT
);

CREATE TABLE measurement.snapshot_score (
    input_snapshot_id uuid NOT NULL,
    academic_year_start smallint NOT NULL,
    score_record_id uuid NOT NULL,
    student_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    raw_score numeric(20,10),
    max_score numeric(20,10) NOT NULL,
    score_status varchar(20) NOT NULL,
    normalized_score numeric(20,10),
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_snapshot_score PRIMARY KEY (input_snapshot_id, academic_year_start, score_record_id),
    CONSTRAINT uq_snapshot_score_1 UNIQUE (input_snapshot_id, academic_year_start, score_record_id, student_id, course_offering_id),
    CONSTRAINT ck_snapshot_score_status CHECK (score_status IN ('SCORED','ABSENT','EXCUSED','NOT_SUBMITTED','DEFERRED','WITHDRAWN','MISSING')),
    CONSTRAINT ck_snapshot_score_values CHECK (max_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_score > 0 AND ((score_status = 'SCORED' AND raw_score IS NOT NULL AND raw_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND raw_score >= 0 AND raw_score <= max_score AND normalized_score IS NOT NULL AND normalized_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND normalized_score BETWEEN 0 AND 100) OR (score_status <> 'SCORED' AND raw_score IS NULL AND normalized_score IS NULL))),
    CONSTRAINT fk_snapshot_score_course_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_score_input_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_score_record_scope FOREIGN KEY (academic_year_start, score_record_id, student_id, course_offering_id) REFERENCES measurement.score_record (academic_year_start, id, student_id, course_offering_id) ON DELETE RESTRICT,
    CONSTRAINT fk_snapshot_score_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT
);

CREATE TABLE integration.staging_score (
    id uuid NOT NULL,
    ingestion_batch_id uuid NOT NULL,
    row_no integer NOT NULL,
    raw_record_id bigint NOT NULL,
    student_code varchar(64) NOT NULL,
    offering_code varchar(64) NOT NULL,
    assessment_code varchar(64) NOT NULL,
    criterion_code varchar(64),
    raw_score numeric(20,10),
    max_score numeric(20,10),
    resolved_score_academic_year_start smallint,
    resolved_score_record_id uuid,
    validation_status varchar(20) NOT NULL,
    row_checksum char(64) NOT NULL,
    CONSTRAINT pk_staging_score PRIMARY KEY (id),
    CONSTRAINT ck_staging_score_checksum CHECK (row_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_staging_score_row_no CHECK (row_no > 0),
    CONSTRAINT ck_staging_score_validation_status CHECK (validation_status IN ('PENDING', 'VALID', 'INVALID', 'PROMOTED')),
    CONSTRAINT ck_staging_score_values CHECK ((raw_score IS NULL OR raw_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)) AND (max_score IS NULL OR (max_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND max_score > 0))),
    CONSTRAINT fk_staging_score_batch FOREIGN KEY (ingestion_batch_id) REFERENCES integration.ingestion_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_score_raw_record FOREIGN KEY (raw_record_id) REFERENCES integration.raw_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_staging_score_resolved_score FOREIGN KEY (resolved_score_academic_year_start, resolved_score_record_id) REFERENCES measurement.score_record (academic_year_start, id) ON DELETE RESTRICT
);

CREATE TABLE result.criterion_pi_contribution (
    academic_year_start smallint NOT NULL,
    id uuid NOT NULL,
    batch_id uuid NOT NULL,
    input_snapshot_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    student_id uuid NOT NULL,
    student_path_id uuid NOT NULL,
    course_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    assessment_item_id uuid NOT NULL,
    rubric_criterion_id uuid NOT NULL,
    program_pi_id uuid NOT NULL,
    syllabus_traceability_id uuid NOT NULL,
    student_criterion_result_id uuid NOT NULL,
    normalized_score numeric(20,10) NOT NULL,
    direct_weight_ratio numeric(12,10) NOT NULL,
    allocation_ratio numeric(12,10) NOT NULL,
    weighted_contribution numeric(20,10) NOT NULL,
    is_core boolean NOT NULL,
    included boolean NOT NULL,
    exclusion_reason text,
    CONSTRAINT pk_criterion_pi_contribution PRIMARY KEY (academic_year_start, id),
    CONSTRAINT ck_criterion_pi_contribution_exclusion CHECK ((included AND exclusion_reason IS NULL) OR (NOT included AND char_length(btrim(exclusion_reason)) > 0)),
    CONSTRAINT ck_criterion_pi_contribution_numeric CHECK (normalized_score NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND normalized_score >= 0 AND normalized_score <= 100 AND direct_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND direct_weight_ratio >= 0 AND direct_weight_ratio <= 1 AND allocation_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND allocation_ratio >= 0 AND allocation_ratio <= 1 AND weighted_contribution NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)),
    CONSTRAINT fk_criterion_pi_contribution_assessment FOREIGN KEY (assessment_item_id) REFERENCES portfolio.assessment_item (id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_batch_snapshot_scope FOREIGN KEY (batch_id, input_snapshot_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) REFERENCES result.result_batch (id, input_snapshot_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_course FOREIGN KEY (course_id) REFERENCES academic.course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_criterion FOREIGN KEY (rubric_criterion_id) REFERENCES portfolio.rubric_criterion (id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_criterion_result FOREIGN KEY (academic_year_start, student_criterion_result_id, batch_id, student_id, course_offering_id, rubric_criterion_id) REFERENCES result.student_criterion_result (academic_year_start, id, batch_id, student_id, course_offering_id, rubric_criterion_id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_offering FOREIGN KEY (course_offering_id) REFERENCES academic.course_offering (id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_program_pi FOREIGN KEY (program_pi_id) REFERENCES academic.program_pi (id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_program_version FOREIGN KEY (program_version_id, program_id) REFERENCES academic.program_version (id, program_id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_snapshot_weight FOREIGN KEY (input_snapshot_id, syllabus_traceability_id) REFERENCES measurement.snapshot_direct_pi_weight (input_snapshot_id, syllabus_traceability_id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_student FOREIGN KEY (student_id) REFERENCES academic.student (person_id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_student_path FOREIGN KEY (student_path_id) REFERENCES academic.student_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_criterion_pi_contribution_traceability FOREIGN KEY (syllabus_traceability_id) REFERENCES portfolio.syllabus_traceability (id) ON DELETE RESTRICT
);

CREATE TABLE result.student_criterion_score_lineage (
    academic_year_start smallint NOT NULL,
    student_criterion_result_id uuid NOT NULL,
    score_record_id uuid NOT NULL,
    batch_id uuid NOT NULL,
    input_snapshot_id uuid NOT NULL,
    org_unit_id uuid NOT NULL,
    program_id uuid NOT NULL,
    program_version_id uuid NOT NULL,
    measurement_period_id uuid NOT NULL,
    cohort_id uuid NOT NULL,
    curriculum_path_id uuid NOT NULL,
    course_id uuid NOT NULL,
    student_id uuid NOT NULL,
    course_offering_id uuid NOT NULL,
    rubric_criterion_id uuid NOT NULL,
    assessment_question_id uuid,
    source_weight_ratio numeric(12,10) NOT NULL,
    weighted_contribution numeric(20,10) NOT NULL,
    CONSTRAINT pk_student_criterion_score_lineage PRIMARY KEY (academic_year_start, student_criterion_result_id, score_record_id),
    CONSTRAINT ck_student_criterion_score_lineage_weight CHECK (source_weight_ratio NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric) AND source_weight_ratio > 0 AND source_weight_ratio <= 1 AND weighted_contribution NOT IN ('NaN'::numeric, 'Infinity'::numeric, '-Infinity'::numeric)),
    CONSTRAINT fk_student_criterion_lineage_batch_snapshot_scope FOREIGN KEY (batch_id, input_snapshot_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) REFERENCES result.result_batch (id, input_snapshot_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_lineage_cohort FOREIGN KEY (cohort_id) REFERENCES academic.cohort (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_lineage_course FOREIGN KEY (course_id) REFERENCES academic.course (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_lineage_org_unit FOREIGN KEY (org_unit_id) REFERENCES academic.org_unit (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_lineage_path FOREIGN KEY (curriculum_path_id) REFERENCES academic.curriculum_path (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_lineage_period FOREIGN KEY (measurement_period_id) REFERENCES measurement.measurement_period (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_lineage_program FOREIGN KEY (program_id) REFERENCES academic.program (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_lineage_program_version FOREIGN KEY (program_version_id, program_id) REFERENCES academic.program_version (id, program_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_lineage_question_weight FOREIGN KEY (input_snapshot_id, assessment_question_id, rubric_criterion_id) REFERENCES measurement.snapshot_question_criterion_weight (input_snapshot_id, assessment_question_id, rubric_criterion_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_lineage_result_scope FOREIGN KEY (academic_year_start, student_criterion_result_id, batch_id, student_id, course_offering_id, rubric_criterion_id) REFERENCES result.student_criterion_result (academic_year_start, id, batch_id, student_id, course_offering_id, rubric_criterion_id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_lineage_snapshot FOREIGN KEY (input_snapshot_id) REFERENCES measurement.input_snapshot (id) ON DELETE RESTRICT,
    CONSTRAINT fk_student_criterion_lineage_snapshot_score FOREIGN KEY (input_snapshot_id, academic_year_start, score_record_id, student_id, course_offering_id) REFERENCES measurement.snapshot_score (input_snapshot_id, academic_year_start, score_record_id, student_id, course_offering_id) ON DELETE RESTRICT
);

CREATE TABLE ai.activation_decision (
    id uuid NOT NULL,
    evaluation_run_id uuid NOT NULL,
    model_deployment_version_id uuid NOT NULL,
    prompt_version_id uuid NOT NULL,
    output_schema_version_id uuid NOT NULL,
    data_handling_policy_version_id uuid NOT NULL,
    tool_policy_version_id uuid NOT NULL,
    decision_record_id uuid NOT NULL,
    approved_by uuid NOT NULL,
    approved_at timestamptz NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_activation_decision PRIMARY KEY (id),
    CONSTRAINT uq_activation_decision_id_data_policy_version UNIQUE (id, data_handling_policy_version_id),
    CONSTRAINT uq_activation_decision_id_model_version UNIQUE (id, model_deployment_version_id),
    CONSTRAINT uq_activation_decision_id_output_schema_version UNIQUE (id, output_schema_version_id),
    CONSTRAINT uq_activation_decision_id_prompt_version UNIQUE (id, prompt_version_id),
    CONSTRAINT uq_activation_decision_id_tool_policy_version UNIQUE (id, tool_policy_version_id),
    CONSTRAINT ck_activation_decision_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT fk_activation_decision_approved_by FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_activation_decision_decision_record FOREIGN KEY (decision_record_id) REFERENCES academic.decision_record (id) ON DELETE RESTRICT
);

CREATE TABLE ai.data_handling_policy_version (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    version_no integer NOT NULL,
    allowed_providers jsonb NOT NULL,
    allowed_regions jsonb NOT NULL,
    input_retention_days integer NOT NULL,
    output_retention_days integer NOT NULL,
    provider_training_opt_out boolean NOT NULL,
    maximum_classification varchar(20) NOT NULL,
    redaction_rules jsonb NOT NULL,
    checksum char(64) NOT NULL,
    status varchar(20) NOT NULL,
    approved_by uuid,
    approved_at timestamptz,
    effective_from date NOT NULL,
    effective_to date,
    activation_decision_id uuid,
    CONSTRAINT pk_data_handling_policy_version PRIMARY KEY (id),
    CONSTRAINT ck_data_handling_policy_version_activation CHECK (status <> 'ACTIVE' OR activation_decision_id IS NOT NULL),
    CONSTRAINT ck_data_handling_policy_version_approval CHECK ((approved_by IS NULL) = (approved_at IS NULL)),
    CONSTRAINT ck_data_handling_policy_version_approved_state CHECK (status NOT IN ('APPROVED','ACTIVE','EXPIRED') OR approved_by IS NOT NULL),
    CONSTRAINT ck_data_handling_policy_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_data_handling_policy_version_classification CHECK (maximum_classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')),
    CONSTRAINT ck_data_handling_policy_version_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_data_handling_policy_version_no CHECK (version_no > 0),
    CONSTRAINT ck_data_handling_policy_version_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_data_handling_policy_version_retention CHECK (input_retention_days >= 0 AND output_retention_days >= 0),
    CONSTRAINT ck_data_handling_policy_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_data_handling_policy_version_approved_by FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_data_handling_policy_version_exact_activation FOREIGN KEY (activation_decision_id, id) REFERENCES ai.activation_decision (id, data_handling_policy_version_id) ON DELETE RESTRICT
);

CREATE TABLE ai.model_deployment_version (
    id uuid NOT NULL,
    model_deployment_id uuid NOT NULL,
    version_no integer NOT NULL,
    provider varchar(64) NOT NULL,
    provider_model_id varchar(128) NOT NULL,
    provider_model_revision varchar(128),
    deployment_name varchar(128) NOT NULL,
    region varchar(64) NOT NULL,
    capability varchar(64) NOT NULL,
    secret_reference varchar(512) NOT NULL,
    configuration jsonb NOT NULL,
    checksum char(64) NOT NULL,
    status varchar(20) NOT NULL,
    effective_from date NOT NULL,
    effective_to date,
    approved_by uuid,
    approved_at timestamptz,
    activation_decision_id uuid,
    CONSTRAINT pk_model_deployment_version PRIMARY KEY (id),
    CONSTRAINT ck_model_deployment_version_activation CHECK (status <> 'ACTIVE' OR activation_decision_id IS NOT NULL),
    CONSTRAINT ck_model_deployment_version_approval CHECK ((approved_by IS NULL) = (approved_at IS NULL)),
    CONSTRAINT ck_model_deployment_version_approved_state CHECK (status NOT IN ('APPROVED','ACTIVE','EXPIRED') OR approved_by IS NOT NULL),
    CONSTRAINT ck_model_deployment_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_model_deployment_version_no CHECK (version_no > 0),
    CONSTRAINT ck_model_deployment_version_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_model_deployment_version_secret_reference CHECK (secret_reference = btrim(secret_reference) AND char_length(secret_reference) > 0),
    CONSTRAINT ck_model_deployment_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_model_deployment_version_approved_by FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_model_deployment_version_deployment FOREIGN KEY (model_deployment_id) REFERENCES ai.model_deployment (id) ON DELETE RESTRICT,
    CONSTRAINT fk_model_deployment_version_exact_activation FOREIGN KEY (activation_decision_id, id) REFERENCES ai.activation_decision (id, model_deployment_version_id) ON DELETE RESTRICT
);

CREATE TABLE ai.output_schema_version (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    version_no integer NOT NULL,
    json_schema jsonb NOT NULL,
    checksum char(64) NOT NULL,
    status varchar(20) NOT NULL,
    approved_by uuid,
    approved_at timestamptz,
    activation_decision_id uuid,
    CONSTRAINT pk_output_schema_version PRIMARY KEY (id),
    CONSTRAINT ck_output_schema_version_activation CHECK (status <> 'ACTIVE' OR activation_decision_id IS NOT NULL),
    CONSTRAINT ck_output_schema_version_approval CHECK ((approved_by IS NULL) = (approved_at IS NULL)),
    CONSTRAINT ck_output_schema_version_approved_state CHECK (status NOT IN ('APPROVED','ACTIVE','EXPIRED') OR approved_by IS NOT NULL),
    CONSTRAINT ck_output_schema_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_output_schema_version_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_output_schema_version_no CHECK (version_no > 0),
    CONSTRAINT ck_output_schema_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_output_schema_version_approved_by FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_output_schema_version_exact_activation FOREIGN KEY (activation_decision_id, id) REFERENCES ai.activation_decision (id, output_schema_version_id) ON DELETE RESTRICT
);

CREATE TABLE ai.tool_policy_version (
    id uuid NOT NULL,
    code varchar(64) NOT NULL,
    version_no integer NOT NULL,
    allowed_tools jsonb NOT NULL,
    timeout_seconds integer NOT NULL,
    network_policy jsonb NOT NULL,
    file_sandbox_policy jsonb NOT NULL,
    rate_limit integer NOT NULL,
    cost_limit numeric(20,10) NOT NULL,
    checksum char(64) NOT NULL,
    status varchar(20) NOT NULL,
    approved_by uuid,
    approved_at timestamptz,
    effective_from date NOT NULL,
    effective_to date,
    activation_decision_id uuid,
    CONSTRAINT pk_tool_policy_version PRIMARY KEY (id),
    CONSTRAINT ck_tool_policy_version_activation CHECK (status <> 'ACTIVE' OR activation_decision_id IS NOT NULL),
    CONSTRAINT ck_tool_policy_version_approval CHECK ((approved_by IS NULL) = (approved_at IS NULL)),
    CONSTRAINT ck_tool_policy_version_approved_state CHECK (status NOT IN ('APPROVED','ACTIVE','EXPIRED') OR approved_by IS NOT NULL),
    CONSTRAINT ck_tool_policy_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_tool_policy_version_code CHECK (code = upper(btrim(code)) AND char_length(code) > 0),
    CONSTRAINT ck_tool_policy_version_limits CHECK (timeout_seconds > 0 AND rate_limit >= 0 AND cost_limit >= 0 AND cost_limit <> 'NaN'::numeric AND cost_limit NOT IN ('Infinity'::numeric, '-Infinity'::numeric)),
    CONSTRAINT ck_tool_policy_version_no CHECK (version_no > 0),
    CONSTRAINT ck_tool_policy_version_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_tool_policy_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_tool_policy_version_approved_by FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_tool_policy_version_exact_activation FOREIGN KEY (activation_decision_id, id) REFERENCES ai.activation_decision (id, tool_policy_version_id) ON DELETE RESTRICT
);

CREATE TABLE ai.prompt_version (
    id uuid NOT NULL,
    prompt_id uuid NOT NULL,
    version_no integer NOT NULL,
    system_template text NOT NULL,
    input_contract jsonb NOT NULL,
    output_schema_version_id uuid NOT NULL,
    checksum char(64) NOT NULL,
    status varchar(20) NOT NULL,
    approved_by uuid,
    approved_at timestamptz,
    effective_from date NOT NULL,
    effective_to date,
    activation_decision_id uuid,
    CONSTRAINT pk_prompt_version PRIMARY KEY (id),
    CONSTRAINT uq_prompt_version_id_output_schema UNIQUE (id, output_schema_version_id),
    CONSTRAINT ck_prompt_version_activation CHECK (status <> 'ACTIVE' OR activation_decision_id IS NOT NULL),
    CONSTRAINT ck_prompt_version_approval CHECK ((approved_by IS NULL) = (approved_at IS NULL)),
    CONSTRAINT ck_prompt_version_approved_state CHECK (status NOT IN ('APPROVED','ACTIVE','EXPIRED') OR approved_by IS NOT NULL),
    CONSTRAINT ck_prompt_version_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_prompt_version_no CHECK (version_no > 0),
    CONSTRAINT ck_prompt_version_range CHECK (effective_to IS NULL OR effective_to > effective_from),
    CONSTRAINT ck_prompt_version_status CHECK (status IN ('DRAFT','IN_REVIEW','APPROVED','ACTIVE','EXPIRED','REJECTED')),
    CONSTRAINT fk_prompt_version_approved_by FOREIGN KEY (approved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_prompt_version_exact_activation FOREIGN KEY (activation_decision_id, id) REFERENCES ai.activation_decision (id, prompt_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_prompt_version_output_schema FOREIGN KEY (output_schema_version_id) REFERENCES ai.output_schema_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_prompt_version_prompt FOREIGN KEY (prompt_id) REFERENCES ai.prompt (id) ON DELETE RESTRICT
);

CREATE TABLE ai.ai_job (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    job_type varchar(24) NOT NULL,
    status varchar(24) NOT NULL,
    classification varchar(20) NOT NULL,
    requested_by uuid NOT NULL,
    access_scope_id uuid NOT NULL,
    model_deployment_version_id uuid NOT NULL,
    prompt_version_id uuid NOT NULL,
    output_schema_version_id uuid NOT NULL,
    data_handling_policy_version_id uuid NOT NULL,
    tool_policy_version_id uuid NOT NULL,
    generation_parameters jsonb NOT NULL,
    input_checksum char(64) NOT NULL,
    request_id uuid NOT NULL,
    correlation_id uuid NOT NULL,
    queued_at timestamptz NOT NULL,
    started_at timestamptz,
    completed_at timestamptz,
    input_tokens bigint,
    output_tokens bigint,
    estimated_cost numeric(20,10),
    error_code varchar(64),
    error_detail_redacted text,
    target_resource_type varchar(64) NOT NULL,
    target_resource_id uuid NOT NULL,
    target_resource_version bigint NOT NULL,
    target_content_checksum char(64) NOT NULL,
    target_row_version bigint NOT NULL,
    CONSTRAINT pk_ai_job PRIMARY KEY (id),
    CONSTRAINT uq_ai_job_id_target UNIQUE (id, target_resource_type, target_resource_id),
    CONSTRAINT ck_ai_job_checksums CHECK (input_checksum ~ '^[0-9a-f]{64}$' AND target_content_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_ai_job_classification CHECK (classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')),
    CONSTRAINT ck_ai_job_estimated_cost CHECK (estimated_cost IS NULL OR (estimated_cost >= 0 AND estimated_cost <> 'NaN'::numeric AND estimated_cost NOT IN ('Infinity'::numeric, '-Infinity'::numeric))),
    CONSTRAINT ck_ai_job_failure CHECK (status <> 'FAILED' OR error_code IS NOT NULL),
    CONSTRAINT ck_ai_job_job_type CHECK (job_type IN ('EXTRACT','GENERATE','CHAT','DETECT_ANOMALY')),
    CONSTRAINT ck_ai_job_status CHECK (status IN ('QUEUED','RUNNING','NEEDS_REVIEW','PARTIAL','ACCEPTED','REJECTED','APPLIED','FAILED','CANCELLED')),
    CONSTRAINT ck_ai_job_status_timestamps CHECK ((status = 'QUEUED' AND started_at IS NULL AND completed_at IS NULL) OR (status = 'RUNNING' AND started_at IS NOT NULL AND completed_at IS NULL) OR (status = 'CANCELLED' AND completed_at IS NOT NULL) OR (status NOT IN ('QUEUED','RUNNING','CANCELLED') AND started_at IS NOT NULL AND completed_at IS NOT NULL)),
    CONSTRAINT ck_ai_job_target_versions CHECK (target_resource_version >= 0 AND target_row_version >= 0),
    CONSTRAINT ck_ai_job_timestamps CHECK ((started_at IS NULL OR started_at >= queued_at) AND (completed_at IS NULL OR completed_at >= COALESCE(started_at, queued_at))),
    CONSTRAINT ck_ai_job_token_counts CHECK ((input_tokens IS NULL OR input_tokens >= 0) AND (output_tokens IS NULL OR output_tokens >= 0)),
    CONSTRAINT fk_ai_job_access_scope FOREIGN KEY (access_scope_id) REFERENCES iam.access_scope (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_job_data_handling_policy_version FOREIGN KEY (data_handling_policy_version_id) REFERENCES ai.data_handling_policy_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_job_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_job_model_deployment_version FOREIGN KEY (model_deployment_version_id) REFERENCES ai.model_deployment_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_job_output_schema_version FOREIGN KEY (output_schema_version_id) REFERENCES ai.output_schema_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_job_prompt_output_schema_bundle FOREIGN KEY (prompt_version_id, output_schema_version_id) REFERENCES ai.prompt_version (id, output_schema_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_job_requested_by FOREIGN KEY (requested_by) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_job_tool_policy_version FOREIGN KEY (tool_policy_version_id) REFERENCES ai.tool_policy_version (id) ON DELETE RESTRICT
);

CREATE TABLE ai.evaluation_run (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    suite_version_id uuid NOT NULL,
    suite_checksum char(64) NOT NULL,
    evaluation_policy_version_id uuid NOT NULL,
    evaluation_policy_checksum char(64) NOT NULL,
    model_deployment_version_id uuid NOT NULL,
    prompt_version_id uuid NOT NULL,
    output_schema_version_id uuid NOT NULL,
    data_handling_policy_version_id uuid NOT NULL,
    tool_policy_version_id uuid NOT NULL,
    config_bundle_checksum char(64) NOT NULL,
    status varchar(20) NOT NULL,
    result_checksum char(64),
    started_at timestamptz NOT NULL,
    completed_at timestamptz,
    CONSTRAINT pk_evaluation_run PRIMARY KEY (id),
    CONSTRAINT uq_evaluation_run_exact_bundle UNIQUE (id, model_deployment_version_id, prompt_version_id, output_schema_version_id, data_handling_policy_version_id, tool_policy_version_id),
    CONSTRAINT ck_evaluation_run_checksums CHECK (suite_checksum ~ '^[0-9a-f]{64}$' AND evaluation_policy_checksum ~ '^[0-9a-f]{64}$' AND config_bundle_checksum ~ '^[0-9a-f]{64}$' AND (result_checksum IS NULL OR result_checksum ~ '^[0-9a-f]{64}$')),
    CONSTRAINT ck_evaluation_run_completion CHECK (status = 'RUNNING' OR completed_at IS NOT NULL),
    CONSTRAINT ck_evaluation_run_result_checksum CHECK (status <> 'PASSED' OR result_checksum IS NOT NULL),
    CONSTRAINT ck_evaluation_run_status CHECK (status IN ('RUNNING','PASSED','FAILED','CANCELLED')),
    CONSTRAINT ck_evaluation_run_time CHECK (completed_at IS NULL OR completed_at >= started_at),
    CONSTRAINT fk_evaluation_run_data_handling_policy_version FOREIGN KEY (data_handling_policy_version_id) REFERENCES ai.data_handling_policy_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_evaluation_run_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_evaluation_run_model_deployment_version FOREIGN KEY (model_deployment_version_id) REFERENCES ai.model_deployment_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_evaluation_run_output_schema_version FOREIGN KEY (output_schema_version_id) REFERENCES ai.output_schema_version (id) ON DELETE RESTRICT,
    CONSTRAINT fk_evaluation_run_policy_checksum FOREIGN KEY (evaluation_policy_version_id, evaluation_policy_checksum) REFERENCES ai.evaluation_policy_version (id, checksum) ON DELETE RESTRICT,
    CONSTRAINT fk_evaluation_run_prompt_output_schema_bundle FOREIGN KEY (prompt_version_id, output_schema_version_id) REFERENCES ai.prompt_version (id, output_schema_version_id) ON DELETE RESTRICT,
    CONSTRAINT fk_evaluation_run_suite_checksum FOREIGN KEY (suite_version_id, suite_checksum) REFERENCES ai.ground_truth_suite_version (id, checksum) ON DELETE RESTRICT,
    CONSTRAINT fk_evaluation_run_tool_policy_version FOREIGN KEY (tool_policy_version_id) REFERENCES ai.tool_policy_version (id) ON DELETE RESTRICT
);

CREATE TABLE ai.ai_artifact (
    id uuid NOT NULL,
    governed_resource_id uuid NOT NULL,
    ai_job_id uuid NOT NULL,
    artifact_type varchar(32) NOT NULL,
    target_resource_type varchar(64) NOT NULL,
    target_resource_id uuid NOT NULL,
    field_path varchar(512) NOT NULL,
    proposed_value jsonb NOT NULL,
    confidence numeric(5,4) NOT NULL,
    is_inferred boolean NOT NULL,
    review_status varchar(20) NOT NULL,
    reviewed_by uuid,
    reviewed_at timestamptz,
    applied_resource_version bigint,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_ai_artifact PRIMARY KEY (id),
    CONSTRAINT uq_ai_artifact_id_job UNIQUE (id, ai_job_id),
    CONSTRAINT ck_ai_artifact_applied_version CHECK ((review_status = 'APPLIED') = (applied_resource_version IS NOT NULL)),
    CONSTRAINT ck_ai_artifact_confidence CHECK (confidence >= 0 AND confidence <= 1 AND confidence <> 'NaN'::numeric AND confidence NOT IN ('Infinity'::numeric, '-Infinity'::numeric)),
    CONSTRAINT ck_ai_artifact_review_pair CHECK ((review_status = 'PENDING' AND reviewed_by IS NULL AND reviewed_at IS NULL) OR (review_status <> 'PENDING' AND reviewed_by IS NOT NULL AND reviewed_at IS NOT NULL)),
    CONSTRAINT ck_ai_artifact_review_status CHECK (review_status IN ('PENDING','ACCEPTED','EDITED','REJECTED','APPLIED')),
    CONSTRAINT fk_ai_artifact_governed_resource FOREIGN KEY (governed_resource_id) REFERENCES governance.governed_resource (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_artifact_job_target FOREIGN KEY (ai_job_id, target_resource_type, target_resource_id) REFERENCES ai.ai_job (id, target_resource_type, target_resource_id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_artifact_reviewed_by FOREIGN KEY (reviewed_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE ai.ai_job_input (
    ai_job_id uuid NOT NULL,
    sequence_no integer NOT NULL,
    source_snapshot_id uuid NOT NULL,
    input_role varchar(32) NOT NULL,
    source_checksum char(64) NOT NULL,
    CONSTRAINT pk_ai_job_input PRIMARY KEY (ai_job_id, sequence_no),
    CONSTRAINT ck_ai_job_input_checksum CHECK (source_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_ai_job_input_role CHECK (input_role = upper(btrim(input_role)) AND char_length(input_role) > 0),
    CONSTRAINT ck_ai_job_input_sequence CHECK (sequence_no > 0),
    CONSTRAINT fk_ai_job_input_job FOREIGN KEY (ai_job_id) REFERENCES ai.ai_job (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_job_input_source_snapshot_checksum FOREIGN KEY (source_snapshot_id, source_checksum) REFERENCES ai.ai_source_snapshot (id, source_checksum) ON DELETE RESTRICT
);

CREATE TABLE ai.safety_event (
    id uuid NOT NULL,
    ai_job_id uuid NOT NULL,
    event_type varchar(64) NOT NULL,
    severity varchar(20) NOT NULL,
    detector_version varchar(64) NOT NULL,
    blocked boolean NOT NULL,
    details_redacted jsonb NOT NULL,
    occurred_at timestamptz NOT NULL,
    CONSTRAINT pk_safety_event PRIMARY KEY (id),
    CONSTRAINT ck_safety_event_severity CHECK (severity IN ('INFO','WARNING','ERROR','BLOCKING')),
    CONSTRAINT ck_safety_event_type CHECK (event_type = upper(btrim(event_type)) AND char_length(event_type) > 0),
    CONSTRAINT fk_safety_event_ai_job FOREIGN KEY (ai_job_id) REFERENCES ai.ai_job (id) ON DELETE RESTRICT
);

CREATE TABLE ai.evaluation_result (
    id uuid NOT NULL,
    run_id uuid NOT NULL,
    case_id uuid NOT NULL,
    actual_output jsonb NOT NULL,
    field_precision numeric(5,4) NOT NULL,
    field_recall numeric(5,4) NOT NULL,
    citation_accuracy numeric(5,4) NOT NULL,
    schema_valid boolean NOT NULL,
    passed boolean NOT NULL,
    classification varchar(20) NOT NULL,
    checksum char(64) NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_evaluation_result PRIMARY KEY (id),
    CONSTRAINT ck_evaluation_result_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_evaluation_result_classification CHECK (classification IN ('PUBLIC','INTERNAL','CONFIDENTIAL','RESTRICTED')),
    CONSTRAINT ck_evaluation_result_metrics CHECK (field_precision >= 0 AND field_precision <= 1 AND field_recall >= 0 AND field_recall <= 1 AND citation_accuracy >= 0 AND citation_accuracy <= 1 AND field_precision <> 'NaN'::numeric AND field_recall <> 'NaN'::numeric AND citation_accuracy <> 'NaN'::numeric AND field_precision NOT IN ('Infinity'::numeric, '-Infinity'::numeric) AND field_recall NOT IN ('Infinity'::numeric, '-Infinity'::numeric) AND citation_accuracy NOT IN ('Infinity'::numeric, '-Infinity'::numeric)),
    CONSTRAINT fk_evaluation_result_case FOREIGN KEY (case_id) REFERENCES ai.ground_truth_case (id) ON DELETE RESTRICT,
    CONSTRAINT fk_evaluation_result_run FOREIGN KEY (run_id) REFERENCES ai.evaluation_run (id) ON DELETE RESTRICT
);

CREATE TABLE ai.ai_citation (
    id uuid NOT NULL,
    artifact_id uuid NOT NULL,
    source_snapshot_id uuid NOT NULL,
    page_no integer,
    region_polygon jsonb,
    row_locator jsonb,
    source_text_excerpt text,
    source_checksum char(64) NOT NULL,
    CONSTRAINT pk_ai_citation PRIMARY KEY (id),
    CONSTRAINT ck_ai_citation_checksum CHECK (source_checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_ai_citation_locator CHECK (page_no IS NOT NULL OR row_locator IS NOT NULL),
    CONSTRAINT ck_ai_citation_page CHECK (page_no IS NULL OR page_no > 0),
    CONSTRAINT ck_ai_citation_region CHECK (region_polygon IS NULL OR page_no IS NOT NULL),
    CONSTRAINT fk_ai_citation_artifact FOREIGN KEY (artifact_id) REFERENCES ai.ai_artifact (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_citation_source_snapshot_checksum FOREIGN KEY (source_snapshot_id, source_checksum) REFERENCES ai.ai_source_snapshot (id, source_checksum) ON DELETE RESTRICT
);

CREATE TABLE ai.ai_review_event (
    id uuid NOT NULL,
    artifact_id uuid NOT NULL,
    decision varchar(20) NOT NULL,
    proposed_before jsonb NOT NULL,
    final_value jsonb,
    reason text,
    reviewer_principal_id uuid NOT NULL,
    occurred_at timestamptz NOT NULL,
    CONSTRAINT pk_ai_review_event PRIMARY KEY (id),
    CONSTRAINT ck_ai_review_event_decision CHECK (decision IN ('ACCEPTED','EDITED','REJECTED','APPLIED')),
    CONSTRAINT ck_ai_review_event_final_value CHECK (decision <> 'EDITED' OR final_value IS NOT NULL),
    CONSTRAINT fk_ai_review_event_artifact FOREIGN KEY (artifact_id) REFERENCES ai.ai_artifact (id) ON DELETE RESTRICT,
    CONSTRAINT fk_ai_review_event_reviewer FOREIGN KEY (reviewer_principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE ai.chat_turn (
    id uuid NOT NULL,
    chat_session_id uuid NOT NULL,
    turn_no integer NOT NULL,
    user_message_ciphertext bytea NOT NULL,
    ai_job_id uuid NOT NULL,
    assistant_artifact_id uuid,
    data_as_of timestamptz NOT NULL,
    created_at timestamptz NOT NULL,
    CONSTRAINT pk_chat_turn PRIMARY KEY (id),
    CONSTRAINT ck_chat_turn_data_as_of CHECK (data_as_of <= created_at),
    CONSTRAINT ck_chat_turn_no CHECK (turn_no > 0),
    CONSTRAINT fk_chat_turn_ai_job FOREIGN KEY (ai_job_id) REFERENCES ai.ai_job (id) ON DELETE RESTRICT,
    CONSTRAINT fk_chat_turn_assistant_artifact_job FOREIGN KEY (assistant_artifact_id, ai_job_id) REFERENCES ai.ai_artifact (id, ai_job_id) ON DELETE RESTRICT,
    CONSTRAINT fk_chat_turn_session FOREIGN KEY (chat_session_id) REFERENCES ai.chat_session (id) ON DELETE RESTRICT
);

CREATE TABLE integration.quarantine_correction (
    id uuid NOT NULL,
    quarantine_record_id uuid NOT NULL,
    revision_no integer NOT NULL,
    normalized_payload jsonb NOT NULL,
    reason text NOT NULL,
    corrected_by uuid NOT NULL,
    corrected_at timestamptz NOT NULL,
    checksum char(64) NOT NULL,
    CONSTRAINT pk_quarantine_correction PRIMARY KEY (id),
    CONSTRAINT ck_quarantine_correction_checksum CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_quarantine_correction_reason CHECK (char_length(btrim(reason)) > 0),
    CONSTRAINT ck_quarantine_correction_revision CHECK (revision_no > 0),
    CONSTRAINT fk_quarantine_correction_corrected_by FOREIGN KEY (corrected_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE TABLE integration.quarantine_record (
    id uuid NOT NULL,
    ingestion_batch_id uuid NOT NULL,
    raw_record_id bigint NOT NULL,
    reason_code varchar(64) NOT NULL,
    status varchar(20) NOT NULL,
    owner_principal_id uuid,
    current_correction_id uuid,
    resolution_reason text,
    resolved_by uuid,
    resolved_at timestamptz,
    reprocess_batch_id uuid,
    row_version bigint NOT NULL DEFAULT 1,
    CONSTRAINT pk_quarantine_record PRIMARY KEY (id),
    CONSTRAINT ck_quarantine_record_resolution CHECK ((resolved_by IS NULL AND resolved_at IS NULL) OR (resolved_by IS NOT NULL AND resolved_at IS NOT NULL AND char_length(btrim(resolution_reason)) > 0)),
    CONSTRAINT ck_quarantine_record_row_version CHECK (row_version > 0),
    CONSTRAINT fk_quarantine_record_current_correction FOREIGN KEY (current_correction_id) REFERENCES integration.quarantine_correction (id) ON DELETE RESTRICT,
    CONSTRAINT fk_quarantine_record_ingestion_batch FOREIGN KEY (ingestion_batch_id) REFERENCES integration.ingestion_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_quarantine_record_owner FOREIGN KEY (owner_principal_id) REFERENCES iam.principal (id) ON DELETE RESTRICT,
    CONSTRAINT fk_quarantine_record_raw_record FOREIGN KEY (raw_record_id) REFERENCES integration.raw_record (id) ON DELETE RESTRICT,
    CONSTRAINT fk_quarantine_record_reprocess_batch FOREIGN KEY (reprocess_batch_id) REFERENCES integration.ingestion_batch (id) ON DELETE RESTRICT,
    CONSTRAINT fk_quarantine_record_resolved_by FOREIGN KEY (resolved_by) REFERENCES iam.principal (id) ON DELETE RESTRICT
);

CREATE INDEX "IX_access_scope_cohort_id" ON iam.access_scope (cohort_id);

CREATE INDEX "IX_access_scope_course_id" ON iam.access_scope (course_id);

CREATE INDEX "IX_access_scope_course_offering_id" ON iam.access_scope (course_offering_id);

CREATE INDEX "IX_access_scope_curriculum_path_id" ON iam.access_scope (curriculum_path_id);

CREATE INDEX "IX_access_scope_measurement_period_id" ON iam.access_scope (measurement_period_id);

CREATE INDEX "IX_access_scope_org_unit_id" ON iam.access_scope (org_unit_id);

CREATE INDEX "IX_access_scope_program_id" ON iam.access_scope (program_id);

CREATE INDEX "IX_access_scope_program_version_id" ON iam.access_scope (program_version_id);

CREATE INDEX "IX_access_scope_subject_principal_id" ON iam.access_scope (subject_principal_id);

CREATE UNIQUE INDEX uq_access_scope_anchor ON iam.access_scope (scope_type, org_unit_id, program_id, program_version_id, cohort_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id, subject_principal_id, include_descendants) NULLS NOT DISTINCT;

CREATE INDEX "IX_activation_decision_approved_by" ON ai.activation_decision (approved_by);

CREATE INDEX "IX_activation_decision_data_handling_policy_version_id" ON ai.activation_decision (data_handling_policy_version_id);

CREATE INDEX ix_activation_decision_decision_record ON ai.activation_decision (decision_record_id);

CREATE INDEX "IX_activation_decision_model_deployment_version_id" ON ai.activation_decision (model_deployment_version_id);

CREATE INDEX "IX_activation_decision_output_schema_version_id" ON ai.activation_decision (output_schema_version_id);

CREATE INDEX "IX_activation_decision_prompt_version_id_output_schema_version~" ON ai.activation_decision (prompt_version_id, output_schema_version_id);

CREATE INDEX "IX_activation_decision_tool_policy_version_id" ON ai.activation_decision (tool_policy_version_id);

CREATE UNIQUE INDEX uq_activation_decision_exact_bundle ON ai.activation_decision (evaluation_run_id, model_deployment_version_id, prompt_version_id, output_schema_version_id, data_handling_policy_version_id, tool_policy_version_id);

CREATE INDEX "IX_ai_artifact_ai_job_id_target_resource_type_target_resource_~" ON ai.ai_artifact (ai_job_id, target_resource_type, target_resource_id);

CREATE INDEX ix_ai_artifact_job_review_status_type ON ai.ai_artifact (ai_job_id, review_status, artifact_type);

CREATE INDEX ix_ai_artifact_reviewed_by ON ai.ai_artifact (reviewed_by);

CREATE UNIQUE INDEX uq_ai_artifact_governed_resource ON ai.ai_artifact (governed_resource_id);

CREATE INDEX ix_ai_citation_artifact_source ON ai.ai_citation (artifact_id, source_snapshot_id);

CREATE INDEX "IX_ai_citation_source_snapshot_id_source_checksum" ON ai.ai_citation (source_snapshot_id, source_checksum);

CREATE INDEX "IX_ai_job_access_scope_id" ON ai.ai_job (access_scope_id);

CREATE INDEX ix_ai_job_correlation_id ON ai.ai_job (correlation_id);

CREATE INDEX "IX_ai_job_data_handling_policy_version_id" ON ai.ai_job (data_handling_policy_version_id);

CREATE INDEX "IX_ai_job_model_deployment_version_id" ON ai.ai_job (model_deployment_version_id);

CREATE INDEX "IX_ai_job_output_schema_version_id" ON ai.ai_job (output_schema_version_id);

CREATE INDEX "IX_ai_job_prompt_version_id_output_schema_version_id" ON ai.ai_job (prompt_version_id, output_schema_version_id);

CREATE INDEX "IX_ai_job_requested_by" ON ai.ai_job (requested_by);

CREATE INDEX ix_ai_job_status_queued_at ON ai.ai_job (status, queued_at);

CREATE INDEX "IX_ai_job_tool_policy_version_id" ON ai.ai_job (tool_policy_version_id);

CREATE UNIQUE INDEX uq_ai_job_governed_resource ON ai.ai_job (governed_resource_id);

CREATE UNIQUE INDEX uq_ai_job_request_id ON ai.ai_job (request_id);

CREATE INDEX ix_ai_job_input_source_snapshot ON ai.ai_job_input (source_snapshot_id);

CREATE INDEX "IX_ai_job_input_source_snapshot_id_source_checksum" ON ai.ai_job_input (source_snapshot_id, source_checksum);

CREATE INDEX ix_ai_review_event_artifact_time ON ai.ai_review_event (artifact_id, occurred_at);

CREATE INDEX "IX_ai_review_event_reviewer_principal_id" ON ai.ai_review_event (reviewer_principal_id);

CREATE INDEX ix_ai_source_scope_security_scope ON ai.ai_source_scope (resource_security_scope_id);

CREATE INDEX "IX_ai_source_snapshot_document_version_id" ON ai.ai_source_snapshot (document_version_id);

CREATE INDEX "IX_ai_source_snapshot_export_manifest_id" ON ai.ai_source_snapshot (export_manifest_id);

CREATE INDEX "IX_ai_source_snapshot_improvement_plan_id" ON ai.ai_source_snapshot (improvement_plan_id);

CREATE INDEX ix_ai_source_snapshot_kind_data_as_of ON ai.ai_source_snapshot (source_kind, data_as_of);

CREATE INDEX "IX_ai_source_snapshot_result_batch_id" ON ai.ai_source_snapshot (result_batch_id);

CREATE INDEX ix_ai_source_snapshot_source_resource ON ai.ai_source_snapshot (source_governed_resource_id);

CREATE UNIQUE INDEX uq_ai_source_snapshot_governed_resource ON ai.ai_source_snapshot (governed_resource_id);

CREATE INDEX "IX_anchor_assessment_assessment_item_id_syllabus_version_id" ON academic.anchor_assessment (assessment_item_id, syllabus_version_id);

CREATE INDEX "IX_anchor_assessment_syllabus_version_id" ON academic.anchor_assessment (syllabus_version_id);

CREATE UNIQUE INDEX uq_anchor_assessment_source_item_role ON academic.anchor_assessment (direct_measurement_source_id, assessment_item_id, anchor_role);

CREATE INDEX "IX_anchor_criterion_syllabus_traceability_id" ON academic.anchor_criterion (syllabus_traceability_id);

CREATE UNIQUE INDEX uq_archive_manifest_resource_period ON audit.archive_manifest (governed_resource_id, period_from, period_to);

CREATE INDEX "IX_assessment_item_parent_id_syllabus_version_id" ON portfolio.assessment_item (parent_id, syllabus_version_id);

CREATE UNIQUE INDEX uq_assessment_item_version_code ON portfolio.assessment_item (syllabus_version_id, assessment_code);

CREATE INDEX "IX_assessment_question_assessment_item_id_syllabus_version_id" ON portfolio.assessment_question (assessment_item_id, syllabus_version_id);

CREATE INDEX "IX_assessment_question_syllabus_version_id" ON portfolio.assessment_question (syllabus_version_id);

CREATE UNIQUE INDEX uq_assessment_question_item_code ON portfolio.assessment_question (assessment_item_id, question_code);

CREATE INDEX ix_audit_event_actor_occurred_at ON audit.audit_event (actor_principal_id, occurred_at);

CREATE INDEX ix_audit_event_chain_sequence ON audit.audit_event (chain_id, chain_sequence);

CREATE INDEX "IX_audit_event_cohort_id" ON audit.audit_event (cohort_id);

CREATE INDEX "IX_audit_event_course_id" ON audit.audit_event (course_id);

CREATE INDEX "IX_audit_event_course_offering_id" ON audit.audit_event (course_offering_id);

CREATE INDEX "IX_audit_event_curriculum_path_id" ON audit.audit_event (curriculum_path_id);

CREATE INDEX ix_audit_event_event_hash ON audit.audit_event (event_hash);

CREATE INDEX "IX_audit_event_impersonator_principal_id" ON audit.audit_event (impersonator_principal_id);

CREATE INDEX "IX_audit_event_measurement_period_id" ON audit.audit_event (measurement_period_id);

CREATE INDEX ix_audit_event_occurred_at ON audit.audit_event (occurred_at);

CREATE INDEX "IX_audit_event_org_unit_id" ON audit.audit_event (org_unit_id);

CREATE INDEX "IX_audit_event_program_id" ON audit.audit_event (program_id);

CREATE INDEX ix_audit_event_program_version_occurred_at ON audit.audit_event (program_version_id, occurred_at);

CREATE INDEX ix_audit_event_request_id ON audit.audit_event (request_id);

CREATE INDEX ix_audit_event_resource_occurred_at ON audit.audit_event (resource_type, resource_id, occurred_at);

CREATE INDEX "IX_audit_event_student_id" ON audit.audit_event (student_id);

CREATE INDEX ix_auth_session_idp_session_hash ON iam.auth_session (idp_session_hash);

CREATE INDEX ix_auth_session_principal_expires_at ON iam.auth_session (principal_id, expires_at);

CREATE UNIQUE INDEX uq_auth_session_token_hash ON iam.auth_session (session_token_hash);

CREATE INDEX ix_batch_delta_batches_entity ON result.batch_delta (old_batch_id, new_batch_id, entity_type);

CREATE INDEX "IX_batch_delta_new_batch_id" ON result.batch_delta (new_batch_id);

CREATE INDEX "IX_batch_supersession_created_by" ON result.batch_supersession (created_by);

CREATE INDEX "IX_batch_supersession_new_batch_id" ON result.batch_supersession (new_batch_id);

CREATE INDEX "IX_calculation_policy_owner_org_unit_id" ON measurement.calculation_policy (owner_org_unit_id);

CREATE UNIQUE INDEX uq_calculation_policy_1 ON measurement.calculation_policy (code);

CREATE INDEX "IX_calculation_policy_version_supersedes_id" ON measurement.calculation_policy_version (supersedes_id);

CREATE UNIQUE INDEX uq_calculation_policy_version_1 ON measurement.calculation_policy_version (policy_id, version_no);

CREATE UNIQUE INDEX uq_calculation_policy_version_workflow ON measurement.calculation_policy_version (workflow_instance_id);

CREATE UNIQUE INDEX uq_calculation_run_1 ON result.calculation_run (batch_id, attempt_no);

CREATE INDEX "IX_chat_session_access_scope_id" ON ai.chat_session (access_scope_id);

CREATE INDEX ix_chat_session_owner_activity ON ai.chat_session (owner_principal_id, last_activity_at);

CREATE UNIQUE INDEX uq_chat_session_governed_resource ON ai.chat_session (governed_resource_id);

CREATE INDEX ix_chat_turn_ai_job ON ai.chat_turn (ai_job_id);

CREATE INDEX "IX_chat_turn_assistant_artifact_id_ai_job_id" ON ai.chat_turn (assistant_artifact_id, ai_job_id);

CREATE UNIQUE INDEX uq_chat_turn_assistant_artifact ON ai.chat_turn (assistant_artifact_id) WHERE assistant_artifact_id IS NOT NULL;

CREATE UNIQUE INDEX uq_chat_turn_session_turn_no ON ai.chat_turn (chat_session_id, turn_no);

CREATE UNIQUE INDEX uq_clo_version_code ON portfolio.clo (syllabus_version_id, code);

CREATE UNIQUE INDEX uq_cohort_program_code ON academic.cohort (program_id, code);

CREATE INDEX "IX_cohort_outcome_result_batch_id_academic_year_start_org_unit~" ON result.cohort_outcome_result (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id);

CREATE INDEX "IX_cohort_outcome_result_clo_id" ON result.cohort_outcome_result (clo_id);

CREATE INDEX "IX_cohort_outcome_result_cohort_id" ON result.cohort_outcome_result (cohort_id);

CREATE INDEX "IX_cohort_outcome_result_curriculum_path_id" ON result.cohort_outcome_result (curriculum_path_id);

CREATE INDEX "IX_cohort_outcome_result_measurement_period_id" ON result.cohort_outcome_result (measurement_period_id);

CREATE INDEX "IX_cohort_outcome_result_org_unit_id" ON result.cohort_outcome_result (org_unit_id);

CREATE INDEX "IX_cohort_outcome_result_program_id" ON result.cohort_outcome_result (program_id);

CREATE INDEX "IX_cohort_outcome_result_program_pi_id" ON result.cohort_outcome_result (program_pi_id);

CREATE INDEX "IX_cohort_outcome_result_program_plo_id" ON result.cohort_outcome_result (program_plo_id);

CREATE INDEX "IX_cohort_outcome_result_program_version_id_program_id" ON result.cohort_outcome_result (program_version_id, program_id);

CREATE UNIQUE INDEX uq_cohort_outcome_result_1 ON result.cohort_outcome_result (academic_year_start, batch_id, cohort_id, curriculum_path_id, outcome_level, clo_id, program_pi_id, program_plo_id, method) NULLS NOT DISTINCT;

CREATE INDEX "IX_cohort_population_decision_batch_id_academic_year_start_org~" ON result.cohort_population_decision (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id);

CREATE INDEX "IX_cohort_population_decision_clo_id" ON result.cohort_population_decision (clo_id);

CREATE INDEX "IX_cohort_population_decision_cohort_id" ON result.cohort_population_decision (cohort_id);

CREATE INDEX "IX_cohort_population_decision_curriculum_path_id" ON result.cohort_population_decision (curriculum_path_id);

CREATE INDEX "IX_cohort_population_decision_measurement_period_id" ON result.cohort_population_decision (measurement_period_id);

CREATE INDEX "IX_cohort_population_decision_org_unit_id" ON result.cohort_population_decision (org_unit_id);

CREATE INDEX "IX_cohort_population_decision_program_id" ON result.cohort_population_decision (program_id);

CREATE INDEX "IX_cohort_population_decision_program_pi_id" ON result.cohort_population_decision (program_pi_id);

CREATE INDEX "IX_cohort_population_decision_program_plo_id" ON result.cohort_population_decision (program_plo_id);

CREATE INDEX "IX_cohort_population_decision_program_version_id_program_id" ON result.cohort_population_decision (program_version_id, program_id);

CREATE INDEX "IX_cohort_population_decision_student_id" ON result.cohort_population_decision (student_id);

CREATE UNIQUE INDEX uq_cohort_population_decision_semantic ON result.cohort_population_decision (academic_year_start, batch_id, cohort_id, curriculum_path_id, outcome_level, clo_id, program_pi_id, program_plo_id, method, student_id) NULLS NOT DISTINCT;

CREATE INDEX "IX_comment_author_principal_id" ON workflow.comment (author_principal_id);

CREATE INDEX ix_comment_instance_created_at ON workflow.comment (instance_id, created_at);

CREATE INDEX "IX_competency_parent_id_program_version_id" ON academic.competency (parent_id, program_version_id);

CREATE UNIQUE INDEX uq_competency_version_code ON academic.competency (program_version_id, code);

CREATE INDEX "IX_competency_plo_mapping_competency_id_program_version_id" ON academic.competency_plo_mapping (competency_id, program_version_id);

CREATE INDEX "IX_competency_plo_mapping_program_plo_id_program_version_id" ON academic.competency_plo_mapping (program_plo_id, program_version_id);

CREATE INDEX "IX_competency_plo_mapping_program_version_id" ON academic.competency_plo_mapping (program_version_id);

CREATE INDEX "IX_course_owner_org_unit_id" ON academic.course (owner_org_unit_id);

CREATE UNIQUE INDEX uq_course_code ON academic.course (code);

CREATE INDEX "IX_course_crosswalk_from_program_course_id" ON academic.course_crosswalk (from_program_course_id);

CREATE INDEX "IX_course_crosswalk_to_program_course_id" ON academic.course_crosswalk (to_program_course_id);

CREATE UNIQUE INDEX uq_course_crosswalk_line ON academic.course_crosswalk (program_version_crosswalk_id, from_program_course_id, to_program_course_id, relation_type);

CREATE UNIQUE INDEX uq_course_objective_version_code ON portfolio.course_objective (syllabus_version_id, code);

CREATE INDEX "IX_course_objective_clo_clo_id_syllabus_version_id" ON portfolio.course_objective_clo (clo_id, syllabus_version_id);

CREATE INDEX "IX_course_objective_clo_course_objective_id_syllabus_version_id" ON portfolio.course_objective_clo (course_objective_id, syllabus_version_id);

CREATE INDEX "IX_course_offering_course_version_id" ON academic.course_offering (course_version_id);

CREATE INDEX "IX_course_offering_org_unit_id" ON academic.course_offering (org_unit_id);

CREATE INDEX "IX_course_offering_program_course_id_program_version_id_course~" ON academic.course_offering (program_course_id, program_version_id, course_version_id);

CREATE INDEX "IX_course_offering_program_version_id" ON academic.course_offering (program_version_id);

CREATE INDEX "IX_course_offering_syllabus_version_id_program_course_id_progr~" ON academic.course_offering (syllabus_version_id, program_course_id, program_version_id, course_version_id);

CREATE UNIQUE INDEX uq_course_offering_manual_code ON academic.course_offering (academic_year_start, term_code, code) WHERE source_system_id IS NULL;

CREATE UNIQUE INDEX uq_course_offering_source_code ON academic.course_offering (source_system_id, academic_year_start, term_code, code) WHERE source_system_id IS NOT NULL;

CREATE UNIQUE INDEX uq_course_offering_source_record ON academic.course_offering (source_system_id, source_record_id) WHERE source_system_id IS NOT NULL AND source_record_id IS NOT NULL;

CREATE INDEX "IX_course_offering_instructor_staff_id" ON academic.course_offering_instructor (staff_id);

CREATE UNIQUE INDEX uq_course_offering_instructor_assignment ON academic.course_offering_instructor (course_offering_id, staff_id, assignment_role, effective_from);

CREATE INDEX "IX_course_pi_mapping_exception_decision_id" ON academic.course_pi_mapping (exception_decision_id);

CREATE INDEX "IX_course_pi_mapping_program_course_id_program_version_id" ON academic.course_pi_mapping (program_course_id, program_version_id);

CREATE INDEX "IX_course_pi_mapping_program_pi_id_program_version_id" ON academic.course_pi_mapping (program_pi_id, program_version_id);

CREATE INDEX "IX_course_pi_mapping_source_shared_mapping_id" ON academic.course_pi_mapping (source_shared_mapping_id);

CREATE UNIQUE INDEX uq_course_pi_mapping_version_course_pi ON academic.course_pi_mapping (program_version_id, program_course_id, program_pi_id);

CREATE INDEX "IX_course_pi_path_override_course_pi_mapping_id_program_versio~" ON academic.course_pi_path_override (course_pi_mapping_id, program_version_id);

CREATE INDEX "IX_course_pi_path_override_curriculum_path_id_program_version_~" ON academic.course_pi_path_override (curriculum_path_id, program_version_id);

CREATE INDEX "IX_course_pi_path_override_exception_decision_id" ON academic.course_pi_path_override (exception_decision_id);

CREATE INDEX "IX_course_pi_path_override_program_version_id" ON academic.course_pi_path_override (program_version_id);

CREATE UNIQUE INDEX uq_course_pi_path_override_mapping_path ON academic.course_pi_path_override (course_pi_mapping_id, curriculum_path_id);

CREATE INDEX "IX_course_pi_result_batch_id_academic_year_start_org_unit_id_p~" ON result.course_pi_result (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id);

CREATE INDEX "IX_course_pi_result_cohort_id" ON result.course_pi_result (cohort_id);

CREATE INDEX "IX_course_pi_result_course_id" ON result.course_pi_result (course_id);

CREATE INDEX "IX_course_pi_result_course_offering_id" ON result.course_pi_result (course_offering_id);

CREATE INDEX "IX_course_pi_result_curriculum_path_id" ON result.course_pi_result (curriculum_path_id);

CREATE INDEX "IX_course_pi_result_measurement_period_id" ON result.course_pi_result (measurement_period_id);

CREATE INDEX "IX_course_pi_result_org_unit_id" ON result.course_pi_result (org_unit_id);

CREATE INDEX "IX_course_pi_result_program_id" ON result.course_pi_result (program_id);

CREATE INDEX "IX_course_pi_result_program_pi_id" ON result.course_pi_result (program_pi_id);

CREATE INDEX "IX_course_pi_result_program_version_id_program_id" ON result.course_pi_result (program_version_id, program_id);

CREATE INDEX "IX_course_pi_result_student_id" ON result.course_pi_result (student_id);

CREATE INDEX "IX_course_pi_result_student_path_id" ON result.course_pi_result (student_path_id);

CREATE UNIQUE INDEX uq_course_pi_result_1 ON result.course_pi_result (academic_year_start, batch_id, student_id, course_offering_id, program_pi_id);

CREATE INDEX "IX_course_prerequisite_group_program_version_id" ON academic.course_prerequisite_group (program_version_id);

CREATE INDEX "IX_course_prerequisite_group_target_program_course_id_program_~" ON academic.course_prerequisite_group (target_program_course_id, program_version_id);

CREATE UNIQUE INDEX uq_course_prerequisite_group_target_no ON academic.course_prerequisite_group (target_program_course_id, group_no);

CREATE INDEX "IX_course_prerequisite_item_required_program_course_id" ON academic.course_prerequisite_item (required_program_course_id);

CREATE INDEX "IX_course_version_decision_id" ON academic.course_version (decision_id);

CREATE INDEX "IX_course_version_supersedes_id" ON academic.course_version (supersedes_id);

CREATE UNIQUE INDEX uq_course_version_course_no ON academic.course_version (course_id, version_no);

CREATE UNIQUE INDEX uq_course_version_workflow ON academic.course_version (workflow_instance_id);

CREATE INDEX "IX_course_version_relation_decision_id" ON academic.course_version_relation (decision_id);

CREATE INDEX "IX_course_version_relation_program_version_id" ON academic.course_version_relation (program_version_id);

CREATE INDEX "IX_course_version_relation_to_course_version_id" ON academic.course_version_relation (to_course_version_id);

CREATE UNIQUE INDEX uq_course_version_relation_scope ON academic.course_version_relation (from_course_version_id, to_course_version_id, program_version_id, relation_type);

CREATE INDEX "IX_criterion_pi_contribution_academic_year_start_student_crite~" ON result.criterion_pi_contribution (academic_year_start, student_criterion_result_id, batch_id, student_id, course_offering_id, rubric_criterion_id);

CREATE INDEX "IX_criterion_pi_contribution_assessment_item_id" ON result.criterion_pi_contribution (assessment_item_id);

CREATE INDEX "IX_criterion_pi_contribution_batch_id_input_snapshot_id_academ~" ON result.criterion_pi_contribution (batch_id, input_snapshot_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id);

CREATE INDEX "IX_criterion_pi_contribution_cohort_id" ON result.criterion_pi_contribution (cohort_id);

CREATE INDEX "IX_criterion_pi_contribution_course_id" ON result.criterion_pi_contribution (course_id);

CREATE INDEX "IX_criterion_pi_contribution_course_offering_id" ON result.criterion_pi_contribution (course_offering_id);

CREATE INDEX "IX_criterion_pi_contribution_curriculum_path_id" ON result.criterion_pi_contribution (curriculum_path_id);

CREATE INDEX "IX_criterion_pi_contribution_input_snapshot_id_syllabus_tracea~" ON result.criterion_pi_contribution (input_snapshot_id, syllabus_traceability_id);

CREATE INDEX "IX_criterion_pi_contribution_measurement_period_id" ON result.criterion_pi_contribution (measurement_period_id);

CREATE INDEX "IX_criterion_pi_contribution_org_unit_id" ON result.criterion_pi_contribution (org_unit_id);

CREATE INDEX "IX_criterion_pi_contribution_program_id" ON result.criterion_pi_contribution (program_id);

CREATE INDEX "IX_criterion_pi_contribution_program_pi_id" ON result.criterion_pi_contribution (program_pi_id);

CREATE INDEX "IX_criterion_pi_contribution_program_version_id_program_id" ON result.criterion_pi_contribution (program_version_id, program_id);

CREATE INDEX "IX_criterion_pi_contribution_rubric_criterion_id" ON result.criterion_pi_contribution (rubric_criterion_id);

CREATE INDEX "IX_criterion_pi_contribution_student_id" ON result.criterion_pi_contribution (student_id);

CREATE INDEX "IX_criterion_pi_contribution_student_path_id" ON result.criterion_pi_contribution (student_path_id);

CREATE INDEX "IX_criterion_pi_contribution_syllabus_traceability_id" ON result.criterion_pi_contribution (syllabus_traceability_id);

CREATE UNIQUE INDEX uq_criterion_pi_contribution_1 ON result.criterion_pi_contribution (academic_year_start, batch_id, student_id, course_offering_id, program_pi_id, rubric_criterion_id);

CREATE UNIQUE INDEX "IX_current_publication_batch_id_measurement_period_id" ON result.current_publication (batch_id, measurement_period_id);

CREATE UNIQUE INDEX "IX_current_publication_publication_id_batch_id_measurement_per~" ON result.current_publication (publication_id, batch_id, measurement_period_id);

CREATE INDEX "IX_current_publication_updated_by" ON result.current_publication (updated_by);

CREATE UNIQUE INDEX uq_current_publication_1 ON result.current_publication (publication_id);

CREATE UNIQUE INDEX uq_current_publication_2 ON result.current_publication (batch_id);

CREATE INDEX "IX_curriculum_block_parent_id_curriculum_plan_id" ON academic.curriculum_block (parent_id, curriculum_plan_id);

CREATE UNIQUE INDEX uq_curriculum_block_plan_code ON academic.curriculum_block (curriculum_plan_id, code);

CREATE INDEX "IX_curriculum_elective_group_curriculum_block_id" ON academic.curriculum_elective_group (curriculum_block_id);

CREATE UNIQUE INDEX uq_curriculum_elective_group_path_code ON academic.curriculum_elective_group (curriculum_path_id, code);

CREATE UNIQUE INDEX uq_curriculum_path_version_code ON academic.curriculum_path (program_version_id, code);

CREATE UNIQUE INDEX uq_curriculum_path_workflow ON academic.curriculum_path (workflow_instance_id);

CREATE INDEX "IX_curriculum_path_course_elective_group_id" ON academic.curriculum_path_course (elective_group_id);

CREATE INDEX "IX_curriculum_path_course_program_course_id" ON academic.curriculum_path_course (program_course_id);

CREATE UNIQUE INDEX uq_curriculum_path_course_member ON academic.curriculum_path_course (curriculum_path_id, program_course_id, elective_group_id) NULLS NOT DISTINCT;

CREATE UNIQUE INDEX uq_curriculum_plan_program_version ON academic.curriculum_plan (program_version_id);

CREATE INDEX "IX_data_handling_policy_version_activation_decision_id_id" ON ai.data_handling_policy_version (activation_decision_id, id);

CREATE INDEX "IX_data_handling_policy_version_approved_by" ON ai.data_handling_policy_version (approved_by);

CREATE UNIQUE INDEX uq_data_handling_policy_version_activation_decision ON ai.data_handling_policy_version (activation_decision_id) WHERE activation_decision_id IS NOT NULL;

CREATE UNIQUE INDEX uq_data_handling_policy_version_code_version_no ON ai.data_handling_policy_version (code, version_no);

CREATE INDEX "IX_database_principal_binding_access_scope_id" ON iam.database_principal_binding (access_scope_id);

CREATE INDEX ix_database_principal_binding_role_status_effective ON iam.database_principal_binding (database_role_name, status, effective_from);

CREATE INDEX "IX_database_principal_binding_service_principal_id" ON iam.database_principal_binding (service_principal_id);

CREATE INDEX ix_decision_document_document_version ON academic.decision_document (document_version_id);

CREATE INDEX ix_decision_record_document_version_id ON academic.decision_record (document_version_id);

CREATE UNIQUE INDEX uq_decision_record_issuer_number ON academic.decision_record (issuer_org_unit_id, decision_number);

CREATE UNIQUE INDEX uq_definition_code_version_no ON workflow.definition (code, version_no);

CREATE INDEX ix_deployment_event_release_started ON ops.deployment_event (application_release, started_at);

CREATE INDEX "IX_direct_measurement_plan_curriculum_path_id_program_version_~" ON academic.direct_measurement_plan (curriculum_path_id, program_version_id);

CREATE INDEX "IX_direct_measurement_plan_program_pi_id_program_version_id" ON academic.direct_measurement_plan (program_pi_id, program_version_id);

CREATE INDEX "IX_direct_measurement_plan_supersedes_id" ON academic.direct_measurement_plan (supersedes_id);

CREATE UNIQUE INDEX uq_direct_measurement_plan_version ON academic.direct_measurement_plan (program_version_id, curriculum_path_id, program_pi_id, version_no);

CREATE UNIQUE INDEX uq_direct_measurement_plan_workflow ON academic.direct_measurement_plan (workflow_instance_id);

CREATE INDEX "IX_direct_measurement_source_course_pi_mapping_id_program_vers~" ON academic.direct_measurement_source (course_pi_mapping_id, program_version_id, program_pi_id);

CREATE INDEX "IX_direct_measurement_source_curriculum_path_id_program_versio~" ON academic.direct_measurement_source (curriculum_path_id, program_version_id);

CREATE INDEX "IX_direct_measurement_source_direct_measurement_plan_id_progra~" ON academic.direct_measurement_source (direct_measurement_plan_id, program_version_id, curriculum_path_id, program_pi_id);

CREATE INDEX "IX_direct_measurement_source_owner_org_unit_id" ON academic.direct_measurement_source (owner_org_unit_id);

CREATE INDEX "IX_direct_measurement_source_program_pi_id_program_version_id" ON academic.direct_measurement_source (program_pi_id, program_version_id);

CREATE INDEX "IX_direct_measurement_source_program_version_id" ON academic.direct_measurement_source (program_version_id);

CREATE UNIQUE INDEX uq_direct_measurement_source_plan_mapping ON academic.direct_measurement_source (direct_measurement_plan_id, course_pi_mapping_id);

CREATE UNIQUE INDEX uq_direct_pi_criterion_weight_traceability ON portfolio.direct_pi_criterion_weight (syllabus_traceability_id);

CREATE INDEX "IX_disposition_case_approved_by" ON governance.disposition_case (approved_by);

CREATE INDEX "IX_disposition_case_created_by" ON governance.disposition_case (created_by);

CREATE INDEX ix_disposition_case_status ON governance.disposition_case (status);

CREATE UNIQUE INDEX uq_disposition_case_code ON governance.disposition_case (case_code);

CREATE INDEX "IX_disposition_item_governed_resource_id" ON governance.disposition_item (governed_resource_id);

CREATE INDEX "IX_disposition_item_retention_binding_id" ON governance.disposition_item (retention_binding_id);

CREATE INDEX ix_disposition_item_status_completed ON governance.disposition_item (status, completed_at);

CREATE UNIQUE INDEX uq_disposition_item_case_resource ON governance.disposition_item (disposition_case_id, governed_resource_id);

CREATE INDEX ix_document_owner_type_status ON document.document (owner_org_unit_id, document_type, status);

CREATE UNIQUE INDEX uq_document_governed_resource ON document.document (governed_resource_id);

CREATE INDEX "IX_document_rendition_file_object_id" ON document.document_rendition (file_object_id);

CREATE UNIQUE INDEX uq_document_rendition_version_type ON document.document_rendition (document_version_id, rendition_type);

CREATE INDEX "IX_document_version_approved_by" ON document.document_version (approved_by);

CREATE INDEX "IX_document_version_created_by" ON document.document_version (created_by);

CREATE INDEX "IX_document_version_file_object_id" ON document.document_version (file_object_id);

CREATE INDEX "IX_document_version_source_document_version_id" ON document.document_version (source_document_version_id);

CREATE INDEX "IX_document_version_supersedes_id" ON document.document_version (supersedes_id);

CREATE UNIQUE INDEX uq_document_version_document_no ON document.document_version (document_id, version_no);

CREATE UNIQUE INDEX uq_document_version_governed_resource ON document.document_version (governed_resource_id);

CREATE UNIQUE INDEX uq_document_version_workflow ON document.document_version (workflow_instance_id) WHERE workflow_instance_id IS NOT NULL;

CREATE INDEX "IX_enrollment_student_id" ON measurement.enrollment (student_id);

CREATE UNIQUE INDEX uq_enrollment_1 ON measurement.enrollment (course_offering_id, student_id, attempt_no);

CREATE UNIQUE INDEX uq_enrollment_2 ON measurement.enrollment (id, student_id, course_offering_id, attempt_no);

CREATE UNIQUE INDEX uq_enrollment_3 ON measurement.enrollment (source_system_id, source_record_id);

CREATE INDEX "IX_enrollment_revision_enrollment_id_supersedes_id" ON measurement.enrollment_revision (enrollment_id, supersedes_id);

CREATE INDEX "IX_enrollment_revision_ingestion_batch_id" ON measurement.enrollment_revision (ingestion_batch_id);

CREATE UNIQUE INDEX uq_enrollment_revision_1 ON measurement.enrollment_revision (enrollment_id, id);

CREATE UNIQUE INDEX uq_enrollment_revision_2 ON measurement.enrollment_revision (enrollment_id, revision_no);

CREATE INDEX "IX_evaluation_policy_version_decision_id" ON ai.evaluation_policy_version (decision_id);

CREATE UNIQUE INDEX uq_evaluation_policy_version_code_version_no ON ai.evaluation_policy_version (code, version_no);

CREATE UNIQUE INDEX uq_evaluation_policy_version_governed_resource ON ai.evaluation_policy_version (governed_resource_id);

CREATE UNIQUE INDEX uq_evaluation_policy_version_workflow ON ai.evaluation_policy_version (workflow_instance_id);

CREATE INDEX "IX_evaluation_result_case_id" ON ai.evaluation_result (case_id);

CREATE INDEX ix_evaluation_result_run_passed ON ai.evaluation_result (run_id, passed);

CREATE UNIQUE INDEX uq_evaluation_result_run_case ON ai.evaluation_result (run_id, case_id);

CREATE INDEX "IX_evaluation_run_data_handling_policy_version_id" ON ai.evaluation_run (data_handling_policy_version_id);

CREATE INDEX "IX_evaluation_run_evaluation_policy_version_id_evaluation_poli~" ON ai.evaluation_run (evaluation_policy_version_id, evaluation_policy_checksum);

CREATE INDEX "IX_evaluation_run_model_deployment_version_id" ON ai.evaluation_run (model_deployment_version_id);

CREATE INDEX "IX_evaluation_run_output_schema_version_id" ON ai.evaluation_run (output_schema_version_id);

CREATE INDEX "IX_evaluation_run_prompt_version_id_output_schema_version_id" ON ai.evaluation_run (prompt_version_id, output_schema_version_id);

CREATE INDEX ix_evaluation_run_status_started_at ON ai.evaluation_run (status, started_at);

CREATE INDEX "IX_evaluation_run_suite_version_id_suite_checksum" ON ai.evaluation_run (suite_version_id, suite_checksum);

CREATE INDEX "IX_evaluation_run_tool_policy_version_id" ON ai.evaluation_run (tool_policy_version_id);

CREATE UNIQUE INDEX uq_evaluation_run_governed_resource ON ai.evaluation_run (governed_resource_id);

CREATE INDEX "IX_evidence_owner_principal_id" ON document.evidence (owner_principal_id);

CREATE INDEX ix_evidence_owner_status ON document.evidence (owner_org_unit_id, status);

CREATE UNIQUE INDEX uq_evidence_code ON document.evidence (code);

CREATE INDEX ix_evidence_link_resource ON document.evidence_link (resource_type, resource_id);

CREATE INDEX "IX_evidence_version_approved_by" ON document.evidence_version (approved_by);

CREATE INDEX "IX_evidence_version_created_by" ON document.evidence_version (created_by);

CREATE INDEX "IX_evidence_version_document_version_id" ON document.evidence_version (document_version_id);

CREATE INDEX "IX_evidence_version_url_snapshot_file_object_id" ON document.evidence_version (url_snapshot_file_object_id);

CREATE UNIQUE INDEX uq_evidence_version_evidence_no ON document.evidence_version (evidence_id, version_no);

CREATE UNIQUE INDEX uq_evidence_version_governed_resource ON document.evidence_version (governed_resource_id);

CREATE INDEX "IX_export_manifest_access_scope_id" ON audit.export_manifest (access_scope_id);

CREATE INDEX ix_export_manifest_expires_at ON audit.export_manifest (expires_at);

CREATE INDEX "IX_export_manifest_file_object_id" ON audit.export_manifest (file_object_id);

CREATE INDEX "IX_export_manifest_governed_resource_id" ON audit.export_manifest (governed_resource_id);

CREATE INDEX ix_export_manifest_requested_by_created_at ON audit.export_manifest (requested_by, created_at);

CREATE INDEX ix_export_manifest_batch_result_batch ON audit.export_manifest_batch (result_batch_id);

CREATE INDEX ix_external_identity_user_principal ON iam.external_identity (user_principal_id);

CREATE UNIQUE INDEX uq_external_identity_provider_subject ON iam.external_identity (identity_provider_id, subject);

CREATE INDEX "IX_file_object_created_by" ON document.file_object (created_by);

CREATE INDEX ix_file_object_sha256 ON document.file_object (sha256);

CREATE UNIQUE INDEX uq_file_object_governed_resource ON document.file_object (governed_resource_id);

CREATE UNIQUE INDEX uq_file_object_storage_identity ON document.file_object (storage_provider, bucket, object_key, storage_version);

CREATE INDEX ix_governed_resource_type_status ON governance.governed_resource (resource_type, disposition_status);

CREATE INDEX "IX_grader_assignment_assessment_item_id_syllabus_version_id" ON measurement.grader_assignment (assessment_item_id, syllabus_version_id);

CREATE INDEX "IX_grader_assignment_assigned_by" ON measurement.grader_assignment (assigned_by);

CREATE INDEX "IX_grader_assignment_course_offering_id" ON measurement.grader_assignment (course_offering_id);

CREATE INDEX "IX_grader_assignment_principal_id" ON measurement.grader_assignment (principal_id);

CREATE INDEX "IX_grader_assignment_rubric_criterion_id_assessment_item_id_sy~" ON measurement.grader_assignment (rubric_criterion_id, assessment_item_id, syllabus_version_id);

CREATE INDEX "IX_grader_assignment_syllabus_version_id" ON measurement.grader_assignment (syllabus_version_id);

CREATE UNIQUE INDEX uq_grader_assignment_scope ON measurement.grader_assignment (measurement_period_id, course_offering_id, assessment_item_id, rubric_criterion_id, principal_id, assignment_role, effective_from);

CREATE INDEX ix_ground_truth_case_input_snapshot ON ai.ground_truth_case (input_source_snapshot_id);

CREATE UNIQUE INDEX uq_ground_truth_case_suite_case_code ON ai.ground_truth_case (suite_version_id, case_code);

CREATE UNIQUE INDEX uq_ground_truth_suite_code ON ai.ground_truth_suite (code);

CREATE INDEX "IX_ground_truth_suite_version_decision_id" ON ai.ground_truth_suite_version (decision_id);

CREATE UNIQUE INDEX uq_ground_truth_suite_version_governed_resource ON ai.ground_truth_suite_version (governed_resource_id);

CREATE UNIQUE INDEX uq_ground_truth_suite_version_suite_version_no ON ai.ground_truth_suite_version (suite_id, version_no);

CREATE UNIQUE INDEX uq_ground_truth_suite_version_workflow ON ai.ground_truth_suite_version (workflow_instance_id);

CREATE INDEX ix_idempotency_record_expires_at ON integration.idempotency_record (expires_at);

CREATE INDEX "IX_idempotency_record_locked_by" ON integration.idempotency_record (locked_by);

CREATE INDEX ix_idempotency_record_status_lock ON integration.idempotency_record (status, locked_until);

CREATE UNIQUE INDEX uq_idempotency_record_principal_operation_key ON integration.idempotency_record (principal_id, operation_code, idempotency_key);

CREATE UNIQUE INDEX uq_identity_provider_code ON iam.identity_provider (code);

CREATE UNIQUE INDEX uq_identity_provider_protocol_issuer ON iam.identity_provider (protocol, issuer_or_entity_id);

CREATE INDEX "IX_idp_group_role_mapping_access_scope_id" ON iam.idp_group_role_mapping (access_scope_id);

CREATE INDEX "IX_idp_group_role_mapping_role_id" ON iam.idp_group_role_mapping (role_id);

CREATE INDEX "IX_idp_group_role_mapping_role_version_id_role_id" ON iam.idp_group_role_mapping (role_version_id, role_id);

CREATE UNIQUE INDEX uq_idp_group_role_mapping_provider_group_version ON iam.idp_group_role_mapping (identity_provider_id, external_group_id, version_no);

CREATE UNIQUE INDEX uq_idp_group_role_mapping_supersedes ON iam.idp_group_role_mapping (supersedes_id);

CREATE UNIQUE INDEX uq_idp_group_role_mapping_workflow_instance ON iam.idp_group_role_mapping (workflow_instance_id);

CREATE INDEX "IX_improvement_action_owner_org_unit_id" ON quality.improvement_action (owner_org_unit_id);

CREATE INDEX "IX_improvement_action_owner_principal_id" ON quality.improvement_action (owner_principal_id);

CREATE UNIQUE INDEX uq_improvement_action_1 ON quality.improvement_action (improvement_plan_id, action_no);

CREATE INDEX "IX_improvement_document_document_version_id" ON quality.improvement_document (document_version_id);

CREATE INDEX "IX_improvement_evidence_evidence_version_id" ON quality.improvement_evidence (evidence_version_id);

CREATE INDEX "IX_improvement_evidence_improvement_action_id_improvement_plan~" ON quality.improvement_evidence (improvement_action_id, improvement_plan_id);

CREATE INDEX "IX_improvement_evidence_verified_by" ON quality.improvement_evidence (verified_by);

CREATE UNIQUE INDEX uq_improvement_evidence_plan_version_role ON quality.improvement_evidence (improvement_plan_id, evidence_version_id, link_role);

CREATE INDEX ix_improvement_finding_cohort_result ON quality.improvement_finding (academic_year_start, cohort_outcome_result_id);

CREATE INDEX "IX_improvement_finding_improvement_plan_id" ON quality.improvement_finding (improvement_plan_id);

CREATE INDEX ix_improvement_finding_result_alert ON quality.improvement_finding (academic_year_start, result_alert_id);

CREATE INDEX "IX_improvement_plan_created_by" ON quality.improvement_plan (created_by);

CREATE INDEX "IX_improvement_plan_owner_principal_id" ON quality.improvement_plan (owner_principal_id);

CREATE INDEX "IX_improvement_plan_program_version_id" ON quality.improvement_plan (program_version_id);

CREATE UNIQUE INDEX uq_improvement_plan_1 ON quality.improvement_plan (governed_resource_id);

CREATE UNIQUE INDEX uq_improvement_plan_2 ON quality.improvement_plan (org_unit_id, code);

CREATE UNIQUE INDEX uq_improvement_plan_workflow ON quality.improvement_plan (workflow_instance_id);

CREATE INDEX ix_inbox_message_claim ON integration.inbox_message (status, locked_until, received_at);

CREATE UNIQUE INDEX uq_inbox_message_source_message ON integration.inbox_message (source_system_id, message_id);

CREATE UNIQUE INDEX uq_inbox_message_source_nonce ON integration.inbox_message (source_system_id, nonce);

CREATE INDEX "IX_indirect_instrument_owner_org_unit_id" ON measurement.indirect_instrument (owner_org_unit_id);

CREATE UNIQUE INDEX uq_indirect_instrument_1 ON measurement.indirect_instrument (code);

CREATE UNIQUE INDEX uq_indirect_instrument_version_1 ON measurement.indirect_instrument_version (instrument_id, version_no);

CREATE UNIQUE INDEX uq_indirect_instrument_version_workflow ON measurement.indirect_instrument_version (workflow_instance_id);

CREATE INDEX "IX_indirect_item_program_pi_id_program_version_id" ON measurement.indirect_item (program_pi_id, program_version_id);

CREATE INDEX "IX_indirect_item_program_plo_id_program_version_id" ON measurement.indirect_item (program_plo_id, program_version_id);

CREATE INDEX "IX_indirect_item_program_version_id" ON measurement.indirect_item (program_version_id);

CREATE UNIQUE INDEX uq_indirect_item_1 ON measurement.indirect_item (id, instrument_version_id, program_version_id);

CREATE UNIQUE INDEX uq_indirect_item_2 ON measurement.indirect_item (instrument_version_id, program_version_id, code);

CREATE INDEX "IX_indirect_observation_instrument_version_id" ON measurement.indirect_observation (instrument_version_id);

CREATE INDEX "IX_indirect_observation_item_id_instrument_version_id_program_~" ON measurement.indirect_observation (item_id, instrument_version_id, program_version_id);

CREATE INDEX "IX_indirect_observation_response_batch_id_instrument_version_i~" ON measurement.indirect_observation (response_batch_id, instrument_version_id, program_version_id);

CREATE INDEX "IX_indirect_observation_student_id" ON measurement.indirect_observation (student_id);

CREATE UNIQUE INDEX uq_indirect_observation_response ON measurement.indirect_observation (response_batch_id, item_id, respondent_key);

CREATE INDEX "IX_indirect_response_batch_instrument_version_id" ON measurement.indirect_response_batch (instrument_version_id);

CREATE INDEX "IX_indirect_response_batch_measurement_period_id_program_versi~" ON measurement.indirect_response_batch (measurement_period_id, program_version_id);

CREATE INDEX "IX_indirect_response_batch_program_version_id" ON measurement.indirect_response_batch (program_version_id);

CREATE UNIQUE INDEX uq_indirect_response_batch_1 ON measurement.indirect_response_batch (id, instrument_version_id, program_version_id);

CREATE INDEX "IX_ingestion_batch_file_object_id" ON integration.ingestion_batch (file_object_id);

CREATE INDEX "IX_ingestion_batch_governed_resource_id" ON integration.ingestion_batch (governed_resource_id);

CREATE INDEX ix_ingestion_batch_source_received ON integration.ingestion_batch (source_system_id, received_at);

CREATE UNIQUE INDEX uq_ingestion_batch_source_idempotency ON integration.ingestion_batch (source_system_id, idempotency_key);

CREATE INDEX "IX_input_snapshot_created_by" ON measurement.input_snapshot (created_by);

CREATE INDEX "IX_input_snapshot_institution_template_version_id" ON measurement.input_snapshot (institution_template_version_id);

CREATE INDEX "IX_input_snapshot_measurement_period_id_org_unit_id_program_ve~" ON measurement.input_snapshot (measurement_period_id, org_unit_id, program_version_id, academic_year_start);

CREATE INDEX "IX_input_snapshot_org_unit_id" ON measurement.input_snapshot (org_unit_id);

CREATE INDEX "IX_input_snapshot_parent_snapshot_id_measurement_period_id" ON measurement.input_snapshot (parent_snapshot_id, measurement_period_id);

CREATE INDEX "IX_input_snapshot_policy_version_id" ON measurement.input_snapshot (policy_version_id);

CREATE INDEX "IX_input_snapshot_program_policy_binding_id_program_version_id~" ON measurement.input_snapshot (program_policy_binding_id, program_version_id, policy_version_id);

CREATE INDEX "IX_input_snapshot_program_version_id_institution_template_vers~" ON measurement.input_snapshot (program_version_id, institution_template_version_id);

CREATE INDEX "IX_input_snapshot_sealed_by" ON measurement.input_snapshot (sealed_by);

CREATE UNIQUE INDEX uq_input_snapshot_1 ON measurement.input_snapshot (governed_resource_id);

CREATE UNIQUE INDEX uq_input_snapshot_2 ON measurement.input_snapshot (measurement_period_id, snapshot_no);

CREATE INDEX ix_instance_definition_state ON workflow.instance (definition_id, current_state);

CREATE INDEX "IX_instance_started_by" ON workflow.instance (started_by);

CREATE INDEX "IX_institution_template_owner_org_unit_id" ON academic.institution_template (owner_org_unit_id);

CREATE UNIQUE INDEX uq_institution_template_code ON academic.institution_template (code);

CREATE INDEX "IX_institution_template_version_decision_id" ON academic.institution_template_version (decision_id);

CREATE INDEX "IX_institution_template_version_supersedes_id" ON academic.institution_template_version (supersedes_id);

CREATE UNIQUE INDEX uq_institution_template_version_template_no ON academic.institution_template_version (institution_template_id, version_no);

CREATE UNIQUE INDEX uq_institution_template_version_workflow ON academic.institution_template_version (workflow_instance_id);

CREATE INDEX ix_job_attempt_worker_started ON ops.job_attempt (worker_id, started_at);

CREATE INDEX ix_learning_material_version_sort ON portfolio.learning_material (syllabus_version_id, sort_order);

CREATE INDEX "IX_legal_hold_approved_by" ON governance.legal_hold (approved_by);

CREATE INDEX "IX_legal_hold_created_by" ON governance.legal_hold (created_by);

CREATE INDEX ix_legal_hold_status ON governance.legal_hold (status);

CREATE UNIQUE INDEX uq_legal_hold_code ON governance.legal_hold (code);

CREATE INDEX "IX_legal_hold_item_added_by" ON governance.legal_hold_item (added_by);

CREATE INDEX ix_legal_hold_item_resource ON governance.legal_hold_item (governed_resource_id);

CREATE UNIQUE INDEX uq_llo_version_code ON portfolio.llo (syllabus_version_id, code);

CREATE INDEX "IX_llo_clo_mapping_clo_id_syllabus_version_id" ON portfolio.llo_clo_mapping (clo_id, syllabus_version_id);

CREATE INDEX "IX_llo_clo_mapping_llo_id_syllabus_version_id" ON portfolio.llo_clo_mapping (llo_id, syllabus_version_id);

CREATE INDEX "IX_measurement_period_program_policy_binding_id_program_versio~" ON measurement.measurement_period (program_policy_binding_id, program_version_id);

CREATE INDEX "IX_measurement_period_program_version_id" ON measurement.measurement_period (program_version_id);

CREATE UNIQUE INDEX uq_measurement_period_1 ON measurement.measurement_period (org_unit_id, code);

CREATE UNIQUE INDEX uq_measurement_period_2 ON measurement.measurement_period (id, program_version_id);

CREATE UNIQUE INDEX uq_measurement_period_3 ON measurement.measurement_period (id, program_version_id, academic_year_start);

CREATE UNIQUE INDEX uq_measurement_period_workflow ON measurement.measurement_period (workflow_instance_id);

CREATE INDEX "IX_measurement_period_cohort_cohort_id" ON measurement.measurement_period_cohort (cohort_id);

CREATE INDEX "IX_measurement_period_cohort_program_version_id_cohort_id" ON measurement.measurement_period_cohort (program_version_id, cohort_id);

CREATE UNIQUE INDEX uq_measurement_period_cohort_1 ON measurement.measurement_period_cohort (measurement_period_id, program_version_id, cohort_id);

CREATE INDEX "IX_measurement_period_offering_course_offering_id_program_vers~" ON measurement.measurement_period_offering (course_offering_id, program_version_id, academic_year_start);

CREATE INDEX "IX_measurement_period_offering_measurement_period_id_program_v~" ON measurement.measurement_period_offering (measurement_period_id, program_version_id, academic_year_start);

CREATE INDEX "IX_measurement_period_target_clo_id_syllabus_version_id" ON measurement.measurement_period_target (clo_id, syllabus_version_id);

CREATE INDEX "IX_measurement_period_target_course_offering_id" ON measurement.measurement_period_target (course_offering_id);

CREATE INDEX "IX_measurement_period_target_measurement_period_id_course_offe~" ON measurement.measurement_period_target (measurement_period_id, course_offering_id);

CREATE INDEX "IX_measurement_period_target_measurement_period_id_program_ver~" ON measurement.measurement_period_target (measurement_period_id, program_version_id);

CREATE INDEX "IX_measurement_period_target_program_pi_id_program_version_id" ON measurement.measurement_period_target (program_pi_id, program_version_id);

CREATE INDEX "IX_measurement_period_target_program_plo_id_program_version_id" ON measurement.measurement_period_target (program_plo_id, program_version_id);

CREATE INDEX "IX_measurement_period_target_syllabus_version_id_program_versi~" ON measurement.measurement_period_target (syllabus_version_id, program_version_id);

CREATE UNIQUE INDEX uq_measurement_period_target_1 ON measurement.measurement_period_target (measurement_period_id, outcome_level, course_offering_id, syllabus_version_id, clo_id, program_pi_id, program_plo_id) NULLS NOT DISTINCT;

CREATE INDEX "IX_measurement_threshold_override_clo_id_syllabus_version_id" ON measurement.measurement_threshold_override (clo_id, syllabus_version_id);

CREATE INDEX "IX_measurement_threshold_override_course_offering_id" ON measurement.measurement_threshold_override (course_offering_id);

CREATE INDEX "IX_measurement_threshold_override_measurement_period_id_course~" ON measurement.measurement_threshold_override (measurement_period_id, course_offering_id);

CREATE INDEX "IX_measurement_threshold_override_measurement_period_id_progra~" ON measurement.measurement_threshold_override (measurement_period_id, program_version_id);

CREATE INDEX "IX_measurement_threshold_override_program_pi_id_program_versio~" ON measurement.measurement_threshold_override (program_pi_id, program_version_id);

CREATE INDEX "IX_measurement_threshold_override_program_plo_id_program_versi~" ON measurement.measurement_threshold_override (program_plo_id, program_version_id);

CREATE INDEX "IX_measurement_threshold_override_syllabus_version_id_program_~" ON measurement.measurement_threshold_override (syllabus_version_id, program_version_id);

CREATE UNIQUE INDEX uq_measurement_threshold_override_1 ON measurement.measurement_threshold_override (measurement_period_id, outcome_level, course_offering_id, syllabus_version_id, clo_id, program_pi_id, program_plo_id) NULLS NOT DISTINCT;

CREATE UNIQUE INDEX uq_measurement_threshold_override_workflow ON measurement.measurement_threshold_override (workflow_instance_id);

CREATE INDEX ix_model_deployment_owner_org_unit ON ai.model_deployment (owner_org_unit_id);

CREATE UNIQUE INDEX uq_model_deployment_code ON ai.model_deployment (code);

CREATE INDEX "IX_model_deployment_version_activation_decision_id_id" ON ai.model_deployment_version (activation_decision_id, id);

CREATE INDEX "IX_model_deployment_version_approved_by" ON ai.model_deployment_version (approved_by);

CREATE UNIQUE INDEX uq_model_deployment_version_activation_decision ON ai.model_deployment_version (activation_decision_id) WHERE activation_decision_id IS NOT NULL;

CREATE UNIQUE INDEX uq_model_deployment_version_deployment_version_no ON ai.model_deployment_version (model_deployment_id, version_no);

CREATE INDEX ix_object_reference_file_effective_to ON governance.object_reference (file_object_id, effective_to);

CREATE INDEX "IX_operation_job_access_scope_id" ON ops.operation_job (access_scope_id);

CREATE INDEX "IX_operation_job_cancel_requested_by" ON ops.operation_job (cancel_requested_by);

CREATE INDEX ix_operation_job_claim ON ops.operation_job (status, queue_name, available_at, priority);

CREATE INDEX ix_operation_job_correlation_id ON ops.operation_job (correlation_id);

CREATE INDEX "IX_operation_job_leased_by_principal_id" ON ops.operation_job (leased_by_principal_id);

CREATE INDEX ix_operation_job_request_id ON ops.operation_job (request_id);

CREATE INDEX "IX_operation_job_requested_by" ON ops.operation_job (requested_by);

CREATE INDEX "IX_org_unit_created_by" ON academic.org_unit (created_by);

CREATE INDEX ix_org_unit_parent_id ON academic.org_unit (parent_id);

CREATE INDEX "IX_org_unit_updated_by" ON academic.org_unit (updated_by);

CREATE UNIQUE INDEX uq_org_unit_code ON academic.org_unit (code);

CREATE INDEX ix_outbox_message_aggregate ON integration.outbox_message (aggregate_type, aggregate_id, aggregate_version);

CREATE INDEX ix_outbox_message_claim ON integration.outbox_message (status, available_at, locked_until);

CREATE INDEX ix_outbox_message_correlation_id ON integration.outbox_message (correlation_id);

CREATE INDEX "IX_output_schema_version_activation_decision_id_id" ON ai.output_schema_version (activation_decision_id, id);

CREATE INDEX "IX_output_schema_version_approved_by" ON ai.output_schema_version (approved_by);

CREATE UNIQUE INDEX uq_output_schema_version_activation_decision ON ai.output_schema_version (activation_decision_id) WHERE activation_decision_id IS NOT NULL;

CREATE UNIQUE INDEX uq_output_schema_version_code_version_no ON ai.output_schema_version (code, version_no);

CREATE INDEX "IX_period_population_enrollment_enrollment_revision_id" ON measurement.period_population_enrollment (enrollment_revision_id);

CREATE INDEX "IX_period_population_member_cohort_id" ON measurement.period_population_member (cohort_id);

CREATE INDEX "IX_period_population_member_curriculum_path_id_program_version~" ON measurement.period_population_member (curriculum_path_id, program_version_id);

CREATE INDEX "IX_period_population_member_decided_by" ON measurement.period_population_member (decided_by);

CREATE INDEX "IX_period_population_member_measurement_period_id_program_vers~" ON measurement.period_population_member (measurement_period_id, program_version_id, cohort_id);

CREATE INDEX "IX_period_population_member_student_id_cohort_id" ON measurement.period_population_member (student_id, cohort_id);

CREATE INDEX "IX_period_population_member_student_path_id_student_id_program~" ON measurement.period_population_member (student_path_id, student_id, program_version_id, curriculum_path_id);

CREATE UNIQUE INDEX uq_permission_resource_action_field_scope ON iam.permission (resource_type, action, field_scope);

CREATE INDEX ix_person_contact_lookup_hash ON academic.person (contact_lookup_hash);

CREATE UNIQUE INDEX uq_person_source_identity ON academic.person (source_system_id, source_person_id) NULLS NOT DISTINCT WHERE source_system_id IS NOT NULL AND source_person_id IS NOT NULL;

CREATE INDEX "IX_pi_crosswalk_from_program_pi_id" ON academic.pi_crosswalk (from_program_pi_id);

CREATE INDEX "IX_pi_crosswalk_to_program_pi_id" ON academic.pi_crosswalk (to_program_pi_id);

CREATE UNIQUE INDEX uq_pi_crosswalk_line ON academic.pi_crosswalk (program_version_crosswalk_id, from_program_pi_id, to_program_pi_id, relation_type);

CREATE INDEX "IX_plan_waiver_finding_id" ON quality.plan_waiver (finding_id);

CREATE INDEX "IX_plan_waiver_requested_by" ON quality.plan_waiver (requested_by);

CREATE UNIQUE INDEX uq_plan_waiver_workflow ON quality.plan_waiver (workflow_instance_id);

CREATE INDEX "IX_plo_crosswalk_from_program_plo_id" ON academic.plo_crosswalk (from_program_plo_id);

CREATE INDEX "IX_plo_crosswalk_to_program_plo_id" ON academic.plo_crosswalk (to_program_plo_id);

CREATE UNIQUE INDEX uq_plo_crosswalk_line ON academic.plo_crosswalk (program_version_crosswalk_id, from_program_plo_id, to_program_plo_id, relation_type);

CREATE INDEX "IX_po_competency_mapping_competency_id_program_version_id" ON academic.po_competency_mapping (competency_id, program_version_id);

CREATE INDEX "IX_po_competency_mapping_program_objective_id_program_version_~" ON academic.po_competency_mapping (program_objective_id, program_version_id);

CREATE INDEX "IX_po_competency_mapping_program_version_id" ON academic.po_competency_mapping (program_version_id);

CREATE INDEX "IX_po_plo_mapping_program_objective_id_program_version_id" ON academic.po_plo_mapping (program_objective_id, program_version_id);

CREATE INDEX "IX_po_plo_mapping_program_plo_id_program_version_id" ON academic.po_plo_mapping (program_plo_id, program_version_id);

CREATE INDEX "IX_po_plo_mapping_program_version_id" ON academic.po_plo_mapping (program_version_id);

CREATE INDEX "IX_privacy_request_approved_by" ON governance.privacy_request (approved_by);

CREATE INDEX ix_privacy_request_subject_status ON governance.privacy_request (subject_person_id, status);

CREATE UNIQUE INDEX uq_privacy_request_disposition_case ON governance.privacy_request (disposition_case_id) WHERE disposition_case_id IS NOT NULL;

CREATE INDEX "IX_program_created_by" ON academic.program (created_by);

CREATE INDEX "IX_program_owner_org_unit_id" ON academic.program (owner_org_unit_id);

CREATE INDEX "IX_program_updated_by" ON academic.program (updated_by);

CREATE UNIQUE INDEX uq_program_code ON academic.program (code);

CREATE INDEX "IX_program_course_course_version_id" ON academic.program_course (course_version_id);

CREATE INDEX "IX_program_course_curriculum_block_id" ON academic.program_course (curriculum_block_id);

CREATE UNIQUE INDEX uq_program_course_version_course_version ON academic.program_course (program_version_id, course_version_id);

CREATE UNIQUE INDEX uq_program_objective_version_code ON academic.program_objective (program_version_id, code);

CREATE INDEX "IX_program_pi_program_plo_id_program_version_id" ON academic.program_pi (program_plo_id, program_version_id);

CREATE INDEX "IX_program_pi_source_template_pi_id" ON academic.program_pi (source_template_pi_id);

CREATE UNIQUE INDEX uq_program_pi_version_code ON academic.program_pi (program_version_id, code);

CREATE INDEX "IX_program_plo_source_template_plo_id" ON academic.program_plo (source_template_plo_id);

CREATE UNIQUE INDEX uq_program_plo_version_code ON academic.program_plo (program_version_id, code);

CREATE INDEX "IX_program_policy_binding_decision_id" ON measurement.program_policy_binding (decision_id);

CREATE INDEX "IX_program_policy_binding_policy_version_id" ON measurement.program_policy_binding (policy_version_id);

CREATE INDEX "IX_program_policy_binding_program_version_id" ON measurement.program_policy_binding (program_version_id);

CREATE UNIQUE INDEX uq_program_policy_binding_1 ON measurement.program_policy_binding (id, program_version_id);

CREATE UNIQUE INDEX uq_program_policy_binding_2 ON measurement.program_policy_binding (id, program_version_id, policy_version_id);

CREATE UNIQUE INDEX uq_program_policy_binding_workflow ON measurement.program_policy_binding (workflow_instance_id);

CREATE INDEX "IX_program_policy_threshold_clo_id_syllabus_version_id" ON measurement.program_policy_threshold (clo_id, syllabus_version_id);

CREATE INDEX "IX_program_policy_threshold_program_pi_id" ON measurement.program_policy_threshold (program_pi_id);

CREATE INDEX "IX_program_policy_threshold_program_plo_id" ON measurement.program_policy_threshold (program_plo_id);

CREATE INDEX "IX_program_policy_threshold_syllabus_version_id" ON measurement.program_policy_threshold (syllabus_version_id);

CREATE UNIQUE INDEX uq_program_policy_threshold_1 ON measurement.program_policy_threshold (program_policy_binding_id, outcome_level, syllabus_version_id, clo_id, program_pi_id, program_plo_id) NULLS NOT DISTINCT;

CREATE UNIQUE INDEX uq_program_template_field_section_code ON academic.program_template_field (program_template_section_id, field_code);

CREATE UNIQUE INDEX uq_program_template_section_version_code ON academic.program_template_section (institution_template_version_id, section_code);

CREATE INDEX "IX_program_version_decision_id" ON academic.program_version (decision_id);

CREATE INDEX "IX_program_version_institution_template_version_id" ON academic.program_version (institution_template_version_id);

CREATE INDEX "IX_program_version_supersedes_id" ON academic.program_version (supersedes_id);

CREATE UNIQUE INDEX uq_program_version_program_code ON academic.program_version (program_id, code);

CREATE UNIQUE INDEX uq_program_version_program_no ON academic.program_version (program_id, version_no);

CREATE UNIQUE INDEX uq_program_version_workflow ON academic.program_version (workflow_instance_id);

CREATE INDEX ix_program_version_cohort_default ON academic.program_version_cohort (cohort_id, is_default);

CREATE INDEX "IX_program_version_crosswalk_decision_id" ON academic.program_version_crosswalk (decision_id);

CREATE INDEX "IX_program_version_crosswalk_to_program_version_id" ON academic.program_version_crosswalk (to_program_version_id);

CREATE UNIQUE INDEX uq_program_version_crosswalk_pair ON academic.program_version_crosswalk (from_program_version_id, to_program_version_id);

CREATE INDEX ix_prompt_owner_org_unit ON ai.prompt (owner_org_unit_id);

CREATE UNIQUE INDEX uq_prompt_code ON ai.prompt (code);

CREATE INDEX "IX_prompt_version_activation_decision_id_id" ON ai.prompt_version (activation_decision_id, id);

CREATE INDEX "IX_prompt_version_approved_by" ON ai.prompt_version (approved_by);

CREATE INDEX "IX_prompt_version_output_schema_version_id" ON ai.prompt_version (output_schema_version_id);

CREATE UNIQUE INDEX uq_prompt_version_activation_decision ON ai.prompt_version (activation_decision_id) WHERE activation_decision_id IS NOT NULL;

CREATE UNIQUE INDEX uq_prompt_version_prompt_version_no ON ai.prompt_version (prompt_id, version_no);

CREATE INDEX "IX_publication_batch_id_measurement_period_id" ON result.publication (batch_id, measurement_period_id);

CREATE INDEX "IX_publication_document_version_id" ON result.publication (document_version_id);

CREATE INDEX "IX_publication_measurement_period_id" ON result.publication (measurement_period_id);

CREATE INDEX "IX_publication_published_by" ON result.publication (published_by);

CREATE UNIQUE INDEX uq_publication_1 ON result.publication (id, batch_id, measurement_period_id);

CREATE INDEX "IX_publication_audience_access_scope_id" ON result.publication_audience (access_scope_id);

CREATE INDEX "IX_publication_revocation_decision_id" ON result.publication_revocation (decision_id);

CREATE INDEX "IX_publication_revocation_revoked_by" ON result.publication_revocation (revoked_by);

CREATE UNIQUE INDEX uq_publication_revocation_1 ON result.publication_revocation (publication_id);

CREATE INDEX "IX_quarantine_correction_corrected_by" ON integration.quarantine_correction (corrected_by);

CREATE UNIQUE INDEX uq_quarantine_correction_record_revision ON integration.quarantine_correction (quarantine_record_id, revision_no);

CREATE INDEX "IX_quarantine_record_ingestion_batch_id" ON integration.quarantine_record (ingestion_batch_id);

CREATE INDEX "IX_quarantine_record_owner_principal_id" ON integration.quarantine_record (owner_principal_id);

CREATE INDEX "IX_quarantine_record_reprocess_batch_id" ON integration.quarantine_record (reprocess_batch_id);

CREATE INDEX "IX_quarantine_record_resolved_by" ON integration.quarantine_record (resolved_by);

CREATE INDEX ix_quarantine_record_status_owner ON integration.quarantine_record (status, owner_principal_id);

CREATE UNIQUE INDEX uq_quarantine_record_current_correction ON integration.quarantine_record (current_correction_id);

CREATE UNIQUE INDEX uq_quarantine_record_raw_record ON integration.quarantine_record (raw_record_id);

CREATE INDEX "IX_question_criterion_mapping_question_id_syllabus_version_id" ON portfolio.question_criterion_mapping (question_id, syllabus_version_id);

CREATE INDEX "IX_question_criterion_mapping_rubric_criterion_id_syllabus_ver~" ON portfolio.question_criterion_mapping (rubric_criterion_id, syllabus_version_id);

CREATE INDEX ix_raw_record_received_at ON integration.raw_record (received_at);

CREATE UNIQUE INDEX uq_raw_record_batch_row ON integration.raw_record (ingestion_batch_id, row_no);

CREATE INDEX ix_refresh_registry_status_started_at ON reporting.refresh_registry (status, last_started_at);

CREATE INDEX "IX_remeasurement_evaluation_after_batch_id" ON quality.remeasurement_evaluation (after_batch_id);

CREATE INDEX "IX_remeasurement_evaluation_before_batch_id" ON quality.remeasurement_evaluation (before_batch_id);

CREATE INDEX "IX_remeasurement_evaluation_verified_by" ON quality.remeasurement_evaluation (verified_by);

CREATE UNIQUE INDEX uq_remeasurement_evaluation_plan_batches ON quality.remeasurement_evaluation (improvement_plan_id, before_batch_id, after_batch_id);

CREATE INDEX ix_resource_dependency_child ON governance.resource_dependency (child_governed_resource_id);

CREATE INDEX ix_resource_security_scope_authorization ON governance.resource_security_scope (org_unit_id, program_version_id, course_offering_id, student_id);

CREATE INDEX "IX_resource_security_scope_cohort_id" ON governance.resource_security_scope (cohort_id);

CREATE INDEX "IX_resource_security_scope_course_id" ON governance.resource_security_scope (course_id);

CREATE INDEX "IX_resource_security_scope_course_offering_id" ON governance.resource_security_scope (course_offering_id);

CREATE INDEX "IX_resource_security_scope_curriculum_path_id" ON governance.resource_security_scope (curriculum_path_id);

CREATE INDEX "IX_resource_security_scope_measurement_period_id" ON governance.resource_security_scope (measurement_period_id);

CREATE INDEX "IX_resource_security_scope_program_id" ON governance.resource_security_scope (program_id);

CREATE INDEX "IX_resource_security_scope_program_version_id" ON governance.resource_security_scope (program_version_id);

CREATE INDEX "IX_resource_security_scope_student_id" ON governance.resource_security_scope (student_id);

CREATE UNIQUE INDEX uq_resource_security_scope_dimensions ON governance.resource_security_scope (governed_resource_id, org_unit_id, program_id, program_version_id, cohort_id, curriculum_path_id, course_id, course_offering_id, measurement_period_id, student_id, classification);

CREATE INDEX "IX_result_alert_batch_id_academic_year_start_org_unit_id_progr~" ON result.result_alert (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id);

CREATE INDEX ix_result_alert_batch_status_severity ON result.result_alert (batch_id, academic_year_start, status, severity);

CREATE INDEX "IX_result_alert_clo_id" ON result.result_alert (clo_id);

CREATE INDEX "IX_result_alert_measurement_period_id" ON result.result_alert (measurement_period_id);

CREATE INDEX "IX_result_alert_org_unit_id" ON result.result_alert (org_unit_id);

CREATE INDEX "IX_result_alert_program_id" ON result.result_alert (program_id);

CREATE INDEX "IX_result_alert_program_pi_id" ON result.result_alert (program_pi_id);

CREATE INDEX "IX_result_alert_program_plo_id" ON result.result_alert (program_plo_id);

CREATE INDEX "IX_result_alert_program_version_id_program_id" ON result.result_alert (program_version_id, program_id);

CREATE INDEX "IX_result_alert_student_id" ON result.result_alert (student_id);

CREATE INDEX "IX_result_batch_input_snapshot_id_measurement_period_id_policy~" ON result.result_batch (input_snapshot_id, measurement_period_id, policy_version_id, program_policy_binding_id, org_unit_id, program_version_id, academic_year_start);

CREATE INDEX "IX_result_batch_org_unit_id" ON result.result_batch (org_unit_id);

CREATE INDEX "IX_result_batch_policy_version_id" ON result.result_batch (policy_version_id);

CREATE INDEX "IX_result_batch_program_policy_binding_id" ON result.result_batch (program_policy_binding_id);

CREATE INDEX "IX_result_batch_program_version_id" ON result.result_batch (program_version_id);

CREATE INDEX "IX_result_batch_recalculates_batch_id" ON result.result_batch (recalculates_batch_id);

CREATE INDEX "IX_result_batch_sod_policy_version_id" ON result.result_batch (sod_policy_version_id);

CREATE UNIQUE INDEX uq_result_batch_1 ON result.result_batch (governed_resource_id);

CREATE UNIQUE INDEX uq_result_batch_2 ON result.result_batch (measurement_period_id, batch_no);

CREATE UNIQUE INDEX uq_result_batch_3 ON result.result_batch (measurement_period_id, idempotency_key);

CREATE UNIQUE INDEX uq_result_batch_4 ON result.result_batch (id, measurement_period_id);

CREATE UNIQUE INDEX uq_result_batch_5 ON result.result_batch (id, academic_year_start);

CREATE UNIQUE INDEX uq_result_batch_6 ON result.result_batch (id, input_snapshot_id, academic_year_start);

CREATE UNIQUE INDEX uq_result_batch_workflow_instance ON result.result_batch (workflow_instance_id);

CREATE INDEX "IX_result_batch_evidence_evidence_version_id" ON result.result_batch_evidence (evidence_version_id);

CREATE INDEX "IX_result_report_document_document_version_id" ON result.result_report_document (document_version_id);

CREATE INDEX "IX_retention_binding_retention_policy_version_id" ON governance.retention_binding (retention_policy_version_id);

CREATE INDEX ix_retention_binding_status_until ON governance.retention_binding (status, calculated_until);

CREATE UNIQUE INDEX uq_retention_binding_resource_policy_trigger ON governance.retention_binding (governed_resource_id, retention_policy_version_id, trigger_event_at);

CREATE INDEX "IX_retention_policy_version_approved_by" ON governance.retention_policy_version (approved_by);

CREATE INDEX ix_retention_policy_version_resource_status ON governance.retention_policy_version (resource_type, status);

CREATE UNIQUE INDEX uq_retention_policy_version_code_no ON governance.retention_policy_version (code, version_no);

CREATE UNIQUE INDEX uq_role_code ON iam.role (code);

CREATE INDEX "IX_role_assignment_access_scope_id" ON iam.role_assignment (access_scope_id);

CREATE INDEX ix_role_assignment_active_range ON iam.role_assignment (principal_id, role_id, access_scope_id, status, effective_from);

CREATE INDEX "IX_role_assignment_approved_by" ON iam.role_assignment (approved_by);

CREATE INDEX "IX_role_assignment_granted_by" ON iam.role_assignment (granted_by);

CREATE INDEX ix_role_assignment_principal_status_expiry ON iam.role_assignment (principal_id, status, effective_to);

CREATE INDEX "IX_role_assignment_requested_by" ON iam.role_assignment (requested_by);

CREATE INDEX "IX_role_assignment_role_id" ON iam.role_assignment (role_id);

CREATE INDEX "IX_role_assignment_role_version_id_role_id" ON iam.role_assignment (role_version_id, role_id);

CREATE INDEX "IX_role_assignment_sod_policy_version_id" ON iam.role_assignment (sod_policy_version_id);

CREATE UNIQUE INDEX uq_role_assignment_workflow_instance ON iam.role_assignment (workflow_instance_id);

CREATE INDEX "IX_role_version_created_by" ON iam.role_version (created_by);

CREATE INDEX ix_role_version_decision ON iam.role_version (decision_id);

CREATE UNIQUE INDEX uq_role_version_role_version_no ON iam.role_version (role_id, version_no);

CREATE UNIQUE INDEX uq_role_version_workflow_instance ON iam.role_version (workflow_instance_id);

CREATE INDEX "IX_role_version_permission_granted_by" ON iam.role_version_permission (granted_by);

CREATE INDEX ix_role_version_permission_permission ON iam.role_version_permission (permission_id);

CREATE UNIQUE INDEX "IX_rubric_assessment_item_id_syllabus_version_id" ON portfolio.rubric (assessment_item_id, syllabus_version_id);

CREATE INDEX "IX_rubric_rubric_scale_id_syllabus_template_version_id" ON portfolio.rubric (rubric_scale_id, syllabus_template_version_id);

CREATE INDEX "IX_rubric_syllabus_template_version_id" ON portfolio.rubric (syllabus_template_version_id);

CREATE INDEX "IX_rubric_syllabus_version_id_syllabus_template_version_id" ON portfolio.rubric (syllabus_version_id, syllabus_template_version_id);

CREATE UNIQUE INDEX uq_rubric_assessment_item ON portfolio.rubric (assessment_item_id);

CREATE UNIQUE INDEX uq_rubric_version_code ON portfolio.rubric (syllabus_version_id, code);

CREATE INDEX "IX_rubric_criterion_assessment_item_id_syllabus_version_id" ON portfolio.rubric_criterion (assessment_item_id, syllabus_version_id);

CREATE INDEX "IX_rubric_criterion_rubric_id_assessment_item_id_syllabus_vers~" ON portfolio.rubric_criterion (rubric_id, assessment_item_id, syllabus_version_id);

CREATE INDEX "IX_rubric_criterion_syllabus_version_id" ON portfolio.rubric_criterion (syllabus_version_id);

CREATE UNIQUE INDEX uq_rubric_criterion_code ON portfolio.rubric_criterion (rubric_id, criterion_code);

CREATE UNIQUE INDEX uq_rubric_level_code ON portfolio.rubric_level (rubric_criterion_id, level_code);

CREATE UNIQUE INDEX uq_rubric_level_order ON portfolio.rubric_level (rubric_criterion_id, level_order);

CREATE INDEX ix_safety_event_job_severity_time ON ai.safety_event (ai_job_id, severity, occurred_at);


CREATE INDEX "IX_score_dataset_course_offering_id_academic_year_start" ON measurement.score_dataset (course_offering_id, academic_year_start);

CREATE INDEX "IX_score_dataset_source_system_id" ON measurement.score_dataset (source_system_id);

CREATE UNIQUE INDEX uq_score_dataset_1 ON measurement.score_dataset (governed_resource_id);

CREATE UNIQUE INDEX uq_score_dataset_2 ON measurement.score_dataset (id, course_offering_id, academic_year_start);

CREATE INDEX "IX_score_identity_assessment_item_id_syllabus_version_id" ON measurement.score_identity (assessment_item_id, syllabus_version_id);

CREATE INDEX "IX_score_identity_assessment_question_id_assessment_item_id_sy~" ON measurement.score_identity (assessment_question_id, assessment_item_id, syllabus_version_id);

CREATE INDEX "IX_score_identity_course_offering_id_program_version_id_syllab~" ON measurement.score_identity (course_offering_id, program_version_id, syllabus_version_id, academic_year_start);

CREATE INDEX "IX_score_identity_enrollment_id_student_id_course_offering_id_~" ON measurement.score_identity (enrollment_id, student_id, course_offering_id, attempt_no);

CREATE INDEX "IX_score_identity_program_version_id" ON measurement.score_identity (program_version_id);

CREATE INDEX "IX_score_identity_rubric_criterion_id_assessment_item_id_sylla~" ON measurement.score_identity (rubric_criterion_id, assessment_item_id, syllabus_version_id);

CREATE INDEX "IX_score_identity_score_dataset_id_course_offering_id_academic~" ON measurement.score_identity (score_dataset_id, course_offering_id, academic_year_start);

CREATE INDEX "IX_score_identity_student_id" ON measurement.score_identity (student_id);

CREATE INDEX "IX_score_identity_syllabus_version_id" ON measurement.score_identity (syllabus_version_id);

CREATE UNIQUE INDEX uq_score_identity_1 ON measurement.score_identity (academic_year_start, id);

CREATE UNIQUE INDEX uq_score_identity_logical ON measurement.score_identity (score_dataset_id, student_id, assessment_item_id, rubric_criterion_id, assessment_question_id, attempt_no) NULLS NOT DISTINCT;

CREATE INDEX "IX_score_record_academic_year_start_score_identity_id_student_~" ON measurement.score_record (academic_year_start, score_identity_id, student_id, course_offering_id);

CREATE INDEX "IX_score_record_academic_year_start_score_identity_id_supersed~" ON measurement.score_record (academic_year_start, score_identity_id, supersedes_id);

CREATE INDEX "IX_score_record_course_id" ON measurement.score_record (course_id);

CREATE INDEX "IX_score_record_course_offering_id" ON measurement.score_record (course_offering_id);

CREATE INDEX "IX_score_record_ingestion_batch_id" ON measurement.score_record (ingestion_batch_id);

CREATE INDEX "IX_score_record_org_unit_id" ON measurement.score_record (org_unit_id);

CREATE INDEX "IX_score_record_program_id" ON measurement.score_record (program_id);

CREATE INDEX "IX_score_record_program_version_id" ON measurement.score_record (program_version_id);

CREATE INDEX "IX_score_record_recorded_by" ON measurement.score_record (recorded_by);

CREATE INDEX "IX_score_record_source_system_id" ON measurement.score_record (source_system_id);

CREATE INDEX "IX_score_record_student_id" ON measurement.score_record (student_id);

CREATE UNIQUE INDEX uq_score_record_1 ON measurement.score_record (academic_year_start, score_identity_id, id);

CREATE UNIQUE INDEX uq_score_record_2 ON measurement.score_record (academic_year_start, score_identity_id, revision_no);

CREATE UNIQUE INDEX uq_score_record_3 ON measurement.score_record (academic_year_start, source_system_id, source_record_id, source_revision);

CREATE INDEX "IX_score_source_map_academic_year_start_score_record_id" ON measurement.score_source_map (academic_year_start, score_record_id);

CREATE INDEX ix_service_account_owner_org_unit ON iam.service_account (owner_org_unit_id);

CREATE UNIQUE INDEX uq_service_account_client_id ON iam.service_account (client_id);

CREATE INDEX ix_service_credential_certificate_thumbprint ON iam.service_credential (certificate_thumbprint);

CREATE INDEX ix_service_credential_key_prefix ON iam.service_credential (key_prefix);

CREATE INDEX "IX_service_credential_revoked_by" ON iam.service_credential (revoked_by);

CREATE INDEX ix_service_credential_service_effective_from ON iam.service_credential (service_principal_id, effective_from);

CREATE INDEX "IX_shared_course_pi_mapping_decision_id" ON academic.shared_course_pi_mapping (decision_id);

CREATE INDEX "IX_shared_course_pi_mapping_institution_template_version_id" ON academic.shared_course_pi_mapping (institution_template_version_id);

CREATE INDEX "IX_shared_course_pi_mapping_template_pi_id_institution_templat~" ON academic.shared_course_pi_mapping (template_pi_id, institution_template_version_id);

CREATE UNIQUE INDEX uq_shared_course_pi_mapping_version ON academic.shared_course_pi_mapping (course_version_id, template_pi_id, version_no);

CREATE UNIQUE INDEX uq_shared_course_pi_mapping_workflow ON academic.shared_course_pi_mapping (workflow_instance_id);

CREATE INDEX "IX_shared_syllabus_core_owner_org_unit_id" ON portfolio.shared_syllabus_core (owner_org_unit_id);

CREATE UNIQUE INDEX uq_shared_syllabus_core_course_code ON portfolio.shared_syllabus_core (course_id, code);

CREATE INDEX "IX_shared_syllabus_core_version_course_version_id" ON portfolio.shared_syllabus_core_version (course_version_id);

CREATE INDEX "IX_shared_syllabus_core_version_decision_id" ON portfolio.shared_syllabus_core_version (decision_id);

CREATE INDEX "IX_shared_syllabus_core_version_supersedes_id" ON portfolio.shared_syllabus_core_version (supersedes_id);

CREATE UNIQUE INDEX uq_shared_syllabus_core_version_no ON portfolio.shared_syllabus_core_version (shared_syllabus_core_id, version_no);

CREATE UNIQUE INDEX uq_shared_syllabus_core_version_workflow ON portfolio.shared_syllabus_core_version (workflow_instance_id) WHERE workflow_instance_id IS NOT NULL;

CREATE INDEX "IX_snapshot_direct_pi_weight_course_offering_id" ON measurement.snapshot_direct_pi_weight (course_offering_id);

CREATE INDEX "IX_snapshot_direct_pi_weight_program_pi_id" ON measurement.snapshot_direct_pi_weight (program_pi_id);

CREATE INDEX "IX_snapshot_direct_pi_weight_rubric_criterion_id" ON measurement.snapshot_direct_pi_weight (rubric_criterion_id);

CREATE INDEX "IX_snapshot_direct_pi_weight_syllabus_traceability_id" ON measurement.snapshot_direct_pi_weight (syllabus_traceability_id);

CREATE INDEX "IX_snapshot_enrollment_course_offering_id" ON measurement.snapshot_enrollment (course_offering_id);

CREATE INDEX "IX_snapshot_enrollment_enrollment_revision_id" ON measurement.snapshot_enrollment (enrollment_revision_id);

CREATE INDEX "IX_snapshot_enrollment_student_id" ON measurement.snapshot_enrollment (student_id);

CREATE INDEX "IX_snapshot_indirect_observation_indirect_observation_id" ON measurement.snapshot_indirect_observation (indirect_observation_id);

CREATE INDEX "IX_snapshot_indirect_observation_item_id" ON measurement.snapshot_indirect_observation (item_id);

CREATE INDEX "IX_snapshot_indirect_observation_program_pi_id" ON measurement.snapshot_indirect_observation (program_pi_id);

CREATE INDEX "IX_snapshot_indirect_observation_program_plo_id" ON measurement.snapshot_indirect_observation (program_plo_id);

CREATE INDEX "IX_snapshot_offering_course_offering_id" ON measurement.snapshot_offering (course_offering_id);

CREATE INDEX "IX_snapshot_offering_course_version_id" ON measurement.snapshot_offering (course_version_id);

CREATE INDEX "IX_snapshot_offering_curriculum_path_id" ON measurement.snapshot_offering (curriculum_path_id);

CREATE INDEX "IX_snapshot_offering_program_course_id" ON measurement.snapshot_offering (program_course_id);

CREATE INDEX "IX_snapshot_offering_syllabus_version_id_program_course_id_cou~" ON measurement.snapshot_offering (syllabus_version_id, program_course_id, course_version_id);

CREATE INDEX "IX_snapshot_pi_plo_weight_program_pi_id" ON measurement.snapshot_pi_plo_weight (program_pi_id);

CREATE INDEX "IX_snapshot_pi_plo_weight_program_plo_id" ON measurement.snapshot_pi_plo_weight (program_plo_id);

CREATE INDEX "IX_snapshot_pi_plo_weight_source_program_pi_id" ON measurement.snapshot_pi_plo_weight (source_program_pi_id);

CREATE INDEX "IX_snapshot_pi_source_weight_anchor_assessment_id" ON measurement.snapshot_pi_source_weight (anchor_assessment_id);

CREATE INDEX "IX_snapshot_pi_source_weight_course_offering_id" ON measurement.snapshot_pi_source_weight (course_offering_id);

CREATE INDEX "IX_snapshot_pi_source_weight_program_pi_id" ON measurement.snapshot_pi_source_weight (program_pi_id);

CREATE INDEX "IX_snapshot_pi_source_weight_student_path_id" ON measurement.snapshot_pi_source_weight (student_path_id);

CREATE INDEX "IX_snapshot_population_member_cohort_id" ON measurement.snapshot_population_member (cohort_id);

CREATE INDEX "IX_snapshot_population_member_curriculum_path_id" ON measurement.snapshot_population_member (curriculum_path_id);

CREATE INDEX "IX_snapshot_population_member_student_id" ON measurement.snapshot_population_member (student_id);

CREATE INDEX "IX_snapshot_population_member_student_path_id_student_id_curri~" ON measurement.snapshot_population_member (student_path_id, student_id, curriculum_path_id);

CREATE INDEX "IX_snapshot_question_criterion_weight_assessment_question_id" ON measurement.snapshot_question_criterion_weight (assessment_question_id);

CREATE INDEX "IX_snapshot_question_criterion_weight_rubric_criterion_id" ON measurement.snapshot_question_criterion_weight (rubric_criterion_id);

CREATE INDEX "IX_snapshot_score_academic_year_start_score_record_id_student_~" ON measurement.snapshot_score (academic_year_start, score_record_id, student_id, course_offering_id);

CREATE INDEX "IX_snapshot_score_course_offering_id" ON measurement.snapshot_score (course_offering_id);

CREATE INDEX "IX_snapshot_score_student_id" ON measurement.snapshot_score (student_id);

CREATE INDEX "IX_snapshot_threshold_clo_id" ON measurement.snapshot_threshold (clo_id);

CREATE INDEX "IX_snapshot_threshold_program_pi_id" ON measurement.snapshot_threshold (program_pi_id);

CREATE INDEX "IX_snapshot_threshold_program_plo_id" ON measurement.snapshot_threshold (program_plo_id);

CREATE INDEX "IX_sod_exception_access_scope_id" ON iam.sod_exception (access_scope_id);

CREATE INDEX "IX_sod_exception_approved_by" ON iam.sod_exception (approved_by);

CREATE INDEX "IX_sod_exception_decision_id" ON iam.sod_exception (decision_id);

CREATE INDEX ix_sod_exception_principal_scope_effective ON iam.sod_exception (principal_id, access_scope_id, effective_from, effective_to);

CREATE INDEX "IX_sod_exception_rule_id" ON iam.sod_exception (rule_id);

CREATE UNIQUE INDEX uq_sod_policy_version_version_no ON iam.sod_policy_version (version_no);

CREATE UNIQUE INDEX uq_sod_policy_version_workflow_instance ON iam.sod_policy_version (workflow_instance_id);

CREATE INDEX "IX_sod_rule_permission_a_id" ON iam.sod_rule (permission_a_id);

CREATE INDEX "IX_sod_rule_permission_b_id" ON iam.sod_rule (permission_b_id);

CREATE UNIQUE INDEX uq_sod_rule_semantic ON iam.sod_rule (policy_version_id, resource_type, permission_a_id, permission_b_id, conflict_mode);

CREATE INDEX ix_source_record_map_entity_target ON integration.source_record_map (entity_type, target_id);

CREATE INDEX "IX_source_system_owner_org_unit_id" ON integration.source_system (owner_org_unit_id);

CREATE INDEX "IX_source_system_service_principal_id" ON integration.source_system (service_principal_id);

CREATE UNIQUE INDEX uq_source_system_code ON integration.source_system (code);

CREATE INDEX "IX_staff_home_org_unit_id" ON academic.staff (home_org_unit_id);

CREATE UNIQUE INDEX uq_staff_code ON academic.staff (staff_code);

CREATE INDEX "IX_staging_course_offering_resolved_course_offering_id" ON integration.staging_course_offering (resolved_course_offering_id);

CREATE UNIQUE INDEX uq_staging_course_offering_batch_row ON integration.staging_course_offering (ingestion_batch_id, row_no);

CREATE UNIQUE INDEX uq_staging_course_offering_raw_record ON integration.staging_course_offering (raw_record_id);

CREATE INDEX "IX_staging_course_pi_mapping_resolved_course_pi_mapping_id" ON integration.staging_course_pi_mapping (resolved_course_pi_mapping_id);

CREATE UNIQUE INDEX uq_staging_course_pi_mapping_batch_row ON integration.staging_course_pi_mapping (ingestion_batch_id, row_no);

CREATE UNIQUE INDEX uq_staging_course_pi_mapping_raw_record ON integration.staging_course_pi_mapping (raw_record_id);

CREATE INDEX "IX_staging_direct_measurement_plan_resolved_direct_measurement~" ON integration.staging_direct_measurement_plan (resolved_direct_measurement_plan_id);

CREATE UNIQUE INDEX uq_staging_direct_measurement_plan_batch_row ON integration.staging_direct_measurement_plan (ingestion_batch_id, row_no);

CREATE UNIQUE INDEX uq_staging_direct_measurement_plan_raw_record ON integration.staging_direct_measurement_plan (raw_record_id);

CREATE INDEX "IX_staging_enrollment_resolved_enrollment_id" ON integration.staging_enrollment (resolved_enrollment_id);

CREATE UNIQUE INDEX uq_staging_enrollment_batch_row ON integration.staging_enrollment (ingestion_batch_id, row_no);

CREATE UNIQUE INDEX uq_staging_enrollment_raw_record ON integration.staging_enrollment (raw_record_id);

CREATE INDEX "IX_staging_rubric_criterion_resolved_rubric_criterion_id" ON integration.staging_rubric_criterion (resolved_rubric_criterion_id);

CREATE UNIQUE INDEX uq_staging_rubric_criterion_batch_row ON integration.staging_rubric_criterion (ingestion_batch_id, row_no);

CREATE UNIQUE INDEX uq_staging_rubric_criterion_raw_record ON integration.staging_rubric_criterion (raw_record_id);

CREATE INDEX "IX_staging_score_resolved_score_academic_year_start_resolved_s~" ON integration.staging_score (resolved_score_academic_year_start, resolved_score_record_id);

CREATE UNIQUE INDEX uq_staging_score_batch_row ON integration.staging_score (ingestion_batch_id, row_no);

CREATE UNIQUE INDEX uq_staging_score_raw_record ON integration.staging_score (raw_record_id);

CREATE INDEX "IX_staging_student_resolved_student_id" ON integration.staging_student (resolved_student_id);

CREATE UNIQUE INDEX uq_staging_student_batch_row ON integration.staging_student (ingestion_batch_id, row_no);

CREATE UNIQUE INDEX uq_staging_student_raw_record ON integration.staging_student (raw_record_id);

CREATE INDEX "IX_student_admission_cohort_id" ON academic.student (admission_cohort_id);

CREATE UNIQUE INDEX uq_student_code ON academic.student (student_code);

CREATE INDEX "IX_student_clo_result_batch_id_academic_year_start_org_unit_id~" ON result.student_clo_result (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id);

CREATE INDEX "IX_student_clo_result_clo_id" ON result.student_clo_result (clo_id);

CREATE INDEX "IX_student_clo_result_cohort_id" ON result.student_clo_result (cohort_id);

CREATE INDEX "IX_student_clo_result_course_id" ON result.student_clo_result (course_id);

CREATE INDEX "IX_student_clo_result_course_offering_id" ON result.student_clo_result (course_offering_id);

CREATE INDEX "IX_student_clo_result_curriculum_path_id" ON result.student_clo_result (curriculum_path_id);

CREATE INDEX "IX_student_clo_result_measurement_period_id" ON result.student_clo_result (measurement_period_id);

CREATE INDEX "IX_student_clo_result_org_unit_id" ON result.student_clo_result (org_unit_id);

CREATE INDEX "IX_student_clo_result_program_id" ON result.student_clo_result (program_id);

CREATE INDEX "IX_student_clo_result_program_version_id_program_id" ON result.student_clo_result (program_version_id, program_id);

CREATE INDEX "IX_student_clo_result_student_id" ON result.student_clo_result (student_id);

CREATE UNIQUE INDEX uq_student_clo_result_1 ON result.student_clo_result (academic_year_start, batch_id, student_id, course_offering_id, clo_id);

CREATE INDEX "IX_student_criterion_result_assessment_item_id" ON result.student_criterion_result (assessment_item_id);

CREATE INDEX "IX_student_criterion_result_batch_id_academic_year_start_org_u~" ON result.student_criterion_result (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id);

CREATE INDEX "IX_student_criterion_result_cohort_id" ON result.student_criterion_result (cohort_id);

CREATE INDEX "IX_student_criterion_result_course_id" ON result.student_criterion_result (course_id);

CREATE INDEX "IX_student_criterion_result_course_offering_id" ON result.student_criterion_result (course_offering_id);

CREATE INDEX "IX_student_criterion_result_curriculum_path_id" ON result.student_criterion_result (curriculum_path_id);

CREATE INDEX "IX_student_criterion_result_measurement_period_id" ON result.student_criterion_result (measurement_period_id);

CREATE INDEX "IX_student_criterion_result_org_unit_id" ON result.student_criterion_result (org_unit_id);

CREATE INDEX "IX_student_criterion_result_program_id" ON result.student_criterion_result (program_id);

CREATE INDEX "IX_student_criterion_result_program_version_id_program_id" ON result.student_criterion_result (program_version_id, program_id);

CREATE INDEX "IX_student_criterion_result_rubric_criterion_id" ON result.student_criterion_result (rubric_criterion_id);

CREATE INDEX "IX_student_criterion_result_student_id" ON result.student_criterion_result (student_id);

CREATE INDEX "IX_student_criterion_result_student_path_id" ON result.student_criterion_result (student_path_id);

CREATE UNIQUE INDEX uq_student_criterion_result_1 ON result.student_criterion_result (academic_year_start, batch_id, student_id, course_offering_id, rubric_criterion_id);

CREATE INDEX "IX_student_criterion_score_lineage_academic_year_start_student~" ON result.student_criterion_score_lineage (academic_year_start, student_criterion_result_id, batch_id, student_id, course_offering_id, rubric_criterion_id);

CREATE INDEX "IX_student_criterion_score_lineage_batch_id_input_snapshot_id_~" ON result.student_criterion_score_lineage (batch_id, input_snapshot_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id);

CREATE INDEX "IX_student_criterion_score_lineage_cohort_id" ON result.student_criterion_score_lineage (cohort_id);

CREATE INDEX "IX_student_criterion_score_lineage_course_id" ON result.student_criterion_score_lineage (course_id);

CREATE INDEX "IX_student_criterion_score_lineage_curriculum_path_id" ON result.student_criterion_score_lineage (curriculum_path_id);

CREATE INDEX "IX_student_criterion_score_lineage_input_snapshot_id_academic_~" ON result.student_criterion_score_lineage (input_snapshot_id, academic_year_start, score_record_id, student_id, course_offering_id);

CREATE INDEX "IX_student_criterion_score_lineage_input_snapshot_id_assessmen~" ON result.student_criterion_score_lineage (input_snapshot_id, assessment_question_id, rubric_criterion_id);

CREATE INDEX "IX_student_criterion_score_lineage_measurement_period_id" ON result.student_criterion_score_lineage (measurement_period_id);

CREATE INDEX "IX_student_criterion_score_lineage_org_unit_id" ON result.student_criterion_score_lineage (org_unit_id);

CREATE INDEX "IX_student_criterion_score_lineage_program_id" ON result.student_criterion_score_lineage (program_id);

CREATE INDEX "IX_student_criterion_score_lineage_program_version_id_program_~" ON result.student_criterion_score_lineage (program_version_id, program_id);

CREATE INDEX "IX_student_path_curriculum_path_id_program_version_id" ON academic.student_path (curriculum_path_id, program_version_id);

CREATE INDEX "IX_student_path_decision_id" ON academic.student_path (decision_id);

CREATE INDEX ix_student_path_primary_period ON academic.student_path (student_id, program_id, is_primary, effective_from, effective_to);

CREATE INDEX "IX_student_path_program_id" ON academic.student_path (program_id);

CREATE INDEX "IX_student_path_program_version_id_program_id" ON academic.student_path (program_version_id, program_id);

CREATE INDEX "IX_student_pi_result_batch_id_academic_year_start_org_unit_id_~" ON result.student_pi_result (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id);

CREATE INDEX "IX_student_pi_result_cohort_id" ON result.student_pi_result (cohort_id);

CREATE INDEX "IX_student_pi_result_curriculum_path_id" ON result.student_pi_result (curriculum_path_id);

CREATE INDEX "IX_student_pi_result_measurement_period_id" ON result.student_pi_result (measurement_period_id);

CREATE INDEX "IX_student_pi_result_org_unit_id" ON result.student_pi_result (org_unit_id);

CREATE INDEX "IX_student_pi_result_program_id" ON result.student_pi_result (program_id);

CREATE INDEX "IX_student_pi_result_program_pi_id" ON result.student_pi_result (program_pi_id);

CREATE INDEX "IX_student_pi_result_program_version_id_program_id" ON result.student_pi_result (program_version_id, program_id);

CREATE INDEX "IX_student_pi_result_student_id" ON result.student_pi_result (student_id);

CREATE INDEX "IX_student_pi_result_student_path_id" ON result.student_pi_result (student_path_id);

CREATE UNIQUE INDEX uq_student_pi_result_1 ON result.student_pi_result (academic_year_start, batch_id, student_id, student_path_id, program_pi_id, method);

CREATE INDEX "IX_student_pi_source_contribution_academic_year_start_course_p~" ON result.student_pi_source_contribution (academic_year_start, course_pi_result_id, batch_id, student_id, student_path_id, program_pi_id, course_offering_id);

CREATE INDEX "IX_student_pi_source_contribution_academic_year_start_student_~" ON result.student_pi_source_contribution (academic_year_start, student_pi_result_id, batch_id, student_id, student_path_id, program_pi_id, method);

CREATE INDEX "IX_student_pi_source_contribution_anchor_assessment_id" ON result.student_pi_source_contribution (anchor_assessment_id);

CREATE INDEX "IX_student_pi_source_contribution_batch_id_input_snapshot_id_a~" ON result.student_pi_source_contribution (batch_id, input_snapshot_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id);

CREATE INDEX "IX_student_pi_source_contribution_cohort_id" ON result.student_pi_source_contribution (cohort_id);

CREATE INDEX "IX_student_pi_source_contribution_course_id" ON result.student_pi_source_contribution (course_id);

CREATE INDEX "IX_student_pi_source_contribution_course_offering_id" ON result.student_pi_source_contribution (course_offering_id);

CREATE INDEX "IX_student_pi_source_contribution_curriculum_path_id" ON result.student_pi_source_contribution (curriculum_path_id);

CREATE INDEX "IX_student_pi_source_contribution_input_snapshot_id_student_pa~" ON result.student_pi_source_contribution (input_snapshot_id, student_path_id, program_pi_id, course_offering_id);

CREATE INDEX "IX_student_pi_source_contribution_measurement_period_id" ON result.student_pi_source_contribution (measurement_period_id);

CREATE INDEX "IX_student_pi_source_contribution_org_unit_id" ON result.student_pi_source_contribution (org_unit_id);

CREATE INDEX "IX_student_pi_source_contribution_program_id" ON result.student_pi_source_contribution (program_id);

CREATE INDEX "IX_student_pi_source_contribution_program_pi_id" ON result.student_pi_source_contribution (program_pi_id);

CREATE INDEX "IX_student_pi_source_contribution_program_version_id_program_id" ON result.student_pi_source_contribution (program_version_id, program_id);

CREATE INDEX "IX_student_pi_source_contribution_student_id" ON result.student_pi_source_contribution (student_id);

CREATE INDEX "IX_student_pi_source_contribution_student_path_id" ON result.student_pi_source_contribution (student_path_id);

CREATE INDEX "IX_student_plo_pi_contribution_academic_year_start_student_pi_~" ON result.student_plo_pi_contribution (academic_year_start, student_pi_result_id, batch_id, student_id, student_path_id, program_pi_id, method);

CREATE INDEX "IX_student_plo_pi_contribution_academic_year_start_student_plo~" ON result.student_plo_pi_contribution (academic_year_start, student_plo_result_id, batch_id, student_id, student_path_id, program_plo_id, method);

CREATE INDEX "IX_student_plo_pi_contribution_batch_id_input_snapshot_id_acad~" ON result.student_plo_pi_contribution (batch_id, input_snapshot_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id);

CREATE INDEX "IX_student_plo_pi_contribution_cohort_id" ON result.student_plo_pi_contribution (cohort_id);

CREATE INDEX "IX_student_plo_pi_contribution_curriculum_path_id" ON result.student_plo_pi_contribution (curriculum_path_id);

CREATE INDEX "IX_student_plo_pi_contribution_input_snapshot_id_program_pi_id~" ON result.student_plo_pi_contribution (input_snapshot_id, program_pi_id, program_plo_id);

CREATE INDEX "IX_student_plo_pi_contribution_measurement_period_id" ON result.student_plo_pi_contribution (measurement_period_id);

CREATE INDEX "IX_student_plo_pi_contribution_org_unit_id" ON result.student_plo_pi_contribution (org_unit_id);

CREATE INDEX "IX_student_plo_pi_contribution_program_id" ON result.student_plo_pi_contribution (program_id);

CREATE INDEX "IX_student_plo_pi_contribution_program_pi_id" ON result.student_plo_pi_contribution (program_pi_id);

CREATE INDEX "IX_student_plo_pi_contribution_program_plo_id" ON result.student_plo_pi_contribution (program_plo_id);

CREATE INDEX "IX_student_plo_pi_contribution_program_version_id_program_id" ON result.student_plo_pi_contribution (program_version_id, program_id);

CREATE INDEX "IX_student_plo_pi_contribution_student_id" ON result.student_plo_pi_contribution (student_id);

CREATE INDEX "IX_student_plo_pi_contribution_student_path_id" ON result.student_plo_pi_contribution (student_path_id);

CREATE INDEX "IX_student_plo_result_batch_id_academic_year_start_org_unit_id~" ON result.student_plo_result (batch_id, academic_year_start, org_unit_id, program_version_id, measurement_period_id);

CREATE INDEX "IX_student_plo_result_cohort_id" ON result.student_plo_result (cohort_id);

CREATE INDEX "IX_student_plo_result_curriculum_path_id" ON result.student_plo_result (curriculum_path_id);

CREATE INDEX "IX_student_plo_result_measurement_period_id" ON result.student_plo_result (measurement_period_id);

CREATE INDEX "IX_student_plo_result_org_unit_id" ON result.student_plo_result (org_unit_id);

CREATE INDEX "IX_student_plo_result_program_id" ON result.student_plo_result (program_id);

CREATE INDEX "IX_student_plo_result_program_plo_id" ON result.student_plo_result (program_plo_id);

CREATE INDEX "IX_student_plo_result_program_version_id_program_id" ON result.student_plo_result (program_version_id, program_id);

CREATE INDEX "IX_student_plo_result_student_id" ON result.student_plo_result (student_id);

CREATE INDEX "IX_student_plo_result_student_path_id" ON result.student_plo_result (student_path_id);

CREATE UNIQUE INDEX uq_student_plo_result_1 ON result.student_plo_result (academic_year_start, batch_id, student_id, student_path_id, program_plo_id, method);

CREATE INDEX "IX_syllabus_owner_org_unit_id" ON portfolio.syllabus (owner_org_unit_id);

CREATE UNIQUE INDEX uq_syllabus_program_course ON portfolio.syllabus (program_course_id);

CREATE INDEX "IX_syllabus_document_document_version_id" ON portfolio.syllabus_document (document_version_id);

CREATE INDEX "IX_syllabus_evidence_evidence_version_id" ON portfolio.syllabus_evidence (evidence_version_id);

CREATE INDEX "IX_syllabus_section_content_last_edited_by" ON portfolio.syllabus_section_content (last_edited_by);

CREATE INDEX "IX_syllabus_section_content_syllabus_template_version_id" ON portfolio.syllabus_section_content (syllabus_template_version_id);

CREATE INDEX "IX_syllabus_section_content_syllabus_version_id_syllabus_templ~" ON portfolio.syllabus_section_content (syllabus_version_id, syllabus_template_version_id);

CREATE INDEX "IX_syllabus_section_content_template_field_id_syllabus_templat~" ON portfolio.syllabus_section_content (template_field_id, syllabus_template_version_id);

CREATE UNIQUE INDEX uq_syllabus_section_content_version_field ON portfolio.syllabus_section_content (syllabus_version_id, template_field_id);

CREATE INDEX "IX_syllabus_template_owner_org_unit_id" ON portfolio.syllabus_template (owner_org_unit_id);

CREATE UNIQUE INDEX uq_syllabus_template_code ON portfolio.syllabus_template (code);

CREATE INDEX "IX_syllabus_template_field_syllabus_template_section_id_syllab~" ON portfolio.syllabus_template_field (syllabus_template_section_id, syllabus_template_version_id);

CREATE INDEX "IX_syllabus_template_field_syllabus_template_version_id" ON portfolio.syllabus_template_field (syllabus_template_version_id);

CREATE UNIQUE INDEX uq_syllabus_template_field_section_code ON portfolio.syllabus_template_field (syllabus_template_section_id, field_code);

CREATE UNIQUE INDEX uq_syllabus_template_rubric_scale_code ON portfolio.syllabus_template_rubric_scale (syllabus_template_version_id, code);

CREATE UNIQUE INDEX uq_syllabus_template_rubric_scale_level_code ON portfolio.syllabus_template_rubric_scale_level (rubric_scale_id, level_code);

CREATE UNIQUE INDEX uq_syllabus_template_rubric_scale_level_order ON portfolio.syllabus_template_rubric_scale_level (rubric_scale_id, level_order);

CREATE UNIQUE INDEX uq_syllabus_template_section_code ON portfolio.syllabus_template_section (syllabus_template_version_id, section_code);

CREATE INDEX "IX_syllabus_template_version_decision_id" ON portfolio.syllabus_template_version (decision_id);

CREATE INDEX "IX_syllabus_template_version_institution_template_version_id" ON portfolio.syllabus_template_version (institution_template_version_id);

CREATE INDEX "IX_syllabus_template_version_supersedes_id" ON portfolio.syllabus_template_version (supersedes_id);

CREATE UNIQUE INDEX uq_syllabus_template_version_template_version_no ON portfolio.syllabus_template_version (syllabus_template_id, version_no);

CREATE UNIQUE INDEX uq_syllabus_template_version_workflow ON portfolio.syllabus_template_version (workflow_instance_id) WHERE workflow_instance_id IS NOT NULL;

CREATE INDEX "IX_syllabus_traceability_clo_id_syllabus_version_id" ON portfolio.syllabus_traceability (clo_id, syllabus_version_id);

CREATE INDEX "IX_syllabus_traceability_course_pi_mapping_id_program_course_i~" ON portfolio.syllabus_traceability (course_pi_mapping_id, program_course_id, program_version_id);

CREATE INDEX "IX_syllabus_traceability_exception_decision_id" ON portfolio.syllabus_traceability (exception_decision_id);

CREATE INDEX "IX_syllabus_traceability_program_course_id_program_version_id" ON portfolio.syllabus_traceability (program_course_id, program_version_id);

CREATE INDEX "IX_syllabus_traceability_program_version_id" ON portfolio.syllabus_traceability (program_version_id);

CREATE INDEX "IX_syllabus_traceability_rubric_criterion_id_syllabus_version_~" ON portfolio.syllabus_traceability (rubric_criterion_id, syllabus_version_id);

CREATE INDEX "IX_syllabus_traceability_syllabus_version_id_program_course_id~" ON portfolio.syllabus_traceability (syllabus_version_id, program_course_id, program_version_id);

CREATE INDEX ix_syllabus_traceability_version_clo ON portfolio.syllabus_traceability (syllabus_version_id, clo_id);

CREATE UNIQUE INDEX uq_syllabus_traceability_criterion_pi ON portfolio.syllabus_traceability (syllabus_version_id, rubric_criterion_id, course_pi_mapping_id) WHERE course_pi_mapping_id IS NOT NULL;

CREATE INDEX "IX_syllabus_version_course_version_id" ON portfolio.syllabus_version (course_version_id);

CREATE INDEX "IX_syllabus_version_institution_template_version_id" ON portfolio.syllabus_version (institution_template_version_id);

CREATE INDEX "IX_syllabus_version_program_course_id_program_version_id_cours~" ON portfolio.syllabus_version (program_course_id, program_version_id, course_version_id);

CREATE INDEX "IX_syllabus_version_program_version_id_institution_template_ve~" ON portfolio.syllabus_version (program_version_id, institution_template_version_id);

CREATE INDEX "IX_syllabus_version_shared_syllabus_core_version_id_course_ver~" ON portfolio.syllabus_version (shared_syllabus_core_version_id, course_version_id);

CREATE INDEX "IX_syllabus_version_supersedes_id" ON portfolio.syllabus_version (supersedes_id);

CREATE INDEX "IX_syllabus_version_syllabus_id_program_course_id" ON portfolio.syllabus_version (syllabus_id, program_course_id);

CREATE INDEX "IX_syllabus_version_syllabus_template_version_id_institution_t~" ON portfolio.syllabus_version (syllabus_template_version_id, institution_template_version_id);

CREATE UNIQUE INDEX uq_syllabus_version_program_course_no ON portfolio.syllabus_version (program_version_id, program_course_id, version_no);

CREATE UNIQUE INDEX uq_syllabus_version_syllabus_no ON portfolio.syllabus_version (syllabus_id, version_no);

CREATE UNIQUE INDEX uq_syllabus_version_workflow ON portfolio.syllabus_version (workflow_instance_id) WHERE workflow_instance_id IS NOT NULL;

CREATE INDEX "IX_sync_cursor_last_successful_job_id" ON integration.sync_cursor (last_successful_job_id);

CREATE INDEX ix_sync_job_request_id ON integration.sync_job (request_id);

CREATE INDEX ix_sync_job_source_data_started ON integration.sync_job (source_system_id, data_type, started_at);

CREATE INDEX ix_task_assignee_principal_status ON workflow.task (assignee_principal_id, status);

CREATE INDEX ix_task_assignee_role_status ON workflow.task (assignee_role_id, status);

CREATE INDEX ix_task_instance_status ON workflow.task (instance_id, status);

CREATE UNIQUE INDEX uq_teaching_session_version_no ON portfolio.teaching_session (syllabus_version_id, session_no);

CREATE INDEX "IX_teaching_session_assessment_assessment_item_id_syllabus_ver~" ON portfolio.teaching_session_assessment (assessment_item_id, syllabus_version_id);

CREATE INDEX "IX_teaching_session_assessment_teaching_session_id_syllabus_ve~" ON portfolio.teaching_session_assessment (teaching_session_id, syllabus_version_id);

CREATE INDEX "IX_teaching_session_clo_clo_id_syllabus_version_id" ON portfolio.teaching_session_clo (clo_id, syllabus_version_id);

CREATE INDEX "IX_teaching_session_clo_teaching_session_id_syllabus_version_id" ON portfolio.teaching_session_clo (teaching_session_id, syllabus_version_id);

CREATE INDEX "IX_teaching_session_llo_llo_id_syllabus_version_id" ON portfolio.teaching_session_llo (llo_id, syllabus_version_id);

CREATE INDEX "IX_teaching_session_llo_teaching_session_id_syllabus_version_id" ON portfolio.teaching_session_llo (teaching_session_id, syllabus_version_id);

CREATE INDEX "IX_teaching_session_material_learning_material_id_syllabus_ver~" ON portfolio.teaching_session_material (learning_material_id, syllabus_version_id);

CREATE INDEX "IX_teaching_session_material_teaching_session_id_syllabus_vers~" ON portfolio.teaching_session_material (teaching_session_id, syllabus_version_id);

CREATE INDEX "IX_template_pi_template_plo_id_institution_template_version_id" ON academic.template_pi (template_plo_id, institution_template_version_id);

CREATE UNIQUE INDEX uq_template_pi_version_code ON academic.template_pi (institution_template_version_id, code);

CREATE UNIQUE INDEX uq_template_plo_version_code ON academic.template_plo (institution_template_version_id, code);

CREATE INDEX "IX_tool_policy_version_activation_decision_id_id" ON ai.tool_policy_version (activation_decision_id, id);

CREATE INDEX "IX_tool_policy_version_approved_by" ON ai.tool_policy_version (approved_by);

CREATE UNIQUE INDEX uq_tool_policy_version_activation_decision ON ai.tool_policy_version (activation_decision_id) WHERE activation_decision_id IS NOT NULL;

CREATE UNIQUE INDEX uq_tool_policy_version_code_version_no ON ai.tool_policy_version (code, version_no);

CREATE INDEX "IX_traceability_evidence_evidence_version_id" ON portfolio.traceability_evidence (evidence_version_id);

CREATE INDEX "IX_transition_actor_principal_id" ON workflow.transition (actor_principal_id);

CREATE INDEX ix_transition_instance_occurred_at ON workflow.transition (instance_id, occurred_at);

CREATE INDEX ix_transition_request_id ON workflow.transition (request_id);

CREATE UNIQUE INDEX uq_user_account_email_lookup_hash ON iam.user_account (email_lookup_hash);

CREATE UNIQUE INDEX uq_user_account_person_id ON iam.user_account (person_id);

CREATE UNIQUE INDEX uq_user_account_username ON iam.user_account (username);

CREATE INDEX ix_validation_issue_run_severity_rule ON academic.validation_issue (validation_run_id, severity, rule_code);

CREATE INDEX ix_validation_issue_batch_severity_status ON integration.validation_issue (ingestion_batch_id, severity, status);

CREATE INDEX "IX_validation_issue_raw_record_id" ON integration.validation_issue (raw_record_id);

CREATE INDEX "IX_validation_issue_resolved_by" ON integration.validation_issue (resolved_by);

CREATE INDEX ix_validation_issue_staging_locator ON integration.validation_issue (staging_table, staging_row_id);

CREATE INDEX ix_validation_run_aggregate_time ON academic.validation_run (aggregate_type, aggregate_id, run_at);

CREATE INDEX "IX_validation_run_requested_by" ON academic.validation_run (requested_by);

CREATE UNIQUE INDEX uq_webhook_attempt_delivery_nonce ON integration.webhook_attempt (delivery_id, nonce);

CREATE INDEX "IX_webhook_delivery_outbox_message_id" ON integration.webhook_delivery (outbox_message_id);

CREATE INDEX ix_webhook_delivery_retry ON integration.webhook_delivery (status, next_retry_at);

CREATE UNIQUE INDEX uq_webhook_delivery_subscription_outbox ON integration.webhook_delivery (subscription_id, outbox_message_id);

CREATE INDEX "IX_webhook_subscription_access_scope_id" ON integration.webhook_subscription (access_scope_id);

CREATE INDEX ix_webhook_subscription_principal_status ON integration.webhook_subscription (principal_id, status);

ALTER TABLE ai.activation_decision ADD CONSTRAINT fk_activation_decision_data_handling_policy_version FOREIGN KEY (data_handling_policy_version_id) REFERENCES ai.data_handling_policy_version (id) ON DELETE RESTRICT;

ALTER TABLE ai.activation_decision ADD CONSTRAINT fk_activation_decision_exact_evaluation_bundle FOREIGN KEY (evaluation_run_id, model_deployment_version_id, prompt_version_id, output_schema_version_id, data_handling_policy_version_id, tool_policy_version_id) REFERENCES ai.evaluation_run (id, model_deployment_version_id, prompt_version_id, output_schema_version_id, data_handling_policy_version_id, tool_policy_version_id) ON DELETE RESTRICT;

ALTER TABLE ai.activation_decision ADD CONSTRAINT fk_activation_decision_model_deployment_version FOREIGN KEY (model_deployment_version_id) REFERENCES ai.model_deployment_version (id) ON DELETE RESTRICT;

ALTER TABLE ai.activation_decision ADD CONSTRAINT fk_activation_decision_output_schema_version FOREIGN KEY (output_schema_version_id) REFERENCES ai.output_schema_version (id) ON DELETE RESTRICT;

ALTER TABLE ai.activation_decision ADD CONSTRAINT fk_activation_decision_prompt_output_schema_bundle FOREIGN KEY (prompt_version_id, output_schema_version_id) REFERENCES ai.prompt_version (id, output_schema_version_id) ON DELETE RESTRICT;

ALTER TABLE ai.activation_decision ADD CONSTRAINT fk_activation_decision_tool_policy_version FOREIGN KEY (tool_policy_version_id) REFERENCES ai.tool_policy_version (id) ON DELETE RESTRICT;

ALTER TABLE integration.quarantine_correction ADD CONSTRAINT fk_quarantine_correction_record FOREIGN KEY (quarantine_record_id) REFERENCES integration.quarantine_record (id) ON DELETE RESTRICT;


DROP INDEX integration.ix_outbox_message_claim;

DROP INDEX ops.ix_operation_job_claim;

DROP INDEX audit.ix_audit_event_actor_occurred_at;

DROP INDEX audit.ix_audit_event_chain_sequence;

DROP INDEX audit.ix_audit_event_event_hash;

DROP INDEX audit.ix_audit_event_occurred_at;

DROP INDEX audit.ix_audit_event_program_version_occurred_at;

DROP INDEX audit.ix_audit_event_resource_occurred_at;

CREATE EXTENSION IF NOT EXISTS btree_gist;
CREATE EXTENSION IF NOT EXISTS citext;

ALTER TABLE iam.database_principal_binding
    ADD CONSTRAINT ex_database_principal_binding_active_range
    EXCLUDE USING gist (
        database_role_name WITH =,
        daterange(effective_from, COALESCE(effective_to, 'infinity'::date), '[)') WITH &&
    ) WHERE (status = 'ACTIVE');

ALTER TABLE iam.role_assignment
    ADD CONSTRAINT ex_role_assignment_active_range
    EXCLUDE USING gist (
        principal_id WITH =,
        role_id WITH =,
        access_scope_id WITH =,
        tstzrange(effective_from, effective_to, '[)') WITH &&
    ) WHERE (status = 'ACTIVE');

ALTER TABLE academic.program_version_cohort
    ADD CONSTRAINT ex_program_version_cohort_default_range
    EXCLUDE USING gist (
        cohort_id WITH =,
        daterange(effective_from, COALESCE(effective_to, 'infinity'::date), '[)') WITH &&
    ) WHERE (is_default);

ALTER TABLE academic.direct_measurement_plan
    ADD CONSTRAINT ex_direct_measurement_plan_active_range
    EXCLUDE USING gist (
        program_version_id WITH =,
        curriculum_path_id WITH =,
        program_pi_id WITH =,
        daterange(effective_from, COALESCE(effective_to, 'infinity'::date), '[)') WITH &&
    ) WHERE (status = 'ACTIVE');

ALTER TABLE academic.student_path
    ADD CONSTRAINT ex_student_path_primary_overlap
    EXCLUDE USING gist (
        student_id WITH =,
        program_id WITH =,
        daterange(effective_from, COALESCE(effective_to, 'infinity'::date), '[)') WITH &&
    ) WHERE (is_primary AND path_status = 'ACTIVE');

ALTER TABLE portfolio.syllabus_template_rubric_scale_level
    ADD CONSTRAINT ex_syllabus_template_rubric_scale_level_range
    EXCLUDE USING gist (
        rubric_scale_id WITH =,
        numrange(score_from, score_to, '[)') WITH &&
    );

ALTER TABLE portfolio.rubric_level
    ADD CONSTRAINT ex_rubric_level_range
    EXCLUDE USING gist (
        rubric_criterion_id WITH =,
        score_range WITH &&
    );

ALTER TABLE measurement.enrollment_revision
    ADD CONSTRAINT ex_enrollment_revision_effective_range
    EXCLUDE USING gist (
        enrollment_id WITH =,
        tstzrange(effective_from, effective_to, '[)') WITH &&
    );

ALTER TABLE measurement.program_policy_binding
    ADD CONSTRAINT ex_program_policy_binding_active_range
    EXCLUDE USING gist (
        program_version_id WITH =,
        daterange(effective_from, COALESCE(effective_to, 'infinity'::date), '[)') WITH &&
    ) WHERE (status = 'ACTIVE');

CREATE FUNCTION audit.reject_mutation()
RETURNS trigger
LANGUAGE plpgsql
SET search_path = pg_catalog, audit
AS $function$
BEGIN
    RAISE EXCEPTION 'audit events are immutable' USING ERRCODE = '55000';
END;
$function$;

CREATE TRIGGER trg_audit_event_immutable
BEFORE UPDATE OR DELETE ON audit.audit_event
FOR EACH ROW EXECUTE FUNCTION audit.reject_mutation();

CREATE INDEX ix_outbox_message_claim ON integration.outbox_message (available_at, occurred_at) WHERE published_at IS NULL;

CREATE INDEX ix_operation_job_claim ON ops.operation_job (queue_name, status, available_at, priority DESC, created_at) WHERE status IN ('QUEUED','RETRY_WAIT');

CREATE INDEX ix_operation_job_expired_lease ON ops.operation_job (lease_until) WHERE status = 'RUNNING';

CREATE INDEX ix_audit_event_actor_occurred_at ON audit.audit_event (actor_principal_id, occurred_at DESC);

CREATE INDEX ix_audit_event_occurred_at ON audit.audit_event (occurred_at DESC);

CREATE INDEX ix_audit_event_program_version_occurred_at ON audit.audit_event (program_version_id, occurred_at DESC);

CREATE INDEX ix_audit_event_resource_occurred_at ON audit.audit_event (resource_type, resource_id, occurred_at DESC);

CREATE UNIQUE INDEX uq_audit_event_chain_sequence ON audit.audit_event (chain_id, chain_sequence);

CREATE UNIQUE INDEX uq_audit_event_event_hash ON audit.audit_event (event_hash);


GRANT USAGE ON SCHEMA iam, academic TO outcomehub_authorizer;
GRANT CREATE ON SCHEMA iam TO outcomehub_authorizer;
REVOKE CREATE ON SCHEMA academic FROM outcomehub_authorizer;

GRANT SELECT ON TABLE
    iam.principal,
    iam.role,
    iam.role_version,
    iam.role_version_permission,
    iam.permission,
    iam.role_assignment,
    iam.access_scope,
    academic.org_unit
TO outcomehub_authorizer;

CREATE FUNCTION iam.current_context_uuid(setting_name text)
RETURNS uuid
LANGUAGE plpgsql
STABLE
SECURITY INVOKER
SET search_path = pg_catalog, iam, pg_temp
AS $function$
DECLARE
    setting_value text;
BEGIN
    IF setting_name IS NULL OR setting_name NOT IN (
        'app.principal_id',
        'app.request_id',
        'app.job_id') THEN
        RETURN NULL;
    END IF;

    setting_value := pg_catalog.current_setting(setting_name, true);

    IF setting_value IS NULL OR pg_catalog.btrim(setting_value) = '' THEN
        RETURN NULL;
    END IF;

    BEGIN
        RETURN setting_value::uuid;
    EXCEPTION
        WHEN invalid_text_representation THEN
            RETURN NULL;
    END;
END;
$function$;

CREATE FUNCTION iam.has_permission(
    requested_resource_type text,
    requested_action text,
    requested_field_scope text,
    target_org_unit_id uuid,
    target_program_id uuid,
    target_program_version_id uuid,
    target_cohort_id uuid,
    target_curriculum_path_id uuid,
    target_course_id uuid,
    target_course_offering_id uuid,
    target_measurement_period_id uuid,
    target_student_id uuid,
    target_classification text)
RETURNS boolean
LANGUAGE sql
STABLE
SECURITY DEFINER
SET search_path = pg_catalog, iam, academic, pg_temp
AS $function$
    SELECT
        iam.current_context_uuid('app.principal_id') IS NOT NULL
        AND iam.current_context_uuid('app.request_id') IS NOT NULL
        AND NULLIF(
            pg_catalog.btrim(
                pg_catalog.current_setting('app.purpose', true)),
            '') IS NOT NULL
        AND requested_resource_type IS NOT NULL
        AND pg_catalog.btrim(requested_resource_type) <> ''
        AND requested_action IS NOT NULL
        AND pg_catalog.btrim(requested_action) <> ''
        AND requested_field_scope IS NOT NULL
        AND pg_catalog.btrim(requested_field_scope) <> ''
        AND target_program_id IS NULL
        AND target_program_version_id IS NULL
        AND target_cohort_id IS NULL
        AND target_curriculum_path_id IS NULL
        AND target_course_offering_id IS NULL
        AND target_measurement_period_id IS NULL
        AND target_student_id IS NULL
        AND target_classification IS NULL
        AND EXISTS (
            SELECT 1
            FROM iam.principal AS principal
            INNER JOIN iam.role_assignment AS assignment
                ON assignment.principal_id = principal.id
            INNER JOIN iam.role AS assigned_role
                ON assigned_role.id = assignment.role_id
            INNER JOIN iam.role_version AS role_version
                ON role_version.id = assignment.role_version_id
                AND role_version.role_id = assignment.role_id
            INNER JOIN iam.role_version_permission AS role_permission
                ON role_permission.role_version_id = role_version.id
            INNER JOIN iam.permission AS permission
                ON permission.id = role_permission.permission_id
            INNER JOIN iam.access_scope AS access_scope
                ON access_scope.id = assignment.access_scope_id
            WHERE principal.id = iam.current_context_uuid('app.principal_id')
              AND principal.status = 'ACTIVE'
              AND assigned_role.status = 'ACTIVE'
              AND role_version.status = 'ACTIVE'
              AND role_version.effective_from <= CURRENT_DATE
              AND (
                  role_version.effective_to IS NULL
                  OR role_version.effective_to > CURRENT_DATE)
              AND assignment.status = 'ACTIVE'
              AND assignment.effective_from <= CURRENT_TIMESTAMP
              AND assignment.effective_to > CURRENT_TIMESTAMP
              AND permission.resource_type = requested_resource_type
              AND permission.action = requested_action
              AND permission.field_scope = requested_field_scope
              AND (
                  access_scope.scope_type = 'SYSTEM'
                  OR (
                      access_scope.scope_type = 'COURSE'
                      AND target_course_id IS NOT NULL
                      AND access_scope.course_id = target_course_id)
                  OR (
                      access_scope.scope_type = 'ORG_UNIT'
                      AND target_org_unit_id IS NOT NULL
                      AND (
                          access_scope.org_unit_id = target_org_unit_id
                          OR (
                              access_scope.include_descendants
                              AND EXISTS (
                                  WITH RECURSIVE org_ancestors AS (
                                      SELECT
                                          org_unit.id,
                                          org_unit.parent_id,
                                          ARRAY[org_unit.id]::uuid[] AS visited
                                      FROM academic.org_unit AS org_unit
                                      WHERE org_unit.id = target_org_unit_id

                                      UNION ALL

                                      SELECT
                                          parent.id,
                                          parent.parent_id,
                                          ancestor.visited || parent.id
                                      FROM org_ancestors AS ancestor
                                      INNER JOIN academic.org_unit AS parent
                                          ON parent.id = ancestor.parent_id
                                      WHERE NOT parent.id = ANY(ancestor.visited)
                                  )
                                  SELECT 1
                                  FROM org_ancestors AS ancestor
                                  WHERE ancestor.id = access_scope.org_unit_id)))
                  )));
$function$;

ALTER FUNCTION iam.current_context_uuid(text)
    OWNER TO outcomehub_authorizer;

ALTER FUNCTION iam.has_permission(
    text,
    text,
    text,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    text)
    OWNER TO outcomehub_authorizer;

REVOKE CREATE ON SCHEMA iam FROM outcomehub_authorizer;

REVOKE ALL ON FUNCTION iam.current_context_uuid(text) FROM PUBLIC;
REVOKE ALL ON FUNCTION iam.has_permission(
    text,
    text,
    text,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    text)
FROM PUBLIC;

GRANT USAGE ON SCHEMA iam, academic TO outcomehub_app;
REVOKE CREATE ON SCHEMA iam, academic FROM outcomehub_app;
GRANT EXECUTE ON FUNCTION iam.current_context_uuid(text)
    TO outcomehub_app;
GRANT EXECUTE ON FUNCTION iam.has_permission(
    text,
    text,
    text,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    uuid,
    text)
    TO outcomehub_app;

INSERT INTO iam.permission (
    id,
    resource_type,
    action,
    field_scope,
    description)
VALUES
    ('10000000-0000-7000-8000-000000000001', 'academic.course', 'READ', '*', 'Read courses inside the assigned access scope.'),
    ('10000000-0000-7000-8000-000000000002', 'academic.course', 'CREATE', '*', 'Create courses inside the assigned access scope.'),
    ('10000000-0000-7000-8000-000000000003', 'academic.course', 'UPDATE', '*', 'Update courses inside the assigned access scope.'),
    ('10000000-0000-7000-8000-000000000004', 'academic.course', 'DELETE', '*', 'Delete courses inside the assigned access scope.');

ALTER TABLE academic.course ENABLE ROW LEVEL SECURITY;
ALTER TABLE academic.course FORCE ROW LEVEL SECURITY;

CREATE POLICY course_select_policy
ON academic.course
FOR SELECT
TO outcomehub_app
USING (
    iam.has_permission(
        'academic.course',
        'READ',
        '*',
        owner_org_unit_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

CREATE POLICY course_insert_policy
ON academic.course
FOR INSERT
TO outcomehub_app
WITH CHECK (
    iam.has_permission(
        'academic.course',
        'CREATE',
        '*',
        owner_org_unit_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

CREATE POLICY course_update_policy
ON academic.course
FOR UPDATE
TO outcomehub_app
USING (
    iam.has_permission(
        'academic.course',
        'UPDATE',
        '*',
        owner_org_unit_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text))
WITH CHECK (
    iam.has_permission(
        'academic.course',
        'UPDATE',
        '*',
        owner_org_unit_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

CREATE POLICY course_delete_policy
ON academic.course
FOR DELETE
TO outcomehub_app
USING (
    iam.has_permission(
        'academic.course',
        'DELETE',
        '*',
        owner_org_unit_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

REVOKE ALL PRIVILEGES ON TABLE academic.course FROM PUBLIC;
REVOKE ALL PRIVILEGES ON TABLE academic.course FROM outcomehub_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE academic.course
    TO outcomehub_app;

