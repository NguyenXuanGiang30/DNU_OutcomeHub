namespace OutcomeHub.Domain.Entities.Reporting;

public sealed class RefreshRegistry
{
    private RefreshRegistry()
    {
    }

    public string ViewName { get; private set; } = null!;

    public DateTimeOffset? LastStartedAt { get; private set; }

    public DateTimeOffset? LastCompletedAt { get; private set; }

    public string Status { get; private set; } = null!;

    public string? SourceWatermark { get; private set; }

    public long? RowCount { get; private set; }

    public long? DurationMs { get; private set; }

    public string? Error { get; private set; }
}
