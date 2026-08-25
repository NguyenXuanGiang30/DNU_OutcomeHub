namespace OutcomeHub.Domain.Entities.Ai;

public sealed class GroundTruthCase
{
    private GroundTruthCase()
    {
    }

    public Guid Id { get; private set; }

    public Guid SuiteVersionId { get; private set; }

    public string CaseCode { get; private set; } = null!;

    public Guid InputSourceSnapshotId { get; private set; }

    public string ExpectedOutput { get; private set; } = null!;

    public string AcceptanceRule { get; private set; } = null!;

    public string Classification { get; private set; } = null!;

    public string Checksum { get; private set; } = null!;

    public GroundTruthSuiteVersion SuiteVersion { get; private set; } = null!;

    public AiSourceSnapshot InputSourceSnapshot { get; private set; } = null!;
}
