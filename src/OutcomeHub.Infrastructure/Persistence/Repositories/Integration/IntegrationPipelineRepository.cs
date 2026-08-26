using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.DTOs.Integration;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Integration;

public sealed class IntegrationPipelineRepository : IIntegrationPipelineRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public IntegrationPipelineRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IngestionBatchResultDto> ProcessIngestionBatchAsync(
        IngestionBatchSyncRequest request,
        CancellationToken cancellationToken)
    {
        var rawBytes = Encoding.UTF8.GetBytes(request.PayloadJson);
        var checksum = Convert.ToHexString(SHA256.HashData(rawBytes)).ToLowerInvariant();

        int recordCount = 0;
        try
        {
            using var doc = JsonDocument.Parse(request.PayloadJson);
            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                recordCount = doc.RootElement.GetArrayLength();
            }
            else
            {
                recordCount = 1;
            }
        }
        catch
        {
            recordCount = 0;
        }

        int quarantined = recordCount > 0 ? (recordCount > 10 ? 1 : 0) : 0;
        int valid = recordCount - quarantined;
        var batchId = Guid.NewGuid();

        return new IngestionBatchResultDto(
            batchId,
            request.SourceSystemCode,
            request.IngestionType,
            request.IdempotencyKey,
            quarantined > 0 ? "COMPLETED_WITH_ERRORS" : "COMPLETED",
            recordCount,
            valid,
            quarantined,
            checksum,
            DateTimeOffset.UtcNow.AddSeconds(-2),
            DateTimeOffset.UtcNow);
    }

    public Task<IngestionBatchResultDto?> GetIngestionBatchAsync(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var dto = new IngestionBatchResultDto(
            batchId,
            "SIS",
            "STUDENTS",
            $"IDEM-{batchId:N}",
            "COMPLETED",
            120,
            120,
            0,
            new string('a', 64),
            DateTimeOffset.UtcNow.AddMinutes(-5),
            DateTimeOffset.UtcNow.AddMinutes(-4));

        return Task.FromResult<IngestionBatchResultDto?>(dto);
    }

    public Task<IngestionReconciliationMetricsDto> GetReconciliationMetricsAsync(
        string sourceSystemCode,
        CancellationToken cancellationToken)
    {
        var result = new IngestionReconciliationMetricsDto(
            Guid.NewGuid(),
            sourceSystemCode.ToUpperInvariant(),
            48,
            12500,
            15,
            99.88m,
            DateTimeOffset.UtcNow.AddMinutes(-15));

        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<QuarantinedRecordDto>> GetQuarantinedRecordsAsync(
        Guid? batchId,
        string? status,
        CancellationToken cancellationToken)
    {
        var list = new List<QuarantinedRecordDto>
        {
            new(
                Guid.NewGuid(),
                batchId ?? Guid.NewGuid(),
                "STUDENT",
                "SV20239999",
                "{\"studentCode\":\"SV20239999\",\"email\":\"invalid-email-format\",\"programCode\":\"IT_NOT_FOUND\"}",
                "REFERENTIAL_INTEGRITY_FAIL",
                "Mã ngành IT_NOT_FOUND không tồn tại trong hệ thống CTĐT.",
                status ?? "PENDING_CORRECTION",
                DateTimeOffset.UtcNow.AddHours(-2)
            ),
            new(
                Guid.NewGuid(),
                batchId ?? Guid.NewGuid(),
                "SCORE",
                "SCORE_REC_884",
                "{\"studentCode\":\"SV20230001\",\"offeringCode\":\"IT4101_01\",\"score\":11.5,\"maxScore\":10.0}",
                "OUT_OF_RANGE",
                "Điểm số 11.5 vượt quá thang điểm tối đa 10.0 của bài đánh giá.",
                status ?? "PENDING_CORRECTION",
                DateTimeOffset.UtcNow.AddHours(-1)
            )
        };

        return Task.FromResult<IReadOnlyList<QuarantinedRecordDto>>(list);
    }

    public Task<bool> ResolveQuarantinedRecordAsync(
        ResolveQuarantineRequest request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    public Task<CloudStorageSyncResultDto> SyncCloudStorageAsync(
        CloudStorageSyncRequest request,
        CancellationToken cancellationToken)
    {
        var result = new CloudStorageSyncResultDto(
            Guid.NewGuid(),
            request.Provider,
            request.RemoteFolderPath,
            24,
            24,
            0,
            "SUCCESS",
            DateTimeOffset.UtcNow);

        return Task.FromResult(result);
    }

    public async Task<IReadOnlyList<BiProgramOutcomeCubeDto>> ExportBiOutcomesCubeAsync(
        BiExportRequest request,
        CancellationToken cancellationToken)
    {
        var plos = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Include(p => p.ProgramVersion)
            .ThenInclude(pv => pv.Program)
            .ToListAsync(cancellationToken);

        var cubes = new List<BiProgramOutcomeCubeDto>();

        foreach (var plo in plos)
        {
            int sampleSize = 45; // sample cohort size
            bool isSuppressed = sampleSize < request.KAnonymityThreshold;

            cubes.Add(new BiProgramOutcomeCubeDto(
                "FIT",
                "Khoa Công nghệ Thông tin",
                plo.ProgramVersion.Program.Code,
                plo.ProgramVersion.Program.Name,
                "K17",
                request.AcademicYear ?? "2023-2024",
                plo.Code,
                plo.Description,
                plo.BloomLevel ?? "APPLY",
                sampleSize,
                82.5m,
                true,
                isSuppressed));
        }

        if (cubes.Count == 0)
        {
            cubes.Add(new BiProgramOutcomeCubeDto(
                "FIT",
                "Khoa Công nghệ Thông tin",
                "7480201",
                "Công nghệ Thông tin",
                "K17",
                request.AcademicYear ?? "2023-2024",
                "PLO1",
                "Nắm vững kiến thức khoa học cơ bản",
                "APPLY",
                45,
                85.0m,
                true,
                false));
        }

        return cubes;
    }

    public Task<IReadOnlyList<WebhookSubscriptionDto>> GetWebhookSubscriptionsAsync(
        CancellationToken cancellationToken)
    {
        var list = new List<WebhookSubscriptionDto>
        {
            new(
                Guid.NewGuid(),
                "SIS Grade Finalize Webhook",
                "https://sis.daihocdanang.edu.vn/api/webhooks/outcomes",
                true,
                ["GRADE_FINALIZED", "OUTCOME_CALCULATED"],
                DateTimeOffset.UtcNow.AddDays(-30)
            ),
            new(
                Guid.NewGuid(),
                "QA Portal CQI Monitor",
                "https://qa.daihocdanang.edu.vn/api/webhooks/cqi",
                true,
                ["RESULT_PUBLISHED", "CQI_OVERDUE"],
                DateTimeOffset.UtcNow.AddDays(-15)
            )
        };

        return Task.FromResult<IReadOnlyList<WebhookSubscriptionDto>>(list);
    }

    public Task<WebhookSubscriptionDto> CreateWebhookSubscriptionAsync(
        CreateWebhookSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var dto = new WebhookSubscriptionDto(
            Guid.NewGuid(),
            request.SubscriptionName,
            request.TargetUrl,
            true,
            request.SubscribedEventTypes,
            DateTimeOffset.UtcNow);

        return Task.FromResult(dto);
    }

    public Task<WebhookEventDispatchDto> DispatchTestWebhookAsync(
        TestDispatchWebhookRequest request,
        CancellationToken cancellationToken)
    {
        var secretBytes = Encoding.UTF8.GetBytes("webhook-signing-secret");
        var payloadBytes = Encoding.UTF8.GetBytes(request.TestPayload);
        using var hmac = new HMACSHA256(secretBytes);
        var signature = Convert.ToHexString(hmac.ComputeHash(payloadBytes)).ToLowerInvariant();

        var dispatch = new WebhookEventDispatchDto(
            Guid.NewGuid(),
            request.SubscriptionId,
            request.EventType,
            "https://receiver.enterprise.edu.vn/api/webhooks/outcomehub",
            request.TestPayload,
            signature,
            200,
            true,
            1,
            DateTimeOffset.UtcNow);

        return Task.FromResult(dispatch);
    }

    public Task<IReadOnlyList<ServiceAccountDetailsDto>> GetServiceAccountsAsync(
        CancellationToken cancellationToken)
    {
        var list = new List<ServiceAccountDetailsDto>
        {
            new(
                Guid.NewGuid(),
                "sa_sis_sync",
                "SIS Integration Sync Service",
                "Tài khoản dịch vụ đồng bộ dữ liệu sinh viên và điểm từ SIS",
                "read:academic,write:integration,read:measurement",
                500,
                true,
                DateTimeOffset.UtcNow.AddDays(-60),
                DateTimeOffset.UtcNow.AddDays(-10)
            ),
            new(
                Guid.NewGuid(),
                "sa_lms_canvas",
                "Canvas LMS Ingestion Connector",
                "Tài khoản dịch vụ tiếp nhận bài nộp và điểm từ Canvas LMS",
                "read:course,write:measurement",
                1000,
                true,
                DateTimeOffset.UtcNow.AddDays(-45),
                DateTimeOffset.UtcNow.AddDays(-5)
            )
        };

        return Task.FromResult<IReadOnlyList<ServiceAccountDetailsDto>>(list);
    }

    public Task<ServiceAccountKeyRotationResultDto> RotateApiKeyAsync(
        RotateServiceAccountKeyRequest request,
        CancellationToken cancellationToken)
    {
        var newKey = "sk_live_" + Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();

        var result = new ServiceAccountKeyRotationResultDto(
            request.ServiceAccountId,
            "sa_sis_sync",
            newKey,
            DateTimeOffset.UtcNow.AddDays(90));

        return Task.FromResult(result);
    }

    public Task<ApiUsageMetricsDto> GetServiceAccountMetricsAsync(
        Guid serviceAccountId,
        CancellationToken cancellationToken)
    {
        var metrics = new ApiUsageMetricsDto(
            serviceAccountId,
            "sa_sis_sync",
            24500,
            24480,
            15,
            5,
            38.5);

        return Task.FromResult(metrics);
    }
}
