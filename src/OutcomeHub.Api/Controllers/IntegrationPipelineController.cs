using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Integration;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Api.Controllers;

[Route("api/v1/integration")]
public sealed class IntegrationPipelineController : ApiControllerBase
{
    private readonly IIntegrationPipelineService _service;

    public IntegrationPipelineController(IIntegrationPipelineService service)
    {
        _service = service;
    }

    /// <summary>
    /// Đồng bộ gói dữ liệu gia tăng từ SIS / LMS (FR-INT-02, FR-INT-03).
    /// </summary>
    [HttpPost("sync/batch")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IngestionBatchResultDto>>> ProcessIngestionBatch(
        [FromBody] IngestionBatchSyncRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.ProcessIngestionBatchAsync(request, cancellationToken);

        return OkResponse(result, "Tiếp nhận và xử lý gói đồng bộ thành công.");
    }

    /// <summary>
    /// Kiểm tra trạng thái và tiến độ xử lý của Ingestion Batch (FR-INT-03).
    /// </summary>
    [HttpGet("batches/{batchId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IngestionBatchResultDto>>> GetIngestionBatch(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetIngestionBatchAsync(batchId, cancellationToken);
        if (result == null)
        {
            return NotFound(ApiResponse.Fail("Không tìm thấy thông tin gói đồng bộ."));
        }

        return OkResponse(result, "Lấy thông tin gói đồng bộ thành công.");
    }

    /// <summary>
    /// Báo cáo đối soát chất lượng dữ liệu đồng bộ theo hệ thống nguồn (FR-INT-04).
    /// </summary>
    [HttpGet("metrics/reconciliation/{sourceSystemCode}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IngestionReconciliationMetricsDto>>> GetReconciliationMetrics(
        string sourceSystemCode,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetReconciliationMetricsAsync(sourceSystemCode, cancellationToken);

        return OkResponse(result, "Lấy báo cáo đối soát chất lượng dữ liệu thành công.");
    }

    /// <summary>
    /// Danh sách bản ghi lỗi bị cách ly trong Staging Quarantine (FR-INT-04).
    /// </summary>
    [HttpGet("quarantine")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<QuarantinedRecordDto>>>> GetQuarantinedRecords(
        [FromQuery] Guid? batchId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetQuarantinedRecordsAsync(batchId, status, cancellationToken);

        return OkResponse(result, "Lấy danh sách bản ghi cách ly thành công.");
    }

    /// <summary>
    /// Xử lý bản ghi cách ly (Retry / Override / Discard) (FR-INT-04).
    /// </summary>
    [HttpPost("quarantine/resolve")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> ResolveQuarantinedRecord(
        [FromBody] ResolveQuarantineRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.ResolveQuarantinedRecordAsync(request, cancellationToken);

        return OkResponse(result, "Xử lý bản ghi cách ly thành công.");
    }

    /// <summary>
    /// Đồng bộ thư mục tài liệu từ Cloud Storage (Google Drive / SharePoint / S3) (FR-INT-05).
    /// </summary>
    [HttpPost("cloud-storage/sync")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<CloudStorageSyncResultDto>>> SyncCloudStorage(
        [FromBody] CloudStorageSyncRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.SyncCloudStorageAsync(request, cancellationToken);

        return OkResponse(result, "Đồng bộ tài liệu từ Cloud Storage thành công.");
    }

    /// <summary>
    /// Xuất dữ liệu Data Warehouse / BI Cube có cơ chế bảo vệ quyền riêng tư k-anonymity (FR-INT-06).
    /// </summary>
    [HttpPost("bi-export/outcomes")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BiProgramOutcomeCubeDto>>>> ExportBiOutcomesCube(
        [FromBody] BiExportRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.ExportBiOutcomesCubeAsync(request, cancellationToken);

        return OkResponse(result, "Xuất dữ liệu Data Warehouse / BI Cube thành công.");
    }

    /// <summary>
    /// Danh sách cấu hình Webhook subscriptions (FR-INT-07).
    /// </summary>
    [HttpGet("webhooks/subscriptions")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WebhookSubscriptionDto>>>> GetWebhookSubscriptions(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetWebhookSubscriptionsAsync(cancellationToken);

        return OkResponse(result, "Lấy danh sách Webhook subscriptions thành công.");
    }

    /// <summary>
    /// Đăng ký Webhook subscription mới (FR-INT-07).
    /// </summary>
    [HttpPost("webhooks/subscriptions")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<WebhookSubscriptionDto>>> CreateWebhookSubscription(
        [FromBody] CreateWebhookSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateWebhookSubscriptionAsync(request, cancellationToken);

        return OkResponse(result, "Đăng ký Webhook subscription thành công.");
    }

    /// <summary>
    /// Kích hoạt gửi thử nghiệm Webhook event có chữ ký HMAC-SHA256 (FR-INT-07).
    /// </summary>
    [HttpPost("webhooks/test-dispatch")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<WebhookEventDispatchDto>>> DispatchTestWebhook(
        [FromBody] TestDispatchWebhookRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.DispatchTestWebhookAsync(request, cancellationToken);

        return OkResponse(result, "Bắn Webhook thử nghiệm thành công.");
    }

    /// <summary>
    /// Danh sách Service Accounts tích hợp ngoại vi (FR-INT-08).
    /// </summary>
    [HttpGet("service-accounts")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ServiceAccountDetailsDto>>>> GetServiceAccounts(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetServiceAccountsAsync(cancellationToken);

        return OkResponse(result, "Lấy danh sách Service Accounts thành công.");
    }

    /// <summary>
    /// Đổi khóa bí mật API Key cho Service Account (FR-INT-08).
    /// </summary>
    [HttpPost("service-accounts/rotate-key")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ServiceAccountKeyRotationResultDto>>> RotateApiKey(
        [FromBody] RotateServiceAccountKeyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.RotateApiKeyAsync(request, cancellationToken);

        return OkResponse(result, "Đổi khóa API Key cho Service Account thành công.");
    }

    /// <summary>
    /// Thống kê lưu lượng API, Rate Limit và độ trễ (FR-INT-08).
    /// </summary>
    [HttpGet("service-accounts/{serviceAccountId:guid}/metrics")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ApiUsageMetricsDto>>> GetServiceAccountMetrics(
        Guid serviceAccountId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetServiceAccountMetricsAsync(serviceAccountId, cancellationToken);

        return OkResponse(result, "Lấy thống kê lưu lượng API thành công.");
    }
}
