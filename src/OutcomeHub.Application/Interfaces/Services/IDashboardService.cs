using OutcomeHub.Application.DTOs.Analytics;

namespace OutcomeHub.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<UniversityExecutiveDashboardDto> GetUniversityExecutiveDashboardAsync(
        CancellationToken cancellationToken);

    Task<FacultyDashboardDto> GetFacultyDashboardAsync(
        Guid orgUnitId,
        CancellationToken cancellationToken);

    Task<ProgramDashboardDto> GetProgramDashboardAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<LecturerDashboardDto> GetLecturerDashboardAsync(
        Guid lecturerId,
        CancellationToken cancellationToken);

    Task<StudentOutcomeDashboardDto> GetStudentDashboardAsync(
        Guid studentId,
        CancellationToken cancellationToken);

    Task<DrillDownNodeDto> GetDrillDownTreeAsync(
        string rootNodeType,
        Guid rootNodeId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DashboardAlertItemDto>> GetAlertsAsync(
        Guid? orgUnitId,
        Guid? programVersionId,
        CancellationToken cancellationToken);
}
