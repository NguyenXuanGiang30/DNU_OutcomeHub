INSERT INTO iam.permission (
    id,
    resource_type,
    action,
    field_scope,
    description)
VALUES (
    '10000000-0000-7000-8000-000000000034',
    'measurement.score_record',
    'READ',
    '*',
    'Read source score revisions inside the assigned organization, program, or course scope.');

ALTER TABLE measurement.score_record ENABLE ROW LEVEL SECURITY;
ALTER TABLE measurement.score_record FORCE ROW LEVEL SECURITY;

CREATE POLICY score_record_select_policy
ON measurement.score_record
FOR SELECT
TO outcomehub_app
USING (
    iam.has_permission(
        'measurement.score_record',
        'READ',
        '*',
        org_unit_id,
        program_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        course_id,
        NULL::uuid,
        NULL::uuid,
        NULL::uuid,
        NULL::text));

REVOKE ALL PRIVILEGES ON TABLE measurement.score_record
FROM PUBLIC, outcomehub_app;

GRANT SELECT ON TABLE measurement.score_record
TO outcomehub_app;
