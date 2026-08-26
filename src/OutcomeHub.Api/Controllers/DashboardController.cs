using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Analytics;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
public sealed class DashboardController : ApiControllerBase
{
    private readonly IDashboardService _service;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public DashboardController(
        IDashboardService service,
        IRlsTransactionExecutor rlsExecutor)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpGet("executive")]
    [ProducesResponseType(typeof(ApiResponse<UniversityExecutiveDashboardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UniversityExecutiveDashboardDto>>> GetExecutiveDashboard(
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read Executive Dashboard");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetUniversityExecutiveDashboardAsync(ct),
            cancellationToken);

        return OkResponse(result, "Tổng quan KPI chuẩn đầu ra cấp Trường (Ban Giám hiệu).");
    }

    [HttpGet("faculty/{orgUnitId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<FacultyDashboardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<FacultyDashboardDto>>> GetFacultyDashboard(
        Guid orgUnitId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Faculty Dashboard {orgUnitId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetFacultyDashboardAsync(orgUnitId, ct),
            cancellationToken);

        return OkResponse(result, "Tổng quan KPI chuẩn đầu ra cấp Khoa.");
    }

    [HttpGet("program/{programVersionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProgramDashboardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProgramDashboardDto>>> GetProgramDashboard(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Program Dashboard {programVersionId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetProgramDashboardAsync(programVersionId, ct),
            cancellationToken);

        return OkResponse(result, "Tổng quan KPI chuẩn đầu ra cấp Chương trình đào tạo.");
    }

    [HttpGet("lecturer/{lecturerId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LecturerDashboardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<LecturerDashboardDto>>> GetLecturerDashboard(
        Guid lecturerId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Lecturer Dashboard {lecturerId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetLecturerDashboardAsync(lecturerId, ct),
            cancellationToken);

        return OkResponse(result, "Tổng quan phân công và tiến độ chấm điểm cấp Giảng viên.");
    }

    [HttpGet("student/{studentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<StudentOutcomeDashboardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<StudentOutcomeDashboardDto>>> GetStudentDashboard(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Student Outcome Dashboard {studentId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetStudentDashboardAsync(studentId, ct),
            cancellationToken);

        return OkResponse(result, "Bảng theo dõi tiến độ chuẩn đầu ra cá nhân của Sinh viên.");
    }

    [HttpGet("drilldown")]
    [ProducesResponseType(typeof(ApiResponse<DrillDownNodeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DrillDownNodeDto>>> GetDrillDown(
        [FromQuery] string nodeType,
        [FromQuery] Guid nodeId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"DrillDown {nodeType} {nodeId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetDrillDownTreeAsync(nodeType, nodeId, ct),
            cancellationToken);

        return OkResponse(result, "Cây phân tích drill-down đa tầng chuẩn đầu ra.");
    }

    [HttpGet("alerts")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DashboardAlertItemDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DashboardAlertItemDto>>>> GetAlerts(
        [FromQuery] Guid? orgUnitId,
        [FromQuery] Guid? programVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read Dashboard Alerts");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetAlertsAsync(orgUnitId, programVersionId, ct),
            cancellationToken);

        return OkResponse(result, "Danh sách cảnh báo chỉ số đỏ và việc cần xử lý.");
    }
}
