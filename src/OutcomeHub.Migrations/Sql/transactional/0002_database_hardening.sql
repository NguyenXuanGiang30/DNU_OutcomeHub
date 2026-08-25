REVOKE ALL ON FUNCTION audit.reject_mutation() FROM PUBLIC;

DROP TRIGGER IF EXISTS trg_audit_event_reject_truncate
    ON audit.audit_event;
CREATE TRIGGER trg_audit_event_reject_truncate
BEFORE TRUNCATE ON audit.audit_event
FOR EACH STATEMENT
EXECUTE FUNCTION audit.reject_mutation();

CREATE OR REPLACE FUNCTION integration.guard_outbox_envelope()
RETURNS trigger
LANGUAGE plpgsql
SET search_path = pg_catalog, integration
AS $function$
BEGIN
    IF ROW(
        NEW.id,
        NEW.aggregate_type,
        NEW.aggregate_id,
        NEW.aggregate_version,
        NEW.event_type,
        NEW.event_schema_version,
        NEW.payload,
        NEW.headers,
        NEW.classification,
        NEW.correlation_id,
        NEW.causation_id,
        NEW.trace_id,
        NEW.occurred_at)
       IS DISTINCT FROM
       ROW(
        OLD.id,
        OLD.aggregate_type,
        OLD.aggregate_id,
        OLD.aggregate_version,
        OLD.event_type,
        OLD.event_schema_version,
        OLD.payload,
        OLD.headers,
        OLD.classification,
        OLD.correlation_id,
        OLD.causation_id,
        OLD.trace_id,
        OLD.occurred_at) THEN
        RAISE EXCEPTION 'Outbox event envelope is immutable.'
            USING ERRCODE = '55000';
    END IF;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION integration.guard_outbox_envelope() FROM PUBLIC;

CREATE TRIGGER trg_outbox_message_envelope_immutable
BEFORE UPDATE ON integration.outbox_message
FOR EACH ROW
EXECUTE FUNCTION integration.guard_outbox_envelope();

CREATE OR REPLACE FUNCTION measurement.validate_score_record_scope()
RETURNS trigger
LANGUAGE plpgsql
SET search_path = pg_catalog, measurement, academic
AS $function$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM measurement.score_identity AS identity_row
        INNER JOIN academic.course_offering AS offering
            ON offering.id = identity_row.course_offering_id
        INNER JOIN academic.program_version AS program_version
            ON program_version.id = offering.program_version_id
        INNER JOIN academic.course_version AS course_version
            ON course_version.id = offering.course_version_id
        WHERE identity_row.id = NEW.score_identity_id
          AND identity_row.academic_year_start = NEW.academic_year_start
          AND identity_row.student_id = NEW.student_id
          AND identity_row.course_offering_id = NEW.course_offering_id
          AND offering.id = NEW.course_offering_id
          AND offering.academic_year_start = NEW.academic_year_start
          AND offering.org_unit_id = NEW.org_unit_id
          AND offering.program_version_id = NEW.program_version_id
          AND program_version.program_id = NEW.program_id
          AND course_version.course_id = NEW.course_id) THEN
        RAISE EXCEPTION 'Score record scope does not match its identity and offering.'
            USING ERRCODE = '23514';
    END IF;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION measurement.validate_score_record_scope() FROM PUBLIC;

CREATE TRIGGER trg_score_record_validate_scope
BEFORE INSERT ON measurement.score_record
FOR EACH ROW
EXECUTE FUNCTION measurement.validate_score_record_scope();

CREATE TRIGGER trg_score_record_update_immutable
BEFORE UPDATE ON measurement.score_record
FOR EACH ROW
EXECUTE FUNCTION audit.reject_mutation();

CREATE TRIGGER trg_raw_record_update_immutable
BEFORE UPDATE ON integration.raw_record
FOR EACH ROW
EXECUTE FUNCTION audit.reject_mutation();

DO $migration$
DECLARE
    target_table regclass;
BEGIN
    FOREACH target_table IN ARRAY ARRAY[
        'iam.permission'::regclass,
        'iam.access_scope'::regclass,
        'iam.database_principal_binding'::regclass,
        'workflow.transition'::regclass,
        'integration.quarantine_correction'::regclass,
        'result.publication'::regclass,
        'result.publication_revocation'::regclass,
        'result.batch_supersession'::regclass,
        'ai.ai_review_event'::regclass,
        'ai.evaluation_result'::regclass]
    LOOP
        EXECUTE pg_catalog.format(
            'CREATE TRIGGER trg_append_only_immutable '
            'BEFORE UPDATE OR DELETE ON %s '
            'FOR EACH ROW EXECUTE FUNCTION audit.reject_mutation()',
            target_table);
        EXECUTE pg_catalog.format(
            'CREATE TRIGGER trg_append_only_reject_truncate '
            'BEFORE TRUNCATE ON %s '
            'FOR EACH STATEMENT EXECUTE FUNCTION audit.reject_mutation()',
            target_table);
    END LOOP;
END;
$migration$;

DO $migration$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM pg_catalog.pg_roles
        WHERE rolname = 'outcomehub_app') THEN
        REVOKE INSERT, UPDATE, DELETE, TRUNCATE
            ON audit.audit_event
            FROM outcomehub_app;
        REVOKE UPDATE, DELETE, TRUNCATE
            ON iam.permission,
               iam.access_scope,
               iam.database_principal_binding,
               workflow.transition,
               integration.quarantine_correction,
               result.publication,
               result.publication_revocation,
               result.batch_supersession,
               ai.ai_review_event,
               ai.evaluation_result
            FROM outcomehub_app;
        REVOKE UPDATE
            ON integration.raw_record,
               measurement.score_record
            FROM outcomehub_app;
    END IF;
END;
$migration$;

DROP POLICY course_insert_policy ON academic.course;
CREATE POLICY course_insert_policy
ON academic.course
FOR INSERT
TO outcomehub_app
WITH CHECK (
    status = 'DRAFT'
    AND iam.has_permission(
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

DROP POLICY course_update_policy ON academic.course;
CREATE POLICY course_update_policy
ON academic.course
FOR UPDATE
TO outcomehub_app
USING (
    status = 'DRAFT'
    AND iam.has_permission(
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
    status = 'DRAFT'
    AND iam.has_permission(
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

DROP POLICY course_delete_policy ON academic.course;
CREATE POLICY course_delete_policy
ON academic.course
FOR DELETE
TO outcomehub_app
USING (
    status = 'DRAFT'
    AND iam.has_permission(
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
