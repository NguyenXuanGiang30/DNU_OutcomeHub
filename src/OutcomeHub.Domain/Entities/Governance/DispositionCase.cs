using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Governance;

public sealed class DispositionCase
{
    private DispositionCase() { }
    public Guid Id { get; private set; }
    public string CaseCode { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public string RequestedAction { get; private set; } = null!;
    public Guid? ApprovedBy { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public string? DisposalCertificateChecksum { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public Principal Creator { get; private set; } = null!;
    public Principal? Approver { get; private set; }
    public ICollection<DispositionItem> Items { get; private set; } = new List<DispositionItem>();
}
