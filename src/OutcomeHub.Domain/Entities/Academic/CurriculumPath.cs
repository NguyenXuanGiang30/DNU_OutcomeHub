using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CurriculumPath
{
    private CurriculumPath() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string PathType { get; private set; } = null!;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public bool IsDefault { get; private set; }
    public Guid WorkflowInstanceId { get; private set; }

    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public WorkflowInstance WorkflowInstance { get; private set; } = null!;

    public static CurriculumPath Create(
        Guid id,
        Guid programVersionId,
        string code,
        string name,
        string pathType,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo,
        bool isDefault,
        Guid workflowInstanceId)
    {
        return new CurriculumPath
        {
            Id = id,
            ProgramVersionId = programVersionId,
            Code = code,
            Name = name,
            PathType = pathType,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            IsDefault = isDefault,
            WorkflowInstanceId = workflowInstanceId
        };
    }
}
