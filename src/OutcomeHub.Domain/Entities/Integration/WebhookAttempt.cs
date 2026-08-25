namespace OutcomeHub.Domain.Entities.Integration;

public sealed class WebhookAttempt
{
    private WebhookAttempt() { }

    public Guid DeliveryId { get; private set; }
    public int AttemptNo { get; private set; }
    public string Nonce { get; private set; } = null!;
    public string Signature { get; private set; } = null!;
    public DateTimeOffset RequestedAt { get; private set; }
    public int? ResponseStatus { get; private set; }
    public DateTimeOffset? ResponseAt { get; private set; }
    public string? ErrorCode { get; private set; }
    public string? ResponseExcerpt { get; private set; }

    public WebhookDelivery Delivery { get; private set; } = null!;
}
