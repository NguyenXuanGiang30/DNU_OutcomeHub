namespace OutcomeHub.Application.DTOs.Academic;

public sealed record ProgramDto(
    Guid Id,
    string Code,
    string Name,
    string DegreeLevel,
    string EducationMode,
    Guid OwnerOrgUnitId,
    string OwnerOrgUnitName,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateProgramRequest(
    string Code,
    string Name,
    string DegreeLevel,
    string EducationMode,
    Guid OwnerOrgUnitId,
    string Status = "DRAFT");

public sealed record UpdateProgramRequest(
    string Name,
    string DegreeLevel,
    string EducationMode,
    string Status);

public sealed record ProgramVersionDto(
    Guid Id,
    Guid ProgramId,
    int VersionNo,
    string Code,
    Guid InstitutionTemplateVersionId,
    Guid DecisionId,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    string Status,
    decimal TotalCredits,
    string Checksum,
    long RowVersion);

public sealed record CreateProgramVersionRequest(
    int VersionNo,
    string Code,
    Guid InstitutionTemplateVersionId,
    Guid DecisionId,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    decimal TotalCredits,
    Guid WorkflowInstanceId,
    Guid? SupersedesId = null);
