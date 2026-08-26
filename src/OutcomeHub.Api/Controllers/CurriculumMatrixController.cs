using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/curriculum-matrix")]
public sealed class CurriculumMatrixController : ApiControllerBase
{
    private readonly ICurriculumMatrixService _service;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public CurriculumMatrixController(
        ICurriculumMatrixService service,
        IRlsTransactionExecutor rlsExecutor)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpGet("coverage/{programVersionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<StudentPathCoverageAnalysisDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<StudentPathCoverageAnalysisDto>>> AnalyzeCoverage(
        Guid programVersionId,
        [FromQuery] Guid? curriculumPathId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Analyze Coverage {programVersionId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.AnalyzeCoverageAsync(programVersionId, curriculumPathId, ct),
            cancellationToken);

        return OkResponse(result, "Phân tích độ phủ chuẩn đầu ra (PLO/PI) theo StudentPath.");
    }

    [HttpGet("roadmap/{programVersionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CompetencyRoadmapDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CompetencyRoadmapDto>>> GetCompetencyRoadmap(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Get Competency Roadmap {programVersionId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetCompetencyRoadmapAsync(programVersionId, ct),
            cancellationToken);

        return OkResponse(result, "Lộ trình phát triển năng lực và ma trận Bloom theo học kỳ.");
    }

    [HttpGet("diff")]
    [ProducesResponseType(typeof(ApiResponse<ProgramVersionDiffDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProgramVersionDiffDto>>> CompareVersions(
        [FromQuery] Guid sourceVersionId,
        [FromQuery] Guid targetVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Compare Versions {sourceVersionId} vs {targetVersionId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.CompareVersionsAsync(sourceVersionId, targetVersionId, ct),
            cancellationToken);

        return OkResponse(result, "So sánh khác biệt giữa 2 phiên bản chương trình đào tạo.");
    }

    [HttpGet("crosswalk")]
    [ProducesResponseType(typeof(ApiResponse<PloCrosswalkDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PloCrosswalkDto>>> GetPloCrosswalk(
        [FromQuery] Guid sourceVersionId,
        [FromQuery] Guid targetVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Get PLO Crosswalk {sourceVersionId} to {targetVersionId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetPloCrosswalkAsync(sourceVersionId, targetVersionId, ct),
            cancellationToken);

        return OkResponse(result, "Bảng đối chiếu crosswalk chuẩn đầu ra giữa 2 phiên bản CTĐT.");
    }

    [HttpGet("dmp/{programVersionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DirectMeasurementPlanDetailsDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DirectMeasurementPlanDetailsDto>>>> GetDirectMeasurementPlans(
        Guid programVersionId,
        [FromQuery] Guid? curriculumPathId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Get Direct Measurement Plans {programVersionId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetDirectMeasurementPlansAsync(programVersionId, curriculumPathId, ct),
            cancellationToken);

        return OkResponse(result, "Kế hoạch đo lường trực tiếp (Direct Measurement Plan) theo PI.");
    }

    [HttpPost("dmp")]
    [ProducesResponseType(typeof(ApiResponse<DirectMeasurementPlanDetailsDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<DirectMeasurementPlanDetailsDto>>> SaveDirectMeasurementPlan(
        [FromBody] CreateDirectMeasurementPlanRequest request,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Save Direct Measurement Plan PI {request.ProgramPiId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.SaveDirectMeasurementPlanAsync(request, ct),
            cancellationToken);

        return CreatedResponse(
            nameof(GetDirectMeasurementPlans),
            new { programVersionId = request.ProgramVersionId },
            result,
            "Thiết lập kế hoạch đo lường trực tiếp (DMP) thành công.");
    }

    [HttpGet("po-matrix/{programVersionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProgramObjectiveMatrixDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProgramObjectiveMatrixDto>>> GetProgramObjectiveMatrix(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Get PO Matrix {programVersionId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetProgramObjectiveMatrixAsync(programVersionId, ct),
            cancellationToken);

        return OkResponse(result, "Ma trận mục tiêu đào tạo PO - PLO và khung năng lực 3 tầng.");
    }

    [HttpGet("prerequisites/{programVersionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PrerequisiteGraphDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PrerequisiteGraphDto>>> GetPrerequisiteGraph(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Get Prerequisite Graph {programVersionId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetPrerequisiteGraphAsync(programVersionId, ct),
            cancellationToken);

        return OkResponse(result, "Sơ đồ tiên quyết và đồ thị tiến trình học tập.");
    }

    [HttpGet("knowledge-blocks/{programVersionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<KnowledgeBlockStructureDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<KnowledgeBlockStructureDto>>> GetKnowledgeBlockStructure(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Get Knowledge Blocks {programVersionId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetKnowledgeBlockStructureAsync(programVersionId, ct),
            cancellationToken);

        return OkResponse(result, "Cơ cấu khối kiến thức chương trình đào tạo.");
    }

    [HttpGet("specification/{programVersionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CurriculumSpecificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CurriculumSpecificationDto>>> GenerateCurriculumSpecification(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Generate Specification {programVersionId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.GetCurriculumSpecificationAsync(programVersionId, ct),
            cancellationToken);

        return OkResponse(result, "Bản mô tả chương trình đào tạo (Curriculum Specification) hoàn chỉnh.");
    }

    [HttpGet("publishing-checklist/{programVersionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PublishingReadinessChecklistDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PublishingReadinessChecklistDto>>> ValidatePublishingChecklist(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Validate Publishing Checklist {programVersionId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.CheckPublishingReadinessAsync(programVersionId, ct),
            cancellationToken);

        return OkResponse(result, "Kiểm tra danh mục điều kiện ban hành chương trình đào tạo.");
    }
}
