namespace OutcomeHub.Domain.Entities.Result;

public sealed class CurrentPublication
{
    private CurrentPublication()
    {
    }

    public Guid MeasurementPeriodId { get; private set; }

    public Guid PublicationId { get; private set; }

    public Guid BatchId { get; private set; }

    public Guid UpdatedBy { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public long RowVersion { get; private set; }

    public OutcomeHub.Domain.Entities.Measurement.MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public Publication Publication { get; private set; } = null!;
    public ResultBatch Batch { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal UpdatedByPrincipal { get; private set; } = null!;
}
