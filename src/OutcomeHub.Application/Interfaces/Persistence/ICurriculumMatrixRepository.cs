using OutcomeHub.Application.DTOs.Academic;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface ICurriculumMatrixRepository
{
    Task<StudentPathCoverageAnalysisDto?> AnalyzeStudentPathCoverageAsync(
        Guid programVersionId,
        Guid? curriculumPathId,
        CancellationToken cancellationToken);

    Task<CompetencyRoadmapDto?> GetCompetencyRoadmapAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<ProgramVersionDiffDto?> CompareProgramVersionsAsync(
        Guid sourceVersionId,
        Guid targetVersionId,
        CancellationToken cancellationToken);

    Task<PloCrosswalkDto?> GeneratePloCrosswalkAsync(
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

    Task<ProgramObjectiveMatrixDto?> GetProgramObjectiveMatrixAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<PrerequisiteGraphDto?> GetPrerequisiteGraphAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<KnowledgeBlockStructureDto?> GetKnowledgeBlockStructureAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<CurriculumSpecificationDto?> GenerateCurriculumSpecificationAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);

    Task<PublishingReadinessChecklistDto?> ValidatePublishingReadinessAsync(
        Guid programVersionId,
        CancellationToken cancellationToken);
}
