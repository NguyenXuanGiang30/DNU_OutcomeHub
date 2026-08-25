using Npgsql;
using OutcomeHub.Migrations;
using Testcontainers.PostgreSql;
using Xunit;

namespace OutcomeHub.DatabaseTests;

public sealed class DatabaseBaselineTests
{
    [Fact(Timeout = 180_000)]
    public async Task FreshDatabaseAppliesCanonicalMigrationsAndPassesCourseRlsSmokeTest()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using PostgreSqlContainer postgreSql = new PostgreSqlBuilder("postgres:18")
            .WithDatabase("outcomehub_tests")
            .WithUsername("outcomehub_test_owner")
            .WithPassword("outcomehub_test_owner_password")
            .Build();

        await postgreSql.StartAsync(cancellationToken);
        string ownerConnectionString = postgreSql.GetConnectionString();

        string migrationConnectionString = await ProvisionDatabaseRolesAsync(
            ownerConnectionString,
            cancellationToken);
        string migrationRoot = Path.Combine(AppContext.BaseDirectory, "MigrationSql");

        var firstRunner = new SqlMigrationRunner(migrationConnectionString, migrationRoot);
        var concurrentRunner = new SqlMigrationRunner(migrationConnectionString, migrationRoot);
        MigrationRunResult[] concurrentResults = await Task.WhenAll(
            firstRunner.RunAsync(cancellationToken),
            concurrentRunner.RunAsync(cancellationToken));
        MigrationRunResult noOpResult = await firstRunner.RunAsync(cancellationToken);

        Assert.Equal(11, concurrentResults.Sum(result => result.AppliedCount));
        Assert.Equal(11, concurrentResults.Sum(result => result.SkippedCount));
        Assert.Equal(0, noOpResult.AppliedCount);
        Assert.Equal(11, noOpResult.SkippedCount);

        await RunCourseRlsSmokeTestAsync(ownerConnectionString, cancellationToken);
        await RunDatabaseHardeningSmokeTestAsync(ownerConnectionString, cancellationToken);
        await RunCriticalBusinessInvariantsSmokeTestAsync(
            ownerConnectionString,
            cancellationToken);
        await RunOrgOwnedRootsRlsSmokeTestAsync(
            ownerConnectionString,
            cancellationToken);
        await RunScoreRecordRlsSmokeTestAsync(
            ownerConnectionString,
            cancellationToken);
        await RunSyllabusOfferingRlsSmokeTestAsync(
            ownerConnectionString,
            cancellationToken);
        await RunResultAndStudentSelfRlsSmokeTestAsync(
            ownerConnectionString,
            cancellationToken);
        await RunSnapshotResultImmutabilitySmokeTestAsync(
            ownerConnectionString,
            cancellationToken);
        await AssertDatabaseMetadataAsync(ownerConnectionString, cancellationToken);
        await AssertApplicationLoginIsHardenedAsync(
            ownerConnectionString,
            cancellationToken);
        await AssertChecksumDriftIsRejectedAsync(
            migrationConnectionString,
            migrationRoot,
            cancellationToken);
    }

    internal static async Task<string> ProvisionDatabaseRolesAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database ?? "outcomehub_tests";

        string sql = $"""
            CREATE ROLE outcomehub_authorizer
                NOLOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT NOREPLICATION NOBYPASSRLS;

            CREATE ROLE outcomehub_app
                LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT NOREPLICATION NOBYPASSRLS
                PASSWORD 'outcomehub_test_app_password';

            CREATE ROLE outcomehub_migrator
                LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT NOREPLICATION NOBYPASSRLS
                PASSWORD 'outcomehub_test_migrator_password';

            GRANT outcomehub_authorizer TO outcomehub_migrator;
            ALTER DATABASE "{databaseName}" OWNER TO outcomehub_migrator;
            """;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);

        var migrationConnectionString = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Username = "outcomehub_migrator",
            Password = "outcomehub_test_migrator_password",
            Pooling = false,
        };
        return migrationConnectionString.ConnectionString;
    }

    private static async Task RunCourseRlsSmokeTestAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        await RunDatabaseScriptAsync(
            connectionString,
            "course_rls_smoke.sql",
            cancellationToken);
    }

    private static async Task RunDatabaseHardeningSmokeTestAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        await RunDatabaseScriptAsync(
            connectionString,
            "database_hardening_smoke.sql",
            cancellationToken);
    }

    private static async Task RunCriticalBusinessInvariantsSmokeTestAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        await RunDatabaseScriptAsync(
            connectionString,
            "critical_business_invariants_smoke.sql",
            cancellationToken);
    }

    private static async Task RunOrgOwnedRootsRlsSmokeTestAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        await RunDatabaseScriptAsync(
            connectionString,
            "org_owned_roots_rls_smoke.sql",
            cancellationToken);
    }

    private static async Task RunSnapshotResultImmutabilitySmokeTestAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        await RunDatabaseScriptAsync(
            connectionString,
            "snapshot_result_immutability_smoke.sql",
            cancellationToken);
    }

    private static async Task RunScoreRecordRlsSmokeTestAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        await RunDatabaseScriptAsync(
            connectionString,
            "score_record_rls_smoke.sql",
            cancellationToken);
    }

    private static async Task RunSyllabusOfferingRlsSmokeTestAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        await RunDatabaseScriptAsync(
            connectionString,
            "syllabus_offering_rls_smoke.sql",
            cancellationToken);
    }

    private static async Task RunResultAndStudentSelfRlsSmokeTestAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        await RunDatabaseScriptAsync(
            connectionString,
            "result_and_student_self_rls_smoke.sql",
            cancellationToken);
    }

    internal static async Task RunDatabaseScriptAsync(
        string connectionString,
        string scriptName,
        CancellationToken cancellationToken)
    {
        string scriptPath = Path.Combine(AppContext.BaseDirectory, "DatabaseScripts", scriptName);

        string sql = await File.ReadAllTextAsync(scriptPath, cancellationToken);
        sql = string.Join(
            Environment.NewLine,
            sql.Split('\n').Where(line => !line.TrimStart().StartsWith('\\')));

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            CommandTimeout = 120,
        };

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task AssertDatabaseMetadataAsync(
        string connectionString,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                (SELECT count(*) FROM academic.course) AS course_count,
                (SELECT count(*) FROM pg_catalog.pg_policies
                 WHERE schemaname = 'academic' AND tablename = 'course') AS policy_count,
                (SELECT relrowsecurity AND relforcerowsecurity
                 FROM pg_catalog.pg_class AS relation
                 INNER JOIN pg_catalog.pg_namespace AS namespace
                    ON namespace.oid = relation.relnamespace
                 WHERE namespace.nspname = 'academic' AND relation.relname = 'course') AS rls_forced,
                (SELECT count(*) FROM ops.schema_migration
                 WHERE status = 'APPLIED') AS applied_migration_count,
                pg_catalog.to_regclass('public."__EFMigrationsHistory"') IS NULL
                    AS ef_history_absent,
                (SELECT count(*)
                 FROM pg_catalog.pg_class AS relation
                 INNER JOIN pg_catalog.pg_namespace AS namespace
                    ON namespace.oid = relation.relnamespace
                 WHERE (namespace.nspname, relation.relname) IN (
                    ('academic', 'org_unit'),
                    ('academic', 'institution_template'),
                    ('academic', 'program'),
                    ('portfolio', 'syllabus_template'),
                    ('portfolio', 'shared_syllabus_core'),
                    ('integration', 'source_system'),
                    ('measurement', 'calculation_policy'),
                    ('measurement', 'indirect_instrument'))
                   AND relation.relrowsecurity
                   AND relation.relforcerowsecurity) AS protected_root_count,
                (SELECT count(*)
                 FROM pg_catalog.pg_policies AS policy
                 WHERE (policy.schemaname, policy.tablename) IN (
                    ('academic', 'org_unit'),
                    ('academic', 'institution_template'),
                    ('academic', 'program'),
                    ('portfolio', 'syllabus_template'),
                    ('portfolio', 'shared_syllabus_core'),
                    ('integration', 'source_system'),
                    ('measurement', 'calculation_policy'),
                    ('measurement', 'indirect_instrument'))) AS protected_root_policy_count,
                (SELECT count(*)
                 FROM iam.permission AS permission
                 WHERE permission.id::text BETWEEN
                    '10000000-0000-7000-8000-000000000005'
                    AND '10000000-0000-7000-8000-000000000033')
                    AS org_root_permission_count,
                (SELECT count(*)
                 FROM pg_catalog.pg_trigger AS database_trigger
                 WHERE database_trigger.tgname LIKE
                    'trg_%_reject_direct_scope_anchor_change'
                   AND NOT database_trigger.tgisinternal) AS scope_anchor_trigger_count,
                (SELECT relrowsecurity AND relforcerowsecurity
                 FROM pg_catalog.pg_class AS relation
                 INNER JOIN pg_catalog.pg_namespace AS namespace
                    ON namespace.oid = relation.relnamespace
                 WHERE namespace.nspname = 'measurement'
                   AND relation.relname = 'score_record') AS score_record_rls_forced,
                (SELECT count(*)
                 FROM pg_catalog.pg_policies AS policy
                 WHERE policy.schemaname = 'measurement'
                   AND policy.tablename = 'score_record'
                   AND policy.policyname = 'score_record_select_policy')
                    AS score_record_policy_count,
                pg_catalog.has_table_privilege(
                    'outcomehub_app',
                    'measurement.score_record',
                    'SELECT') AS score_record_select_granted,
                NOT pg_catalog.has_table_privilege(
                    'outcomehub_app',
                    'measurement.score_record',
                    'INSERT') AS score_record_insert_denied,
                NOT pg_catalog.has_table_privilege(
                    'outcomehub_app',
                    'measurement.score_record',
                    'UPDATE') AS score_record_update_denied,
                NOT pg_catalog.has_table_privilege(
                    'outcomehub_app',
                    'measurement.score_record',
                    'DELETE') AS score_record_delete_denied;
            """;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        Assert.True(await reader.ReadAsync(cancellationToken));
        Assert.Equal(0L, reader.GetInt64(0));
        Assert.Equal(4L, reader.GetInt64(1));
        Assert.True(reader.GetBoolean(2));
        Assert.Equal(11L, reader.GetInt64(3));
        Assert.True(reader.GetBoolean(4));
        Assert.Equal(8L, reader.GetInt64(5));
        Assert.Equal(30L, reader.GetInt64(6));
        Assert.Equal(29L, reader.GetInt64(7));
        Assert.Equal(10L, reader.GetInt64(8));
        Assert.True(reader.GetBoolean(9));
        Assert.Equal(1L, reader.GetInt64(10));
        Assert.True(reader.GetBoolean(11));
        Assert.True(reader.GetBoolean(12));
        Assert.True(reader.GetBoolean(13));
        Assert.True(reader.GetBoolean(14));
    }

    private static async Task AssertApplicationLoginIsHardenedAsync(
        string ownerConnectionString,
        CancellationToken cancellationToken)
    {
        var appConnectionString = new NpgsqlConnectionStringBuilder(ownerConnectionString)
        {
            Username = "outcomehub_app",
            Password = "outcomehub_test_app_password",
            Pooling = false,
        };

        const string roleSql = """
            SELECT
                session_user = 'outcomehub_app',
                current_user = 'outcomehub_app',
                role_metadata.rolcanlogin,
                NOT role_metadata.rolsuper,
                NOT role_metadata.rolcreatedb,
                NOT role_metadata.rolcreaterole,
                NOT role_metadata.rolreplication,
                NOT role_metadata.rolbypassrls,
                NOT pg_catalog.pg_has_role(
                    'outcomehub_app',
                    'outcomehub_authorizer',
                    'MEMBER'),
                iam.current_context_uuid('app.principal_id') IS NULL,
                iam.current_context_uuid('app.request_id') IS NULL
            FROM pg_catalog.pg_roles AS role_metadata
            WHERE role_metadata.rolname = 'outcomehub_app';
            """;

        await using var connection = new NpgsqlConnection(appConnectionString.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using (var command = new NpgsqlCommand(roleSql, connection))
        await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            Assert.True(await reader.ReadAsync(cancellationToken));
            for (int columnIndex = 0; columnIndex < reader.FieldCount; columnIndex++)
            {
                Assert.True(reader.GetBoolean(columnIndex));
            }
        }

        await using var setRoleCommand = new NpgsqlCommand(
            "SET ROLE outcomehub_authorizer;",
            connection);
        PostgresException exception = await Assert.ThrowsAsync<PostgresException>(
            () => setRoleCommand.ExecuteNonQueryAsync(cancellationToken));
        Assert.Equal(PostgresErrorCodes.InsufficientPrivilege, exception.SqlState);
    }

    private static async Task AssertChecksumDriftIsRejectedAsync(
        string connectionString,
        string migrationRoot,
        CancellationToken cancellationToken)
    {
        string temporaryRoot = Path.Combine(
            Path.GetTempPath(),
            $"outcomehub-migration-drift-{Guid.NewGuid():N}");

        try
        {
            foreach (string sourcePath in Directory.EnumerateFiles(
                migrationRoot,
                "*",
                SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(migrationRoot, sourcePath);
                string destinationPath = Path.Combine(temporaryRoot, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                File.Copy(sourcePath, destinationPath);
            }

            string baselinePath = Path.Combine(
                temporaryRoot,
                "transactional",
                "0001_baseline.sql");
            await File.AppendAllTextAsync(
                baselinePath,
                "\n-- checksum drift\n",
                cancellationToken);

            var driftedRunner = new SqlMigrationRunner(connectionString, temporaryRoot);
            InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => driftedRunner.RunAsync(cancellationToken));
            Assert.Contains("Checksum drift", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(temporaryRoot))
            {
                Directory.Delete(temporaryRoot, recursive: true);
            }
        }
    }
}
