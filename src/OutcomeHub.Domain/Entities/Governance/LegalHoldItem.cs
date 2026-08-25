using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Governance;

public sealed class LegalHoldItem
{
    private LegalHoldItem() { }
    public Guid LegalHoldId { get; private set; }
    public Guid GovernedResourceId { get; private set; }
    public DateTimeOffset AddedAt { get; private set; }
    public Guid AddedBy { get; private set; }
    public LegalHold LegalHold { get; private set; } = null!;
    public GovernedResource GovernedResource { get; private set; } = null!;
    public Principal AddedByPrincipal { get; private set; } = null!;
}
