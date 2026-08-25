namespace OutcomeHub.Domain.Entities.Ai;

public sealed class AiJobInput
{
    private AiJobInput()
    {
    }

    public Guid AiJobId { get; private set; }

    public int SequenceNo { get; private set; }

    public Guid SourceSnapshotId { get; private set; }

    public string InputRole { get; private set; } = null!;

    public string SourceChecksum { get; private set; } = null!;

    public AiJob AiJob { get; private set; } = null!;

    public AiSourceSnapshot SourceSnapshot { get; private set; } = null!;
}
