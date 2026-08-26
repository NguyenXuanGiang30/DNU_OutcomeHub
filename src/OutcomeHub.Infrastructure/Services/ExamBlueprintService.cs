using OutcomeHub.Application.DTOs.Portfolio;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Infrastructure.Services;

public sealed class ExamBlueprintService : IExamBlueprintService
{
    private readonly IExamBlueprintRepository _repository;

    public ExamBlueprintService(IExamBlueprintRepository repository)
    {
        _repository = repository;
    }

    public Task<ExamBlueprintDto?> GetExamBlueprintAsync(Guid syllabusVersionId, Guid assessmentItemId, CancellationToken cancellationToken)
    {
        return _repository.GetExamBlueprintAsync(syllabusVersionId, assessmentItemId, cancellationToken);
    }

    public Task<ExamBlueprintDto> SaveExamBlueprintAsync(CreateExamBlueprintRequest request, CancellationToken cancellationToken)
    {
        return _repository.SaveExamBlueprintAsync(request, cancellationToken);
    }

    public Task<SyllabusTraceabilityMatrix831Dto?> GetTraceabilityMatrix831Async(Guid syllabusVersionId, CancellationToken cancellationToken)
    {
        return _repository.GetTraceabilityMatrix831Async(syllabusVersionId, cancellationToken);
    }

    public Task<DirectAssessmentMatrix832Dto?> GetDirectAssessmentMatrix832Async(Guid syllabusVersionId, CancellationToken cancellationToken)
    {
        return _repository.GetDirectAssessmentMatrix832Async(syllabusVersionId, cancellationToken);
    }

    public Task<WeeklyScheduleDto?> GetWeeklyScheduleAsync(Guid syllabusVersionId, CancellationToken cancellationToken)
    {
        return _repository.GetWeeklyScheduleAsync(syllabusVersionId, cancellationToken);
    }

    public Task<WeeklyScheduleDto> SaveWeeklyScheduleAsync(SaveWeeklyScheduleRequest request, CancellationToken cancellationToken)
    {
        return _repository.SaveWeeklyScheduleAsync(request, cancellationToken);
    }

    public Task<IReadOnlyList<DocumentVaultItemDto>> GetDocumentsAsync(Guid syllabusVersionId, CancellationToken cancellationToken)
    {
        return _repository.GetDocumentsAsync(syllabusVersionId, cancellationToken);
    }

    public Task<DocumentVaultItemDto> UploadDocumentAsync(UploadDocumentRequest request, CancellationToken cancellationToken)
    {
        return _repository.UploadDocumentAsync(request, cancellationToken);
    }

    public Task<PortfolioPackageDto?> ExportPortfolioPackageAsync(ExportPortfolioPackageRequest request, CancellationToken cancellationToken)
    {
        return _repository.ExportPortfolioPackageAsync(request, cancellationToken);
    }

    public Task<AiSyllabusDraftResultDto> GenerateAiSyllabusDraftAsync(AiSyllabusDraftRequest request, CancellationToken cancellationToken)
    {
        return _repository.GenerateAiSyllabusDraftAsync(request, cancellationToken);
    }

    public Task<SyllabusPublishingChecklistDto?> ValidateSyllabusPublishingReadinessAsync(Guid syllabusVersionId, CancellationToken cancellationToken)
    {
        return _repository.ValidateSyllabusPublishingReadinessAsync(syllabusVersionId, cancellationToken);
    }
}
