namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class IndirectResponseBatch
{
    private IndirectResponseBatch()
    {
    }

    public Guid Id { get; private set; }

    public Guid InstrumentVersionId { get; private set; }

    public Guid MeasurementPeriodId { get; private set; }

    public Guid ProgramVersionId { get; private set; }

    public string Status { get; private set; } = null!;

    public string Checksum { get; private set; } = null!;

    public IndirectInstrumentVersion InstrumentVersion { get; private set; } = null!;
    public MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramVersion ProgramVersion { get; private set; } = null!;
    public ICollection<IndirectObservation> Observations { get; private set; } = new List<IndirectObservation>();
}
