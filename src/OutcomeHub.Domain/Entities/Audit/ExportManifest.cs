using OutcomeHub.Domain.Entities.Document;
using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Audit;

public sealed class ExportManifest
{
    private readonly List<ExportManifestBatch> _resultBatches = [];

    private ExportManifest() { }

    public Guid Id { get; private set; }
    public Guid GovernedResourceId { get; private set; }
    public Guid RequestedBy { get; private set; }
    public string Purpose { get; private set; } = null!;
    public string CanonicalFilter { get; private set; } = null!;
    public string FilterChecksum { get; private set; } = null!;
    public string ReportDefinitionVersion { get; private set; } = null!;
    public Guid AccessScopeId { get; private set; }
    public string PermissionSnapshotChecksum { get; private set; } = null!;
    public DateTimeOffset DataAsOf { get; private set; }
    public long RowCount { get; private set; }
    public Guid FileObjectId { get; private set; }
    public string? Watermark { get; private set; }
    public string GeneratorVersion { get; private set; } = null!;
    public string Checksum { get; private set; } = null!;
    public string Classification { get; private set; } = null!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public GovernedResource GovernedResource { get; private set; } = null!;
    public Principal RequestedByPrincipal { get; private set; } = null!;
    public AccessScope AccessScope { get; private set; } = null!;
    public FileObject FileObject { get; private set; } = null!;
    public IReadOnlyCollection<ExportManifestBatch> ResultBatches => _resultBatches;
}
