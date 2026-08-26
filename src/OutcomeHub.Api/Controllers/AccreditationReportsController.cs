using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Analytics;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/reports/accreditation")]
public sealed class AccreditationReportsController : ApiControllerBase
{
    private readonly IAccreditationReportService _service;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public AccreditationReportsController(
        IAccreditationReportService service,
        IRlsTransactionExecutor rlsExecutor)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpGet("moet/{programVersionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MoetAccreditationReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<MoetAccreditationReportDto>>> GetMoetReport(
        Guid programVersionId,
        [FromQuery] Guid? measurementPeriodId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Generate MOET Report {programVersionId}");
        var report = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetMoetReportAsync(programVersionId, measurementPeriodId, ct),
            cancellationToken);

        return OkResponse(report, "Báo cáo tự đánh giá chuẩn đầu ra phục vụ kiểm định Bộ Giáo dục và Đào tạo (MOET).");
    }

    [HttpGet("aun-qa/{programVersionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AunQaAccreditationReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AunQaAccreditationReportDto>>> GetAunQaReport(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Generate AUN-QA Report {programVersionId}");
        var report = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetAunQaReportAsync(programVersionId, ct),
            cancellationToken);

        return OkResponse(report, "Báo cáo kiểm định chất lượng theo bộ tiêu chuẩn AUN-QA Version 4.0.");
    }

    [HttpGet("abet/{programVersionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AbetAccreditationReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AbetAccreditationReportDto>>> GetAbetReport(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Generate ABET Report {programVersionId}");
        var report = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetAbetReportAsync(programVersionId, ct),
            cancellationToken);

        return OkResponse(report, "Báo cáo tự đánh giá sinh viên (Student Outcomes) theo chuẩn kiểm định quốc tế ABET.");
    }

    [HttpGet("dossier/{programVersionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AccreditationDossierDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AccreditationDossierDto>>> GetAccreditationDossier(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Generate Accreditation Dossier {programVersionId}");
        var dossier = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetAccreditationDossierAsync(programVersionId, ct),
            cancellationToken);

        return OkResponse(dossier, "Hồ sơ minh chứng kiểm định chất lượng toàn diện (Accreditation Dossier Package).");
    }

    [HttpGet("transcripts/student/{studentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<StudentObeTranscriptDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<StudentObeTranscriptDto>>> GetStudentTranscript(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Generate Student OBE Transcript {studentId}");
        var transcript = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetStudentObeTranscriptAsync(studentId, ct),
            cancellationToken);

        return OkResponse(transcript, "Phụ lục bảng điểm chuẩn đầu ra cá nhân (OBE Transcript Supplement).");
    }
}
