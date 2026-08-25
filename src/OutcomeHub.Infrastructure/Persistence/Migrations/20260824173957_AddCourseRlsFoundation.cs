using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutcomeHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseRlsFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                GRANT USAGE ON SCHEMA iam, academic TO outcomehub_authorizer;
                REVOKE CREATE ON SCHEMA iam, academic FROM outcomehub_authorizer;

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
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                REVOKE SELECT, INSERT, UPDATE, DELETE ON TABLE academic.course
                    FROM outcomehub_app;

                REVOKE EXECUTE ON FUNCTION iam.has_permission(
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
                    FROM outcomehub_app;
                REVOKE EXECUTE ON FUNCTION iam.current_context_uuid(text)
                    FROM outcomehub_app;

                DROP POLICY IF EXISTS course_delete_policy ON academic.course;
                DROP POLICY IF EXISTS course_update_policy ON academic.course;
                DROP POLICY IF EXISTS course_insert_policy ON academic.course;
                DROP POLICY IF EXISTS course_select_policy ON academic.course;

                ALTER TABLE academic.course NO FORCE ROW LEVEL SECURITY;
                ALTER TABLE academic.course DISABLE ROW LEVEL SECURITY;

                DROP FUNCTION iam.has_permission(
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
                    text);
                DROP FUNCTION iam.current_context_uuid(text);

                DELETE FROM iam.permission
                WHERE id IN (
                    '10000000-0000-7000-8000-000000000001',
                    '10000000-0000-7000-8000-000000000002',
                    '10000000-0000-7000-8000-000000000003',
                    '10000000-0000-7000-8000-000000000004');

                REVOKE SELECT ON TABLE
                    iam.principal,
                    iam.role,
                    iam.role_version,
                    iam.role_version_permission,
                    iam.permission,
                    iam.role_assignment,
                    iam.access_scope,
                    academic.org_unit
                FROM outcomehub_authorizer;

                REVOKE USAGE ON SCHEMA iam, academic FROM outcomehub_authorizer;
                REVOKE USAGE ON SCHEMA iam, academic FROM outcomehub_app;
                """);
        }
    }
}
