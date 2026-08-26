using OutcomeHub.Application.DTOs.Integration;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IIntegrationPipelineRepository
{
    // SIS / LMS Ingestion Pipeline (FR-INT-02, FR-INT-03)
    Task<IngestionBatchResultDto> ProcessIngestionBatchAsync(IngestionBatchSyncRequest request, CancellationToken cancellationToken);
    Task<IngestionBatchResultDto?> GetIngestionBatchAsync(Guid batchId, CancellationToken cancellationToken);
    Task<IngestionReconciliationMetricsDto> GetReconciliationMetricsAsync(string sourceSystemCode, CancellationToken cancellationToken);

    // Staging & Data Quality Quarantine (FR-INT-04)
    Task<IReadOnlyList<QuarantinedRecordDto>> GetQuarantinedRecordsAsync(Guid? batchId, string? status, CancellationToken cancellationToken);
    Task<bool> ResolveQuarantinedRecordAsync(ResolveQuarantineRequest request, CancellationToken cancellationToken);

    // Cloud DMS & Storage Connectors (FR-INT-05)
    Task<CloudStorageSyncResultDto> SyncCloudStorageAsync(CloudStorageSyncRequest request, CancellationToken cancellationToken);

    // BI / Data Warehouse Export (FR-INT-06)
    Task<IReadOnlyList<BiProgramOutcomeCubeDto>> ExportBiOutcomesCubeAsync(BiExportRequest request, CancellationToken cancellationToken);

    // Webhook Dispatcher & Event Pipeline (FR-INT-07)
    Task<IReadOnlyList<WebhookSubscriptionDto>> GetWebhookSubscriptionsAsync(CancellationToken cancellationToken);
    Task<WebhookSubscriptionDto> CreateWebhookSubscriptionAsync(CreateWebhookSubscriptionRequest request, CancellationToken cancellationToken);
    Task<WebhookEventDispatchDto> DispatchTestWebhookAsync(TestDispatchWebhookRequest request, CancellationToken cancellationToken);

    // Service Account Governance (FR-INT-08)
    Task<IReadOnlyList<ServiceAccountDetailsDto>> GetServiceAccountsAsync(CancellationToken cancellationToken);
    Task<ServiceAccountKeyRotationResultDto> RotateApiKeyAsync(RotateServiceAccountKeyRequest request, CancellationToken cancellationToken);
    Task<ApiUsageMetricsDto> GetServiceAccountMetricsAsync(Guid serviceAccountId, CancellationToken cancellationToken);
}
