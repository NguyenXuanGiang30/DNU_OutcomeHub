namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class SnapshotPiPloWeight
{
    private SnapshotPiPloWeight()
    {
    }

    public Guid InputSnapshotId { get; private set; }

    public Guid ProgramPiId { get; private set; }

    public Guid ProgramPloId { get; private set; }

    public decimal PiWeightRatio { get; private set; }

    public bool IsCore { get; private set; }

    public Guid SourceProgramPiId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public InputSnapshot InputSnapshot { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramPi ProgramPi { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramPlo ProgramPlo { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramPi SourceProgramPi { get; private set; } = null!;
}
