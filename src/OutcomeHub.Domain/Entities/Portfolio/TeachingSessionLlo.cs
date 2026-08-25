namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class TeachingSessionLlo
{
    private TeachingSessionLlo() { }
    public Guid TeachingSessionId { get; private set; }
    public Guid LloId { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public TeachingSession TeachingSession { get; private set; } = null!;
    public Llo Llo { get; private set; } = null!;
}
