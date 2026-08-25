namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class SnapshotThreshold
{
    private SnapshotThreshold()
    {
    }

    public Guid InputSnapshotId { get; private set; }

    public string OutcomeLevel { get; private set; } = null!;

    public Guid OutcomeKey { get; private set; }

    public Guid? CloId { get; private set; }

    public Guid? ProgramPiId { get; private set; }

    public Guid? ProgramPloId { get; private set; }

    public decimal ThetaInd { get; private set; }

    public decimal ThetaCoh { get; private set; }

    public decimal? NearThreshold { get; private set; }

    public int MinSampleSize { get; private set; }

    public string ThresholdSource { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public InputSnapshot InputSnapshot { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.Clo? Clo { get; private set; }
    public OutcomeHub.Domain.Entities.Academic.ProgramPi? ProgramPi { get; private set; }
    public OutcomeHub.Domain.Entities.Academic.ProgramPlo? ProgramPlo { get; private set; }
}
