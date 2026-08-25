namespace OutcomeHub.Domain.Entities.Workflow;

public sealed class WorkflowDefinition
{
    private readonly List<WorkflowInstance> _instances = [];

    private WorkflowDefinition()
    {
    }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public int VersionNo { get; private set; }
    public string SubjectType { get; private set; } = null!;
    public string Configuration { get; private set; } = null!;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string Status { get; private set; } = null!;
    public string Checksum { get; private set; } = null!;

    public IReadOnlyCollection<WorkflowInstance> Instances => _instances;
}
