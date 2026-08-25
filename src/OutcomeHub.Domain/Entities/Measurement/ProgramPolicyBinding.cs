namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class ProgramPolicyBinding
{
    private ProgramPolicyBinding()
    {
    }

    public Guid Id { get; private set; }

    public Guid ProgramVersionId { get; private set; }

    public Guid PolicyVersionId { get; private set; }

    public DateOnly EffectiveFrom { get; private set; }

    public DateOnly? EffectiveTo { get; private set; }

    public string Status { get; private set; } = null!;

    public Guid DecisionId { get; private set; }

    public Guid WorkflowInstanceId { get; private set; }

    public string Checksum { get; private set; } = null!;

    public OutcomeHub.Domain.Entities.Academic.ProgramVersion ProgramVersion { get; private set; } = null!;
    public CalculationPolicyVersion PolicyVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.DecisionRecord Decision { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Workflow.WorkflowInstance WorkflowInstance { get; private set; } = null!;
}
