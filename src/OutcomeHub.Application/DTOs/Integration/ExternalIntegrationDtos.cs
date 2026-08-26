namespace OutcomeHub.Application.DTOs.Integration;

// ── Ingestion Pipeline & Synchronization DTOs (FR-INT-02, FR-INT-03) ──

public sealed record SyncStudentPayloadDto(
    string StudentCode,
    string FullName,
    string Email,
    string? PhoneNumber,
    string ProgramCode,
    string CohortCode,
    string AcademicStatus, // ENROLLED, SUSPENDED, GRADUATED, DROPPED_OUT
    DateTimeOffset UpdatedAt);

public sealed record SyncOfferingPayloadDto(
    string CourseCode,
    string OfferingCode,
    string AcademicYear,
    string Semester,
    string InstructorStaffCode,
    int MaxCapacity,
    DateTimeOffset UpdatedAt);

public sealed record SyncEnrollmentGradePayloadDto(
    string StudentCode,
    string OfferingCode,
    string AssessmentCode,
    decimal Score,
    decimal MaxScore,
    string GraderStaffCode,
    DateTimeOffset GradedAt);

public sealed record IngestionBatchSyncRequest(
    string SourceSystemCode, // SIS, LMS_CANVAS, LMS_MOODLE
    string IngestionType,    // STUDENTS, OFFERINGS, ENROLLMENTS_GRADES
    string IdempotencyKey,
    DateTimeOffset? UpdatedSince,
    string PayloadJson);

public sealed record IngestionBatchResultDto(
    Guid BatchId,
    string SourceSystemCode,
    string IngestionType,
    string IdempotencyKey,
    string Status, // PENDING, PROCESSING, COMPLETED, COMPLETED_WITH_ERRORS, FAILED
    int TotalRecords,
    int ValidRecords,
    int QuarantinedRecords,
    string PayloadChecksum,
    DateTimeOffset StartedAt,
    DateTimeOffset? CompletedAt);

// ── Staging & Data Quality Quarantine DTOs (FR-INT-04) ──

public sealed record QuarantinedRecordDto(
    Guid QuarantineId,
    Guid BatchId,
    string EntityType,
    string SourceRecordIdentifier,
    string RawDataJson,
    string ErrorCategory, // SCHEMA_MISMATCH, REFERENTIAL_INTEGRITY_FAIL, OUT_OF_RANGE, DUPLICATE
    string ErrorMessage,
    string ResolutionStatus, // PENDING_CORRECTION, RESOLVED_AT_SOURCE, OVERRIDDEN, DISCARDED
    DateTimeOffset QuarantinedAt);

public sealed record ResolveQuarantineRequest(
    Guid QuarantineId,
    string Action, // RETRY, OVERRIDE, DISCARD
    string? CorrectedDataJson,
    string ResolutionNotes);

public sealed record IngestionReconciliationMetricsDto(
    Guid SourceSystemId,
    string SourceSystemName,
    int TotalBatchesProcessed,
    long TotalRecordsIngested,
    long TotalQuarantinedRecords,
    decimal IngestionSuccessRate,
    DateTimeOffset LastSyncTimestamp);

// ── Cloud DMS & External Storage Connector DTOs (FR-INT-05) ──

public sealed record CloudStorageSyncRequest(
    string Provider, // GOOGLE_DRIVE, MICROSOFT_SHAREPOINT, AWS_S3
    string RemoteFolderPath,
    Guid TargetOrgUnitId,
    bool ScanMalwareBeforeImport);

public sealed record CloudStorageSyncResultDto(
    Guid SyncJobId,
    string Provider,
    string RemoteFolderPath,
    int FilesDiscovered,
    int FilesImported,
    int FilesSkipped,
    string SyncStatus,
    DateTimeOffset CompletedAt);

// ── Data Warehouse / BI Analytics Export Pipeline DTOs (FR-INT-06) ──

public sealed record BiProgramOutcomeCubeDto(
    string FacultyCode,
    string FacultyName,
    string ProgramCode,
    string ProgramName,
    string CohortCode,
    string AcademicYear,
    string PloCode,
    string PloDescription,
    string BloomLevel,
    int CohortSampleSize,
    decimal AttainmentRatePercentage,
    bool IsMetTarget,
    bool IsSuppressedForPrivacy); // True if SampleSize < k-anonymity threshold (e.g. < 5)

public sealed record BiExportRequest(
    Guid? FacultyOrgUnitId,
    string? AcademicYear,
    int KAnonymityThreshold = 5);

// ── Webhook Dispatcher & Event Pipeline DTOs (FR-INT-07) ──

public sealed record CreateWebhookSubscriptionRequest(
    string SubscriptionName,
    string TargetUrl,
    string SecretKey,
    IReadOnlyList<string> SubscribedEventTypes); // GRADE_FINALIZED, OUTCOME_CALCULATED, RESULT_PUBLISHED, SYNC_FAILED, CQI_OVERDUE

public sealed record WebhookSubscriptionDto(
    Guid Id,
    string SubscriptionName,
    string TargetUrl,
    bool IsActive,
    IReadOnlyList<string> SubscribedEventTypes,
    DateTimeOffset CreatedAt);

public sealed record WebhookEventDispatchDto(
    Guid DeliveryId,
    Guid SubscriptionId,
    string EventType,
    string TargetUrl,
    string PayloadJson,
    string HmacSignature,
    int StatusCode,
    bool IsSuccessful,
    int AttemptCount,
    DateTimeOffset DispatchedAt);

public sealed record TestDispatchWebhookRequest(
    Guid SubscriptionId,
    string EventType,
    string TestPayload);

// ── Service Account & API Governance DTOs (FR-INT-08) ──

public sealed record ServiceAccountDetailsDto(
    Guid Id,
    string ClientId,
    string Name,
    string Description,
    string AllowedScopes,
    int RateLimitPerMinute,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastApiKeyRotatedAt);

public sealed record RotateServiceAccountKeyRequest(
    Guid ServiceAccountId);

public sealed record ServiceAccountKeyRotationResultDto(
    Guid ServiceAccountId,
    string ClientId,
    string NewApiKey,
    DateTimeOffset ExpiresAt);

public sealed record ApiUsageMetricsDto(
    Guid ServiceAccountId,
    string ClientId,
    long TotalRequests24h,
    long SuccessCount24h,
    long RateLimitedCount24h,
    long ErrorCount24h,
    double AverageLatencyMs);
