using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class SyllabusTemplateVersion
{
    private SyllabusTemplateVersion() { }

    public Guid Id { get; private set; }
    public Guid SyllabusTemplateId { get; private set; }
    public Guid InstitutionTemplateVersionId { get; private set; }
    public int VersionNo { get; private set; }
    public Guid? DecisionId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string Status { get; private set; } = null!;
    public Guid? WorkflowInstanceId { get; private set; }
    public Guid? SupersedesId { get; private set; }
    public string Checksum { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid UpdatedBy { get; private set; }
    public long RowVersion { get; private set; }
    public SyllabusTemplate SyllabusTemplate { get; private set; } = null!;
    public InstitutionTemplateVersion InstitutionTemplateVersion { get; private set; } = null!;
    public DecisionRecord? Decision { get; private set; }
    public WorkflowInstance? WorkflowInstance { get; private set; }
    public SyllabusTemplateVersion? Supersedes { get; private set; }
    public ICollection<SyllabusTemplateVersion> Successors { get; private set; } = new List<SyllabusTemplateVersion>();
    public ICollection<SyllabusTemplateSection> Sections { get; private set; } = new List<SyllabusTemplateSection>();
    public ICollection<SyllabusTemplateRubricScale> RubricScales { get; private set; } = new List<SyllabusTemplateRubricScale>();
}
