namespace OutcomeHub.Application.DTOs.Academic;

public sealed record CohortDto(
    Guid Id,
    Guid ProgramId,
    string ProgramCode,
    string ProgramName,
    string Code,
    string Name,
    int AdmissionYear,
    DateOnly StartDate,
    DateOnly? EndDate);

public sealed record CreateCohortRequest(
    Guid ProgramId,
    string Code,
    string Name,
    int AdmissionYear,
    DateOnly StartDate,
    DateOnly? EndDate = null);

public sealed record UpdateCohortRequest(
    string Name,
    DateOnly? EndDate);
