using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Document;

public sealed class EvidenceVersion
{
    private EvidenceVersion() { }
    public Guid Id { get; private set; }
    public Guid GovernedResourceId { get; private set; }
    public Guid EvidenceId { get; private set; }
    public int VersionNo { get; private set; }
    public Guid? DocumentVersionId { get; private set; }
    public string? ExternalUrl { get; private set; }
    public Guid? UrlSnapshotFileObjectId { get; private set; }
    public string? SystemRecordReference { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset CollectedAt { get; private set; }
    public string Checksum { get; private set; } = null!;
    public string? Metadata { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public GovernedResource GovernedResource { get; private set; } = null!;
    public Evidence Evidence { get; private set; } = null!;
    public DocumentVersion? DocumentVersion { get; private set; }
    public FileObject? UrlSnapshotFileObject { get; private set; }
    public Principal? Approver { get; private set; }
    public Principal Creator { get; private set; } = null!;
}
