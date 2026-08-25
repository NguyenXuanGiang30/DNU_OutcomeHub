using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Iam;

public sealed class SodPolicyVersion
{
    private readonly List<SodRule> _rules = [];

    private SodPolicyVersion()
    {
    }

    public Guid Id { get; private set; }
    public int VersionNo { get; private set; }
    public string Status { get; private set; } = null!;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public Guid WorkflowInstanceId { get; private set; }
    public string Checksum { get; private set; } = null!;

    public WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public IReadOnlyCollection<SodRule> Rules => _rules;
}
