using OutcomeHub.Domain.Entities.Audit;
using OutcomeHub.Domain.Entities.Document;
using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Quality;
using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class AiSourceSnapshot
{
    private AiSourceSnapshot()
    {
    }

    public Guid Id { get; private set; }

    public Guid GovernedResourceId { get; private set; }

    public string SourceKind { get; private set; } = null!;

    public Guid SourceGovernedResourceId { get; private set; }

    public Guid? DocumentVersionId { get; private set; }

    public Guid? ResultBatchId { get; private set; }

    public Guid? ExportManifestId { get; private set; }

    public Guid? ImprovementPlanId { get; private set; }

    public string SourceChecksum { get; private set; } = null!;

    public DateTimeOffset DataAsOf { get; private set; }

    public string ScopeSnapshotChecksum { get; private set; } = null!;

    public string PermissionSnapshotChecksum { get; private set; } = null!;

    public string SnapshotPayloadReference { get; private set; } = null!;

    public GovernedResource GovernedResource { get; private set; } = null!;

    public GovernedResource SourceGovernedResource { get; private set; } = null!;

    public DocumentVersion? DocumentVersion { get; private set; }

    public ResultBatch? ResultBatch { get; private set; }

    public ExportManifest? ExportManifest { get; private set; }

    public ImprovementPlan? ImprovementPlan { get; private set; }
}
