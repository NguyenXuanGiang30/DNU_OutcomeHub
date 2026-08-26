using Microsoft.EntityFrameworkCore;
using Npgsql;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.Common.Security;
using OutcomeHub.Application.DTOs.Integration;
using OutcomeHub.Infrastructure.Persistence;
using OutcomeHub.Infrastructure.Persistence.Interceptors;
using OutcomeHub.Infrastructure.Persistence.Repositories.Integration;
using OutcomeHub.Infrastructure.Persistence.Rls;
using OutcomeHub.Infrastructure.Services;
using OutcomeHub.Migrations;
using Testcontainers.PostgreSql;
using Xunit;

namespace OutcomeHub.DatabaseTests;

public sealed class IntegrationPipelineIntegrationTests
{
    [Fact(Timeout = 180_000)]
    public async Task CompleteIntegrationPipelineLifecycleSucceedsUnderRls()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using PostgreSqlContainer postgreSql = new PostgreSqlBuilder("postgres:18")
            .WithDatabase("outcomehub_integration_tests")
            .WithUsername("outcomehub_test_owner")
            .WithPassword("outcomehub_test_owner_password")
            .Build();

        await postgreSql.StartAsync(cancellationToken);
        string ownerConnectionString = postgreSql.GetConnectionString();

        // ── Step 1: Provision database roles and run all 18 migrations ──
        string migrationConnectionString = await DatabaseBaselineTests.ProvisionDatabaseRolesAsync(
            ownerConnectionString,
            cancellationToken);

        string migrationRoot = Path.Combine(AppContext.BaseDirectory, "MigrationSql");
        var runner = new SqlMigrationRunner(migrationConnectionString, migrationRoot);
        var migrationResult = await runner.RunAsync(cancellationToken);
        Assert.Equal(19, migrationResult.AppliedCount);

        // ── Step 2: Seed development dataset ──
        await DatabaseBaselineTests.RunDatabaseScriptAsync(
            ownerConnectionString,
            "seed_development_dataset.sql",
            cancellationToken);

        // ── Step 3: App-role connection (RLS-enforced) ──
        var appConnectionString = new NpgsqlConnectionStringBuilder(ownerConnectionString)
        {
            Username = "outcomehub_app",
            Password = "outcomehub_test_app_password",
            Pooling = false,
        }.ConnectionString;

        var dbOptions = new DbContextOptionsBuilder<OutcomeHubDbContext>()
            .UseNpgsql(appConnectionString)
            .AddInterceptors(new RowVersionSaveChangesInterceptor())
            .Options;

        var adminPrincipalId = Guid.Parse("10000000-0000-7000-8000-000000000001");
        var adminContext = new DatabaseRequestContext(adminPrincipalId, Guid.NewGuid(), "Integration Pipeline Test");

        await using var ctx = new OutcomeHubDbContext(dbOptions);
        var rlsExecutor = new RlsTransactionExecutor(ctx);
        var integrationRepo = new IntegrationPipelineRepository(ctx);
        var integrationService = new IntegrationPipelineService(integrationRepo);

        // ── Step 4: SIS / LMS Ingestion Pipeline (FR-INT-02, FR-INT-03) ──
        var batchSyncRequest = new IngestionBatchSyncRequest(
            "SIS",
            "STUDENTS",
            "IDEM_KEY_BATCH_001",
            DateTimeOffset.UtcNow.AddDays(-1),
            "[{\"studentCode\":\"SV20230001\",\"fullName\":\"Nguyen Van A\",\"email\":\"a@dnu.edu.vn\"},{\"studentCode\":\"SV20230002\",\"fullName\":\"Tran Thi B\",\"email\":\"b@dnu.edu.vn\"}]");

        var batchResult = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => integrationService.ProcessIngestionBatchAsync(batchSyncRequest, ct),
            cancellationToken);

        Assert.NotNull(batchResult);
        Assert.Equal("SIS", batchResult.SourceSystemCode);
        Assert.Equal(2, batchResult.TotalRecords);
        Assert.NotEmpty(batchResult.PayloadChecksum);

        var retrievedBatch = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => integrationService.GetIngestionBatchAsync(batchResult.BatchId, ct),
            cancellationToken);

        Assert.NotNull(retrievedBatch);
        Assert.Equal(batchResult.BatchId, retrievedBatch.BatchId);

        var metrics = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => integrationService.GetReconciliationMetricsAsync("SIS", ct),
            cancellationToken);

        Assert.NotNull(metrics);
        Assert.True(metrics.TotalBatchesProcessed > 0);
        Assert.True(metrics.IngestionSuccessRate > 90.0m);

        // ── Step 5: Staging & Data Quality Quarantine Pipeline (FR-INT-04) ──
        var quarantinedRecords = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => integrationService.GetQuarantinedRecordsAsync(batchResult.BatchId, "PENDING_CORRECTION", ct),
            cancellationToken);

        Assert.NotNull(quarantinedRecords);
        Assert.NotEmpty(quarantinedRecords);

        var firstQuarantine = quarantinedRecords[0];
        var resolveRequest = new ResolveQuarantineRequest(
            firstQuarantine.QuarantineId,
            "RETRY",
            "{\"studentCode\":\"SV20239999\",\"email\":\"valid@dnu.edu.vn\",\"programCode\":\"7480201\"}",
            "Đã sửa mã ngành hợp lệ.");

        var isResolved = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => integrationService.ResolveQuarantinedRecordAsync(resolveRequest, ct),
            cancellationToken);

        Assert.True(isResolved);

        // ── Step 6: Cloud Storage Connector (Google Drive / SharePoint / S3) (FR-INT-05) ──
        var cloudSyncRequest = new CloudStorageSyncRequest(
            "GOOGLE_DRIVE",
            "/Shared/Accreditation_Evidences_2023",
            Guid.Parse("00000000-0000-7000-8000-000000000002"),
            true);

        var cloudSyncResult = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => integrationService.SyncCloudStorageAsync(cloudSyncRequest, ct),
            cancellationToken);

        Assert.NotNull(cloudSyncResult);
        Assert.Equal("SUCCESS", cloudSyncResult.SyncStatus);
        Assert.True(cloudSyncResult.FilesImported > 0);

        // ── Step 7: Data Warehouse / BI Export with Privacy Protection (FR-INT-06) ──
        var biExportRequest = new BiExportRequest(
            Guid.Parse("00000000-0000-7000-8000-000000000002"),
            "2023-2024",
            5);

        var biCubes = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => integrationService.ExportBiOutcomesCubeAsync(biExportRequest, ct),
            cancellationToken);

        Assert.NotNull(biCubes);
        Assert.NotEmpty(biCubes);
        Assert.All(biCubes, cube => Assert.False(cube.IsSuppressedForPrivacy));

        // ── Step 8: Webhook Dispatcher & Events (FR-INT-07) ──
        var webhookRequest = new CreateWebhookSubscriptionRequest(
            "Portal CQI Webhook",
            "https://portal.dnu.edu.vn/api/webhooks/obe",
            "secret_key_webhook_123",
            ["GRADE_FINALIZED", "RESULT_PUBLISHED"]);

        var subscription = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => integrationService.CreateWebhookSubscriptionAsync(webhookRequest, ct),
            cancellationToken);

        Assert.NotNull(subscription);
        Assert.Equal("Portal CQI Webhook", subscription.SubscriptionName);

        var subscriptions = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => integrationService.GetWebhookSubscriptionsAsync(ct),
            cancellationToken);

        Assert.NotNull(subscriptions);
        Assert.NotEmpty(subscriptions);

        var testDispatch = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => integrationService.DispatchTestWebhookAsync(new TestDispatchWebhookRequest(
                subscription.Id,
                "RESULT_PUBLISHED",
                "{\"event\":\"RESULT_PUBLISHED\",\"programCode\":\"7480201\",\"academicYear\":\"2023-2024\"}"), ct),
            cancellationToken);

        Assert.NotNull(testDispatch);
        Assert.True(testDispatch.IsSuccessful);
        Assert.NotEmpty(testDispatch.HmacSignature);

        // ── Step 9: Service Account Governance & API Metrics (FR-INT-08) ──
        var serviceAccounts = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => integrationService.GetServiceAccountsAsync(ct),
            cancellationToken);

        Assert.NotNull(serviceAccounts);
        Assert.NotEmpty(serviceAccounts);

        var firstSa = serviceAccounts[0];
        var rotatedKey = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => integrationService.RotateApiKeyAsync(new RotateServiceAccountKeyRequest(firstSa.Id), ct),
            cancellationToken);

        Assert.NotNull(rotatedKey);
        Assert.StartsWith("sk_live_", rotatedKey.NewApiKey);

        var saMetrics = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => integrationService.GetServiceAccountMetricsAsync(firstSa.Id, ct),
            cancellationToken);

        Assert.NotNull(saMetrics);
        Assert.True(saMetrics.TotalRequests24h > 0);
        Assert.True(saMetrics.AverageLatencyMs > 0);
    }
}
