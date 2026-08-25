namespace OutcomeHub.Domain.Entities.Ai;

public sealed class SafetyEvent
{
    private SafetyEvent()
    {
    }

    public Guid Id { get; private set; }

    public Guid AiJobId { get; private set; }

    public string EventType { get; private set; } = null!;

    public string Severity { get; private set; } = null!;

    public string DetectorVersion { get; private set; } = null!;

    public bool Blocked { get; private set; }

    public string DetailsRedacted { get; private set; } = null!;

    public DateTimeOffset OccurredAt { get; private set; }

    public AiJob AiJob { get; private set; } = null!;
}
