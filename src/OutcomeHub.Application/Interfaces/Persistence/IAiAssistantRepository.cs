using OutcomeHub.Application.DTOs.Ai;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IAiAssistantRepository
{
    // OBE RAG Chatbot (FR-AI-01, FR-AI-02, FR-AI-03)
    Task<AiChatResponseDto> QueryChatbotAsync(AiChatQueryRequest request, CancellationToken cancellationToken);

    // AI Extraction Engine (FR-AI-04)
    Task<AiExtractionResultDto> ExtractDocumentAsync(AiDocumentExtractionRequest request, CancellationToken cancellationToken);

    // Anomaly & Discrepancy Diagnostics (FR-AI-05)
    Task<AiAnomalyDetectionResultDto> RunDiagnosticsAsync(Guid programVersionId, CancellationToken cancellationToken);

    // Human-In-The-Loop Review Queue (FR-AI-06)
    Task<IReadOnlyList<HitlReviewItemDto>> GetHitlQueueAsync(Guid? extractionId, string? status, CancellationToken cancellationToken);
    Task<HitlDecisionResultDto> SubmitHitlDecisionAsync(HitlDecisionRequest request, CancellationToken cancellationToken);

    // Prompt Governance & Benchmarking (FR-AI-07)
    Task<IReadOnlyList<PromptTemplateVersionDto>> GetPromptVersionsAsync(string? promptCode, CancellationToken cancellationToken);
    Task<PromptTemplateVersionDto> RegisterPromptVersionAsync(RegisterPromptVersionRequest request, CancellationToken cancellationToken);
    Task<PromptBenchmarkTestResultDto> RunPromptBenchmarkAsync(string promptCode, int versionNumber, CancellationToken cancellationToken);

    // Security Hardening & Guardrails (FR-AI-08)
    Task<PromptInjectionScanResultDto> ScanPromptInjectionAsync(string textToScan, CancellationToken cancellationToken);
    Task<IReadOnlyList<AiSecurityAuditLogDto>> GetSecurityAuditLogsAsync(CancellationToken cancellationToken);
}
