using OutcomeHub.Migrations;

try
{
    MigrationCommandOptions options = MigrationCommandOptions.Parse(args);
    string connectionString = Environment.GetEnvironmentVariable(
        options.ConnectionEnvironmentVariable)
        ?? throw new InvalidOperationException(
            $"Missing connection string environment variable '{options.ConnectionEnvironmentVariable}'.");

    var runner = new SqlMigrationRunner(
        connectionString,
        options.MigrationRoot,
        options.LockTimeout);
    MigrationRunResult result = await runner.RunAsync();

    Console.WriteLine(
        $"OutcomeHub migrations complete. Applied: {result.AppliedCount}; skipped: {result.SkippedCount}.");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine($"OutcomeHub migrations failed: {exception.Message}");
    return 1;
}
