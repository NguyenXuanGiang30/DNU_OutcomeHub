namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class SnapshotIndirectObservation
{
    private SnapshotIndirectObservation()
    {
    }

    public Guid InputSnapshotId { get; private set; }

    public Guid IndirectObservationId { get; private set; }

    public Guid ItemId { get; private set; }

    public Guid? ProgramPiId { get; private set; }

    public Guid? ProgramPloId { get; private set; }

    public decimal RawValue { get; private set; }

    public decimal MaxValue { get; private set; }

    public decimal NormalizedValue { get; private set; }

    public string SourceChecksum { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public InputSnapshot InputSnapshot { get; private set; } = null!;
    public IndirectObservation IndirectObservation { get; private set; } = null!;
    public IndirectItem Item { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramPi? ProgramPi { get; private set; }
    public OutcomeHub.Domain.Entities.Academic.ProgramPlo? ProgramPlo { get; private set; }
}
