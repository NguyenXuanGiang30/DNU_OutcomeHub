namespace OutcomeHub.Domain.Entities.Result;

public sealed class Publication
{
    private Publication()
    {
    }

    public Guid Id { get; private set; }

    public Guid MeasurementPeriodId { get; private set; }

    public Guid BatchId { get; private set; }

    public string PublicationType { get; private set; } = null!;

    public Guid PublishedBy { get; private set; }

    public DateTimeOffset PublishedAt { get; private set; }

    public string? WatermarkTemplate { get; private set; }

    public Guid? DocumentVersionId { get; private set; }

    public OutcomeHub.Domain.Entities.Measurement.MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public ResultBatch Batch { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal PublishedByPrincipal { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Document.DocumentVersion? DocumentVersion { get; private set; }
}
