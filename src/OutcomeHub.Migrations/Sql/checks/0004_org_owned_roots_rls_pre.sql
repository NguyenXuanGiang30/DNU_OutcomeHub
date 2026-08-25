SELECT
    pg_catalog.to_regprocedure(
        'iam.has_permission(text,text,text,uuid,uuid,uuid,uuid,uuid,uuid,uuid,uuid,uuid,text)')
        IS NOT NULL
    AND (
        SELECT count(*)
        FROM iam.permission) = 4
    AND NOT EXISTS (
        SELECT 1 FROM academic.org_unit
        UNION ALL
        SELECT 1 FROM academic.institution_template
        UNION ALL
        SELECT 1 FROM academic.program
        UNION ALL
        SELECT 1 FROM portfolio.syllabus_template
        UNION ALL
        SELECT 1 FROM portfolio.shared_syllabus_core
        UNION ALL
        SELECT 1 FROM integration.source_system
        UNION ALL
        SELECT 1 FROM measurement.calculation_policy
        UNION ALL
        SELECT 1 FROM measurement.indirect_instrument)
    AND NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_policies
        WHERE (schemaname, tablename) IN (
            ('academic', 'org_unit'),
            ('academic', 'institution_template'),
            ('academic', 'program'),
            ('portfolio', 'syllabus_template'),
            ('portfolio', 'shared_syllabus_core'),
            ('integration', 'source_system'),
            ('measurement', 'calculation_policy'),
            ('measurement', 'indirect_instrument')));
