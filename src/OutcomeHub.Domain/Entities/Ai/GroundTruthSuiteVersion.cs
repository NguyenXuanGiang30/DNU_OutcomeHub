using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class GroundTruthSuiteVersion
{
    private GroundTruthSuiteVersion()
    {
    }

    public Guid Id { get; private set; }

    public Guid GovernedResourceId { get; private set; }

    public Guid SuiteId { get; private set; }

    public int VersionNo { get; private set; }

    public string JobType { get; private set; } = null!;

    public string Classification { get; private set; } = null!;

    public string Status { get; private set; } = null!;

    public Guid WorkflowInstanceId { get; private set; }

    public Guid DecisionId { get; private set; }

    public string Checksum { get; private set; } = null!;

    public DateOnly EffectiveFrom { get; private set; }

    public DateOnly? EffectiveTo { get; private set; }

    public GovernedResource GovernedResource { get; private set; } = null!;

    public GroundTruthSuite Suite { get; private set; } = null!;

    public WorkflowInstance WorkflowInstance { get; private set; } = null!;

    public DecisionRecord Decision { get; private set; } = null!;
}
