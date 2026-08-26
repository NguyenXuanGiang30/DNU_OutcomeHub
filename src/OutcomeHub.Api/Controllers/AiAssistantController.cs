using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Ai;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Api.Controllers;

[Route("api/v1/ai")]
public sealed class AiAssistantController : ApiControllerBase
{
    private readonly IAiAssistantService _service;

    public AiAssistantController(IAiAssistantService service)
    {
        _service = service;
    }

    /// <summary>
    /// Chatbot OBE RAG: Hỏi đáp thông tin chuẩn đầu ra, CTĐT và kế hoạch CQI có trích dẫn nguồn (FR-AI-01, FR-AI-02, FR-AI-03).
    /// </summary>
    [HttpPost("chat/query")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<AiChatResponseDto>>> QueryChatbot(
        [FromBody] AiChatQueryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.QueryChatbotAsync(request, cancellationToken);

        return OkResponse(result, "Xử lý câu hỏi trợ lý AI thành công.");
    }

    /// <summary>
    /// Trích xuất dữ liệu có cấu trúc từ tài liệu BM13 / PDF / Word (FR-AI-04).
    /// </summary>
    [HttpPost("extract/document")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<AiExtractionResultDto>>> ExtractDocument(
        [FromBody] AiDocumentExtractionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.ExtractDocumentAsync(request, cancellationToken);

        return OkResponse(result, "Trích xuất tài liệu học thuật bằng AI thành công.");
    }

    /// <summary>
    /// Chẩn đoán mâu thuẫn ma trận, Bloom level và độ phủ CĐR bằng AI (FR-AI-05).
    /// </summary>
    [HttpGet("diagnostics/curriculum/{programVersionId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<AiAnomalyDetectionResultDto>>> RunDiagnostics(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _service.RunDiagnosticsAsync(programVersionId, cancellationToken);

        return OkResponse(result, "Chẩn đoán mâu thuẫn chương trình đào tạo thành công.");
    }

    /// <summary>
    /// Lấy danh sách hàng đợi Human-In-The-Loop (HITL) phê duyệt kết quả trích xuất (FR-AI-06).
    /// </summary>
    [HttpGet("hitl/queue")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<HitlReviewItemDto>>>> GetHitlQueue(
        [FromQuery] Guid? extractionId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetHitlQueueAsync(extractionId, status, cancellationToken);

        return OkResponse(result, "Lấy hàng đợi kiểm duyệt HITL thành công.");
    }

    /// <summary>
    /// Gửi quyết định phê duyệt / hiệu chỉnh / từ chối trong hàng đợi HITL (FR-AI-06).
    /// </summary>
    [HttpPost("hitl/decide")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<HitlDecisionResultDto>>> SubmitHitlDecision(
        [FromBody] HitlDecisionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.SubmitHitlDecisionAsync(request, cancellationToken);

        return OkResponse(result, "Ghi nhận quyết định phê duyệt HITL thành công.");
    }

    /// <summary>
    /// Danh sách phiên bản Prompt template và model AI (FR-AI-07).
    /// </summary>
    [HttpGet("prompts")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PromptTemplateVersionDto>>>> GetPromptVersions(
        [FromQuery] string? promptCode,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetPromptVersionsAsync(promptCode, cancellationToken);

        return OkResponse(result, "Lấy danh sách phiên bản Prompt thành công.");
    }

    /// <summary>
    /// Đăng ký phiên bản Prompt template mới (FR-AI-07).
    /// </summary>
    [HttpPost("prompts/register")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PromptTemplateVersionDto>>> RegisterPromptVersion(
        [FromBody] RegisterPromptVersionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.RegisterPromptVersionAsync(request, cancellationToken);

        return OkResponse(result, "Đăng ký phiên bản Prompt mới thành công.");
    }

    /// <summary>
    /// Chạy bộ kiểm thử Ground-Truth Benchmark cho Prompt template (FR-AI-07).
    /// </summary>
    [HttpGet("prompts/{promptCode}/benchmark/{versionNumber:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PromptBenchmarkTestResultDto>>> RunPromptBenchmark(
        string promptCode,
        int versionNumber,
        CancellationToken cancellationToken)
    {
        var result = await _service.RunPromptBenchmarkAsync(promptCode, versionNumber, cancellationToken);

        return OkResponse(result, "Kiểm thử Benchmark Prompt hoàn thành.");
    }

    /// <summary>
    /// Quét phát hiện tấn công Prompt Injection và mã độc trong prompt (FR-AI-08).
    /// </summary>
    [HttpPost("security/scan-prompt")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PromptInjectionScanResultDto>>> ScanPromptInjection(
        [FromBody] string textToScan,
        CancellationToken cancellationToken)
    {
        var result = await _service.ScanPromptInjectionAsync(textToScan, cancellationToken);

        return OkResponse(result, "Quét an toàn Prompt hoàn thành.");
    }

    /// <summary>
    /// Lịch sử Audit Log các câu truy vấn và tương tác AI (FR-AI-08).
    /// </summary>
    [HttpGet("security/audit-logs")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AiSecurityAuditLogDto>>>> GetSecurityAuditLogs(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetSecurityAuditLogsAsync(cancellationToken);

        return OkResponse(result, "Lấy danh sách Audit Log AI thành công.");
    }
}
