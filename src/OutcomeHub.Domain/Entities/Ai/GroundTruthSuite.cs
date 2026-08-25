namespace OutcomeHub.Domain.Entities.Ai;

public sealed class GroundTruthSuite
{
    private GroundTruthSuite()
    {
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;
}
