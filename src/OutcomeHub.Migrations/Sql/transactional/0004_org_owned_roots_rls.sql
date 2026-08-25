GRANT CREATE ON SCHEMA iam TO outcomehub_authorizer;

SET LOCAL ROLE outcomehub_authorizer;

CREATE OR REPLACE FUNCTION iam.has_permission(
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
SET search_path = pg_catalog, iam, pg_temp
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
                      access_scope.scope_type = 'PROGRAM'
                      AND target_program_id IS NOT NULL
                      AND access_scope.program_id = target_program_id)
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
                                      WHERE NOT parent.id = ANY(ancestor.visited))
                                  SELECT 1
                                  FROM org_ancestors AS ancestor
                                  WHERE ancestor.id = access_scope.org_unit_id))))));
$function$;

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

RESET ROLE;

REVOKE CREATE ON SCHEMA iam FROM outcomehub_authorizer;

CREATE FUNCTION iam.reject_direct_scope_anchor_change()
RETURNS trigger
LANGUAGE plpgsql
SECURITY INVOKER
SET search_path = pg_catalog, pg_temp
AS $function$
DECLARE
    anchor_column text;
    old_value jsonb;
    new_value jsonb;
BEGIN
    IF CURRENT_USER <> 'outcomehub_app' THEN
        RETURN NEW;
    END IF;

    FOREACH anchor_column IN ARRAY TG_ARGV
    LOOP
        old_value := pg_catalog.to_jsonb(OLD) -> anchor_column;
        new_value := pg_catalog.to_jsonb(NEW) -> anchor_column;

        IF old_value IS DISTINCT FROM new_value THEN
            RAISE EXCEPTION
                'Direct scope-anchor change is not allowed for %.% column %.',
                TG_TABLE_SCHEMA,
                TG_TABLE_NAME,
                anchor_column
                USING ERRCODE = '42501';
        END IF;
    END LOOP;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION iam.reject_direct_scope_anchor_change()
FROM PUBLIC;

INSERT INTO iam.permission (
    id,
    resource_type,
    action,
    field_scope,
    description)
SELECT seed.id, seed.resource_type, seed.action, '*', seed.description
FROM (VALUES
    ('10000000-0000-7000-8000-000000000005'::uuid, 'academic.org_unit', 'READ', 'Read organization units'),
    ('10000000-0000-7000-8000-000000000006'::uuid, 'academic.institution_template', 'READ', 'Read institution templates'),
    ('10000000-0000-7000-8000-000000000007'::uuid, 'academic.institution_template', 'CREATE', 'Create institution templates'),
    ('10000000-0000-7000-8000-000000000008'::uuid, 'academic.institution_template', 'UPDATE', 'Update institution templates'),
    ('10000000-0000-7000-8000-000000000009'::uuid, 'academic.institution_template', 'DELETE', 'Delete institution templates'),
    ('10000000-0000-7000-8000-000000000010'::uuid, 'academic.program', 'READ', 'Read programs'),
    ('10000000-0000-7000-8000-000000000011'::uuid, 'academic.program', 'CREATE', 'Create draft programs'),
    ('10000000-0000-7000-8000-000000000012'::uuid, 'academic.program', 'UPDATE', 'Update draft programs'),
    ('10000000-0000-7000-8000-000000000013'::uuid, 'academic.program', 'DELETE', 'Delete draft programs'),
    ('10000000-0000-7000-8000-000000000014'::uuid, 'portfolio.syllabus_template', 'READ', 'Read syllabus templates'),
    ('10000000-0000-7000-8000-000000000015'::uuid, 'portfolio.syllabus_template', 'CREATE', 'Create syllabus templates'),
    ('10000000-0000-7000-8000-000000000016'::uuid, 'portfolio.syllabus_template', 'UPDATE', 'Update syllabus templates'),
    ('10000000-0000-7000-8000-000000000017'::uuid, 'portfolio.syllabus_template', 'DELETE', 'Delete syllabus templates'),
    ('10000000-0000-7000-8000-000000000018'::uuid, 'portfolio.shared_syllabus_core', 'READ', 'Read shared syllabus cores'),
    ('10000000-0000-7000-8000-000000000019'::uuid, 'portfolio.shared_syllabus_core', 'CREATE', 'Create shared syllabus cores'),
    ('10000000-0000-7000-8000-000000000020'::uuid, 'portfolio.shared_syllabus_core', 'UPDATE', 'Update shared syllabus cores'),
    ('10000000-0000-7000-8000-000000000021'::uuid, 'portfolio.shared_syllabus_core', 'DELETE', 'Delete shared syllabus cores'),
    ('10000000-0000-7000-8000-000000000022'::uuid, 'integration.source_system', 'READ', 'Read source systems'),
    ('10000000-0000-7000-8000-000000000023'::uuid, 'integration.source_system', 'CREATE', 'Create source systems'),
    ('10000000-0000-7000-8000-000000000024'::uuid, 'integration.source_system', 'UPDATE', 'Update source systems'),
    ('10000000-0000-7000-8000-000000000025'::uuid, 'integration.source_system', 'DELETE', 'Delete source systems'),
    ('10000000-0000-7000-8000-000000000026'::uuid, 'measurement.calculation_policy', 'READ', 'Read calculation policies'),
    ('10000000-0000-7000-8000-000000000027'::uuid, 'measurement.calculation_policy', 'CREATE', 'Create calculation policies'),
    ('10000000-0000-7000-8000-000000000028'::uuid, 'measurement.calculation_policy', 'UPDATE', 'Update calculation policies'),
    ('10000000-0000-7000-8000-000000000029'::uuid, 'measurement.calculation_policy', 'DELETE', 'Delete calculation policies'),
    ('10000000-0000-7000-8000-000000000030'::uuid, 'measurement.indirect_instrument', 'READ', 'Read indirect instruments'),
    ('10000000-0000-7000-8000-000000000031'::uuid, 'measurement.indirect_instrument', 'CREATE', 'Create indirect instruments'),
    ('10000000-0000-7000-8000-000000000032'::uuid, 'measurement.indirect_instrument', 'UPDATE', 'Update indirect instruments'),
    ('10000000-0000-7000-8000-000000000033'::uuid, 'measurement.indirect_instrument', 'DELETE', 'Delete indirect instruments'))
    AS seed(id, resource_type, action, description)
ON CONFLICT (id) DO NOTHING;

ALTER TABLE academic.org_unit ENABLE ROW LEVEL SECURITY;
ALTER TABLE academic.org_unit FORCE ROW LEVEL SECURITY;

CREATE POLICY org_unit_authorizer_lookup_policy
ON academic.org_unit
FOR SELECT
TO outcomehub_authorizer
USING (true);

CREATE POLICY org_unit_select_policy
ON academic.org_unit
FOR SELECT
TO outcomehub_app
USING (
    iam.has_permission(
        'academic.org_unit',
        'READ',
        '*',
        id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

DO $migration$
DECLARE
    target record;
    table_reference text;
    permission_predicate text;
    write_guard text;
BEGIN
    FOR target IN
        SELECT *
        FROM (VALUES
            ('academic', 'institution_template', 'academic.institution_template', 'owner_org_unit_id', 'NULL::uuid', 'NULL::uuid', 'true'),
            ('academic', 'program', 'academic.program', 'owner_org_unit_id', 'id', 'NULL::uuid', 'status = ''DRAFT'''),
            ('portfolio', 'syllabus_template', 'portfolio.syllabus_template', 'owner_org_unit_id', 'NULL::uuid', 'NULL::uuid', 'true'),
            ('portfolio', 'shared_syllabus_core', 'portfolio.shared_syllabus_core', 'owner_org_unit_id', 'NULL::uuid', 'course_id', 'true'),
            ('integration', 'source_system', 'integration.source_system', 'owner_org_unit_id', 'NULL::uuid', 'NULL::uuid', 'true'),
            ('measurement', 'calculation_policy', 'measurement.calculation_policy', 'owner_org_unit_id', 'NULL::uuid', 'NULL::uuid', 'true'),
            ('measurement', 'indirect_instrument', 'measurement.indirect_instrument', 'owner_org_unit_id', 'NULL::uuid', 'NULL::uuid', 'true'))
            AS policy_target(
                schema_name,
                table_name,
                resource_type,
                org_expression,
                program_expression,
                course_expression,
                write_guard)
    LOOP
        table_reference := pg_catalog.format(
            '%I.%I',
            target.schema_name,
            target.table_name);
        write_guard := target.write_guard;

        EXECUTE pg_catalog.format(
            'ALTER TABLE %s ENABLE ROW LEVEL SECURITY',
            table_reference);
        EXECUTE pg_catalog.format(
            'ALTER TABLE %s FORCE ROW LEVEL SECURITY',
            table_reference);

        permission_predicate := pg_catalog.format(
            'iam.has_permission(%L, %%L, ''*'', %s, %s, '
            'NULL::uuid, NULL::uuid, NULL::uuid, %s, '
            'NULL::uuid, NULL::uuid, NULL::uuid, NULL::text)',
            target.resource_type,
            target.org_expression,
            target.program_expression,
            target.course_expression);

        EXECUTE pg_catalog.format(
            'CREATE POLICY %I ON %s FOR SELECT TO outcomehub_app '
            'USING (%s)',
            target.table_name || '_select_policy',
            table_reference,
            pg_catalog.format(permission_predicate, 'READ'));
        EXECUTE pg_catalog.format(
            'CREATE POLICY %I ON %s FOR INSERT TO outcomehub_app '
            'WITH CHECK ((%s) AND %s)',
            target.table_name || '_insert_policy',
            table_reference,
            write_guard,
            pg_catalog.format(permission_predicate, 'CREATE'));
        EXECUTE pg_catalog.format(
            'CREATE POLICY %I ON %s FOR UPDATE TO outcomehub_app '
            'USING ((%s) AND %s) WITH CHECK ((%s) AND %s)',
            target.table_name || '_update_policy',
            table_reference,
            write_guard,
            pg_catalog.format(permission_predicate, 'UPDATE'),
            write_guard,
            pg_catalog.format(permission_predicate, 'UPDATE'));
        EXECUTE pg_catalog.format(
            'CREATE POLICY %I ON %s FOR DELETE TO outcomehub_app '
            'USING ((%s) AND %s)',
            target.table_name || '_delete_policy',
            table_reference,
            write_guard,
            pg_catalog.format(permission_predicate, 'DELETE'));
    END LOOP;
END;
$migration$;

CREATE TRIGGER trg_course_reject_direct_scope_anchor_change
BEFORE UPDATE OF owner_org_unit_id
ON academic.course
FOR EACH ROW
EXECUTE FUNCTION iam.reject_direct_scope_anchor_change('owner_org_unit_id');

CREATE TRIGGER trg_institution_template_reject_direct_scope_anchor_change
BEFORE UPDATE OF owner_org_unit_id
ON academic.institution_template
FOR EACH ROW
EXECUTE FUNCTION iam.reject_direct_scope_anchor_change('owner_org_unit_id');

CREATE TRIGGER trg_program_reject_direct_scope_anchor_change
BEFORE UPDATE OF owner_org_unit_id
ON academic.program
FOR EACH ROW
EXECUTE FUNCTION iam.reject_direct_scope_anchor_change('owner_org_unit_id');

CREATE TRIGGER trg_syllabus_template_reject_direct_scope_anchor_change
BEFORE UPDATE OF owner_org_unit_id
ON portfolio.syllabus_template
FOR EACH ROW
EXECUTE FUNCTION iam.reject_direct_scope_anchor_change('owner_org_unit_id');

CREATE TRIGGER trg_shared_syllabus_core_reject_direct_scope_anchor_change
BEFORE UPDATE OF owner_org_unit_id, course_id
ON portfolio.shared_syllabus_core
FOR EACH ROW
EXECUTE FUNCTION iam.reject_direct_scope_anchor_change(
    'owner_org_unit_id',
    'course_id');

CREATE TRIGGER trg_source_system_reject_direct_scope_anchor_change
BEFORE UPDATE OF owner_org_unit_id
ON integration.source_system
FOR EACH ROW
EXECUTE FUNCTION iam.reject_direct_scope_anchor_change('owner_org_unit_id');

CREATE TRIGGER trg_calculation_policy_reject_direct_scope_anchor_change
BEFORE UPDATE OF owner_org_unit_id
ON measurement.calculation_policy
FOR EACH ROW
EXECUTE FUNCTION iam.reject_direct_scope_anchor_change('owner_org_unit_id');

CREATE TRIGGER trg_indirect_instrument_reject_direct_scope_anchor_change
BEFORE UPDATE OF owner_org_unit_id
ON measurement.indirect_instrument
FOR EACH ROW
EXECUTE FUNCTION iam.reject_direct_scope_anchor_change('owner_org_unit_id');

GRANT USAGE ON SCHEMA portfolio, integration, measurement
TO outcomehub_app;

REVOKE CREATE ON SCHEMA academic, portfolio, integration, measurement
FROM outcomehub_app;

REVOKE ALL PRIVILEGES ON TABLE
    academic.org_unit,
    academic.institution_template,
    academic.program,
    portfolio.syllabus_template,
    portfolio.shared_syllabus_core,
    integration.source_system,
    measurement.calculation_policy,
    measurement.indirect_instrument
FROM PUBLIC, outcomehub_app;

GRANT SELECT ON TABLE academic.org_unit
TO outcomehub_app;

GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE
    academic.institution_template,
    academic.program,
    portfolio.syllabus_template,
    portfolio.shared_syllabus_core,
    integration.source_system,
    measurement.calculation_policy,
    measurement.indirect_instrument
TO outcomehub_app;
