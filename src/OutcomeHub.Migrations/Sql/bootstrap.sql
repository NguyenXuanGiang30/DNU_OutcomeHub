CREATE SCHEMA IF NOT EXISTS ops;

REVOKE CREATE ON SCHEMA ops FROM PUBLIC;

CREATE TABLE IF NOT EXISTS ops.schema_migration (
    id uuid NOT NULL,
    migration_name varchar(255) NOT NULL,
    checksum char(64) NOT NULL,
    transaction_mode varchar(20) NOT NULL,
    status varchar(20) NOT NULL,
    started_at timestamptz NOT NULL,
    applied_at timestamptz,
    runner_version varchar(64) NOT NULL,
    error_code varchar(64),
    CONSTRAINT pk_schema_migration PRIMARY KEY (id),
    CONSTRAINT ck_schema_migration_applied_at
        CHECK (applied_at IS NULL OR applied_at >= started_at),
    CONSTRAINT ck_schema_migration_checksum
        CHECK (checksum ~ '^[0-9a-f]{64}$'),
    CONSTRAINT ck_schema_migration_status
        CHECK (status IN ('PENDING', 'RUNNING', 'APPLIED', 'FAILED')),
    CONSTRAINT ck_schema_migration_transaction_mode
        CHECK (transaction_mode IN ('TRANSACTIONAL', 'OPERATIONAL')),
    CONSTRAINT ck_schema_migration_state
        CHECK (
            (status = 'APPLIED'
                AND applied_at IS NOT NULL
                AND error_code IS NULL)
            OR
            (status IN ('PENDING', 'RUNNING')
                AND applied_at IS NULL
                AND error_code IS NULL)
            OR
            (status = 'FAILED'
                AND applied_at IS NULL
                AND error_code IS NOT NULL))
);

DO $bootstrap$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_constraint
        WHERE conrelid = 'ops.schema_migration'::regclass
          AND conname = 'ck_schema_migration_state') THEN
        ALTER TABLE ops.schema_migration
            ADD CONSTRAINT ck_schema_migration_state
            CHECK (
                (status = 'APPLIED'
                    AND applied_at IS NOT NULL
                    AND error_code IS NULL)
                OR
                (status IN ('PENDING', 'RUNNING')
                    AND applied_at IS NULL
                    AND error_code IS NULL)
                OR
                (status = 'FAILED'
                    AND applied_at IS NULL
                    AND error_code IS NOT NULL));
    END IF;
END;
$bootstrap$;

CREATE UNIQUE INDEX IF NOT EXISTS uq_schema_migration_name
    ON ops.schema_migration (migration_name);

CREATE OR REPLACE FUNCTION ops.guard_schema_migration()
RETURNS trigger
LANGUAGE plpgsql
SET search_path = pg_catalog, ops
AS $function$
BEGIN
    IF TG_OP IN ('DELETE', 'TRUNCATE') THEN
        RAISE EXCEPTION 'Schema migration ledger is immutable.'
            USING ERRCODE = '55000';
    END IF;

    IF ROW(NEW.id, NEW.migration_name, NEW.checksum, NEW.transaction_mode)
       IS DISTINCT FROM
       ROW(OLD.id, OLD.migration_name, OLD.checksum, OLD.transaction_mode) THEN
        RAISE EXCEPTION 'Schema migration identity cannot be changed.'
            USING ERRCODE = '55000';
    END IF;

    IF OLD.status = 'APPLIED' THEN
        RAISE EXCEPTION 'An applied schema migration cannot be changed.'
            USING ERRCODE = '55000';
    END IF;

    RETURN NEW;
END;
$function$;

REVOKE ALL ON FUNCTION ops.guard_schema_migration() FROM PUBLIC;

DROP TRIGGER IF EXISTS trg_schema_migration_guard
    ON ops.schema_migration;
CREATE TRIGGER trg_schema_migration_guard
BEFORE UPDATE OR DELETE ON ops.schema_migration
FOR EACH ROW
EXECUTE FUNCTION ops.guard_schema_migration();

DROP TRIGGER IF EXISTS trg_schema_migration_reject_truncate
    ON ops.schema_migration;
CREATE TRIGGER trg_schema_migration_reject_truncate
BEFORE TRUNCATE ON ops.schema_migration
FOR EACH STATEMENT
EXECUTE FUNCTION ops.guard_schema_migration();

REVOKE ALL ON TABLE ops.schema_migration FROM PUBLIC;

DO $bootstrap$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM pg_catalog.pg_roles
        WHERE rolname = 'outcomehub_app') THEN
        REVOKE ALL ON TABLE ops.schema_migration FROM outcomehub_app;
    END IF;

    IF (
        SELECT tableowner
        FROM pg_catalog.pg_tables
        WHERE schemaname = 'ops'
          AND tablename = 'schema_migration') <> CURRENT_USER THEN
        RAISE EXCEPTION 'The migration runner must own ops.schema_migration.';
    END IF;

    IF (
        SELECT count(*)
        FROM information_schema.columns
        WHERE table_schema = 'ops'
          AND table_name = 'schema_migration') <> 9 THEN
        RAISE EXCEPTION 'ops.schema_migration has an unexpected column contract.';
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM pg_catalog.pg_indexes
        WHERE schemaname = 'ops'
          AND tablename = 'schema_migration'
          AND indexname = 'uq_schema_migration_name') THEN
        RAISE EXCEPTION 'ops.schema_migration is missing its unique name index.';
    END IF;
END;
$bootstrap$;
