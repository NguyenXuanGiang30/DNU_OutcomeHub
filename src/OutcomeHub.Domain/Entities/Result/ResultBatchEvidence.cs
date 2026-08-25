namespace OutcomeHub.Domain.Entities.Result;

public sealed class ResultBatchEvidence
{
    private ResultBatchEvidence()
    {
    }

    public Guid BatchId { get; private set; }

    public Guid EvidenceVersionId { get; private set; }

    public string LinkRole { get; private set; } = null!;

    public ResultBatch Batch { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Document.EvidenceVersion EvidenceVersion { get; private set; } = null!;
}
