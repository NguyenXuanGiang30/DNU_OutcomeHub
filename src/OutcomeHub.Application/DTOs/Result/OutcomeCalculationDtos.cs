namespace OutcomeHub.Application.DTOs.Result;

public record TriggerCalculationRequest(
    Guid MeasurementPeriodId,
    string? CalculationReason);

public record ResultBatchDto(
    Guid Id,
    Guid MeasurementPeriodId,
    string MeasurementPeriodCode,
    Guid ProgramVersionId,
    int BatchNo,
    string Status,
    string EngineVersion,
    DateTimeOffset? CompletedAt,
    string? ResultChecksum);

public record StudentCloResultDto(
    Guid Id,
    Guid BatchId,
    Guid StudentId,
    string StudentCode,
    string StudentFullName,
    Guid CourseOfferingId,
    string CourseOfferingCode,
    Guid CloId,
    string CloCode,
    decimal? Score,
    decimal ThetaInd,
    string AttainmentStatus,
    string DataStatus);

public record StudentPiResultDto(
    Guid Id,
    Guid BatchId,
    Guid StudentId,
    string StudentCode,
    string StudentFullName,
    Guid ProgramPiId,
    string PiCode,
    string PiDescription,
    decimal? Score,
    decimal ThetaInd,
    string AttainmentStatus,
    string CoreGateStatus,
    string DataStatus);

public record StudentPloResultDto(
    Guid Id,
    Guid BatchId,
    Guid StudentId,
    string StudentCode,
    string StudentFullName,
    Guid ProgramPloId,
    string PloCode,
    string PloDescription,
    decimal? Score,
    decimal ThetaInd,
    string AttainmentStatus,
    string CoreGateStatus,
    string DataStatus);

public record CohortOutcomeResultDto(
    Guid Id,
    Guid BatchId,
    Guid ProgramVersionId,
    Guid CohortId,
    string OutcomeLevel,
    Guid? TargetId,
    string TargetCode,
    string TargetDescription,
    long PopulationCount,
    long DenominatorCount,
    long AttainedCount,
    decimal? AttainmentRate,
    decimal ThetaCoh,
    string OutcomeStatus);

public record ProgramOutcomeDashboardDto(
    Guid ProgramVersionId,
    string ProgramName,
    string VersionCode,
    Guid CohortId,
    string CohortCode,
    Guid MeasurementPeriodId,
    string PeriodCode,
    int TotalStudents,
    int TotalPlos,
    int AttainedPlos,
    decimal PloAttainmentRate,
    IReadOnlyList<CohortOutcomeResultDto> PloResults,
    IReadOnlyList<CohortOutcomeResultDto> PiResults);
