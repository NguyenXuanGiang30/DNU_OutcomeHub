namespace OutcomeHub.Domain.Entities.Integration;

public sealed class WebhookDelivery
{
    private readonly List<WebhookAttempt> _attempts = [];

    private WebhookDelivery() { }

    public Guid Id { get; private set; }
    public Guid SubscriptionId { get; private set; }
    public Guid OutboxMessageId { get; private set; }
    public string PayloadChecksum { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public int AttemptCount { get; private set; }
    public DateTimeOffset? NextRetryAt { get; private set; }
    public DateTimeOffset? DeliveredAt { get; private set; }

    public WebhookSubscription Subscription { get; private set; } = null!;
    public OutboxMessage OutboxMessage { get; private set; } = null!;
    public IReadOnlyCollection<WebhookAttempt> Attempts => _attempts;
}
