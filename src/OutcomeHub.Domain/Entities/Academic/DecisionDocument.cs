using OutcomeHub.Domain.Entities.Document;

namespace OutcomeHub.Domain.Entities.Academic;

public sealed class DecisionDocument
{
    private DecisionDocument() { }

    public Guid DecisionRecordId { get; private set; }
    public Guid DocumentVersionId { get; private set; }
    public string DocumentRole { get; private set; } = null!;

    public DecisionRecord DecisionRecord { get; private set; } = null!;
    public DocumentVersion DocumentVersion { get; private set; } = null!;
}
