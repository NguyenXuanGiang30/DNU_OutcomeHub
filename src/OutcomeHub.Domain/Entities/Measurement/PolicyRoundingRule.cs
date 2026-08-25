namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class PolicyRoundingRule
{
    private PolicyRoundingRule()
    {
    }

    public Guid PolicyVersionId { get; private set; }

    public string ResultLevel { get; private set; } = null!;

    public int Scale { get; private set; }

    public string RoundingMode { get; private set; } = null!;

    public CalculationPolicyVersion PolicyVersion { get; private set; } = null!;
}
