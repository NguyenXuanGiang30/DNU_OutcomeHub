using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Portfolio;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Api.Controllers;

[Route("api/v1/syllabus-assessment")]
public sealed class ExamBlueprintAndPortfolioController : ApiControllerBase
{
    private readonly IExamBlueprintService _service;

    public ExamBlueprintAndPortfolioController(IExamBlueprintService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lấy ma trận đề thi (Exam Blueprint) theo học phần và bài đánh giá (FR-PRT-03, FR-PRT-05).
    /// </summary>
    [HttpGet("blueprint/{syllabusVersionId:guid}/{assessmentItemId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ExamBlueprintDto>>> GetExamBlueprint(
        Guid syllabusVersionId,
        Guid assessmentItemId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetExamBlueprintAsync(syllabusVersionId, assessmentItemId, cancellationToken);
        if (result == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy ma trận đề thi cho bài đánh giá được chỉ định."));
        }

        return OkResponse(result, "Lấy ma trận đề thi thành công.");
    }

    /// <summary>
    /// Lưu / Cập nhật ma trận đề thi (Exam Blueprint) (FR-PRT-03, FR-PRT-05, FR-PRT-06).
    /// </summary>
    [HttpPost("blueprint")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ExamBlueprintDto>>> SaveExamBlueprint(
        [FromBody] CreateExamBlueprintRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.SaveExamBlueprintAsync(request, cancellationToken);

        return CreatedResponse(
            nameof(GetExamBlueprint),
            new { syllabusVersionId = request.SyllabusVersionId, assessmentItemId = request.AssessmentItemId },
            result,
            "Lưu ma trận đề thi thành công.");
    }

    /// <summary>
    /// Lấy bảng 8.3.1 ma trận truy vết CLO - PI - Bài đánh giá - Tiêu chí - Minh chứng (FR-PRT-17).
    /// </summary>
    [HttpGet("traceability-831/{syllabusVersionId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SyllabusTraceabilityMatrix831Dto>>> GetTraceabilityMatrix831(
        Guid syllabusVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetTraceabilityMatrix831Async(syllabusVersionId, cancellationToken);
        if (result == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy đề cương chi tiết học phần."));
        }

        return OkResponse(result, "Lấy bảng 8.3.1 ma trận truy vết thành công.");
    }

    /// <summary>
    /// Lấy bảng 8.3.2 ma trận đo lường trực tiếp Level A (tổng trọng số = 100%) (FR-PRT-05, FR-PRT-18, FR-PRT-19).
    /// </summary>
    [HttpGet("direct-matrix-832/{syllabusVersionId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<DirectAssessmentMatrix832Dto>>> GetDirectAssessmentMatrix832(
        Guid syllabusVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetDirectAssessmentMatrix832Async(syllabusVersionId, cancellationToken);
        if (result == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy đề cương chi tiết học phần."));
        }

        return OkResponse(result, "Lấy bảng 8.3.2 ma trận đo lường trực tiếp thành công.");
    }

    /// <summary>
    /// Lấy kế hoạch tiến trình giảng dạy từng buổi học (FR-PRT-20).
    /// </summary>
    [HttpGet("teaching-schedule/{syllabusVersionId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<WeeklyScheduleDto>>> GetWeeklySchedule(
        Guid syllabusVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetWeeklyScheduleAsync(syllabusVersionId, cancellationToken);
        if (result == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy kế hoạch giảng dạy."));
        }

        return OkResponse(result, "Lấy kế hoạch tiến trình giảng dạy thành công.");
    }

    /// <summary>
    /// Cập nhật kế hoạch tiến trình giảng dạy từng buổi học (FR-PRT-20).
    /// </summary>
    [HttpPost("teaching-schedule")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<WeeklyScheduleDto>>> SaveWeeklySchedule(
        [FromBody] SaveWeeklyScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.SaveWeeklyScheduleAsync(request, cancellationToken);

        return OkResponse(result, "Cập nhật kế hoạch tiến trình giảng dạy thành công.");
    }

    /// <summary>
    /// Danh sách tài liệu và minh chứng học thuật trong Kho (Academic Document Vault) (FR-PRT-07, FR-PRT-08, FR-PRT-10, FR-PRT-11).
    /// </summary>
    [HttpGet("documents/{syllabusVersionId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DocumentVaultItemDto>>>> GetDocuments(
        Guid syllabusVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetDocumentsAsync(syllabusVersionId, cancellationToken);

        return OkResponse(result, "Lấy danh mục tài liệu minh chứng thành công.");
    }

    /// <summary>
    /// Tải lên tài liệu minh chứng / đề cương / đề thi với checksum SHA-256 (FR-PRT-07, FR-PRT-11).
    /// </summary>
    [HttpPost("documents/upload")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<DocumentVaultItemDto>>> UploadDocument(
        [FromBody] UploadDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.UploadDocumentAsync(request, cancellationToken);

        return OkResponse(result, "Tải lên tài liệu và xác thực toàn vẹn SHA-256 thành công.");
    }

    /// <summary>
    /// Xuất trọn gói Hồ sơ học phần (Portfolio Package) có mục lục và watermark (FR-PRT-12).
    /// </summary>
    [HttpPost("portfolio-package/export")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PortfolioPackageDto>>> ExportPortfolioPackage(
        [FromBody] ExportPortfolioPackageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.ExportPortfolioPackageAsync(request, cancellationToken);
        if (result == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy đề cương chi tiết học phần để xuất portfolio."));
        }

        return OkResponse(result, "Xuất gói portfolio học phần thành công.");
    }

    /// <summary>
    /// AI Assistant sinh nháp nội dung Đề cương chi tiết và ma trận đề thi (FR-PRT-09).
    /// </summary>
    [HttpPost("ai-draft")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<AiSyllabusDraftResultDto>>> GenerateAiSyllabusDraft(
        [FromBody] AiSyllabusDraftRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.GenerateAiSyllabusDraftAsync(request, cancellationToken);

        return OkResponse(result, "AI Assistant đã sinh nháp cấu trúc ĐCCT thành công.");
    }

    /// <summary>
    /// Kiểm tra 5 tiêu chí sẵn sàng ban hành ĐCCT (Syllabus Publishing Readiness Gates) (FR-PRT-21).
    /// </summary>
    [HttpGet("publishing-checklist/{syllabusVersionId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SyllabusPublishingChecklistDto>>> ValidatePublishingReadiness(
        Guid syllabusVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _service.ValidateSyllabusPublishingReadinessAsync(syllabusVersionId, cancellationToken);
        if (result == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy đề cương chi tiết học phần."));
        }

        return OkResponse(result, "Kiểm tra điều kiện ban hành ĐCCT thành công.");
    }
}
