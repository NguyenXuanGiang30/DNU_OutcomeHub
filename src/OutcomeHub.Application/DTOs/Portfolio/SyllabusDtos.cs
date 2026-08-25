namespace OutcomeHub.Application.DTOs.Portfolio;

public sealed record SyllabusDto(
    Guid Id,
    Guid ProgramCourseId,
    string Code,
    Guid OwnerOrgUnitId,
    DateTimeOffset CreatedAt,
    int VersionCount);

public sealed record SyllabusVersionDto(
    Guid Id,
    Guid SyllabusId,
    Guid ProgramCourseId,
    Guid ProgramVersionId,
    Guid InstitutionTemplateVersionId,
    Guid CourseVersionId,
    Guid SyllabusTemplateVersionId,
    int VersionNo,
    DateOnly ApplicableFrom,
    DateOnly? ApplicableTo,
    string Status,
    string ContentChecksum);

public sealed record CreateSyllabusRequest(
    Guid ProgramCourseId,
    string Code,
    Guid OwnerOrgUnitId);

public sealed record CreateSyllabusVersionRequest(
    Guid InstitutionTemplateVersionId,
    Guid CourseVersionId,
    Guid SyllabusTemplateVersionId,
    int VersionNo,
    DateOnly ApplicableFrom,
    DateOnly? ApplicableTo,
    Guid? SharedSyllabusCoreVersionId,
    Guid? WorkflowInstanceId,
    Guid? SupersedesId,
    string? ContentChecksum);
