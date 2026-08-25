namespace OutcomeHub.Application.DTOs.Academic;

public sealed record CourseDto(
    Guid Id,
    string Code,
    string Name,
    Guid OwnerOrgUnitId,
    string? OwnerOrgUnitName,
    string Status);

public sealed record CourseVersionDto(
    Guid Id,
    Guid CourseId,
    int VersionNo,
    string Name,
    decimal CreditValue,
    string CourseType,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    bool SharedCoreFlag,
    string Status,
    Guid DecisionId,
    Guid WorkflowInstanceId,
    Guid? SupersedesId,
    string Checksum);

public sealed record ProgramCourseDto(
    Guid Id,
    Guid ProgramVersionId,
    Guid CourseVersionId,
    string CourseCode,
    string CourseName,
    Guid CurriculumBlockId,
    string CatalogRole,
    decimal CreditValue,
    decimal? CreditOverride,
    bool IsLocked,
    string Status);

public sealed record CreateCourseRequest(
    string Code,
    string Name,
    Guid OwnerOrgUnitId,
    string Status = "DRAFT");

public sealed record CreateCourseVersionRequest(
    int VersionNo,
    string Name,
    decimal CreditValue,
    string CourseType,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    bool SharedCoreFlag,
    Guid DecisionId,
    Guid WorkflowInstanceId,
    Guid? SupersedesId,
    string? Checksum);

public sealed record AddCourseToProgramRequest(
    Guid CourseVersionId,
    Guid CurriculumBlockId,
    string CatalogRole,
    decimal? CreditOverride);
