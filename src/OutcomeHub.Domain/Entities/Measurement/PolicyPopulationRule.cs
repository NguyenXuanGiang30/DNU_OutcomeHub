namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class PolicyPopulationRule
{
    private PolicyPopulationRule()
    {
    }

    public Guid PolicyVersionId { get; private set; }

    public string EnrollmentStatus { get; private set; } = null!;

    public string DenominatorAction { get; private set; } = null!;

    public CalculationPolicyVersion PolicyVersion { get; private set; } = null!;
}
