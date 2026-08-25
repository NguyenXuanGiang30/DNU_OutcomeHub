using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class ChatSession
{
    private ChatSession()
    {
    }

    public Guid Id { get; private set; }

    public Guid GovernedResourceId { get; private set; }

    public Guid OwnerPrincipalId { get; private set; }

    public Guid AccessScopeId { get; private set; }

    public string Title { get; private set; } = null!;

    public string Status { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset LastActivityAt { get; private set; }

    public GovernedResource GovernedResource { get; private set; } = null!;

    public Principal OwnerPrincipal { get; private set; } = null!;

    public AccessScope AccessScope { get; private set; } = null!;
}
