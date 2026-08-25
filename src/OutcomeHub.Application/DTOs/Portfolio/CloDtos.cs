namespace OutcomeHub.Application.DTOs.Portfolio;

public sealed record CloDto(
    Guid Id,
    Guid SyllabusVersionId,
    string Code,
    string Description,
    string Domain,
    string BloomLevel,
    bool IsCore,
    int SortOrder);

public sealed record CreateCloRequest(
    string Code,
    string Description,
    string Domain,
    string BloomLevel,
    bool IsCore,
    int SortOrder);

public sealed record UpdateCloRequest(
    string Description,
    string Domain,
    string BloomLevel,
    bool IsCore,
    int SortOrder);

public sealed record CoursePiMappingDto(
    Guid Id,
    Guid ProgramVersionId,
    Guid ProgramCourseId,
    string CourseCode,
    string CourseName,
    Guid ProgramPiId,
    string PiCode,
    string ContributionLevel,
    bool IsDirectAssessment,
    string? Rationale,
    string SourceType,
    bool IsLocked);

public sealed record SetCoursePiMappingRequest(
    Guid ProgramCourseId,
    Guid ProgramPiId,
    string ContributionLevel,
    bool IsDirectAssessment,
    string? Rationale,
    string SourceType = "PROGRAM");
