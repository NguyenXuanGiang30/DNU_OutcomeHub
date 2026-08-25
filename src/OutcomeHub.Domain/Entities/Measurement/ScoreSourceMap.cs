namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class ScoreSourceMap
{
    private ScoreSourceMap()
    {
    }

    public Guid SourceSystemId { get; private set; }

    public string SourceRecordId { get; private set; } = null!;

    public string SourceRevision { get; private set; } = null!;

    public short AcademicYearStart { get; private set; }

    public Guid ScoreRecordId { get; private set; }

    public string PayloadChecksum { get; private set; } = null!;

    public OutcomeHub.Domain.Entities.Integration.SourceSystem SourceSystem { get; private set; } = null!;
    public ScoreRecord ScoreRecord { get; private set; } = null!;
}
