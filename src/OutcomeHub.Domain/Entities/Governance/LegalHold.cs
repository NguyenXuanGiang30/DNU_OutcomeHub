using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Governance;

public sealed class LegalHold
{
    private LegalHold() { }
    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Reason { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public DateTimeOffset EffectiveFrom { get; private set; }
    public DateTimeOffset? ReleasedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public Principal Creator { get; private set; } = null!;
    public Principal? Approver { get; private set; }
    public ICollection<LegalHoldItem> Items { get; private set; } = new List<LegalHoldItem>();
}
