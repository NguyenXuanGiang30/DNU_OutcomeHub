namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class DirectPiCriterionWeight
{
    private DirectPiCriterionWeight() { }
    public Guid Id { get; private set; }
    public Guid SyllabusTraceabilityId { get; private set; }
    public decimal DirectWeightRatio { get; private set; }
    public bool IsCoreGate { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public SyllabusTraceability SyllabusTraceability { get; private set; } = null!;
}
