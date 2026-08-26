using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.DTOs.Analytics;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Infrastructure.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repository;

    public DashboardService(IDashboardRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<UniversityExecutiveDashboardDto> GetUniversityExecutiveDashboardAsync(CancellationToken cancellationToken)
    {
        return await _repository.GetUniversityExecutiveDashboardAsync(cancellationToken);
    }

    public async Task<FacultyDashboardDto> GetFacultyDashboardAsync(Guid orgUnitId, CancellationToken cancellationToken)
    {
        var result = await _repository.GetFacultyDashboardAsync(orgUnitId, cancellationToken);
        return result ?? throw new NotFoundException("Faculty", orgUnitId);
    }

    public async Task<ProgramDashboardDto> GetProgramDashboardAsync(Guid programVersionId, CancellationToken cancellationToken)
    {
        var result = await _repository.GetProgramDashboardAsync(programVersionId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", programVersionId);
    }

    public async Task<LecturerDashboardDto> GetLecturerDashboardAsync(Guid lecturerId, CancellationToken cancellationToken)
    {
        var result = await _repository.GetLecturerDashboardAsync(lecturerId, cancellationToken);
        return result ?? throw new NotFoundException("LecturerStaff", lecturerId);
    }

    public async Task<StudentOutcomeDashboardDto> GetStudentDashboardAsync(Guid studentId, CancellationToken cancellationToken)
    {
        var result = await _repository.GetStudentDashboardAsync(studentId, cancellationToken);
        return result ?? throw new NotFoundException("Student", studentId);
    }

    public async Task<DrillDownNodeDto> GetDrillDownTreeAsync(string rootNodeType, Guid rootNodeId, CancellationToken cancellationToken)
    {
        var result = await _repository.GetDrillDownTreeAsync(rootNodeType, rootNodeId, cancellationToken);
        return result ?? throw new NotFoundException(rootNodeType, rootNodeId);
    }

    public async Task<IReadOnlyList<DashboardAlertItemDto>> GetAlertsAsync(Guid? orgUnitId, Guid? programVersionId, CancellationToken cancellationToken)
    {
        return await _repository.GetSystemAlertsAsync(orgUnitId, programVersionId, cancellationToken);
    }
}
