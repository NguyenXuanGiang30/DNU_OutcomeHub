\set ON_ERROR_STOP on

DO $bootstrap$
DECLARE
    app_password_file text := COALESCE(
        NULLIF(
            pg_catalog.current_setting(
                'outcomehub.bootstrap_app_password_file',
                true),
            ''),
        '/run/secrets/app_password');
    app_password text;
BEGIN
    app_password := pg_catalog.btrim(
        pg_catalog.pg_read_file(app_password_file));

    IF app_password = '' THEN
        RAISE EXCEPTION 'The Development application password file is empty.';
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_roles
        WHERE rolname = 'outcomehub_authorizer') THEN
        CREATE ROLE outcomehub_authorizer
            NOLOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE
            NOINHERIT NOREPLICATION NOBYPASSRLS;
    END IF;

    ALTER ROLE outcomehub_authorizer
        WITH NOLOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE
        NOINHERIT NOREPLICATION NOBYPASSRLS;

    IF NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_roles
        WHERE rolname = 'outcomehub_app') THEN
        EXECUTE pg_catalog.format(
            'CREATE ROLE outcomehub_app '
            'LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE '
            'NOINHERIT NOREPLICATION NOBYPASSRLS PASSWORD %L',
            app_password);
    ELSE
        EXECUTE pg_catalog.format(
            'ALTER ROLE outcomehub_app '
            'WITH LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE '
            'NOINHERIT NOREPLICATION NOBYPASSRLS PASSWORD %L',
            app_password);
    END IF;

    EXECUTE pg_catalog.format(
        'GRANT CONNECT ON DATABASE %I TO outcomehub_app',
        pg_catalog.current_database());
END;
$bootstrap$;

REVOKE CREATE ON SCHEMA public FROM outcomehub_app;
