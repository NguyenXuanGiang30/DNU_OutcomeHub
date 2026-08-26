using OutcomeHub.Application.DTOs.Portfolio;

namespace OutcomeHub.Application.Interfaces.Services;

public interface IExamBlueprintService
{
    // Exam Blueprint
    Task<ExamBlueprintDto?> GetExamBlueprintAsync(Guid syllabusVersionId, Guid assessmentItemId, CancellationToken cancellationToken);
    Task<ExamBlueprintDto> SaveExamBlueprintAsync(CreateExamBlueprintRequest request, CancellationToken cancellationToken);

    // Traceability Matrices (Table 8.3.1 & 8.3.2)
    Task<SyllabusTraceabilityMatrix831Dto?> GetTraceabilityMatrix831Async(Guid syllabusVersionId, CancellationToken cancellationToken);
    Task<DirectAssessmentMatrix832Dto?> GetDirectAssessmentMatrix832Async(Guid syllabusVersionId, CancellationToken cancellationToken);

    // Teaching Schedule
    Task<WeeklyScheduleDto?> GetWeeklyScheduleAsync(Guid syllabusVersionId, CancellationToken cancellationToken);
    Task<WeeklyScheduleDto> SaveWeeklyScheduleAsync(SaveWeeklyScheduleRequest request, CancellationToken cancellationToken);

    // Academic Document Vault & Portfolio
    Task<IReadOnlyList<DocumentVaultItemDto>> GetDocumentsAsync(Guid syllabusVersionId, CancellationToken cancellationToken);
    Task<DocumentVaultItemDto> UploadDocumentAsync(UploadDocumentRequest request, CancellationToken cancellationToken);
    Task<PortfolioPackageDto?> ExportPortfolioPackageAsync(ExportPortfolioPackageRequest request, CancellationToken cancellationToken);

    // AI Syllabus Draft Assistant
    Task<AiSyllabusDraftResultDto> GenerateAiSyllabusDraftAsync(AiSyllabusDraftRequest request, CancellationToken cancellationToken);

    // Syllabus Publishing Gatekeeper
    Task<SyllabusPublishingChecklistDto?> ValidateSyllabusPublishingReadinessAsync(Guid syllabusVersionId, CancellationToken cancellationToken);
}
