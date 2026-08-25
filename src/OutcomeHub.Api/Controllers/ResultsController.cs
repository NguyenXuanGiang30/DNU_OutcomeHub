using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Result;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/results")]
public sealed class ResultsController : ApiControllerBase
{
    private readonly IResultRepository _resultRepository;
    private readonly IOutcomeCalculationService _calculationService;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public ResultsController(
        IResultRepository resultRepository,
        IOutcomeCalculationService calculationService,
        IRlsTransactionExecutor rlsExecutor)
    {
        _resultRepository = resultRepository ?? throw new ArgumentNullException(nameof(resultRepository));
        _calculationService = calculationService ?? throw new ArgumentNullException(nameof(calculationService));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpPost("calculate")]
    [ProducesResponseType(typeof(ApiResponse<ResultBatchDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ResultBatchDto>>> TriggerCalculation(
        [FromBody] TriggerCalculationRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext("Trigger OBE outcome calculation batch");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _calculationService.CalculatePeriodOutcomesAsync(
                request.MeasurementPeriodId,
                request.CalculationReason,
                context.PrincipalId,
                ct),
            cancellationToken);

        return OkResponse(result, "Đã tính toán kết quả chuẩn đầu ra (CLO -> PI -> PLO) thành công.");
    }

    [HttpGet("batches/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ResultBatchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ResultBatchDto>>> GetBatchById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read ResultBatch {id}");
        var batch = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _resultRepository.GetResultBatchByIdAsync(id, ct),
            cancellationToken);

        if (batch == null)
        {
            throw new NotFoundException("ResultBatch", id);
        }

        return OkResponse(batch, "Thông tin đợt tính toán kết quả.");
    }

    [HttpGet("periods/{periodId:guid}/batches")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ResultBatchDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ResultBatchDto>>>> GetBatchesByPeriodId(
        Guid periodId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read ResultBatches for period {periodId}");
        var batches = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _resultRepository.GetBatchesByPeriodIdAsync(periodId, ct),
            cancellationToken);

        return OkResponse(batches, "Danh sách các đợt tính toán của đợt đo lường.");
    }

    [HttpGet("batches/{batchId:guid}/clos")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<StudentCloResultDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<StudentCloResultDto>>>> GetStudentCloResults(
        Guid batchId,
        [FromQuery] Guid? studentId,
        [FromQuery] Guid? courseOfferingId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read StudentCloResults for batch {batchId}");
        var results = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _resultRepository.GetStudentCloResultsAsync(batchId, studentId, courseOfferingId, ct),
            cancellationToken);

        return OkResponse(results, "Kết quả đạt chuẩn đầu ra học phần (CLO) của sinh viên.");
    }

    [HttpGet("batches/{batchId:guid}/pis")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<StudentPiResultDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<StudentPiResultDto>>>> GetStudentPiResults(
        Guid batchId,
        [FromQuery] Guid? studentId,
        [FromQuery] Guid? programVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read StudentPiResults for batch {batchId}");
        var results = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _resultRepository.GetStudentPiResultsAsync(batchId, studentId, programVersionId, ct),
            cancellationToken);

        return OkResponse(results, "Kết quả đạt chỉ số thực hiện (PI) của sinh viên.");
    }

    [HttpGet("batches/{batchId:guid}/plos")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<StudentPloResultDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<StudentPloResultDto>>>> GetStudentPloResults(
        Guid batchId,
        [FromQuery] Guid? studentId,
        [FromQuery] Guid? programVersionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read StudentPloResults for batch {batchId}");
        var results = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _resultRepository.GetStudentPloResultsAsync(batchId, studentId, programVersionId, ct),
            cancellationToken);

        return OkResponse(results, "Kết quả đạt chuẩn đầu ra chương trình (PLO) của sinh viên.");
    }

    [HttpGet("batches/{batchId:guid}/cohort-outcomes")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CohortOutcomeResultDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CohortOutcomeResultDto>>>> GetCohortOutcomeResults(
        Guid batchId,
        [FromQuery] Guid? cohortId,
        [FromQuery] string? outcomeLevel,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read CohortOutcomeResults for batch {batchId}");
        var results = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _resultRepository.GetCohortOutcomeResultsAsync(batchId, cohortId, outcomeLevel, ct),
            cancellationToken);

        return OkResponse(results, "Kết quả tổng hợp đạt chuẩn đầu ra cấp Khóa / CTĐT.");
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ApiResponse<ProgramOutcomeDashboardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProgramOutcomeDashboardDto>>> GetProgramDashboard(
        [FromQuery] Guid periodId,
        [FromQuery] Guid programVersionId,
        [FromQuery] Guid cohortId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read Program Outcome Dashboard");
        var dashboard = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _resultRepository.GetProgramOutcomeDashboardAsync(periodId, programVersionId, cohortId, ct),
            cancellationToken);

        if (dashboard == null)
        {
            throw new NotFoundException("ProgramOutcomeDashboard", $"{periodId}_{programVersionId}_{cohortId}");
        }

        return OkResponse(dashboard, "Bảng tổng hợp tiến độ và tỷ lệ đạt chuẩn đầu ra CTĐT.");
    }
}
