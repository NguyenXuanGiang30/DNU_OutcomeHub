namespace OutcomeHub.Application.DTOs.Ai;

// ── OBE RAG Chatbot DTOs (FR-AI-01, FR-AI-02, FR-AI-03) ──

public sealed record AiChatQueryRequest(
    string Prompt,
    Guid? ContextOrgUnitId,
    Guid? ContextProgramVersionId,
    string? ContextAcademicYear,
    string ConversationSessionId);

public sealed record AiCitationDto(
    string SourceType, // CURRICULUM_VERSION, SYLLABUS_VERSION, MEASUREMENT_PERIOD, CQI_PLAN, BM13_SPEC
    string SourceIdentifier,
    string Title,
    string? PageOrSection,
    string DataTimestamp,
    string? FormulaApplied);

public sealed record AiChatResponseDto(
    string Answer,
    IReadOnlyList<AiCitationDto> Citations,
    bool ContainsMaskedPersonalData,
    int TotalSourcesRetrieved,
    double ConfidenceScore,
    string ModelUsed,
    DateTimeOffset GeneratedAt);

// ── AI Document Extraction & BM13 Parsing DTOs (FR-AI-04) ──

public sealed record AiDocumentExtractionRequest(
    Guid DocumentId,
    string DocumentType, // BM13_SYLLABUS, EXAM_SPEC, ACCREDITATION_EVIDENCE
    string FilePath,
    string TargetSchemaVersion);

public sealed record AiExtractedFieldDto(
    string FieldName,
    string ExtractedValue,
    int SourcePageNumber,
    string? SourceBoundingBox,
    double Confidence,
    bool IsInferred);

public sealed record AiExtractionResultDto(
    Guid ExtractionId,
    Guid DocumentId,
    string DocumentType,
    string SchemaVersion,
    IReadOnlyList<AiExtractedFieldDto> ExtractedFields,
    double OverallConfidence,
    string Status, // PENDING_REVIEW, APPROVED, REJECTED
    DateTimeOffset ExtractedAt);

// ── AI Discrepancy & Matrix Diagnostics DTOs (FR-AI-05) ──

public sealed record AiAnomalyIssueDto(
    string IssueCode,
    string Severity, // CRITICAL, WARNING, INFO
    string Category, // WEIGHT_SUM_INVALID, BLOOM_CONFLICT, PI_COVERAGE_GAP, DUPLICATE_CODE
    string Description,
    string AffectedEntity,
    string SuggestedRemediation);

public sealed record AiAnomalyDetectionResultDto(
    Guid ProgramVersionId,
    string ProgramCode,
    int TotalIssuesFound,
    int CriticalCount,
    int WarningCount,
    IReadOnlyList<AiAnomalyIssueDto> Issues,
    DateTimeOffset DiagnosedAt);

// ── Human-In-The-Loop (HITL) Review Queue DTOs (FR-AI-06) ──

public sealed record HitlReviewItemDto(
    Guid ReviewItemId,
    Guid ExtractionId,
    string EntityType,
    string FieldName,
    string OriginalExtractedValue,
    double Confidence,
    bool IsInferred,
    string ReviewStatus, // PENDING, ACCEPTED, MODIFIED, REJECTED
    DateTimeOffset CreatedAt);

public sealed record HitlDecisionRequest(
    Guid ReviewItemId,
    string Action, // ACCEPT, MODIFY, REJECT
    string? CorrectedValue,
    string ReasonNotes);

public sealed record HitlDecisionResultDto(
    Guid ReviewItemId,
    string FinalValue,
    string Status,
    string DecidedByStaffCode,
    DateTimeOffset DecidedAt);

// ── Prompt Versioning & Governance DTOs (FR-AI-07) ──

public sealed record PromptTemplateVersionDto(
    Guid Id,
    string PromptCode, // OBE_RAG_SYNTHESIS, BM13_EXTRACTION, CQI_RECOMMENDER
    int VersionNumber,
    string ModelProvider, // OPENAI, ANTHROPIC, GOOGLE, META
    string ModelName,     // gpt-4o, claude-3-5-sonnet, gemini-1.5-pro, llama-3-70b
    string SystemPromptTemplate,
    string OutputJsonSchema,
    bool IsActive,
    DateTimeOffset CreatedAt);

public sealed record RegisterPromptVersionRequest(
    string PromptCode,
    string ModelProvider,
    string ModelName,
    string SystemPromptTemplate,
    string OutputJsonSchema);

public sealed record PromptBenchmarkTestResultDto(
    string PromptCode,
    int VersionNumber,
    int TotalTestCases,
    int PassedTestCases,
    double AccuracyPercentage,
    double LatencyAverageMs,
    bool MeetsDeploymentCriteria);

// ── Security Hardening & Prompt Injection Guardrails DTOs (FR-AI-08) ──

public sealed record PromptInjectionScanResultDto(
    bool IsSafe,
    double RiskScore,
    string? DetectedThreatCategory, // INSTRUCTION_OVERRIDE, SYSTEM_LEAKAGE, SQL_INJECTION_PAYLOAD
    string MitigationActionTaken,   // ALLOWED, SANITIZED, BLOCKED
    DateTimeOffset ScannedAt);

public sealed record AiSecurityAuditLogDto(
    Guid Id,
    string PrincipalName,
    string ActionType,
    string QuerySanitized,
    bool TriggeredGuardrail,
    string GuardrailRule,
    DateTimeOffset Timestamp);
