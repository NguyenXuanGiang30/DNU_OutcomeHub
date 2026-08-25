namespace OutcomeHub.Application.DTOs.Academic;

public sealed record StudentDto(
    Guid PersonId,
    string StudentCode,
    string FullName,
    Guid AdmissionCohortId,
    string AdmissionCohortCode,
    string AdmissionCohortName,
    string CurrentStatus,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    IReadOnlyList<StudentPathDto>? Paths = null);

public sealed record StudentPathDto(
    Guid Id,
    Guid StudentId,
    Guid ProgramId,
    string ProgramCode,
    string ProgramName,
    Guid ProgramVersionId,
    string ProgramVersionCode,
    Guid CurriculumPathId,
    string PathStatus,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    Guid? DecisionId,
    bool IsPrimary);

public sealed record CreateStudentRequest(
    string StudentCode,
    string FullName,
    Guid AdmissionCohortId,
    DateOnly EffectiveFrom,
    string CurrentStatus = "ACTIVE",
    Guid? SourceSystemId = null,
    string? SourcePersonId = null);

public sealed record UpdateStudentRequest(
    string FullName,
    string CurrentStatus,
    DateOnly? EffectiveTo);

public sealed record AssignStudentPathRequest(
    Guid ProgramId,
    Guid ProgramVersionId,
    Guid CurriculumPathId,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo = null,
    string PathStatus = "ACTIVE",
    Guid? DecisionId = null,
    bool IsPrimary = true);
