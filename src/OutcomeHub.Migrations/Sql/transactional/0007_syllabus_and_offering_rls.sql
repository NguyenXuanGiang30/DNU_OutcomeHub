GRANT CREATE ON SCHEMA iam TO outcomehub_authorizer;
GRANT SELECT ON TABLE iam.user_account TO outcomehub_authorizer;

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
                      access_scope.scope_type = 'PROGRAM_VERSION'
                      AND target_program_version_id IS NOT NULL
                      AND access_scope.program_version_id = target_program_version_id)
                  OR (
                      access_scope.scope_type = 'COURSE'
                      AND target_course_id IS NOT NULL
                      AND access_scope.course_id = target_course_id)
                  OR (
                      access_scope.scope_type = 'OFFERING'
                      AND target_course_offering_id IS NOT NULL
                      AND access_scope.course_offering_id = target_course_offering_id)
                  OR (
                      access_scope.scope_type = 'COHORT'
                      AND target_cohort_id IS NOT NULL
                      AND access_scope.cohort_id = target_cohort_id)
                  OR (
                      access_scope.scope_type = 'CURRICULUM_PATH'
                      AND target_curriculum_path_id IS NOT NULL
                      AND access_scope.curriculum_path_id = target_curriculum_path_id)
                  OR (
                      access_scope.scope_type = 'MEASUREMENT_PERIOD'
                      AND target_measurement_period_id IS NOT NULL
                      AND access_scope.measurement_period_id = target_measurement_period_id)
                  OR (
                      access_scope.scope_type = 'SELF'
                      AND target_student_id IS NOT NULL
                      AND access_scope.subject_principal_id = principal.id
                      AND EXISTS (
                          SELECT 1
                          FROM iam.user_account AS user_acc
                          WHERE user_acc.principal_id = principal.id
                            AND user_acc.person_id = target_student_id))
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

RESET ROLE;

REVOKE CREATE ON SCHEMA iam FROM outcomehub_authorizer;

INSERT INTO iam.permission (
    id,
    resource_type,
    action,
    field_scope,
    description)
SELECT seed.id, seed.resource_type, seed.action, '*', seed.description
FROM (VALUES
    ('10000000-0000-7000-8000-000000000035'::uuid, 'portfolio.syllabus', 'READ', 'Read syllabi'),
    ('10000000-0000-7000-8000-000000000036'::uuid, 'portfolio.syllabus', 'CREATE', 'Create syllabi'),
    ('10000000-0000-7000-8000-000000000037'::uuid, 'portfolio.syllabus', 'UPDATE', 'Update syllabi'),
    ('10000000-0000-7000-8000-000000000038'::uuid, 'portfolio.syllabus', 'DELETE', 'Delete syllabi'),
    ('10000000-0000-7000-8000-000000000039'::uuid, 'portfolio.syllabus_version', 'READ', 'Read syllabus versions'),
    ('10000000-0000-7000-8000-00000000003a'::uuid, 'portfolio.syllabus_version', 'CREATE', 'Create syllabus versions'),
    ('10000000-0000-7000-8000-00000000003b'::uuid, 'portfolio.syllabus_version', 'UPDATE', 'Update syllabus versions'),
    ('10000000-0000-7000-8000-00000000003c'::uuid, 'academic.course_offering', 'READ', 'Read course offerings'),
    ('10000000-0000-7000-8000-00000000003d'::uuid, 'academic.course_offering', 'CREATE', 'Create course offerings'),
    ('10000000-0000-7000-8000-00000000003e'::uuid, 'academic.course_offering', 'UPDATE', 'Update course offerings'),
    ('10000000-0000-7000-8000-00000000003f'::uuid, 'measurement.enrollment', 'READ', 'Read student course enrollments'))
    AS seed(id, resource_type, action, description)
ON CONFLICT (id) DO NOTHING;

-- RLS: portfolio.syllabus
ALTER TABLE portfolio.syllabus ENABLE ROW LEVEL SECURITY;
ALTER TABLE portfolio.syllabus FORCE ROW LEVEL SECURITY;

CREATE POLICY syllabus_select_policy
ON portfolio.syllabus
FOR SELECT
TO outcomehub_app
USING (
    iam.has_permission(
        'portfolio.syllabus',
        'READ',
        '*',
        owner_org_unit_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

CREATE POLICY syllabus_insert_policy
ON portfolio.syllabus
FOR INSERT
TO outcomehub_app
WITH CHECK (
    iam.has_permission(
        'portfolio.syllabus',
        'CREATE',
        '*',
        owner_org_unit_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

CREATE POLICY syllabus_update_policy
ON portfolio.syllabus
FOR UPDATE
TO outcomehub_app
USING (
    iam.has_permission(
        'portfolio.syllabus',
        'UPDATE',
        '*',
        owner_org_unit_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text))
WITH CHECK (
    iam.has_permission(
        'portfolio.syllabus',
        'UPDATE',
        '*',
        owner_org_unit_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

CREATE POLICY syllabus_delete_policy
ON portfolio.syllabus
FOR DELETE
TO outcomehub_app
USING (
    iam.has_permission(
        'portfolio.syllabus',
        'DELETE',
        '*',
        owner_org_unit_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

CREATE POLICY syllabus_migrator_policy
ON portfolio.syllabus
FOR ALL
TO outcomehub_migrator
USING (true)
WITH CHECK (true);

CREATE TRIGGER trg_syllabus_reject_direct_scope_anchor_change
BEFORE UPDATE ON portfolio.syllabus
FOR EACH ROW
EXECUTE FUNCTION iam.reject_direct_scope_anchor_change('owner_org_unit_id');

-- RLS: portfolio.syllabus_version
ALTER TABLE portfolio.syllabus_version ENABLE ROW LEVEL SECURITY;
ALTER TABLE portfolio.syllabus_version FORCE ROW LEVEL SECURITY;

CREATE POLICY syllabus_version_migrator_policy
ON portfolio.syllabus_version
FOR ALL
TO outcomehub_migrator
USING (true)
WITH CHECK (true);

CREATE POLICY syllabus_version_select_policy
ON portfolio.syllabus_version
FOR SELECT
TO outcomehub_app
USING (
    iam.has_permission(
        'portfolio.syllabus_version',
        'READ',
        '*',
        NULL::uuid,
        NULL::uuid,
        program_version_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

CREATE POLICY syllabus_version_insert_policy
ON portfolio.syllabus_version
FOR INSERT
TO outcomehub_app
WITH CHECK (
    iam.has_permission(
        'portfolio.syllabus_version',
        'CREATE',
        '*',
        NULL::uuid,
        NULL::uuid,
        program_version_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

CREATE POLICY syllabus_version_update_policy
ON portfolio.syllabus_version
FOR UPDATE
TO outcomehub_app
USING (
    iam.has_permission(
        'portfolio.syllabus_version',
        'UPDATE',
        '*',
        NULL::uuid,
        NULL::uuid,
        program_version_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text))
WITH CHECK (
    iam.has_permission(
        'portfolio.syllabus_version',
        'UPDATE',
        '*',
        NULL::uuid,
        NULL::uuid,
        program_version_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

-- RLS: academic.course_offering
ALTER TABLE academic.course_offering ENABLE ROW LEVEL SECURITY;
ALTER TABLE academic.course_offering FORCE ROW LEVEL SECURITY;

CREATE POLICY course_offering_migrator_policy
ON academic.course_offering
FOR ALL
TO outcomehub_migrator
USING (true)
WITH CHECK (true);

CREATE POLICY course_offering_select_policy
ON academic.course_offering
FOR SELECT
TO outcomehub_app
USING (
    iam.has_permission(
        'academic.course_offering',
        'READ',
        '*',
        org_unit_id,
        NULL::uuid,
        program_version_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        id,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

CREATE POLICY course_offering_insert_policy
ON academic.course_offering
FOR INSERT
TO outcomehub_app
WITH CHECK (
    iam.has_permission(
        'academic.course_offering',
        'CREATE',
        '*',
        org_unit_id,
        NULL::uuid,
        program_version_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        id,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

CREATE POLICY course_offering_update_policy
ON academic.course_offering
FOR UPDATE
TO outcomehub_app
USING (
    iam.has_permission(
        'academic.course_offering',
        'UPDATE',
        '*',
        org_unit_id,
        NULL::uuid,
        program_version_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        id,
        NULL::uuid,
        NULL::uuid,
        NULL::text))
WITH CHECK (
    iam.has_permission(
        'academic.course_offering',
        'UPDATE',
        '*',
        org_unit_id,
        NULL::uuid,
        program_version_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        id,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

CREATE TRIGGER trg_course_offering_reject_direct_scope_anchor_change
BEFORE UPDATE ON academic.course_offering
FOR EACH ROW
EXECUTE FUNCTION iam.reject_direct_scope_anchor_change('org_unit_id', 'program_version_id');

-- RLS: measurement.enrollment
ALTER TABLE measurement.enrollment ENABLE ROW LEVEL SECURITY;
ALTER TABLE measurement.enrollment FORCE ROW LEVEL SECURITY;

CREATE POLICY enrollment_migrator_policy
ON measurement.enrollment
FOR ALL
TO outcomehub_migrator
USING (true)
WITH CHECK (true);

CREATE POLICY enrollment_select_policy
ON measurement.enrollment
FOR SELECT
TO outcomehub_app
USING (
    iam.has_permission(
        'measurement.enrollment',
        'READ',
        '*',
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        course_offering_id,
        NULL::uuid,
        student_id,
        NULL::text));

-- Privileges
REVOKE ALL PRIVILEGES ON TABLE portfolio.syllabus, portfolio.syllabus_version, academic.course_offering, measurement.enrollment
FROM PUBLIC, outcomehub_app;

GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE portfolio.syllabus
TO outcomehub_app;

GRANT SELECT, INSERT, UPDATE ON TABLE portfolio.syllabus_version, academic.course_offering
TO outcomehub_app;

GRANT SELECT ON TABLE measurement.enrollment
TO outcomehub_app;
