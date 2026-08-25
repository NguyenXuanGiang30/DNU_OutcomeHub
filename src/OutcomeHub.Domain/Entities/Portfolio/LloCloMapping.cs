namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class LloCloMapping
{
    private LloCloMapping() { }
    public Guid LloId { get; private set; }
    public Guid CloId { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public decimal ContributionRatio { get; private set; }
    public string? Rationale { get; private set; }
    public Llo Llo { get; private set; } = null!;
    public Clo Clo { get; private set; } = null!;
}
