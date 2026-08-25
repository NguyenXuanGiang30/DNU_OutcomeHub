CREATE FUNCTION measurement.guard_input_snapshot_mutation()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
BEGIN
    IF TG_OP = 'TRUNCATE' THEN
        RAISE EXCEPTION 'InputSnapshot aggregates cannot be truncated.'
            USING ERRCODE = '55000';
    END IF;

    IF TG_OP = 'INSERT' THEN
        IF NEW.status <> 'BUILDING' THEN
            RAISE EXCEPTION
                'InputSnapshot % must be created in BUILDING status.',
                NEW.id
                USING ERRCODE = '55000';
        END IF;

        RETURN NEW;
    END IF;

    IF TG_OP = 'DELETE' THEN
        IF OLD.status <> 'BUILDING' THEN
            RAISE EXCEPTION
                'InputSnapshot % is immutable in % status.',
                OLD.id,
                OLD.status
                USING ERRCODE = '55000';
        END IF;

        RETURN OLD;
    END IF;

    IF ROW(
        NEW.id,
        NEW.governed_resource_id,
        NEW.measurement_period_id,
        NEW.org_unit_id,
        NEW.snapshot_no,
        NEW.policy_version_id,
        NEW.program_policy_binding_id,
        NEW.institution_template_version_id,
        NEW.program_version_id,
        NEW.academic_year_start,
        NEW.schema_version,
        NEW.hash_algorithm,
        NEW.parent_snapshot_id,
        NEW.created_by,
        NEW.created_at)
       IS DISTINCT FROM
       ROW(
        OLD.id,
        OLD.governed_resource_id,
        OLD.measurement_period_id,
        OLD.org_unit_id,
        OLD.snapshot_no,
        OLD.policy_version_id,
        OLD.program_policy_binding_id,
        OLD.institution_template_version_id,
        OLD.program_version_id,
        OLD.academic_year_start,
        OLD.schema_version,
        OLD.hash_algorithm,
        OLD.parent_snapshot_id,
        OLD.created_by,
        OLD.created_at) THEN
        RAISE EXCEPTION 'InputSnapshot % identity and binding are immutable.', OLD.id
            USING ERRCODE = '55000';
    END IF;

    IF OLD.status <> 'BUILDING' THEN
        RAISE EXCEPTION
            'InputSnapshot % is immutable in % status.',
            OLD.id,
            OLD.status
            USING ERRCODE = '55000';
    END IF;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION measurement.guard_input_snapshot_mutation()
    FROM PUBLIC;

CREATE FUNCTION measurement.guard_snapshot_child_mutation()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
DECLARE
    target_snapshot_id uuid;
    snapshot_status text;
BEGIN
    IF TG_OP = 'TRUNCATE' THEN
        RAISE EXCEPTION 'InputSnapshot child tables cannot be truncated.'
            USING ERRCODE = '55000';
    END IF;

    IF TG_OP = 'UPDATE'
       AND NEW.input_snapshot_id IS DISTINCT FROM OLD.input_snapshot_id THEN
        RAISE EXCEPTION
            'Snapshot child rows cannot move between InputSnapshot aggregates.'
            USING ERRCODE = '55000';
    END IF;

    target_snapshot_id := CASE
        WHEN TG_OP = 'DELETE' THEN OLD.input_snapshot_id
        ELSE NEW.input_snapshot_id
    END;

    SELECT input_snapshot.status
    INTO snapshot_status
    FROM measurement.input_snapshot AS input_snapshot
    WHERE input_snapshot.id = target_snapshot_id
    FOR SHARE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'InputSnapshot % does not exist.', target_snapshot_id
            USING ERRCODE = '23503';
    END IF;

    IF snapshot_status <> 'BUILDING' THEN
        RAISE EXCEPTION
            'Snapshot child %.% is immutable because InputSnapshot % is in % status.',
            TG_TABLE_SCHEMA,
            TG_TABLE_NAME,
            target_snapshot_id,
            snapshot_status
            USING ERRCODE = '55000';
    END IF;

    IF TG_OP = 'DELETE' THEN
        RETURN OLD;
    END IF;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION measurement.guard_snapshot_child_mutation()
    FROM PUBLIC;

CREATE FUNCTION result.guard_result_batch_mutation()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
DECLARE
    snapshot_status text;
BEGIN
    IF TG_OP = 'TRUNCATE' THEN
        RAISE EXCEPTION 'ResultBatch aggregates cannot be truncated.'
            USING ERRCODE = '55000';
    END IF;

    IF TG_OP = 'DELETE' THEN
        RAISE EXCEPTION 'ResultBatch % cannot be deleted.', OLD.id
            USING ERRCODE = '55000';
    END IF;

    IF TG_OP = 'INSERT' THEN
        IF NEW.status NOT IN ('QUEUED', 'RUNNING') THEN
            RAISE EXCEPTION
                'ResultBatch % must be created in QUEUED or RUNNING status.',
                NEW.id
                USING ERRCODE = '55000';
        END IF;

        SELECT input_snapshot.status
        INTO snapshot_status
        FROM measurement.input_snapshot AS input_snapshot
        WHERE input_snapshot.id = NEW.input_snapshot_id
        FOR SHARE;

        IF NOT FOUND THEN
            RAISE EXCEPTION
                'InputSnapshot % does not exist for ResultBatch %.',
                NEW.input_snapshot_id,
                NEW.id
                USING ERRCODE = '23503';
        END IF;

        IF snapshot_status <> 'SEALED' THEN
            RAISE EXCEPTION
                'ResultBatch % requires a SEALED InputSnapshot; snapshot % is in % status.',
                NEW.id,
                NEW.input_snapshot_id,
                snapshot_status
                USING ERRCODE = '55000';
        END IF;

        RETURN NEW;
    END IF;

    IF ROW(
        NEW.id,
        NEW.governed_resource_id,
        NEW.measurement_period_id,
        NEW.input_snapshot_id,
        NEW.policy_version_id,
        NEW.program_policy_binding_id,
        NEW.org_unit_id,
        NEW.program_version_id,
        NEW.academic_year_start,
        NEW.batch_no,
        NEW.engine_version,
        NEW.source_commit,
        NEW.idempotency_key,
        NEW.request_checksum,
        NEW.recalculates_batch_id,
        NEW.recalculation_reason,
        NEW.workflow_instance_id,
        NEW.sod_policy_version_id)
       IS DISTINCT FROM
       ROW(
        OLD.id,
        OLD.governed_resource_id,
        OLD.measurement_period_id,
        OLD.input_snapshot_id,
        OLD.policy_version_id,
        OLD.program_policy_binding_id,
        OLD.org_unit_id,
        OLD.program_version_id,
        OLD.academic_year_start,
        OLD.batch_no,
        OLD.engine_version,
        OLD.source_commit,
        OLD.idempotency_key,
        OLD.request_checksum,
        OLD.recalculates_batch_id,
        OLD.recalculation_reason,
        OLD.workflow_instance_id,
        OLD.sod_policy_version_id) THEN
        RAISE EXCEPTION 'ResultBatch % identity, scope, policy, and engine binding are immutable.', OLD.id
            USING ERRCODE = '55000';
    END IF;

    IF OLD.container_digest IS NOT NULL
       AND NEW.container_digest IS DISTINCT FROM OLD.container_digest THEN
        RAISE EXCEPTION 'ResultBatch % container digest is write-once.', OLD.id
            USING ERRCODE = '55000';
    END IF;

    IF OLD.result_checksum IS NOT NULL
       AND NEW.result_checksum IS DISTINCT FROM OLD.result_checksum THEN
        RAISE EXCEPTION 'ResultBatch % result checksum is write-once.', OLD.id
            USING ERRCODE = '55000';
    END IF;

    IF OLD.started_at IS NOT NULL
       AND NEW.started_at IS DISTINCT FROM OLD.started_at THEN
        RAISE EXCEPTION 'ResultBatch % started_at is write-once.', OLD.id
            USING ERRCODE = '55000';
    END IF;

    IF OLD.completed_at IS NOT NULL
       AND NEW.completed_at IS DISTINCT FROM OLD.completed_at THEN
        RAISE EXCEPTION 'ResultBatch % completed_at is write-once.', OLD.id
            USING ERRCODE = '55000';
    END IF;

    IF OLD.published_at IS NOT NULL
       AND NEW.published_at IS DISTINCT FROM OLD.published_at THEN
        RAISE EXCEPTION 'ResultBatch % published_at is write-once.', OLD.id
            USING ERRCODE = '55000';
    END IF;

    IF OLD.status NOT IN ('QUEUED', 'RUNNING')
       AND NEW.status IN ('QUEUED', 'RUNNING') THEN
        RAISE EXCEPTION
            'ResultBatch % cannot return from % to a mutable status.',
            OLD.id,
            OLD.status
            USING ERRCODE = '55000';
    END IF;

    IF OLD.status NOT IN ('QUEUED', 'RUNNING')
       AND ROW(
            NEW.container_digest,
            NEW.result_checksum,
            NEW.started_at,
            NEW.completed_at)
           IS DISTINCT FROM
           ROW(
            OLD.container_digest,
            OLD.result_checksum,
            OLD.started_at,
            OLD.completed_at) THEN
        RAISE EXCEPTION
            'ResultBatch % calculation artifacts are immutable after % status.',
            OLD.id,
            OLD.status
            USING ERRCODE = '55000';
    END IF;

    IF NEW.status IN (
            'CALCULATED',
            'VALIDATED',
            'IN_REVIEW',
            'APPROVED',
            'PUBLISHED')
       AND (
            NEW.result_checksum IS NULL
            OR NEW.completed_at IS NULL) THEN
        RAISE EXCEPTION
            'ResultBatch % requires result_checksum and completed_at in % status.',
            NEW.id,
            NEW.status
            USING ERRCODE = '55000';
    END IF;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION result.guard_result_batch_mutation()
    FROM PUBLIC;

CREATE FUNCTION result.guard_final_detail_mutation()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
DECLARE
    target_batch_id uuid;
    batch_status text;
BEGIN
    IF TG_OP = 'TRUNCATE' THEN
        RAISE EXCEPTION 'Final result detail tables cannot be truncated.'
            USING ERRCODE = '55000';
    END IF;

    IF TG_OP = 'UPDATE'
       AND NEW.batch_id IS DISTINCT FROM OLD.batch_id THEN
        RAISE EXCEPTION 'Final result rows cannot move between ResultBatch aggregates.'
            USING ERRCODE = '55000';
    END IF;

    target_batch_id := CASE
        WHEN TG_OP = 'DELETE' THEN OLD.batch_id
        ELSE NEW.batch_id
    END;

    SELECT result_batch.status
    INTO batch_status
    FROM result.result_batch AS result_batch
    WHERE result_batch.id = target_batch_id
    FOR SHARE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'ResultBatch % does not exist.', target_batch_id
            USING ERRCODE = '23503';
    END IF;

    IF batch_status NOT IN ('QUEUED', 'RUNNING') THEN
        RAISE EXCEPTION
            'Final result %.% is immutable because ResultBatch % is in % status.',
            TG_TABLE_SCHEMA,
            TG_TABLE_NAME,
            target_batch_id,
            batch_status
            USING ERRCODE = '55000';
    END IF;

    IF TG_OP = 'DELETE' THEN
        RETURN OLD;
    END IF;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION result.guard_final_detail_mutation()
    FROM PUBLIC;

CREATE TRIGGER trg_input_snapshot_guard_mutation
BEFORE INSERT OR UPDATE OR DELETE ON measurement.input_snapshot
FOR EACH ROW
EXECUTE FUNCTION measurement.guard_input_snapshot_mutation();

CREATE TRIGGER trg_input_snapshot_reject_truncate
BEFORE TRUNCATE ON measurement.input_snapshot
FOR EACH STATEMENT
EXECUTE FUNCTION measurement.guard_input_snapshot_mutation();

DO $migration$
DECLARE
    target_table regclass;
BEGIN
    FOREACH target_table IN ARRAY ARRAY[
        'measurement.snapshot_resource'::regclass,
        'measurement.snapshot_offering'::regclass,
        'measurement.snapshot_population_member'::regclass,
        'measurement.snapshot_enrollment'::regclass,
        'measurement.snapshot_score'::regclass,
        'measurement.snapshot_direct_pi_weight'::regclass,
        'measurement.snapshot_question_criterion_weight'::regclass,
        'measurement.snapshot_pi_source_weight'::regclass,
        'measurement.snapshot_pi_plo_weight'::regclass,
        'measurement.snapshot_threshold'::regclass,
        'measurement.snapshot_indirect_observation'::regclass,
        'measurement.snapshot_manifest_chunk'::regclass]
    LOOP
        EXECUTE pg_catalog.format(
            'CREATE TRIGGER trg_snapshot_child_guard_mutation '
            'BEFORE INSERT OR UPDATE OR DELETE ON %s '
            'FOR EACH ROW '
            'EXECUTE FUNCTION measurement.guard_snapshot_child_mutation()',
            target_table);

        EXECUTE pg_catalog.format(
            'CREATE TRIGGER trg_snapshot_child_reject_truncate '
            'BEFORE TRUNCATE ON %s '
            'FOR EACH STATEMENT '
            'EXECUTE FUNCTION measurement.guard_snapshot_child_mutation()',
            target_table);
    END LOOP;
END;
$migration$;

CREATE TRIGGER trg_result_batch_guard_mutation
BEFORE INSERT OR UPDATE OR DELETE ON result.result_batch
FOR EACH ROW
EXECUTE FUNCTION result.guard_result_batch_mutation();

CREATE TRIGGER trg_result_batch_reject_truncate
BEFORE TRUNCATE ON result.result_batch
FOR EACH STATEMENT
EXECUTE FUNCTION result.guard_result_batch_mutation();

DO $migration$
DECLARE
    target_table regclass;
BEGIN
    FOREACH target_table IN ARRAY ARRAY[
        'result.student_criterion_result'::regclass,
        'result.student_criterion_score_lineage'::regclass,
        'result.criterion_pi_contribution'::regclass,
        'result.student_clo_result'::regclass,
        'result.course_pi_result'::regclass,
        'result.student_pi_result'::regclass,
        'result.student_pi_source_contribution'::regclass,
        'result.student_plo_result'::regclass,
        'result.student_plo_pi_contribution'::regclass,
        'result.cohort_outcome_result'::regclass,
        'result.cohort_population_decision'::regclass]
    LOOP
        EXECUTE pg_catalog.format(
            'CREATE TRIGGER trg_final_detail_guard_mutation '
            'BEFORE INSERT OR UPDATE OR DELETE ON %s '
            'FOR EACH ROW '
            'EXECUTE FUNCTION result.guard_final_detail_mutation()',
            target_table);

        EXECUTE pg_catalog.format(
            'CREATE TRIGGER trg_final_detail_reject_truncate '
            'BEFORE TRUNCATE ON %s '
            'FOR EACH STATEMENT '
            'EXECUTE FUNCTION result.guard_final_detail_mutation()',
            target_table);
    END LOOP;
END;
$migration$;

REVOKE TRUNCATE ON TABLE
    measurement.input_snapshot,
    measurement.snapshot_resource,
    measurement.snapshot_offering,
    measurement.snapshot_population_member,
    measurement.snapshot_enrollment,
    measurement.snapshot_score,
    measurement.snapshot_direct_pi_weight,
    measurement.snapshot_question_criterion_weight,
    measurement.snapshot_pi_source_weight,
    measurement.snapshot_pi_plo_weight,
    measurement.snapshot_threshold,
    measurement.snapshot_indirect_observation,
    measurement.snapshot_manifest_chunk,
    result.result_batch,
    result.student_criterion_result,
    result.student_criterion_score_lineage,
    result.criterion_pi_contribution,
    result.student_clo_result,
    result.course_pi_result,
    result.student_pi_result,
    result.student_pi_source_contribution,
    result.student_plo_result,
    result.student_plo_pi_contribution,
    result.cohort_outcome_result,
    result.cohort_population_decision
FROM outcomehub_app;
