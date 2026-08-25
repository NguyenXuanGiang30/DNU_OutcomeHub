using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Domain.Entities.Document;

public sealed class Document
{
    private Document() { }
    public Guid Id { get; private set; }
    public Guid GovernedResourceId { get; private set; }
    public string DocumentType { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public Guid OwnerOrgUnitId { get; private set; }
    public string Classification { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public GovernedResource GovernedResource { get; private set; } = null!;
    public OrgUnit OwnerOrgUnit { get; private set; } = null!;
    public ICollection<DocumentVersion> Versions { get; private set; } = new List<DocumentVersion>();
}
