namespace OutcomeHub.Domain.Entities.Ops;

public sealed class DeploymentEvent
{
    private DeploymentEvent() { }

    public Guid Id { get; private set; }
    public string ApplicationRelease { get; private set; } = null!;
    public string? MigrationVersionFrom { get; private set; }
    public string? MigrationVersionTo { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public string Actor { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public long? DurationMs { get; private set; }
    public string? LogReference { get; private set; }
}
