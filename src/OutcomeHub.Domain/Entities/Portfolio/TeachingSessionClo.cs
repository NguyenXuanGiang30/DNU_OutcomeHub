namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class TeachingSessionClo
{
    private TeachingSessionClo() { }
    public Guid TeachingSessionId { get; private set; }
    public Guid CloId { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public TeachingSession TeachingSession { get; private set; } = null!;
    public Clo Clo { get; private set; } = null!;
}
