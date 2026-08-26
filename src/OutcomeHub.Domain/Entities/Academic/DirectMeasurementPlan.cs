using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Academic;

public sealed class DirectMeasurementPlan
{
    private DirectMeasurementPlan() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid CurriculumPathId { get; private set; }
    public Guid ProgramPiId { get; private set; }
    public int VersionNo { get; private set; }
    public string Status { get; private set; } = null!;
    public Guid WorkflowInstanceId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public Guid? SupersedesId { get; private set; }
    public string Checksum { get; private set; } = null!;

    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public CurriculumPath CurriculumPath { get; private set; } = null!;
    public ProgramPi ProgramPi { get; private set; } = null!;
    public WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public DirectMeasurementPlan? Supersedes { get; private set; }

    public static DirectMeasurementPlan Create(
        Guid id,
        Guid programVersionId,
        Guid curriculumPathId,
        Guid programPiId,
        int versionNo,
        string status,
        Guid workflowInstanceId,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo = null,
        Guid? supersedesId = null,
        string? checksum = null)
    {
        return new DirectMeasurementPlan
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            ProgramVersionId = programVersionId,
            CurriculumPathId = curriculumPathId,
            ProgramPiId = programPiId,
            VersionNo = versionNo <= 0 ? 1 : versionNo,
            Status = string.IsNullOrWhiteSpace(status) ? "ACTIVE" : status.Trim().ToUpperInvariant(),
            WorkflowInstanceId = workflowInstanceId,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            SupersedesId = supersedesId,
            Checksum = string.IsNullOrWhiteSpace(checksum) ? new string('0', 64) : checksum.Trim()
        };
    }
}
