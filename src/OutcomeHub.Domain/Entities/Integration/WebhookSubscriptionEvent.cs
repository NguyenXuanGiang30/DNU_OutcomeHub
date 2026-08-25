namespace OutcomeHub.Domain.Entities.Integration;

public sealed class WebhookSubscriptionEvent
{
    private WebhookSubscriptionEvent() { }

    public Guid SubscriptionId { get; private set; }
    public string EventType { get; private set; } = null!;

    public WebhookSubscription Subscription { get; private set; } = null!;
}
