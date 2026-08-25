CREATE FUNCTION academic.validate_program_version_cohort_same_program()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
DECLARE
    invalid_program_version_id uuid;
    invalid_cohort_id uuid;
BEGIN
    SELECT binding.program_version_id, binding.cohort_id
    INTO invalid_program_version_id, invalid_cohort_id
    FROM academic.program_version_cohort AS binding
    INNER JOIN academic.program_version AS program_version
        ON program_version.id = binding.program_version_id
    INNER JOIN academic.cohort AS cohort
        ON cohort.id = binding.cohort_id
    WHERE program_version.program_id IS DISTINCT FROM cohort.program_id
    LIMIT 1;

    IF FOUND THEN
        RAISE EXCEPTION
            'ProgramVersionCohort (program_version_id=%, cohort_id=%) crosses programs.',
            invalid_program_version_id,
            invalid_cohort_id
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_program_version_cohort_same_program';
    END IF;

    RETURN NULL;
END;
$function$;

REVOKE ALL ON FUNCTION academic.validate_program_version_cohort_same_program()
    FROM PUBLIC;

CREATE CONSTRAINT TRIGGER ctrg_program_version_cohort_same_program
AFTER INSERT OR UPDATE ON academic.program_version_cohort
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION academic.validate_program_version_cohort_same_program();

CREATE CONSTRAINT TRIGGER ctrg_program_version_same_program_cohort
AFTER UPDATE OF program_id ON academic.program_version
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION academic.validate_program_version_cohort_same_program();

CREATE CONSTRAINT TRIGGER ctrg_cohort_same_program_version
AFTER UPDATE OF program_id ON academic.cohort
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION academic.validate_program_version_cohort_same_program();

CREATE FUNCTION academic.validate_program_course_curriculum_plan_version()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
DECLARE
    invalid_program_course_id uuid;
BEGIN
    SELECT program_course.id
    INTO invalid_program_course_id
    FROM academic.program_course AS program_course
    INNER JOIN academic.curriculum_block AS curriculum_block
        ON curriculum_block.id = program_course.curriculum_block_id
    INNER JOIN academic.curriculum_plan AS curriculum_plan
        ON curriculum_plan.id = curriculum_block.curriculum_plan_id
    WHERE curriculum_plan.program_version_id
        IS DISTINCT FROM program_course.program_version_id
    LIMIT 1;

    IF FOUND THEN
        RAISE EXCEPTION
            'ProgramCourse % uses a CurriculumBlock from another ProgramVersion.',
            invalid_program_course_id
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_program_course_curriculum_plan_version';
    END IF;

    RETURN NULL;
END;
$function$;

REVOKE ALL ON FUNCTION academic.validate_program_course_curriculum_plan_version()
    FROM PUBLIC;

CREATE CONSTRAINT TRIGGER ctrg_program_course_curriculum_plan_version
AFTER INSERT OR UPDATE ON academic.program_course
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION academic.validate_program_course_curriculum_plan_version();

CREATE CONSTRAINT TRIGGER ctrg_curriculum_block_program_course_version
AFTER UPDATE OF curriculum_plan_id ON academic.curriculum_block
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION academic.validate_program_course_curriculum_plan_version();

CREATE CONSTRAINT TRIGGER ctrg_curriculum_plan_program_course_version
AFTER UPDATE OF program_version_id ON academic.curriculum_plan
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION academic.validate_program_course_curriculum_plan_version();

CREATE FUNCTION academic.validate_anchor_criterion_typed_binding()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
DECLARE
    invalid_anchor_assessment_id uuid;
    invalid_traceability_id uuid;
BEGIN
    SELECT anchor_criterion.anchor_assessment_id,
           anchor_criterion.syllabus_traceability_id
    INTO invalid_anchor_assessment_id, invalid_traceability_id
    FROM academic.anchor_criterion AS anchor_criterion
    INNER JOIN academic.anchor_assessment AS anchor_assessment
        ON anchor_assessment.id = anchor_criterion.anchor_assessment_id
    INNER JOIN academic.direct_measurement_source AS direct_source
        ON direct_source.id = anchor_assessment.direct_measurement_source_id
    INNER JOIN portfolio.syllabus_traceability AS traceability
        ON traceability.id = anchor_criterion.syllabus_traceability_id
    INNER JOIN portfolio.rubric_criterion AS rubric_criterion
        ON rubric_criterion.id = traceability.rubric_criterion_id
    WHERE traceability.syllabus_version_id
            IS DISTINCT FROM anchor_assessment.syllabus_version_id
       OR rubric_criterion.syllabus_version_id
            IS DISTINCT FROM anchor_assessment.syllabus_version_id
       OR rubric_criterion.assessment_item_id
            IS DISTINCT FROM anchor_assessment.assessment_item_id
       OR traceability.data_role IS DISTINCT FROM 'DIRECT_PI'
       OR traceability.course_pi_mapping_id
            IS DISTINCT FROM direct_source.course_pi_mapping_id
       OR traceability.program_version_id
            IS DISTINCT FROM direct_source.program_version_id
    LIMIT 1;

    IF FOUND THEN
        RAISE EXCEPTION
            'AnchorCriterion (anchor_assessment_id=%, traceability_id=%) has an invalid typed binding.',
            invalid_anchor_assessment_id,
            invalid_traceability_id
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_anchor_criterion_typed_binding';
    END IF;

    RETURN NULL;
END;
$function$;

REVOKE ALL ON FUNCTION academic.validate_anchor_criterion_typed_binding()
    FROM PUBLIC;

CREATE CONSTRAINT TRIGGER ctrg_anchor_criterion_typed_binding
AFTER INSERT OR UPDATE ON academic.anchor_criterion
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION academic.validate_anchor_criterion_typed_binding();

CREATE CONSTRAINT TRIGGER ctrg_anchor_assessment_typed_binding
AFTER UPDATE ON academic.anchor_assessment
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION academic.validate_anchor_criterion_typed_binding();

CREATE CONSTRAINT TRIGGER ctrg_direct_measurement_source_typed_binding
AFTER UPDATE ON academic.direct_measurement_source
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION academic.validate_anchor_criterion_typed_binding();

CREATE CONSTRAINT TRIGGER ctrg_syllabus_traceability_anchor_binding
AFTER UPDATE ON portfolio.syllabus_traceability
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION academic.validate_anchor_criterion_typed_binding();

CREATE CONSTRAINT TRIGGER ctrg_rubric_criterion_anchor_binding
AFTER UPDATE ON portfolio.rubric_criterion
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION academic.validate_anchor_criterion_typed_binding();

CREATE FUNCTION academic.enforce_curriculum_block_acyclic()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
BEGIN
    PERFORM pg_catalog.pg_advisory_xact_lock(
        pg_catalog.hashtextextended(
            'academic.curriculum_block.acyclic'::text,
            0));

    IF NEW.parent_id IS NULL THEN
        RETURN NEW;
    END IF;

    IF EXISTS (
        WITH RECURSIVE ancestors(id) AS (
            SELECT NEW.parent_id

            UNION

            SELECT curriculum_block.parent_id
            FROM academic.curriculum_block AS curriculum_block
            INNER JOIN ancestors AS ancestor
                ON ancestor.id = curriculum_block.id
            WHERE curriculum_block.parent_id IS NOT NULL)
        SELECT 1
        FROM ancestors
        WHERE id = NEW.id) THEN
        RAISE EXCEPTION
            'CurriculumBlock % cannot have parent % because it creates a cycle.',
            NEW.id,
            NEW.parent_id
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_curriculum_block_acyclic';
    END IF;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION academic.enforce_curriculum_block_acyclic()
    FROM PUBLIC;

CREATE TRIGGER trg_curriculum_block_acyclic
BEFORE INSERT OR UPDATE OF id, parent_id ON academic.curriculum_block
FOR EACH ROW
EXECUTE FUNCTION academic.enforce_curriculum_block_acyclic();

CREATE FUNCTION portfolio.enforce_assessment_item_acyclic()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
BEGIN
    PERFORM pg_catalog.pg_advisory_xact_lock(
        pg_catalog.hashtextextended(
            'portfolio.assessment_item.acyclic'::text,
            0));

    IF NEW.parent_id IS NULL THEN
        RETURN NEW;
    END IF;

    IF EXISTS (
        WITH RECURSIVE ancestors(id) AS (
            SELECT NEW.parent_id

            UNION

            SELECT assessment_item.parent_id
            FROM portfolio.assessment_item AS assessment_item
            INNER JOIN ancestors AS ancestor
                ON ancestor.id = assessment_item.id
            WHERE assessment_item.parent_id IS NOT NULL)
        SELECT 1
        FROM ancestors
        WHERE id = NEW.id) THEN
        RAISE EXCEPTION
            'AssessmentItem % cannot have parent % because it creates a cycle.',
            NEW.id,
            NEW.parent_id
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_assessment_item_acyclic';
    END IF;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION portfolio.enforce_assessment_item_acyclic()
    FROM PUBLIC;

CREATE TRIGGER trg_assessment_item_acyclic
BEFORE INSERT OR UPDATE OF id, parent_id ON portfolio.assessment_item
FOR EACH ROW
EXECUTE FUNCTION portfolio.enforce_assessment_item_acyclic();

CREATE FUNCTION governance.enforce_resource_dependency_acyclic()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
DECLARE
    old_parent_id uuid;
    old_child_id uuid;
    old_dependency_role text;
BEGIN
    PERFORM pg_catalog.pg_advisory_xact_lock(
        pg_catalog.hashtextextended(
            'governance.resource_dependency.acyclic'::text,
            0));

    IF TG_OP = 'UPDATE' THEN
        old_parent_id := OLD.parent_governed_resource_id;
        old_child_id := OLD.child_governed_resource_id;
        old_dependency_role := OLD.dependency_role;
    END IF;

    IF NEW.parent_governed_resource_id = NEW.child_governed_resource_id
       OR EXISTS (
            WITH RECURSIVE reachable(id) AS (
                SELECT NEW.child_governed_resource_id

                UNION

                SELECT dependency.child_governed_resource_id
                FROM governance.resource_dependency AS dependency
                INNER JOIN reachable
                    ON reachable.id = dependency.parent_governed_resource_id
                WHERE old_parent_id IS NULL
                   OR ROW(
                        dependency.parent_governed_resource_id,
                        dependency.child_governed_resource_id,
                        dependency.dependency_role)
                      IS DISTINCT FROM
                      ROW(old_parent_id, old_child_id, old_dependency_role))
            SELECT 1
            FROM reachable
            WHERE id = NEW.parent_governed_resource_id) THEN
        RAISE EXCEPTION
            'Governed-resource dependency % -> % creates a cycle.',
            NEW.parent_governed_resource_id,
            NEW.child_governed_resource_id
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_resource_dependency_acyclic';
    END IF;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION governance.enforce_resource_dependency_acyclic()
    FROM PUBLIC;

CREATE TRIGGER trg_resource_dependency_acyclic
BEFORE INSERT OR UPDATE OF
    parent_governed_resource_id,
    child_governed_resource_id
ON governance.resource_dependency
FOR EACH ROW
EXECUTE FUNCTION governance.enforce_resource_dependency_acyclic();

CREATE FUNCTION result.enforce_batch_supersession()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
DECLARE
    old_period_id uuid;
    new_period_id uuid;
    old_edge_source uuid;
    old_edge_target uuid;
BEGIN
    PERFORM pg_catalog.pg_advisory_xact_lock(
        pg_catalog.hashtextextended(
            'result.batch_supersession.acyclic'::text,
            0));

    SELECT old_batch.measurement_period_id,
           new_batch.measurement_period_id
    INTO old_period_id, new_period_id
    FROM result.result_batch AS old_batch
    CROSS JOIN result.result_batch AS new_batch
    WHERE old_batch.id = NEW.old_batch_id
      AND new_batch.id = NEW.new_batch_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Both ResultBatch rows must exist before supersession.'
            USING ERRCODE = '23503';
    END IF;

    IF old_period_id IS DISTINCT FROM new_period_id THEN
        RAISE EXCEPTION
            'ResultBatch supersession must stay within one MeasurementPeriod.'
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_batch_supersession_same_period';
    END IF;

    IF TG_OP = 'UPDATE' THEN
        old_edge_source := OLD.old_batch_id;
        old_edge_target := OLD.new_batch_id;
    END IF;

    IF NEW.old_batch_id = NEW.new_batch_id
       OR EXISTS (
            WITH RECURSIVE reachable(id) AS (
                SELECT NEW.new_batch_id

                UNION

                SELECT supersession.new_batch_id
                FROM result.batch_supersession AS supersession
                INNER JOIN reachable
                    ON reachable.id = supersession.old_batch_id
                WHERE old_edge_source IS NULL
                   OR ROW(
                        supersession.old_batch_id,
                        supersession.new_batch_id)
                      IS DISTINCT FROM
                      ROW(old_edge_source, old_edge_target))
            SELECT 1
            FROM reachable
            WHERE id = NEW.old_batch_id) THEN
        RAISE EXCEPTION
            'ResultBatch supersession % -> % creates a cycle.',
            NEW.old_batch_id,
            NEW.new_batch_id
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_batch_supersession_acyclic';
    END IF;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION result.enforce_batch_supersession()
    FROM PUBLIC;

CREATE TRIGGER trg_batch_supersession_guard
BEFORE INSERT OR UPDATE OF old_batch_id, new_batch_id
ON result.batch_supersession
FOR EACH ROW
EXECUTE FUNCTION result.enforce_batch_supersession();

CREATE FUNCTION result.validate_batch_supersession_same_period()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
DECLARE
    invalid_old_batch_id uuid;
    invalid_new_batch_id uuid;
BEGIN
    SELECT supersession.old_batch_id, supersession.new_batch_id
    INTO invalid_old_batch_id, invalid_new_batch_id
    FROM result.batch_supersession AS supersession
    INNER JOIN result.result_batch AS old_batch
        ON old_batch.id = supersession.old_batch_id
    INNER JOIN result.result_batch AS new_batch
        ON new_batch.id = supersession.new_batch_id
    WHERE old_batch.measurement_period_id
        IS DISTINCT FROM new_batch.measurement_period_id
    LIMIT 1;

    IF FOUND THEN
        RAISE EXCEPTION
            'ResultBatch supersession % -> % crosses MeasurementPeriods.',
            invalid_old_batch_id,
            invalid_new_batch_id
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_batch_supersession_same_period';
    END IF;

    RETURN NULL;
END;
$function$;

REVOKE ALL ON FUNCTION result.validate_batch_supersession_same_period()
    FROM PUBLIC;

CREATE CONSTRAINT TRIGGER ctrg_result_batch_supersession_same_period
AFTER UPDATE OF measurement_period_id ON result.result_batch
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION result.validate_batch_supersession_same_period();

CREATE FUNCTION result.validate_current_publication_not_revoked()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
DECLARE
    invalid_measurement_period_id uuid;
    invalid_publication_id uuid;
BEGIN
    SELECT current_publication.measurement_period_id,
           current_publication.publication_id
    INTO invalid_measurement_period_id, invalid_publication_id
    FROM result.current_publication AS current_publication
    INNER JOIN result.publication_revocation AS revocation
        ON revocation.publication_id = current_publication.publication_id
    LIMIT 1;

    IF FOUND THEN
        RAISE EXCEPTION
            'CurrentPublication for MeasurementPeriod % points to revoked Publication %.',
            invalid_measurement_period_id,
            invalid_publication_id
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_current_publication_not_revoked';
    END IF;

    RETURN NULL;
END;
$function$;

REVOKE ALL ON FUNCTION result.validate_current_publication_not_revoked()
    FROM PUBLIC;

CREATE CONSTRAINT TRIGGER ctrg_current_publication_not_revoked
AFTER INSERT OR UPDATE ON result.current_publication
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION result.validate_current_publication_not_revoked();

CREATE CONSTRAINT TRIGGER ctrg_publication_revocation_not_current
AFTER INSERT OR UPDATE OF publication_id ON result.publication_revocation
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION result.validate_current_publication_not_revoked();

CREATE FUNCTION integration.enforce_validation_issue_locator()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
DECLARE
    locator_exists boolean;
BEGIN
    IF NEW.raw_record_id IS NOT NULL
       AND NOT EXISTS (
            SELECT 1
            FROM integration.raw_record AS raw_record
            WHERE raw_record.id = NEW.raw_record_id
              AND raw_record.ingestion_batch_id = NEW.ingestion_batch_id) THEN
        RAISE EXCEPTION
            'ValidationIssue RawRecord must belong to the same IngestionBatch.'
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_validation_issue_raw_record_batch';
    END IF;

    IF pg_catalog.num_nonnulls(NEW.staging_table, NEW.staging_row_id)
        NOT IN (0, 2) THEN
        RAISE EXCEPTION 'ValidationIssue staging locator must be complete.'
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_validation_issue_staging_locator';
    END IF;

    IF NEW.staging_table IS NULL THEN
        RETURN NEW;
    END IF;

    IF NEW.raw_record_id IS NULL THEN
        RAISE EXCEPTION 'A staging locator requires a RawRecord locator.'
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_validation_issue_staging_requires_raw';
    END IF;

    locator_exists := CASE NEW.staging_table
        WHEN 'staging_student' THEN EXISTS (
            SELECT 1
            FROM integration.staging_student AS staging
            WHERE staging.id = NEW.staging_row_id
              AND staging.ingestion_batch_id = NEW.ingestion_batch_id
              AND staging.raw_record_id = NEW.raw_record_id)
        WHEN 'staging_course_offering' THEN EXISTS (
            SELECT 1
            FROM integration.staging_course_offering AS staging
            WHERE staging.id = NEW.staging_row_id
              AND staging.ingestion_batch_id = NEW.ingestion_batch_id
              AND staging.raw_record_id = NEW.raw_record_id)
        WHEN 'staging_enrollment' THEN EXISTS (
            SELECT 1
            FROM integration.staging_enrollment AS staging
            WHERE staging.id = NEW.staging_row_id
              AND staging.ingestion_batch_id = NEW.ingestion_batch_id
              AND staging.raw_record_id = NEW.raw_record_id)
        WHEN 'staging_score' THEN EXISTS (
            SELECT 1
            FROM integration.staging_score AS staging
            WHERE staging.id = NEW.staging_row_id
              AND staging.ingestion_batch_id = NEW.ingestion_batch_id
              AND staging.raw_record_id = NEW.raw_record_id)
        WHEN 'staging_course_pi_mapping' THEN EXISTS (
            SELECT 1
            FROM integration.staging_course_pi_mapping AS staging
            WHERE staging.id = NEW.staging_row_id
              AND staging.ingestion_batch_id = NEW.ingestion_batch_id
              AND staging.raw_record_id = NEW.raw_record_id)
        WHEN 'staging_direct_measurement_plan' THEN EXISTS (
            SELECT 1
            FROM integration.staging_direct_measurement_plan AS staging
            WHERE staging.id = NEW.staging_row_id
              AND staging.ingestion_batch_id = NEW.ingestion_batch_id
              AND staging.raw_record_id = NEW.raw_record_id)
        WHEN 'staging_rubric_criterion' THEN EXISTS (
            SELECT 1
            FROM integration.staging_rubric_criterion AS staging
            WHERE staging.id = NEW.staging_row_id
              AND staging.ingestion_batch_id = NEW.ingestion_batch_id
              AND staging.raw_record_id = NEW.raw_record_id)
        ELSE false
    END;

    IF NOT locator_exists THEN
        RAISE EXCEPTION
            'ValidationIssue staging locator is unknown or does not match its batch/raw record.'
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_validation_issue_typed_locator';
    END IF;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION integration.enforce_validation_issue_locator()
    FROM PUBLIC;

CREATE TRIGGER trg_validation_issue_typed_locator
BEFORE INSERT OR UPDATE OF
    ingestion_batch_id,
    raw_record_id,
    staging_table,
    staging_row_id
ON integration.validation_issue
FOR EACH ROW
EXECUTE FUNCTION integration.enforce_validation_issue_locator();

CREATE FUNCTION integration.guard_referenced_staging_locator()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog
AS $function$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM integration.validation_issue AS validation_issue
        WHERE validation_issue.staging_table = TG_TABLE_NAME
          AND validation_issue.staging_row_id = OLD.id)
       AND (
            TG_OP = 'DELETE'
            OR ROW(NEW.id, NEW.ingestion_batch_id, NEW.raw_record_id)
               IS DISTINCT FROM
               ROW(OLD.id, OLD.ingestion_batch_id, OLD.raw_record_id)) THEN
        RAISE EXCEPTION
            'Referenced staging locator %.% is immutable.',
            TG_TABLE_NAME,
            OLD.id
            USING ERRCODE = '23514',
                  CONSTRAINT = 'ck_validation_issue_staging_reference';
    END IF;

    IF TG_OP = 'DELETE' THEN
        RETURN OLD;
    END IF;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION integration.guard_referenced_staging_locator()
    FROM PUBLIC;

CREATE TRIGGER trg_staging_student_validation_issue_locator
BEFORE UPDATE OF id, ingestion_batch_id, raw_record_id OR DELETE
ON integration.staging_student
FOR EACH ROW
EXECUTE FUNCTION integration.guard_referenced_staging_locator();

CREATE TRIGGER trg_staging_course_offering_validation_issue_locator
BEFORE UPDATE OF id, ingestion_batch_id, raw_record_id OR DELETE
ON integration.staging_course_offering
FOR EACH ROW
EXECUTE FUNCTION integration.guard_referenced_staging_locator();

CREATE TRIGGER trg_staging_enrollment_validation_issue_locator
BEFORE UPDATE OF id, ingestion_batch_id, raw_record_id OR DELETE
ON integration.staging_enrollment
FOR EACH ROW
EXECUTE FUNCTION integration.guard_referenced_staging_locator();

CREATE TRIGGER trg_staging_score_validation_issue_locator
BEFORE UPDATE OF id, ingestion_batch_id, raw_record_id OR DELETE
ON integration.staging_score
FOR EACH ROW
EXECUTE FUNCTION integration.guard_referenced_staging_locator();

CREATE TRIGGER trg_staging_course_pi_mapping_validation_issue_locator
BEFORE UPDATE OF id, ingestion_batch_id, raw_record_id OR DELETE
ON integration.staging_course_pi_mapping
FOR EACH ROW
EXECUTE FUNCTION integration.guard_referenced_staging_locator();

CREATE TRIGGER trg_staging_direct_measurement_plan_validation_issue_locator
BEFORE UPDATE OF id, ingestion_batch_id, raw_record_id OR DELETE
ON integration.staging_direct_measurement_plan
FOR EACH ROW
EXECUTE FUNCTION integration.guard_referenced_staging_locator();

CREATE TRIGGER trg_staging_rubric_criterion_validation_issue_locator
BEFORE UPDATE OF id, ingestion_batch_id, raw_record_id OR DELETE
ON integration.staging_rubric_criterion
FOR EACH ROW
EXECUTE FUNCTION integration.guard_referenced_staging_locator();
