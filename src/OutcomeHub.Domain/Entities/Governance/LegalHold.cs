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

    public static LegalHold Create(
        Guid id,
        string code,
        string title,
        string reason,
        DateTimeOffset effectiveFrom,
        Guid createdBy,
        Guid? approvedBy)
    {
        return new LegalHold
        {
            Id = id,
            Code = code.Trim().ToUpperInvariant(),
            Title = title.Trim(),
            Reason = reason.Trim(),
            Status = "ACTIVE",
            EffectiveFrom = effectiveFrom,
            ReleasedAt = null,
            CreatedBy = createdBy,
            ApprovedBy = approvedBy
        };
    }

    public void Release(DateTimeOffset releasedAt)
    {
        Status = "RELEASED";
        ReleasedAt = releasedAt;
    }
}
