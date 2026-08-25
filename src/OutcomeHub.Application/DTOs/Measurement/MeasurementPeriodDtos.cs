namespace OutcomeHub.Application.DTOs.Measurement;

public sealed record MeasurementPeriodDto(
    Guid Id,
    string Code,
    string Name,
    Guid OrgUnitId,
    string OrgUnitName,
    Guid ProgramVersionId,
    string ProgramVersionCode,
    short AcademicYearStart,
    string TermCode,
    string Status,
    Guid ProgramPolicyBindingId,
    Guid WorkflowInstanceId,
    DateTimeOffset? CollectionOpenAt,
    DateTimeOffset? CollectionCloseAt,
    DateTimeOffset? DataCutoffAt,
    IReadOnlyList<MeasurementPeriodCohortDto>? Cohorts = null,
    IReadOnlyList<MeasurementPeriodOfferingDto>? Offerings = null,
    IReadOnlyList<MeasurementPeriodTargetDto>? Targets = null);

public sealed record MeasurementPeriodCohortDto(
    Guid MeasurementPeriodId,
    Guid ProgramVersionId,
    Guid CohortId,
    string CohortCode,
    string CohortName);

public sealed record MeasurementPeriodOfferingDto(
    Guid MeasurementPeriodId,
    Guid ProgramVersionId,
    short AcademicYearStart,
    Guid CourseOfferingId,
    string CourseOfferingCode,
    string CourseName,
    string PlannedSourceRole,
    string CollectionStatus,
    DateTimeOffset? DueAt);

public sealed record MeasurementPeriodTargetDto(
    Guid Id,
    Guid MeasurementPeriodId,
    Guid ProgramVersionId,
    string OutcomeLevel,
    string TargetRole,
    Guid? CourseOfferingId,
    string? CourseOfferingCode,
    Guid? SyllabusVersionId,
    Guid? CloId,
    string? CloCode,
    Guid? ProgramPiId,
    string? ProgramPiCode,
    Guid? ProgramPloId,
    string? ProgramPloCode);

public sealed record CreateMeasurementPeriodRequest(
    string Code,
    string Name,
    Guid OrgUnitId,
    Guid ProgramVersionId,
    short AcademicYearStart,
    string TermCode,
    Guid ProgramPolicyBindingId,
    Guid WorkflowInstanceId,
    string Status = "DRAFT",
    DateTimeOffset? CollectionOpenAt = null,
    DateTimeOffset? CollectionCloseAt = null,
    DateTimeOffset? DataCutoffAt = null);

public sealed record UpdateMeasurementPeriodRequest(
    string Name,
    string Status,
    DateTimeOffset? CollectionOpenAt,
    DateTimeOffset? CollectionCloseAt,
    DateTimeOffset? DataCutoffAt);

public sealed record AttachCohortToPeriodRequest(
    Guid ProgramVersionId,
    Guid CohortId);

public sealed record AttachOfferingToPeriodRequest(
    Guid ProgramVersionId,
    short AcademicYearStart,
    Guid CourseOfferingId,
    string PlannedSourceRole = "OFFICIAL",
    string CollectionStatus = "PENDING",
    DateTimeOffset? DueAt = null);

public sealed record CreatePeriodTargetRequest(
    Guid ProgramVersionId,
    string OutcomeLevel,
    string TargetRole = "PRIMARY",
    Guid? CourseOfferingId = null,
    Guid? SyllabusVersionId = null,
    Guid? CloId = null,
    Guid? ProgramPiId = null,
    Guid? ProgramPloId = null);
