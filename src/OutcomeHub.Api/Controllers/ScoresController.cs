using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Measurement;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/scores")]
public sealed class ScoresController : ApiControllerBase
{
    private readonly IScoreRepository _scoreRepository;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public ScoresController(
        IScoreRepository scoreRepository,
        IRlsTransactionExecutor rlsExecutor)
    {
        _scoreRepository = scoreRepository ?? throw new ArgumentNullException(nameof(scoreRepository));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ScoreRecordDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<ScoreRecordDto>>>> GetPaged(
        [FromQuery] PagedRequest request,
        [FromQuery] Guid? courseOfferingId,
        [FromQuery] Guid? studentId,
        [FromQuery] Guid? assessmentItemId,
        [FromQuery] short? academicYearStart,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read scores list");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _scoreRepository.GetPagedScoresAsync(
                request,
                courseOfferingId,
                studentId,
                assessmentItemId,
                academicYearStart,
                ct),
            cancellationToken);

        return PagedResponse(result, "Danh sách điểm chi tiết theo tiêu chí và bài đánh giá.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ScoreRecordDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<ScoreRecordDto>>> SubmitScore(
        [FromBody] SubmitScoreRecordRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var scoreIdentityId = Guid.NewGuid();
        var scoreIdentity = ScoreIdentity.Create(
            scoreIdentityId,
            request.ScoreDatasetId,
            request.AcademicYearStart,
            request.StudentId,
            request.CourseOfferingId,
            request.ProgramVersionId,
            request.SyllabusVersionId,
            request.AttemptNo,
            request.EnrollmentId,
            request.AssessmentItemId,
            request.ScoreLevel,
            request.RubricCriterionId,
            request.AssessmentQuestionId);

        var recordId = Guid.NewGuid();
        var scoreRecord = ScoreRecord.Create(
            request.AcademicYearStart,
            recordId,
            scoreIdentityId,
            request.StudentId,
            request.CourseOfferingId,
            request.OrgUnitId,
            request.ProgramId,
            request.ProgramVersionId,
            request.CourseId,
            request.RevisionNo,
            request.RawScore,
            request.MaxScore,
            request.ScoreStatus,
            request.SourceSystemId,
            request.SourceRecordId,
            request.SourceRevision,
            request.IngestionBatchId,
            request.RecordedBy,
            request.RecordedAt,
            request.Checksum,
            request.SupersedesId,
            request.CorrectionReason);

        var context = GetDatabaseRequestContext("Submit score record");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _scoreRepository.SubmitScoreRecordAsync(scoreIdentity, scoreRecord, ct),
            cancellationToken);

        var pagedResult = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _scoreRepository.GetPagedScoresAsync(
                new PagedRequest { PageNumber = 1, PageSize = 10 },
                request.CourseOfferingId,
                request.StudentId,
                request.AssessmentItemId,
                request.AcademicYearStart,
                ct),
            cancellationToken);

        var dto = pagedResult.Items.FirstOrDefault(s => s.Id == recordId);
        return Created(string.Empty, ApiResponse.Ok(dto!, "Ghi nhận điểm thành công."));
    }
}
