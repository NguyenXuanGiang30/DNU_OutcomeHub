using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Academic;

public sealed class InstitutionTemplateVersion
{
    private InstitutionTemplateVersion() { }

    public Guid Id { get; private set; }
    public Guid InstitutionTemplateId { get; private set; }
    public int VersionNo { get; private set; }
    public Guid DecisionId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string Status { get; private set; } = null!;
    public string LayoutConfiguration { get; private set; } = null!;
    public string PolicyConfiguration { get; private set; } = null!;
    public Guid WorkflowInstanceId { get; private set; }
    public string Checksum { get; private set; } = null!;
    public Guid? SupersedesId { get; private set; }

    public InstitutionTemplate InstitutionTemplate { get; private set; } = null!;
    public DecisionRecord Decision { get; private set; } = null!;
    public WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public InstitutionTemplateVersion? Supersedes { get; private set; }
}
