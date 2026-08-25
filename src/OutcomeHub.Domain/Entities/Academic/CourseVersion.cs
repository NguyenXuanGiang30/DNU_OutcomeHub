using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CourseVersion
{
    private CourseVersion() { }

    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public int VersionNo { get; private set; }
    public string Name { get; private set; } = null!;
    public decimal CreditValue { get; private set; }
    public string CourseType { get; private set; } = null!;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public bool SharedCoreFlag { get; private set; }
    public string Status { get; private set; } = null!;
    public Guid DecisionId { get; private set; }
    public Guid WorkflowInstanceId { get; private set; }
    public Guid? SupersedesId { get; private set; }
    public string Checksum { get; private set; } = null!;

    public Course Course { get; private set; } = null!;
    public DecisionRecord Decision { get; private set; } = null!;
    public WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public CourseVersion? Supersedes { get; private set; }

    public static CourseVersion Create(
        Guid id,
        Guid courseId,
        int versionNo,
        string name,
        decimal creditValue,
        string courseType,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo,
        bool sharedCoreFlag,
        string status,
        Guid decisionId,
        Guid workflowInstanceId,
        Guid? supersedesId,
        string checksum)
    {
        return new CourseVersion
        {
            Id = id,
            CourseId = courseId,
            VersionNo = versionNo,
            Name = name.Trim(),
            CreditValue = creditValue,
            CourseType = courseType.Trim().ToUpperInvariant(),
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            SharedCoreFlag = sharedCoreFlag,
            Status = status.Trim().ToUpperInvariant(),
            DecisionId = decisionId,
            WorkflowInstanceId = workflowInstanceId,
            SupersedesId = supersedesId,
            Checksum = checksum.ToLowerInvariant(),
        };
    }
}
