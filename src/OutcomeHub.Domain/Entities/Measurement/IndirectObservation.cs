namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class IndirectObservation
{
    private IndirectObservation()
    {
    }

    public Guid Id { get; private set; }

    public Guid ResponseBatchId { get; private set; }

    public Guid InstrumentVersionId { get; private set; }

    public Guid ProgramVersionId { get; private set; }

    public Guid ItemId { get; private set; }

    public string RespondentKey { get; private set; } = null!;

    public Guid? StudentId { get; private set; }

    public decimal RawValue { get; private set; }

    public decimal MaxValue { get; private set; }

    public string? GroupDimension { get; private set; }

    public DateTimeOffset RecordedAt { get; private set; }

    public IndirectResponseBatch ResponseBatch { get; private set; } = null!;
    public IndirectInstrumentVersion InstrumentVersion { get; private set; } = null!;
    public IndirectItem Item { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Student? Student { get; private set; }
}
