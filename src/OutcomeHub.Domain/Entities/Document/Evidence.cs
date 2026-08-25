using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Document;

public sealed class Evidence
{
    private Evidence() { }
    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string EvidenceType { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public Guid OwnerPrincipalId { get; private set; }
    public Guid OwnerOrgUnitId { get; private set; }
    public string Classification { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public Principal OwnerPrincipal { get; private set; } = null!;
    public OrgUnit OwnerOrgUnit { get; private set; } = null!;
    public ICollection<EvidenceVersion> Versions { get; private set; } = new List<EvidenceVersion>();
}
