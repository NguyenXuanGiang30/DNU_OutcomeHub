using OutcomeHub.Application.DTOs.Ai;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Infrastructure.Services;

public sealed class AiAssistantService : IAiAssistantService
{
    private readonly IAiAssistantRepository _repository;

    public AiAssistantService(IAiAssistantRepository repository)
    {
        _repository = repository;
    }

    public Task<AiChatResponseDto> QueryChatbotAsync(AiChatQueryRequest request, CancellationToken cancellationToken)
    {
        return _repository.QueryChatbotAsync(request, cancellationToken);
    }

    public Task<AiExtractionResultDto> ExtractDocumentAsync(AiDocumentExtractionRequest request, CancellationToken cancellationToken)
    {
        return _repository.ExtractDocumentAsync(request, cancellationToken);
    }

    public Task<AiAnomalyDetectionResultDto> RunDiagnosticsAsync(Guid programVersionId, CancellationToken cancellationToken)
    {
        return _repository.RunDiagnosticsAsync(programVersionId, cancellationToken);
    }

    public Task<IReadOnlyList<HitlReviewItemDto>> GetHitlQueueAsync(Guid? extractionId, string? status, CancellationToken cancellationToken)
    {
        return _repository.GetHitlQueueAsync(extractionId, status, cancellationToken);
    }

    public Task<HitlDecisionResultDto> SubmitHitlDecisionAsync(HitlDecisionRequest request, CancellationToken cancellationToken)
    {
        return _repository.SubmitHitlDecisionAsync(request, cancellationToken);
    }

    public Task<IReadOnlyList<PromptTemplateVersionDto>> GetPromptVersionsAsync(string? promptCode, CancellationToken cancellationToken)
    {
        return _repository.GetPromptVersionsAsync(promptCode, cancellationToken);
    }

    public Task<PromptTemplateVersionDto> RegisterPromptVersionAsync(RegisterPromptVersionRequest request, CancellationToken cancellationToken)
    {
        return _repository.RegisterPromptVersionAsync(request, cancellationToken);
    }

    public Task<PromptBenchmarkTestResultDto> RunPromptBenchmarkAsync(string promptCode, int versionNumber, CancellationToken cancellationToken)
    {
        return _repository.RunPromptBenchmarkAsync(promptCode, versionNumber, cancellationToken);
    }

    public Task<PromptInjectionScanResultDto> ScanPromptInjectionAsync(string textToScan, CancellationToken cancellationToken)
    {
        return _repository.ScanPromptInjectionAsync(textToScan, cancellationToken);
    }

    public Task<IReadOnlyList<AiSecurityAuditLogDto>> GetSecurityAuditLogsAsync(CancellationToken cancellationToken)
    {
        return _repository.GetSecurityAuditLogsAsync(cancellationToken);
    }
}
