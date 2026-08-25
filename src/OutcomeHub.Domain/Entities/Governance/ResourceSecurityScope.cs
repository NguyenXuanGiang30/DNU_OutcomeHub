using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Domain.Entities.Governance;

public sealed class ResourceSecurityScope
{
    private ResourceSecurityScope() { }
    public Guid Id { get; private set; }
    public Guid GovernedResourceId { get; private set; }
    public Guid? OrgUnitId { get; private set; }
    public Guid? ProgramId { get; private set; }
    public Guid? ProgramVersionId { get; private set; }
    public Guid? CohortId { get; private set; }
    public Guid? CurriculumPathId { get; private set; }
    public Guid? CourseId { get; private set; }
    public Guid? CourseOfferingId { get; private set; }
    public Guid? MeasurementPeriodId { get; private set; }
    public Guid? StudentId { get; private set; }
    public string Classification { get; private set; } = null!;
    public string DerivationChecksum { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public GovernedResource GovernedResource { get; private set; } = null!;
    public OrgUnit? OrgUnit { get; private set; }
    public Program? Program { get; private set; }
    public ProgramVersion? ProgramVersion { get; private set; }
    public Cohort? Cohort { get; private set; }
    public CurriculumPath? CurriculumPath { get; private set; }
    public Course? Course { get; private set; }
    public CourseOffering? CourseOffering { get; private set; }
    public MeasurementPeriod? MeasurementPeriod { get; private set; }
    public Student? Student { get; private set; }
}
