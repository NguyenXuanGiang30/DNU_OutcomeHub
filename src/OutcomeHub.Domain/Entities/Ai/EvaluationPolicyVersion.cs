using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class EvaluationPolicyVersion
{
    private EvaluationPolicyVersion()
    {
    }

    public Guid Id { get; private set; }

    public Guid GovernedResourceId { get; private set; }

    public string Code { get; private set; } = null!;

    public int VersionNo { get; private set; }

    public string MetricDefinition { get; private set; } = null!;

    public string ThresholdDefinition { get; private set; } = null!;

    public string AggregationRule { get; private set; } = null!;

    public string SamplingRule { get; private set; } = null!;

    public string Classification { get; private set; } = null!;

    public string Status { get; private set; } = null!;

    public Guid WorkflowInstanceId { get; private set; }

    public Guid DecisionId { get; private set; }

    public string Checksum { get; private set; } = null!;

    public GovernedResource GovernedResource { get; private set; } = null!;

    public WorkflowInstance WorkflowInstance { get; private set; } = null!;

    public DecisionRecord Decision { get; private set; } = null!;
}
