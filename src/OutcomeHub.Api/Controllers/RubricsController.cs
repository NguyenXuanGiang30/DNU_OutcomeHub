using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Portfolio;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class RubricsController : ApiControllerBase
{
    private readonly IRubricRepository _rubricRepository;
    private readonly IRlsTransactionExecutor _rlsTransactionExecutor;

    public RubricsController(
        IRubricRepository rubricRepository,
        IRlsTransactionExecutor rlsTransactionExecutor)
    {
        _rubricRepository = rubricRepository ?? throw new ArgumentNullException(nameof(rubricRepository));
        _rlsTransactionExecutor = rlsTransactionExecutor ?? throw new ArgumentNullException(nameof(rlsTransactionExecutor));
    }

    [HttpGet("syllabuses/versions/{versionId:guid}/assessment-items")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AssessmentItemDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AssessmentItemDto>>>> GetAssessmentItems(
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read AssessmentItems for SyllabusVersion {versionId}");
        var items = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _rubricRepository.GetAssessmentItemsAsync(versionId, ct),
            cancellationToken);

        return OkResponse(items, "Danh mục đầu điểm đánh giá học phần.");
    }

    [HttpPost("syllabuses/versions/{versionId:guid}/assessment-items")]
    [ProducesResponseType(typeof(ApiResponse<AssessmentItemDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<AssessmentItemDto>>> CreateAssessmentItem(
        Guid versionId,
        [FromBody] CreateAssessmentItemRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var item = AssessmentItem.Create(
            Guid.NewGuid(),
            versionId,
            request.ParentId,
            request.AssessmentCode,
            request.Name,
            request.AssessmentType,
            request.CourseWeightRatio,
            request.IndividualComponentRatio,
            request.IsGroupAssessment,
            request.CountsTowardCourseGrade,
            request.MaxScore,
            request.SortOrder);

        var context = GetDatabaseRequestContext($"Create AssessmentItem for SyllabusVersion {versionId}");
        var created = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _rubricRepository.CreateAssessmentItemAsync(item, ct),
            cancellationToken);

        var dto = new AssessmentItemDto(
            created.Id,
            created.SyllabusVersionId,
            created.ParentId,
            created.AssessmentCode,
            created.Name,
            created.AssessmentType,
            created.CourseWeightRatio,
            created.IndividualComponentRatio,
            created.IsGroupAssessment,
            created.CountsTowardCourseGrade,
            created.MaxScore,
            created.SortOrder,
            HasRubric: false);

        return Created(string.Empty, ApiResponse.Ok(dto, "Tạo đầu điểm đánh giá thành công."));
    }

    [HttpGet("syllabuses/versions/{versionId:guid}/rubrics")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RubricDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RubricDto>>>> GetRubrics(
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read rubrics for SyllabusVersion {versionId}");
        var rubrics = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _rubricRepository.GetRubricsBySyllabusVersionIdAsync(versionId, ct),
            cancellationToken);

        return OkResponse(rubrics, "Danh sách ma trận Rubric và tiêu chí đánh giá.");
    }

    [HttpGet("rubrics/{rubricId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RubricDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<RubricDto>>> GetRubricById(
        Guid rubricId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Rubric {rubricId}");
        var rubric = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _rubricRepository.GetRubricByIdAsync(rubricId, ct),
            cancellationToken);

        if (rubric == null)
        {
            throw new NotFoundException("Rubric", rubricId);
        }

        return OkResponse(rubric, "Chi tiết Rubric.");
    }

    [HttpPost("syllabuses/versions/{versionId:guid}/rubrics")]
    [ProducesResponseType(typeof(ApiResponse<RubricDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<RubricDto>>> CreateRubric(
        Guid versionId,
        [FromBody] CreateRubricRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var rubricId = Guid.NewGuid();
        var checksum = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        var rubric = Rubric.Create(
            rubricId,
            versionId,
            request.SyllabusTemplateVersionId,
            request.AssessmentItemId,
            request.Code,
            request.Name,
            request.MaxScore,
            request.RubricScaleId,
            checksum);

        var criteriaList = new List<RubricCriterion>();
        if (request.Criteria != null)
        {
            foreach (var cReq in request.Criteria)
            {
                var criterionId = Guid.NewGuid();
                var criterion = RubricCriterion.Create(
                    criterionId,
                    rubricId,
                    request.AssessmentItemId,
                    versionId,
                    cReq.CriterionCode,
                    cReq.Description,
                    cReq.MaxScore,
                    cReq.RubricWeightRatio,
                    cReq.ScoreSourceMode,
                    cReq.IsCore,
                    cReq.IndividualEvidence,
                    cReq.SortOrder);

                if (cReq.Levels != null)
                {
                    foreach (var lReq in cReq.Levels)
                    {
                        var level = RubricLevel.Create(
                            Guid.NewGuid(),
                            criterionId,
                            lReq.LevelCode,
                            lReq.LevelOrder,
                            lReq.Label,
                            lReq.Description,
                            lReq.ScoreFrom,
                            lReq.ScoreTo,
                            lReq.NumericValue);
                        criterion.Levels.Add(level);
                    }
                }

                criteriaList.Add(criterion);
            }
        }

        var context = GetDatabaseRequestContext($"Create Rubric for SyllabusVersion {versionId}");
        var created = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _rubricRepository.CreateRubricAsync(rubric, criteriaList, ct),
            cancellationToken);

        var resultDto = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _rubricRepository.GetRubricByIdAsync(created.Id, ct),
            cancellationToken);

        return Created(string.Empty, ApiResponse.Ok(resultDto!, "Tạo Rubric thành công."));
    }
}
