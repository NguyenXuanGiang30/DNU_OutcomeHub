using OutcomeHub.Domain.Entities.Document;

namespace OutcomeHub.Domain.Entities.Academic;

public sealed class DecisionRecord
{
    private DecisionRecord() { }

    public Guid Id { get; private set; }
    public string DecisionNumber { get; private set; } = null!;
    public DateOnly IssuedOn { get; private set; }
    public Guid IssuerOrgUnitId { get; private set; }
    public string Title { get; private set; } = null!;
    public Guid? DocumentVersionId { get; private set; }
    public string Status { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public OrgUnit IssuerOrgUnit { get; private set; } = null!;
    public DocumentVersion? DocumentVersion { get; private set; }
}
