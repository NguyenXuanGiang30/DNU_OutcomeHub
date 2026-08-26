using OutcomeHub.Application.DTOs.Integration;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Infrastructure.Services;

public sealed class IntegrationPipelineService : IIntegrationPipelineService
{
    private readonly IIntegrationPipelineRepository _repository;

    public IntegrationPipelineService(IIntegrationPipelineRepository repository)
    {
        _repository = repository;
    }

    public Task<IngestionBatchResultDto> ProcessIngestionBatchAsync(IngestionBatchSyncRequest request, CancellationToken cancellationToken)
    {
        return _repository.ProcessIngestionBatchAsync(request, cancellationToken);
    }

    public Task<IngestionBatchResultDto?> GetIngestionBatchAsync(Guid batchId, CancellationToken cancellationToken)
    {
        return _repository.GetIngestionBatchAsync(batchId, cancellationToken);
    }

    public Task<IngestionReconciliationMetricsDto> GetReconciliationMetricsAsync(string sourceSystemCode, CancellationToken cancellationToken)
    {
        return _repository.GetReconciliationMetricsAsync(sourceSystemCode, cancellationToken);
    }

    public Task<IReadOnlyList<QuarantinedRecordDto>> GetQuarantinedRecordsAsync(Guid? batchId, string? status, CancellationToken cancellationToken)
    {
        return _repository.GetQuarantinedRecordsAsync(batchId, status, cancellationToken);
    }

    public Task<bool> ResolveQuarantinedRecordAsync(ResolveQuarantineRequest request, CancellationToken cancellationToken)
    {
        return _repository.ResolveQuarantinedRecordAsync(request, cancellationToken);
    }

    public Task<CloudStorageSyncResultDto> SyncCloudStorageAsync(CloudStorageSyncRequest request, CancellationToken cancellationToken)
    {
        return _repository.SyncCloudStorageAsync(request, cancellationToken);
    }

    public Task<IReadOnlyList<BiProgramOutcomeCubeDto>> ExportBiOutcomesCubeAsync(BiExportRequest request, CancellationToken cancellationToken)
    {
        return _repository.ExportBiOutcomesCubeAsync(request, cancellationToken);
    }

    public Task<IReadOnlyList<WebhookSubscriptionDto>> GetWebhookSubscriptionsAsync(CancellationToken cancellationToken)
    {
        return _repository.GetWebhookSubscriptionsAsync(cancellationToken);
    }

    public Task<WebhookSubscriptionDto> CreateWebhookSubscriptionAsync(CreateWebhookSubscriptionRequest request, CancellationToken cancellationToken)
    {
        return _repository.CreateWebhookSubscriptionAsync(request, cancellationToken);
    }

    public Task<WebhookEventDispatchDto> DispatchTestWebhookAsync(TestDispatchWebhookRequest request, CancellationToken cancellationToken)
    {
        return _repository.DispatchTestWebhookAsync(request, cancellationToken);
    }

    public Task<IReadOnlyList<ServiceAccountDetailsDto>> GetServiceAccountsAsync(CancellationToken cancellationToken)
    {
        return _repository.GetServiceAccountsAsync(cancellationToken);
    }

    public Task<ServiceAccountKeyRotationResultDto> RotateApiKeyAsync(RotateServiceAccountKeyRequest request, CancellationToken cancellationToken)
    {
        return _repository.RotateApiKeyAsync(request, cancellationToken);
    }

    public Task<ApiUsageMetricsDto> GetServiceAccountMetricsAsync(Guid serviceAccountId, CancellationToken cancellationToken)
    {
        return _repository.GetServiceAccountMetricsAsync(serviceAccountId, cancellationToken);
    }
}
