SELECT
    pg_catalog.pg_get_functiondef(
        'iam.has_permission(text,text,text,uuid,uuid,uuid,uuid,uuid,uuid,uuid,uuid,uuid,text)'::regprocedure)
        LIKE '%access_scope.scope_type = ''PROGRAM''%'
    AND EXISTS (
        SELECT 1
        FROM pg_catalog.pg_proc AS function_metadata
        INNER JOIN pg_catalog.pg_roles AS owner_role
            ON owner_role.oid = function_metadata.proowner
        WHERE function_metadata.oid =
            'iam.has_permission(text,text,text,uuid,uuid,uuid,uuid,uuid,uuid,uuid,uuid,uuid,text)'::regprocedure
          AND function_metadata.prosecdef
          AND owner_role.rolname = 'outcomehub_authorizer'
          AND function_metadata.proconfig =
              ARRAY['search_path=pg_catalog, iam, pg_temp']::text[]
          AND pg_catalog.has_function_privilege(
              'outcomehub_app',
              function_metadata.oid,
              'EXECUTE')
          AND NOT EXISTS (
              SELECT 1
              FROM pg_catalog.aclexplode(
                  COALESCE(
                      function_metadata.proacl,
                      pg_catalog.acldefault(
                          'f'::"char",
                          function_metadata.proowner))) AS function_acl
              WHERE function_acl.grantee = 0
                AND function_acl.privilege_type = 'EXECUTE'))
    AND EXISTS (
        SELECT 1
        FROM pg_catalog.pg_proc AS function_metadata
        WHERE function_metadata.oid =
            'iam.reject_direct_scope_anchor_change()'::regprocedure
          AND NOT function_metadata.prosecdef
          AND function_metadata.proconfig =
              ARRAY['search_path=pg_catalog, pg_temp']::text[]
          AND NOT EXISTS (
              SELECT 1
              FROM pg_catalog.aclexplode(
                  COALESCE(
                      function_metadata.proacl,
                      pg_catalog.acldefault(
                          'f'::"char",
                          function_metadata.proowner))) AS function_acl
              WHERE function_acl.grantee = 0
                AND function_acl.privilege_type = 'EXECUTE'))
    AND NOT EXISTS (
        SELECT expected.schema_name, expected.table_name
        FROM (VALUES
            ('academic', 'org_unit'),
            ('academic', 'institution_template'),
            ('academic', 'program'),
            ('portfolio', 'syllabus_template'),
            ('portfolio', 'shared_syllabus_core'),
            ('integration', 'source_system'),
            ('measurement', 'calculation_policy'),
            ('measurement', 'indirect_instrument'))
            AS expected(schema_name, table_name)
        WHERE NOT EXISTS (
            SELECT 1
            FROM pg_catalog.pg_class AS relation
            INNER JOIN pg_catalog.pg_namespace AS namespace
                ON namespace.oid = relation.relnamespace
            WHERE namespace.nspname = expected.schema_name
              AND relation.relname = expected.table_name
              AND relation.relrowsecurity
              AND relation.relforcerowsecurity))
    AND NOT EXISTS (
        SELECT expected.schema_name, expected.table_name, expected.trigger_name
        FROM (VALUES
            ('academic', 'course', 'trg_course_reject_direct_scope_anchor_change'),
            ('academic', 'institution_template', 'trg_institution_template_reject_direct_scope_anchor_change'),
            ('academic', 'program', 'trg_program_reject_direct_scope_anchor_change'),
            ('portfolio', 'syllabus_template', 'trg_syllabus_template_reject_direct_scope_anchor_change'),
            ('portfolio', 'shared_syllabus_core', 'trg_shared_syllabus_core_reject_direct_scope_anchor_change'),
            ('integration', 'source_system', 'trg_source_system_reject_direct_scope_anchor_change'),
            ('measurement', 'calculation_policy', 'trg_calculation_policy_reject_direct_scope_anchor_change'),
            ('measurement', 'indirect_instrument', 'trg_indirect_instrument_reject_direct_scope_anchor_change'))
            AS expected(schema_name, table_name, trigger_name)
        WHERE NOT EXISTS (
            SELECT 1
            FROM pg_catalog.pg_trigger AS database_trigger
            INNER JOIN pg_catalog.pg_class AS relation
                ON relation.oid = database_trigger.tgrelid
            INNER JOIN pg_catalog.pg_namespace AS namespace
                ON namespace.oid = relation.relnamespace
            WHERE namespace.nspname = expected.schema_name
              AND relation.relname = expected.table_name
              AND database_trigger.tgname = expected.trigger_name
              AND NOT database_trigger.tgisinternal
              AND database_trigger.tgenabled = 'O'))
    AND NOT EXISTS (
        SELECT expected.schema_name, expected.table_name, expected.policy_count
        FROM (VALUES
            ('academic', 'org_unit', 2),
            ('academic', 'institution_template', 4),
            ('academic', 'program', 4),
            ('portfolio', 'syllabus_template', 4),
            ('portfolio', 'shared_syllabus_core', 4),
            ('integration', 'source_system', 4),
            ('measurement', 'calculation_policy', 4),
            ('measurement', 'indirect_instrument', 4))
            AS expected(schema_name, table_name, policy_count)
        WHERE (
            SELECT count(*)
            FROM pg_catalog.pg_policies AS policy
            WHERE policy.schemaname = expected.schema_name
              AND policy.tablename = expected.table_name)
            <> expected.policy_count)
    AND NOT EXISTS (
        SELECT policy.schemaname, policy.tablename, policy.policyname
        FROM pg_catalog.pg_policies AS policy
        WHERE (
                policy.schemaname,
                policy.tablename) IN (
                    ('academic', 'org_unit'),
                    ('academic', 'institution_template'),
                    ('academic', 'program'),
                    ('portfolio', 'syllabus_template'),
                    ('portfolio', 'shared_syllabus_core'),
                    ('integration', 'source_system'),
                    ('measurement', 'calculation_policy'),
                    ('measurement', 'indirect_instrument'))
          AND (
                policy.permissive <> 'PERMISSIVE'
                OR policy.roles <> CASE
                    WHEN policy.policyname = 'org_unit_authorizer_lookup_policy'
                        THEN ARRAY['outcomehub_authorizer']::name[]
                    ELSE ARRAY['outcomehub_app']::name[]
                END
                OR (policy.cmd = 'SELECT'
                    AND (policy.qual IS NULL OR policy.with_check IS NOT NULL))
                OR (policy.cmd = 'INSERT'
                    AND (policy.qual IS NOT NULL OR policy.with_check IS NULL))
                OR (policy.cmd = 'UPDATE'
                    AND (policy.qual IS NULL OR policy.with_check IS NULL))
                OR (policy.cmd = 'DELETE'
                    AND (policy.qual IS NULL OR policy.with_check IS NOT NULL))
                OR policy.cmd NOT IN ('SELECT', 'INSERT', 'UPDATE', 'DELETE')))
    AND (
        SELECT count(*)
        FROM pg_catalog.pg_policies
        WHERE schemaname = 'academic'
          AND tablename = 'course') = 4
    AND NOT EXISTS (
        SELECT expected.id
        FROM (VALUES
            ('10000000-0000-7000-8000-000000000005'::uuid, 'academic.org_unit', 'READ'),
            ('10000000-0000-7000-8000-000000000006'::uuid, 'academic.institution_template', 'READ'),
            ('10000000-0000-7000-8000-000000000007'::uuid, 'academic.institution_template', 'CREATE'),
            ('10000000-0000-7000-8000-000000000008'::uuid, 'academic.institution_template', 'UPDATE'),
            ('10000000-0000-7000-8000-000000000009'::uuid, 'academic.institution_template', 'DELETE'),
            ('10000000-0000-7000-8000-000000000010'::uuid, 'academic.program', 'READ'),
            ('10000000-0000-7000-8000-000000000011'::uuid, 'academic.program', 'CREATE'),
            ('10000000-0000-7000-8000-000000000012'::uuid, 'academic.program', 'UPDATE'),
            ('10000000-0000-7000-8000-000000000013'::uuid, 'academic.program', 'DELETE'),
            ('10000000-0000-7000-8000-000000000014'::uuid, 'portfolio.syllabus_template', 'READ'),
            ('10000000-0000-7000-8000-000000000015'::uuid, 'portfolio.syllabus_template', 'CREATE'),
            ('10000000-0000-7000-8000-000000000016'::uuid, 'portfolio.syllabus_template', 'UPDATE'),
            ('10000000-0000-7000-8000-000000000017'::uuid, 'portfolio.syllabus_template', 'DELETE'),
            ('10000000-0000-7000-8000-000000000018'::uuid, 'portfolio.shared_syllabus_core', 'READ'),
            ('10000000-0000-7000-8000-000000000019'::uuid, 'portfolio.shared_syllabus_core', 'CREATE'),
            ('10000000-0000-7000-8000-000000000020'::uuid, 'portfolio.shared_syllabus_core', 'UPDATE'),
            ('10000000-0000-7000-8000-000000000021'::uuid, 'portfolio.shared_syllabus_core', 'DELETE'),
            ('10000000-0000-7000-8000-000000000022'::uuid, 'integration.source_system', 'READ'),
            ('10000000-0000-7000-8000-000000000023'::uuid, 'integration.source_system', 'CREATE'),
            ('10000000-0000-7000-8000-000000000024'::uuid, 'integration.source_system', 'UPDATE'),
            ('10000000-0000-7000-8000-000000000025'::uuid, 'integration.source_system', 'DELETE'),
            ('10000000-0000-7000-8000-000000000026'::uuid, 'measurement.calculation_policy', 'READ'),
            ('10000000-0000-7000-8000-000000000027'::uuid, 'measurement.calculation_policy', 'CREATE'),
            ('10000000-0000-7000-8000-000000000028'::uuid, 'measurement.calculation_policy', 'UPDATE'),
            ('10000000-0000-7000-8000-000000000029'::uuid, 'measurement.calculation_policy', 'DELETE'),
            ('10000000-0000-7000-8000-000000000030'::uuid, 'measurement.indirect_instrument', 'READ'),
            ('10000000-0000-7000-8000-000000000031'::uuid, 'measurement.indirect_instrument', 'CREATE'),
            ('10000000-0000-7000-8000-000000000032'::uuid, 'measurement.indirect_instrument', 'UPDATE'),
            ('10000000-0000-7000-8000-000000000033'::uuid, 'measurement.indirect_instrument', 'DELETE'))
            AS expected(id, resource_type, action)
        WHERE NOT EXISTS (
            SELECT 1
            FROM iam.permission AS permission
            WHERE permission.id = expected.id
              AND permission.resource_type = expected.resource_type
              AND permission.action = expected.action
              AND permission.field_scope = '*'))
    AND pg_catalog.has_table_privilege(
        'outcomehub_app',
        'academic.org_unit',
        'SELECT')
    AND NOT pg_catalog.has_table_privilege(
        'outcomehub_app', 'academic.org_unit', 'INSERT')
    AND NOT pg_catalog.has_table_privilege(
        'outcomehub_app', 'academic.org_unit', 'UPDATE')
    AND NOT pg_catalog.has_table_privilege(
        'outcomehub_app', 'academic.org_unit', 'DELETE')
    AND NOT pg_catalog.has_table_privilege(
        'outcomehub_app', 'academic.org_unit', 'TRUNCATE')
    AND NOT EXISTS (
        SELECT expected.table_name
        FROM (VALUES
            ('academic.institution_template'),
            ('academic.program'),
            ('portfolio.syllabus_template'),
            ('portfolio.shared_syllabus_core'),
            ('integration.source_system'),
            ('measurement.calculation_policy'),
            ('measurement.indirect_instrument'))
            AS expected(table_name)
        WHERE NOT (
                pg_catalog.has_table_privilege(
                    'outcomehub_app', expected.table_name, 'SELECT')
                AND pg_catalog.has_table_privilege(
                    'outcomehub_app', expected.table_name, 'INSERT')
                AND pg_catalog.has_table_privilege(
                    'outcomehub_app', expected.table_name, 'UPDATE')
                AND pg_catalog.has_table_privilege(
                    'outcomehub_app', expected.table_name, 'DELETE'))
           OR pg_catalog.has_table_privilege(
                'outcomehub_app',
                expected.table_name,
                'TRUNCATE'));
