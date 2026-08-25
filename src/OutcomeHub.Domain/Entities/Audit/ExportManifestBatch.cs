using OutcomeHub.Domain.Entities.Result;

namespace OutcomeHub.Domain.Entities.Audit;

public sealed class ExportManifestBatch
{
    private ExportManifestBatch() { }

    public Guid ExportManifestId { get; private set; }
    public Guid ResultBatchId { get; private set; }

    public ExportManifest ExportManifest { get; private set; } = null!;
    public ResultBatch ResultBatch { get; private set; } = null!;
}
