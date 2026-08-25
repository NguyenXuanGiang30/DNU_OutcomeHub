using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Academic;

public sealed class SharedCoursePiMapping
{
    private SharedCoursePiMapping() { }

    public Guid Id { get; private set; }
    public Guid CourseVersionId { get; private set; }
    public Guid InstitutionTemplateVersionId { get; private set; }
    public Guid TemplatePiId { get; private set; }
    public int VersionNo { get; private set; }
    public string ContributionLevel { get; private set; } = null!;
    public bool IsDirectAssessment { get; private set; }
    public string Status { get; private set; } = null!;
    public Guid DecisionId { get; private set; }
    public Guid WorkflowInstanceId { get; private set; }
    public string Checksum { get; private set; } = null!;

    public CourseVersion CourseVersion { get; private set; } = null!;
    public InstitutionTemplateVersion InstitutionTemplateVersion { get; private set; } = null!;
    public TemplatePi TemplatePi { get; private set; } = null!;
    public DecisionRecord Decision { get; private set; } = null!;
    public WorkflowInstance WorkflowInstance { get; private set; } = null!;
}
