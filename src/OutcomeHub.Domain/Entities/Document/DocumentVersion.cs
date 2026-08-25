using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Iam;
using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Document;

public sealed class DocumentVersion
{
    private DocumentVersion() { }
    public Guid Id { get; private set; }
    public Guid GovernedResourceId { get; private set; }
    public Guid DocumentId { get; private set; }
    public int VersionNo { get; private set; }
    public Guid FileObjectId { get; private set; }
    public Guid? SourceDocumentVersionId { get; private set; }
    public string? GenerationProvenance { get; private set; }
    public string? StructuredContent { get; private set; }
    public string? ContentSchemaVersion { get; private set; }
    public string? Metadata { get; private set; }
    public string Checksum { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public Guid? WorkflowInstanceId { get; private set; }
    public Guid? SupersedesId { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public GovernedResource GovernedResource { get; private set; } = null!;
    public Document Document { get; private set; } = null!;
    public FileObject FileObject { get; private set; } = null!;
    public DocumentVersion? SourceDocumentVersion { get; private set; }
    public DocumentVersion? Supersedes { get; private set; }
    public WorkflowInstance? WorkflowInstance { get; private set; }
    public Principal? Approver { get; private set; }
    public Principal Creator { get; private set; } = null!;
    public ICollection<DocumentRendition> Renditions { get; private set; } = new List<DocumentRendition>();
}
