namespace OutcomeHub.Application.DTOs.Analytics;

public sealed record UniversityExecutiveDashboardDto(
    int TotalStudents,
    int TotalPrograms,
    int TotalCourses,
    int TotalActivePeriods,
    decimal OverallPloAttainmentRate,
    int TotalCqiPlansCount,
    int ActiveCqiPlansCount,
    IReadOnlyList<FacultyPloSummaryDto> FacultySummaries,
    IReadOnlyList<DashboardAlertItemDto> UrgentAlerts,
    DateTimeOffset GeneratedAt);

public sealed record FacultyPloSummaryDto(
    Guid OrgUnitId,
    string OrgUnitCode,
    string OrgUnitName,
    int ProgramCount,
    int StudentCount,
    decimal PloAttainmentRate,
    int PendingCqiPlans);

public sealed record FacultyDashboardDto(
    Guid OrgUnitId,
    string OrgUnitCode,
    string OrgUnitName,
    int TotalPrograms,
    int TotalStudents,
    int TotalCourses,
    decimal AveragePloAttainmentRate,
    IReadOnlyList<ProgramOutcomeSummaryDto> ProgramSummaries,
    IReadOnlyList<DashboardAlertItemDto> Alerts,
    DateTimeOffset GeneratedAt);

public sealed record ProgramOutcomeSummaryDto(
    Guid ProgramId,
    Guid ProgramVersionId,
    string ProgramCode,
    string ProgramName,
    int VersionNo,
    int StudentCount,
    decimal PloAttainmentRate,
    int TotalPlosCount,
    int AttainedPlosCount);

public sealed record ProgramDashboardDto(
    Guid ProgramVersionId,
    string ProgramCode,
    string ProgramName,
    int VersionNo,
    int TotalStudents,
    int TotalCohorts,
    decimal PloAttainmentRate,
    IReadOnlyList<PloDetailAttainmentDto> PloAttainments,
    IReadOnlyList<CohortAttainmentItemDto> CohortAttainments,
    IReadOnlyList<DashboardAlertItemDto> Alerts,
    DateTimeOffset GeneratedAt);

public sealed record PloDetailAttainmentDto(
    Guid PloId,
    string PloCode,
    string PloDescription,
    decimal BenchmarkTarget,
    decimal ActualAttainmentRate,
    bool IsAttained,
    IReadOnlyList<PiDetailAttainmentDto> PiAttainments);

public sealed record PiDetailAttainmentDto(
    Guid PiId,
    string PiCode,
    string PiDescription,
    decimal BenchmarkTarget,
    decimal ActualAttainmentRate,
    bool IsAttained);

public sealed record CohortAttainmentItemDto(
    Guid CohortId,
    string CohortCode,
    string CohortName,
    int StudentCount,
    decimal AttainmentRate);

public sealed record LecturerDashboardDto(
    Guid LecturerId,
    string LecturerName,
    string Email,
    int TotalAssignedOfferings,
    int PendingGradingOfferingsCount,
    IReadOnlyList<LecturerOfferingDto> Offerings,
    IReadOnlyList<DashboardAlertItemDto> ActionItems,
    DateTimeOffset GeneratedAt);

public sealed record LecturerOfferingDto(
    Guid OfferingId,
    string CourseCode,
    string CourseName,
    string TermCode,
    int StudentCount,
    string GradingStatus,
    decimal? AverageScore,
    decimal? CloAttainmentRate);

public sealed record StudentOutcomeDashboardDto(
    Guid StudentId,
    string StudentCode,
    string FullName,
    string ProgramName,
    string CohortName,
    int TotalEarnedCredits,
    decimal Gpa,
    IReadOnlyList<StudentPloAttainmentDto> PloCompetencies,
    IReadOnlyList<StudentCourseOutcomeDto> CourseOutcomes,
    DateTimeOffset GeneratedAt);

public sealed record StudentPloAttainmentDto(
    Guid PloId,
    string PloCode,
    string PloDescription,
    decimal RequiredLevel,
    decimal StudentAttainedLevel,
    bool IsCompetent,
    string CompetencyLevel);

public sealed record StudentCourseOutcomeDto(
    Guid CourseId,
    string CourseCode,
    string CourseName,
    string TermCode,
    decimal Score,
    string GradeLetter,
    IReadOnlyList<StudentCloScoreDto> CloScores);

public sealed record StudentCloScoreDto(
    Guid CloId,
    string CloCode,
    decimal AttainedScore,
    bool IsMet);

public sealed record DrillDownNodeDto(
    string NodeType,
    Guid NodeId,
    string NodeCode,
    string NodeTitle,
    decimal AttainmentRate,
    decimal BenchmarkTarget,
    bool IsAttained,
    string? ContextMetadata,
    IReadOnlyList<DrillDownNodeDto> Children);

public sealed record DashboardAlertItemDto(
    string AlertType,
    string Severity,
    string Title,
    string Message,
    string ResourceType,
    Guid? ResourceId,
    DateTimeOffset CreatedAt);
