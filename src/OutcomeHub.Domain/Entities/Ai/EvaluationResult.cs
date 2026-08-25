namespace OutcomeHub.Domain.Entities.Ai;

public sealed class EvaluationResult
{
    private EvaluationResult()
    {
    }

    public Guid Id { get; private set; }

    public Guid RunId { get; private set; }

    public Guid CaseId { get; private set; }

    public string ActualOutput { get; private set; } = null!;

    public decimal FieldPrecision { get; private set; }

    public decimal FieldRecall { get; private set; }

    public decimal CitationAccuracy { get; private set; }

    public bool SchemaValid { get; private set; }

    public bool Passed { get; private set; }

    public string Classification { get; private set; } = null!;

    public string Checksum { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public EvaluationRun Run { get; private set; } = null!;

    public GroundTruthCase Case { get; private set; } = null!;
}
