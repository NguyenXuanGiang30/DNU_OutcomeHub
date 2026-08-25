namespace OutcomeHub.Application.DTOs.Measurement;

public sealed record EnrollmentDto(
    Guid Id,
    Guid CourseOfferingId,
    string CourseOfferingCode,
    Guid StudentId,
    string StudentCode,
    string StudentFullName,
    short AttemptNo,
    Guid SourceSystemId,
    string SourceRecordId,
    string? CurrentStatus,
    bool RepeatFlag,
    bool ImprovementFlag,
    DateTimeOffset? EffectiveFrom);

public sealed record CreateEnrollmentRequest(
    Guid CourseOfferingId,
    Guid StudentId,
    short AttemptNo,
    Guid SourceSystemId,
    string SourceRecordId,
    string EnrollmentStatus = "ENROLLED",
    bool RepeatFlag = false,
    bool ImprovementFlag = false,
    DateTimeOffset? EffectiveFrom = null);
