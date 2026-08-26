using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Infrastructure.Services;

public sealed class CurriculumMatrixService : ICurriculumMatrixService
{
    private readonly ICurriculumMatrixRepository _repository;

    public CurriculumMatrixService(ICurriculumMatrixRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<StudentPathCoverageAnalysisDto> AnalyzeCoverageAsync(
        Guid programVersionId,
        Guid? curriculumPathId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.AnalyzeStudentPathCoverageAsync(programVersionId, curriculumPathId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", programVersionId);
    }

    public async Task<CompetencyRoadmapDto> GetCompetencyRoadmapAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetCompetencyRoadmapAsync(programVersionId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", programVersionId);
    }

    public async Task<ProgramVersionDiffDto> CompareVersionsAsync(
        Guid sourceVersionId,
        Guid targetVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.CompareProgramVersionsAsync(sourceVersionId, targetVersionId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", $"{sourceVersionId} or {targetVersionId}");
    }

    public async Task<PloCrosswalkDto> GetPloCrosswalkAsync(
        Guid sourceVersionId,
        Guid targetVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GeneratePloCrosswalkAsync(sourceVersionId, targetVersionId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", $"{sourceVersionId} or {targetVersionId}");
    }

    public async Task<IReadOnlyList<DirectMeasurementPlanDetailsDto>> GetDirectMeasurementPlansAsync(
        Guid programVersionId,
        Guid? curriculumPathId,
        CancellationToken cancellationToken)
    {
        return await _repository.GetDirectMeasurementPlansAsync(programVersionId, curriculumPathId, cancellationToken);
    }

    public async Task<DirectMeasurementPlanDetailsDto> SaveDirectMeasurementPlanAsync(
        CreateDirectMeasurementPlanRequest request,
        CancellationToken cancellationToken)
    {
        return await _repository.SaveDirectMeasurementPlanAsync(request, cancellationToken);
    }

    public async Task<ProgramObjectiveMatrixDto> GetProgramObjectiveMatrixAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetProgramObjectiveMatrixAsync(programVersionId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", programVersionId);
    }

    public async Task<PrerequisiteGraphDto> GetPrerequisiteGraphAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPrerequisiteGraphAsync(programVersionId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", programVersionId);
    }

    public async Task<KnowledgeBlockStructureDto> GetKnowledgeBlockStructureAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetKnowledgeBlockStructureAsync(programVersionId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", programVersionId);
    }

    public async Task<CurriculumSpecificationDto> GetCurriculumSpecificationAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GenerateCurriculumSpecificationAsync(programVersionId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", programVersionId);
    }

    public async Task<PublishingReadinessChecklistDto> CheckPublishingReadinessAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var result = await _repository.ValidatePublishingReadinessAsync(programVersionId, cancellationToken);
        return result ?? throw new NotFoundException("ProgramVersion", programVersionId);
    }
}
