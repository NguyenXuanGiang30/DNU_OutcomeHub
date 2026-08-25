using System.Net;
using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Iam;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Domain.Entities.Audit;

public sealed class AuditEvent
{
    private AuditEvent() { }

    public DateTimeOffset OccurredAt { get; private set; }
    public Guid Id { get; private set; }
    public Guid? RequestId { get; private set; }
    public Guid? CorrelationId { get; private set; }
    public string? TraceId { get; private set; }
    public Guid? ActorPrincipalId { get; private set; }
    public string ActorKind { get; private set; } = null!;
    public Guid? ImpersonatorPrincipalId { get; private set; }
    public string Action { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public string Outcome { get; private set; } = null!;
    public string ResourceType { get; private set; } = null!;
    public Guid? ResourceId { get; private set; }
    public long? ResourceVersion { get; private set; }
    public Guid? OrgUnitId { get; private set; }
    public Guid? ProgramId { get; private set; }
    public Guid? ProgramVersionId { get; private set; }
    public Guid? CohortId { get; private set; }
    public Guid? CurriculumPathId { get; private set; }
    public Guid? CourseId { get; private set; }
    public Guid? CourseOfferingId { get; private set; }
    public Guid? MeasurementPeriodId { get; private set; }
    public Guid? StudentId { get; private set; }
    public string? Purpose { get; private set; }
    public string? Reason { get; private set; }
    public string Classification { get; private set; } = null!;
    public IPAddress? IpAddress { get; private set; }
    public string? UserAgentHash { get; private set; }
    public string? AuthMethod { get; private set; }
    public string? BeforeData { get; private set; }
    public string? AfterData { get; private set; }
    public string? Metadata { get; private set; }
    public Guid ChainId { get; private set; }
    public long ChainSequence { get; private set; }
    public string? PreviousHash { get; private set; }
    public string EventHash { get; private set; } = null!;
    public string HashAlgorithm { get; private set; } = null!;
    public int CanonicalizationVersion { get; private set; }

    public Principal? ActorPrincipal { get; private set; }
    public Principal? ImpersonatorPrincipal { get; private set; }
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
