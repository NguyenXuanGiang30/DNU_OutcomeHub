namespace OutcomeHub.Application.DTOs.Academic;

public sealed record CourseOfferingDto(
    Guid Id,
    string Code,
    Guid ProgramCourseId,
    Guid CourseVersionId,
    string CourseVersionName,
    Guid ProgramVersionId,
    Guid SyllabusVersionId,
    short AcademicYearStart,
    string TermCode,
    Guid OrgUnitId,
    string OrgUnitName,
    string Status,
    DateOnly StartDate,
    DateOnly EndDate,
    IReadOnlyList<CourseOfferingInstructorDto>? Instructors = null);

public sealed record CourseOfferingInstructorDto(
    Guid Id,
    Guid CourseOfferingId,
    Guid StaffId,
    string StaffCode,
    string StaffFullName,
    string AssignmentRole,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsPrimary);

public sealed record CreateCourseOfferingRequest(
    string Code,
    Guid ProgramCourseId,
    Guid CourseVersionId,
    Guid ProgramVersionId,
    Guid SyllabusVersionId,
    short AcademicYearStart,
    string TermCode,
    Guid OrgUnitId,
    DateOnly StartDate,
    DateOnly EndDate,
    string Status = "PLANNED",
    Guid? SourceSystemId = null,
    string? SourceRecordId = null);

public sealed record UpdateCourseOfferingRequest(
    string Status,
    DateOnly StartDate,
    DateOnly EndDate);

public sealed record AssignInstructorRequest(
    Guid StaffId,
    string AssignmentRole,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo = null,
    bool IsPrimary = false);
