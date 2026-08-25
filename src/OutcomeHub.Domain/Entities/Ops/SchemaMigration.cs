namespace OutcomeHub.Domain.Entities.Ops;

public sealed class SchemaMigration
{
    private SchemaMigration() { }

    public Guid Id { get; private set; }
    public string MigrationName { get; private set; } = null!;
    public string Checksum { get; private set; } = null!;
    public string TransactionMode { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? AppliedAt { get; private set; }
    public string RunnerVersion { get; private set; } = null!;
    public string? ErrorCode { get; private set; }
}
