using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Integration;

public sealed class WebhookSubscription
{
    private readonly List<WebhookSubscriptionEvent> _events = [];
    private readonly List<WebhookDelivery> _deliveries = [];

    private WebhookSubscription() { }

    public Guid Id { get; private set; }
    public Guid PrincipalId { get; private set; }
    public Guid AccessScopeId { get; private set; }
    public string EndpointUrl { get; private set; } = null!;
    public string SecretReference { get; private set; } = null!;
    public string SigningAlgorithm { get; private set; } = null!;
    public int KeyVersion { get; private set; }
    public string Status { get; private set; } = null!;
    public DateTimeOffset? VerifiedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }

    public Principal Principal { get; private set; } = null!;
    public AccessScope AccessScope { get; private set; } = null!;
    public IReadOnlyCollection<WebhookSubscriptionEvent> Events => _events;
    public IReadOnlyCollection<WebhookDelivery> Deliveries => _deliveries;
}
