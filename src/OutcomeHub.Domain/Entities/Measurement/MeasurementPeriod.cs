using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Workflow;

namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class MeasurementPeriod
{
    private MeasurementPeriod() { }

    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Guid OrgUnitId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public short AcademicYearStart { get; private set; }
    public string TermCode { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public Guid ProgramPolicyBindingId { get; private set; }
    public Guid WorkflowInstanceId { get; private set; }
    public DateTimeOffset? CollectionOpenAt { get; private set; }
    public DateTimeOffset? CollectionCloseAt { get; private set; }
    public DateTimeOffset? DataCutoffAt { get; private set; }
    public long RowVersion { get; private set; }

    public OrgUnit OrgUnit { get; private set; } = null!;
    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public ProgramPolicyBinding ProgramPolicyBinding { get; private set; } = null!;
    public WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public ICollection<MeasurementPeriodCohort> Cohorts { get; private set; } = new List<MeasurementPeriodCohort>();
    public ICollection<MeasurementPeriodOffering> Offerings { get; private set; } = new List<MeasurementPeriodOffering>();

    public static MeasurementPeriod Create(
        Guid id,
        string code,
        string name,
        Guid orgUnitId,
        Guid programVersionId,
        short academicYearStart,
        string termCode,
        Guid programPolicyBindingId,
        Guid workflowInstanceId,
        string status = "DRAFT",
        DateTimeOffset? collectionOpenAt = null,
        DateTimeOffset? collectionCloseAt = null,
        DateTimeOffset? dataCutoffAt = null)
    {
        return new MeasurementPeriod
        {
            Id = id,
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            OrgUnitId = orgUnitId,
            ProgramVersionId = programVersionId,
            AcademicYearStart = academicYearStart,
            TermCode = termCode.Trim().ToUpperInvariant(),
            Status = status.Trim().ToUpperInvariant(),
            ProgramPolicyBindingId = programPolicyBindingId,
            WorkflowInstanceId = workflowInstanceId,
            CollectionOpenAt = collectionOpenAt,
            CollectionCloseAt = collectionCloseAt,
            DataCutoffAt = dataCutoffAt,
            RowVersion = 1,
        };
    }

    public void Update(
        string name,
        string status,
        DateTimeOffset? collectionOpenAt,
        DateTimeOffset? collectionCloseAt,
        DateTimeOffset? dataCutoffAt)
    {
        Name = name.Trim();
        Status = status.Trim().ToUpperInvariant();
        CollectionOpenAt = collectionOpenAt;
        CollectionCloseAt = collectionCloseAt;
        DataCutoffAt = dataCutoffAt;
    }
}
