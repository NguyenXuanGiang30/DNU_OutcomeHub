\set ON_ERROR_STOP on

DO $bootstrap$
DECLARE
    migrator_password_file text := COALESCE(
        NULLIF(
            pg_catalog.current_setting(
                'outcomehub.bootstrap_migrator_password_file',
                true),
            ''),
        '/run/secrets/migrator_password');
    migrator_password text;
BEGIN
    migrator_password := pg_catalog.btrim(
        pg_catalog.pg_read_file(migrator_password_file));

    IF migrator_password = '' THEN
        RAISE EXCEPTION 'The Development migrator password file is empty.';
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_roles
        WHERE rolname = 'outcomehub_authorizer') THEN
        RAISE EXCEPTION 'Run bootstrap_app_role.sql before provisioning the migrator.';
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_roles
        WHERE rolname = 'outcomehub_migrator') THEN
        EXECUTE pg_catalog.format(
            'CREATE ROLE outcomehub_migrator '
            'LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE '
            'NOINHERIT NOREPLICATION NOBYPASSRLS PASSWORD %L',
            migrator_password);
    ELSE
        EXECUTE pg_catalog.format(
            'ALTER ROLE outcomehub_migrator '
            'WITH LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE '
            'NOINHERIT NOREPLICATION NOBYPASSRLS PASSWORD %L',
            migrator_password);
    END IF;

    GRANT outcomehub_authorizer TO outcomehub_migrator;

    EXECUTE pg_catalog.format(
        'ALTER DATABASE %I OWNER TO outcomehub_migrator',
        pg_catalog.current_database());
END;
$bootstrap$;
