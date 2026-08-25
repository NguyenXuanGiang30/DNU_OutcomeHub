namespace OutcomeHub.Application.DTOs.Academic;

public sealed record ProgramPiDto(
    Guid Id,
    Guid ProgramVersionId,
    Guid ProgramPloId,
    string Code,
    string Description,
    bool IsLocked,
    bool IsCore,
    decimal? WeightRatio,
    int SortOrder);

public sealed record ProgramPloDto(
    Guid Id,
    Guid ProgramVersionId,
    string Code,
    string Description,
    string Domain,
    string? BloomLevel,
    bool IsLocked,
    int SortOrder,
    IReadOnlyList<ProgramPiDto> PerformanceIndicators);

public sealed record ProgramOutcomeTreeDto(
    Guid ProgramVersionId,
    string ProgramVersionCode,
    IReadOnlyList<ProgramPloDto> Plos);

public sealed record CreateProgramPloRequest(
    string Code,
    string Description,
    string Domain,
    string? BloomLevel = null,
    int SortOrder = 1);

public sealed record CreateProgramPiRequest(
    string Code,
    string Description,
    bool IsCore = false,
    decimal? WeightRatio = null,
    int SortOrder = 1);
