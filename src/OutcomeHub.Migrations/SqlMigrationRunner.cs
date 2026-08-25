using System.Data;
using System.Reflection;
using Npgsql;

namespace OutcomeHub.Migrations;

public sealed class SqlMigrationRunner
{
    private const int DefaultLockTimeoutSeconds = 60;
    private readonly string connectionString;
    private readonly string migrationRoot;
    private readonly TimeSpan lockTimeout;

    public SqlMigrationRunner(
        string connectionString,
        string migrationRoot,
        TimeSpan? lockTimeout = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(migrationRoot);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            ApplicationName = "OutcomeHub.Migrations",
            Pooling = false,
        };

        this.connectionString = connectionStringBuilder.ConnectionString;
        this.migrationRoot = Path.GetFullPath(migrationRoot);
        this.lockTimeout = lockTimeout ?? TimeSpan.FromSeconds(DefaultLockTimeoutSeconds);

        if (this.lockTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lockTimeout),
                "The advisory-lock timeout must be positive.");
        }
    }

    public async Task<MigrationRunResult> RunAsync(CancellationToken cancellationToken = default)
    {
        (MigrationManifest manifest, IReadOnlyList<MigrationArtifact> artifacts) =
            await MigrationArtifactLoader.LoadAsync(migrationRoot, cancellationToken);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await EnsurePostgresVersionAsync(connection, manifest.PostgresMajor, cancellationToken);
        await AcquireAdvisoryLockAsync(
            connection,
            manifest.AdvisoryLockKey,
            cancellationToken);

        try
        {
            await BootstrapLedgerAsync(connection, cancellationToken);
            IReadOnlyDictionary<string, MigrationLedgerEntry> ledger =
                await ReadAndValidateLedgerAsync(connection, artifacts, cancellationToken);

            int appliedCount = 0;
            int skippedCount = 0;

            foreach (MigrationArtifact artifact in artifacts)
            {
                if (ledger.TryGetValue(artifact.Definition.MigrationName, out var entry)
                    && string.Equals(entry.Status, "APPLIED", StringComparison.Ordinal))
                {
                    await AssertConditionAsync(
                        connection,
                        transaction: null,
                        artifact.PostconditionSql,
                        artifact.Definition.TimeoutSeconds,
                        $"Postcondition failed for applied migration '{entry.MigrationName}'.",
                        cancellationToken);
                    skippedCount++;
                    continue;
                }

                if (string.Equals(
                    artifact.Definition.TransactionMode,
                    "TRANSACTIONAL",
                    StringComparison.Ordinal))
                {
                    await ExecuteTransactionalAsync(connection, artifact, cancellationToken);
                }
                else
                {
                    await ExecuteOperationalAsync(connection, artifact, entry, cancellationToken);
                }

                appliedCount++;
            }

            return new MigrationRunResult(appliedCount, skippedCount);
        }
        finally
        {
            await ReleaseAdvisoryLockAsync(
                connection,
                manifest.AdvisoryLockKey,
                CancellationToken.None);
        }
    }

    private static async Task EnsurePostgresVersionAsync(
        NpgsqlConnection connection,
        int expectedMajor,
        CancellationToken cancellationToken)
    {
        await using var command = new NpgsqlCommand(
            "SHOW server_version_num;",
            connection);
        string version = (string)(await command.ExecuteScalarAsync(cancellationToken)
            ?? throw new InvalidOperationException("PostgreSQL did not return its version."));

        if (!int.TryParse(version, out int versionNumber)
            || versionNumber / 10_000 != expectedMajor)
        {
            throw new InvalidOperationException(
                $"Migration manifest requires PostgreSQL {expectedMajor}.");
        }
    }

    private async Task AcquireAdvisoryLockAsync(
        NpgsqlConnection connection,
        long advisoryLockKey,
        CancellationToken cancellationToken)
    {
        DateTimeOffset deadline = DateTimeOffset.UtcNow.Add(lockTimeout);
        while (DateTimeOffset.UtcNow < deadline)
        {
            await using var command = new NpgsqlCommand(
                "SELECT pg_catalog.pg_try_advisory_lock($1);",
                connection);
            command.Parameters.AddWithValue(advisoryLockKey);

            if (await command.ExecuteScalarAsync(cancellationToken) is true)
            {
                return;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
        }

        throw new TimeoutException("Timed out while waiting for the migration advisory lock.");
    }

    private static async Task ReleaseAdvisoryLockAsync(
        NpgsqlConnection connection,
        long advisoryLockKey,
        CancellationToken cancellationToken)
    {
        if (connection.FullState != ConnectionState.Open)
        {
            return;
        }

        await using var command = new NpgsqlCommand(
            "SELECT pg_catalog.pg_advisory_unlock($1);",
            connection);
        command.Parameters.AddWithValue(advisoryLockKey);
        _ = await command.ExecuteScalarAsync(cancellationToken);
    }

    private async Task BootstrapLedgerAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        string path = Path.Combine(migrationRoot, "bootstrap.sql");
        string sql = await File.ReadAllTextAsync(path, cancellationToken);

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection, transaction)
        {
            CommandTimeout = 60,
        };
        await command.ExecuteNonQueryAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private static async Task<IReadOnlyDictionary<string, MigrationLedgerEntry>>
        ReadAndValidateLedgerAsync(
            NpgsqlConnection connection,
            IReadOnlyList<MigrationArtifact> artifacts,
            CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id, migration_name, checksum, transaction_mode, status
            FROM ops.schema_migration
            ORDER BY migration_name;
            """;

        var entries = new Dictionary<string, MigrationLedgerEntry>(StringComparer.Ordinal);
        await using (var command = new NpgsqlCommand(sql, connection))
        await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                var entry = new MigrationLedgerEntry(
                    reader.GetGuid(0),
                    reader.GetString(1),
                    reader.GetString(2).TrimEnd(),
                    reader.GetString(3),
                    reader.GetString(4));
                entries.Add(entry.MigrationName, entry);
            }
        }

        var artifactByName = artifacts.ToDictionary(
            x => x.Definition.MigrationName,
            StringComparer.Ordinal);

        foreach (MigrationLedgerEntry entry in entries.Values)
        {
            if (!artifactByName.TryGetValue(entry.MigrationName, out var artifact)
                || entry.Id != artifact.Id
                || !string.Equals(
                    entry.Checksum,
                    artifact.Definition.ScriptChecksum,
                    StringComparison.Ordinal)
                || !string.Equals(
                    entry.TransactionMode,
                    artifact.Definition.TransactionMode,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Migration ledger drift detected at '{entry.MigrationName}'.");
            }
        }

        bool encounteredIncomplete = false;
        foreach (MigrationArtifact artifact in artifacts)
        {
            if (!entries.TryGetValue(artifact.Definition.MigrationName, out var entry))
            {
                encounteredIncomplete = true;
                continue;
            }

            if (encounteredIncomplete)
            {
                throw new InvalidOperationException(
                    "Migration ledger is not a continuous manifest prefix.");
            }

            if (!string.Equals(entry.Status, "APPLIED", StringComparison.Ordinal))
            {
                encounteredIncomplete = true;
            }
        }

        return entries;
    }

    private static async Task ExecuteTransactionalAsync(
        NpgsqlConnection connection,
        MigrationArtifact artifact,
        CancellationToken cancellationToken)
    {
        DateTimeOffset startedAt = DateTimeOffset.UtcNow;
        try
        {
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
            await ClaimAsync(connection, transaction, artifact, startedAt, cancellationToken);
            await AssertConditionAsync(
                connection,
                transaction,
                artifact.PreconditionSql,
                artifact.Definition.TimeoutSeconds,
                $"Precondition failed for migration '{artifact.Definition.MigrationName}'.",
                cancellationToken);
            await ExecuteScriptAsync(connection, transaction, artifact, cancellationToken);
            await AssertConditionAsync(
                connection,
                transaction,
                artifact.PostconditionSql,
                artifact.Definition.TimeoutSeconds,
                $"Postcondition failed for migration '{artifact.Definition.MigrationName}'.",
                cancellationToken);
            await MarkAppliedAsync(connection, transaction, artifact, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            await MarkFailedAsync(
                connection,
                artifact,
                startedAt,
                GetErrorCode(exception),
                CancellationToken.None);
            throw;
        }
    }

    private static async Task ExecuteOperationalAsync(
        NpgsqlConnection connection,
        MigrationArtifact artifact,
        MigrationLedgerEntry? existingEntry,
        CancellationToken cancellationToken)
    {
        DateTimeOffset startedAt = DateTimeOffset.UtcNow;
        try
        {
            if (existingEntry is not null
                && await EvaluateConditionAsync(
                    connection,
                    transaction: null,
                    artifact.PostconditionSql,
                    artifact.Definition.TimeoutSeconds,
                    cancellationToken))
            {
                await ClaimAsync(
                    connection,
                    transaction: null,
                    artifact,
                    startedAt,
                    cancellationToken);
                await MarkAppliedAsync(
                    connection,
                    transaction: null,
                    artifact,
                    cancellationToken);
                return;
            }

            await AssertConditionAsync(
                connection,
                transaction: null,
                artifact.PreconditionSql,
                artifact.Definition.TimeoutSeconds,
                $"Operational migration '{artifact.Definition.MigrationName}' requires manual reconciliation.",
                cancellationToken);
            await ClaimAsync(
                connection,
                transaction: null,
                artifact,
                startedAt,
                cancellationToken);
            await ExecuteScriptAsync(
                connection,
                transaction: null,
                artifact,
                cancellationToken);
            await AssertConditionAsync(
                connection,
                transaction: null,
                artifact.PostconditionSql,
                artifact.Definition.TimeoutSeconds,
                $"Postcondition failed for migration '{artifact.Definition.MigrationName}'.",
                cancellationToken);
            await MarkAppliedAsync(
                connection,
                transaction: null,
                artifact,
                cancellationToken);
        }
        catch (Exception exception)
        {
            await MarkFailedAsync(
                connection,
                artifact,
                startedAt,
                GetErrorCode(exception),
                CancellationToken.None);
            throw;
        }
    }

    private static async Task ClaimAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction? transaction,
        MigrationArtifact artifact,
        DateTimeOffset startedAt,
        CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO ops.schema_migration (
                id,
                migration_name,
                checksum,
                transaction_mode,
                status,
                started_at,
                applied_at,
                runner_version,
                error_code)
            VALUES ($1, $2, $3, $4, 'RUNNING', $5, NULL, $6, NULL)
            ON CONFLICT (migration_name) DO UPDATE
            SET status = 'RUNNING',
                started_at = EXCLUDED.started_at,
                applied_at = NULL,
                runner_version = EXCLUDED.runner_version,
                error_code = NULL;
            """;

        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue(artifact.Id);
        command.Parameters.AddWithValue(artifact.Definition.MigrationName);
        command.Parameters.AddWithValue(artifact.Definition.ScriptChecksum);
        command.Parameters.AddWithValue(artifact.Definition.TransactionMode);
        command.Parameters.AddWithValue(startedAt);
        command.Parameters.AddWithValue(GetRunnerVersion());
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task ExecuteScriptAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction? transaction,
        MigrationArtifact artifact,
        CancellationToken cancellationToken)
    {
        await using var command = new NpgsqlCommand(
            artifact.ScriptSql,
            connection,
            transaction)
        {
            CommandTimeout = artifact.Definition.TimeoutSeconds,
        };
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task AssertConditionAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction? transaction,
        string sql,
        int timeoutSeconds,
        string failureMessage,
        CancellationToken cancellationToken)
    {
        if (!await EvaluateConditionAsync(
            connection,
            transaction,
            sql,
            timeoutSeconds,
            cancellationToken))
        {
            throw new InvalidOperationException(failureMessage);
        }
    }

    private static async Task<bool> EvaluateConditionAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction? transaction,
        string sql,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        await using var command = new NpgsqlCommand(sql, connection, transaction)
        {
            CommandTimeout = timeoutSeconds,
        };
        object? result = await command.ExecuteScalarAsync(cancellationToken);
        return result is bool condition && condition;
    }

    private static async Task MarkAppliedAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction? transaction,
        MigrationArtifact artifact,
        CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE ops.schema_migration
            SET status = 'APPLIED',
                applied_at = clock_timestamp(),
                error_code = NULL
            WHERE migration_name = $1;
            """;

        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue(artifact.Definition.MigrationName);
        if (await command.ExecuteNonQueryAsync(cancellationToken) != 1)
        {
            throw new InvalidOperationException("Migration ledger apply transition failed.");
        }
    }

    private static async Task MarkFailedAsync(
        NpgsqlConnection connection,
        MigrationArtifact artifact,
        DateTimeOffset startedAt,
        string errorCode,
        CancellationToken cancellationToken)
    {
        if (connection.FullState != ConnectionState.Open)
        {
            return;
        }

        const string sql = """
            INSERT INTO ops.schema_migration (
                id,
                migration_name,
                checksum,
                transaction_mode,
                status,
                started_at,
                applied_at,
                runner_version,
                error_code)
            VALUES ($1, $2, $3, $4, 'FAILED', $5, NULL, $6, $7)
            ON CONFLICT (migration_name) DO UPDATE
            SET status = 'FAILED',
                started_at = EXCLUDED.started_at,
                applied_at = NULL,
                runner_version = EXCLUDED.runner_version,
                error_code = EXCLUDED.error_code
            WHERE ops.schema_migration.status <> 'APPLIED';
            """;

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue(artifact.Id);
        command.Parameters.AddWithValue(artifact.Definition.MigrationName);
        command.Parameters.AddWithValue(artifact.Definition.ScriptChecksum);
        command.Parameters.AddWithValue(artifact.Definition.TransactionMode);
        command.Parameters.AddWithValue(startedAt);
        command.Parameters.AddWithValue(GetRunnerVersion());
        command.Parameters.AddWithValue(errorCode);
        _ = await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static string GetErrorCode(Exception exception)
    {
        string errorCode = exception is PostgresException postgresException
            ? postgresException.SqlState
            : exception.GetType().Name;
        return errorCode[..Math.Min(errorCode.Length, 64)];
    }

    private static string GetRunnerVersion()
    {
        return typeof(SqlMigrationRunner).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion.Split('+', 2)[0]
            ?? "1.0.0";
    }
}
