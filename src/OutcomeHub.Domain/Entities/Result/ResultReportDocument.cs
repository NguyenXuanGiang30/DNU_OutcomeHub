namespace OutcomeHub.Domain.Entities.Result;

public sealed class ResultReportDocument
{
    private ResultReportDocument()
    {
    }

    public Guid BatchId { get; private set; }

    public Guid DocumentVersionId { get; private set; }

    public string ReportType { get; private set; } = null!;

    public string FilterChecksum { get; private set; } = null!;

    public ResultBatch Batch { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Document.DocumentVersion DocumentVersion { get; private set; } = null!;
}
