using OutcomeHub.Application.DTOs.Academic;

namespace OutcomeHub.Application.Interfaces.Services;

public interface ICurriculumMatrixService
{
    Task<StudentPathCoverageAnalysisDto> AnalyzeCoverageAsync(
        Guid programVersionId,
        Guid? curriculumPathId,
        CancellationToken cancellationToken);

    Task<CompetencyRoadmapDto> GetCompetencyRoadmapAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<ProgramVersionDiffDto> CompareVersionsAsync(
        Guid sourceVersionId,
        Guid targetVersionId,
        CancellationToken cancellationToken);

    Task<PloCrosswalkDto> GetPloCrosswalkAsync(
        Guid sourceVersionId,
        Guid targetVersionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DirectMeasurementPlanDetailsDto>> GetDirectMeasurementPlansAsync(
        Guid programVersionId,
        Guid? curriculumPathId,
        CancellationToken cancellationToken);

    Task<DirectMeasurementPlanDetailsDto> SaveDirectMeasurementPlanAsync(
        CreateDirectMeasurementPlanRequest request,
        CancellationToken cancellationToken);

    Task<ProgramObjectiveMatrixDto> GetProgramObjectiveMatrixAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<PrerequisiteGraphDto> GetPrerequisiteGraphAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<KnowledgeBlockStructureDto> GetKnowledgeBlockStructureAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<CurriculumSpecificationDto> GetCurriculumSpecificationAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<PublishingReadinessChecklistDto> CheckPublishingReadinessAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);
}
