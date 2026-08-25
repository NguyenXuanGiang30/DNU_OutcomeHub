namespace OutcomeHub.Migrations;

internal sealed record MigrationCommandOptions(
    string ConnectionEnvironmentVariable,
    string MigrationRoot,
    TimeSpan LockTimeout)
{
    public static MigrationCommandOptions Parse(string[] args)
    {
        string connectionEnvironmentVariable = "OUTCOMEHUB_MIGRATIONS_CONNECTION_STRING";
        string migrationRoot = Path.Combine(AppContext.BaseDirectory, "Sql");
        var lockTimeout = TimeSpan.FromSeconds(60);

        for (int index = 0; index < args.Length; index += 2)
        {
            if (index + 1 >= args.Length)
            {
                throw new ArgumentException($"Missing value for argument '{args[index]}'.");
            }

            string value = args[index + 1];
            switch (args[index])
            {
                case "--connection-env":
                    connectionEnvironmentVariable = value;
                    break;
                case "--migrations-dir":
                    migrationRoot = value;
                    break;
                case "--lock-timeout-seconds"
                    when int.TryParse(value, out int seconds) && seconds > 0:
                    lockTimeout = TimeSpan.FromSeconds(seconds);
                    break;
                default:
                    throw new ArgumentException($"Unknown or invalid argument '{args[index]}'.");
            }
        }

        return new MigrationCommandOptions(
            connectionEnvironmentVariable,
            migrationRoot,
            lockTimeout);
    }
}
