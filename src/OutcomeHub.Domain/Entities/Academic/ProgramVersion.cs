using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Academic;

public sealed class ProgramVersion
{
    private ProgramVersion() { }

    public Guid Id { get; private set; }
    public Guid ProgramId { get; private set; }
    public Guid InstitutionTemplateVersionId { get; private set; }
    public int VersionNo { get; private set; }
    public string Code { get; private set; } = null!;
    public Guid DecisionId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string Status { get; private set; } = null!;
    public decimal TotalCredits { get; private set; }
    public Guid WorkflowInstanceId { get; private set; }
    public Guid? SupersedesId { get; private set; }
    public string Checksum { get; private set; } = null!;
    public long RowVersion { get; private set; }

    public Program Program { get; private set; } = null!;
    public InstitutionTemplateVersion InstitutionTemplateVersion { get; private set; } = null!;
    public DecisionRecord Decision { get; private set; } = null!;
    public WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public ProgramVersion? Supersedes { get; private set; }

    public static ProgramVersion Create(
        Guid id,
        Guid programId,
        Guid institutionTemplateVersionId,
        int versionNo,
        string code,
        Guid decisionId,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo,
        string status,
        decimal totalCredits,
        Guid workflowInstanceId,
        Guid? supersedesId,
        string checksum)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(checksum);

        return new ProgramVersion
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            ProgramId = programId,
            InstitutionTemplateVersionId = institutionTemplateVersionId,
            VersionNo = versionNo <= 0 ? 1 : versionNo,
            Code = code.Trim().ToUpperInvariant(),
            DecisionId = decisionId,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            Status = string.IsNullOrWhiteSpace(status) ? "DRAFT" : status.Trim().ToUpperInvariant(),
            TotalCredits = totalCredits,
            WorkflowInstanceId = workflowInstanceId,
            SupersedesId = supersedesId,
            Checksum = checksum.Trim(),
            RowVersion = 1,
        };
    }

    public void Publish()
    {
        Status = "PUBLISHED";
    }
}
