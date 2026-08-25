namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class PolicyThreshold
{
    private PolicyThreshold()
    {
    }

    public Guid PolicyVersionId { get; private set; }

    public string OutcomeLevel { get; private set; } = null!;

    public decimal ThetaInd { get; private set; }

    public decimal ThetaCoh { get; private set; }

    public decimal? NearThreshold { get; private set; }

    public int MinSampleSize { get; private set; }

    public CalculationPolicyVersion PolicyVersion { get; private set; } = null!;
}
