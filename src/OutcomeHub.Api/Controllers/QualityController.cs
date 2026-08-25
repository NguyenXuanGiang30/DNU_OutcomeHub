using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Quality;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/quality")]
public sealed class QualityController : ApiControllerBase
{
    private readonly IImprovementPlanRepository _repository;
    private readonly IImprovementPlanService _service;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public QualityController(
        IImprovementPlanRepository repository,
        IImprovementPlanService service,
        IRlsTransactionExecutor rlsExecutor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpPost("plans")]
    [ProducesResponseType(typeof(ApiResponse<ImprovementPlanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ImprovementPlanDto>>> CreatePlan(
        [FromBody] CreateImprovementPlanRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext("Create CQI Improvement Plan");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.CreatePlanAsync(request, context.PrincipalId, ct),
            cancellationToken);

        return OkResponse(result, "Đã tạo kế hoạch cải tiến chất lượng (CQI) thành công.");
    }

    [HttpGet("plans/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ImprovementPlanDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ImprovementPlanDetailDto>>> GetPlanDetail(
        Guid id,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read ImprovementPlan {id}");
        var plan = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetPlanDetailByIdAsync(id, ct),
            cancellationToken);

        if (plan == null)
        {
            throw new NotFoundException("ImprovementPlan", id);
        }

        return OkResponse(plan, "Chi tiết kế hoạch cải tiến chất lượng.");
    }

    [HttpGet("plans")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ImprovementPlanDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ImprovementPlanDto>>>> GetPlans(
        [FromQuery] Guid? programVersionId,
        [FromQuery] Guid? orgUnitId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read ImprovementPlans");
        var plans = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetPlansAsync(programVersionId, orgUnitId, status, ct),
            cancellationToken);

        return OkResponse(plans, "Danh sách kế hoạch cải tiến chất lượng.");
    }

    [HttpPut("plans/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ImprovementPlanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ImprovementPlanDto>>> UpdatePlan(
        Guid id,
        [FromBody] UpdateImprovementPlanRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Update ImprovementPlan {id}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.UpdatePlanAsync(id, request, ct),
            cancellationToken);

        return OkResponse(result, "Đã cập nhật kế hoạch cải tiến chất lượng.");
    }

    [HttpPost("plans/{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<ImprovementPlanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ImprovementPlanDto>>> TransitionPlanStatus(
        Guid id,
        [FromBody] TransitionPlanStatusRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Transition ImprovementPlan {id} status");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.TransitionPlanStatusAsync(id, request, ct),
            cancellationToken);

        return OkResponse(result, $"Đã chuyển trạng thái kế hoạch cải tiến sang '{request.NewStatus}'.");
    }

    [HttpPost("plans/{id:guid}/actions")]
    [ProducesResponseType(typeof(ApiResponse<ImprovementActionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ImprovementActionDto>>> AddAction(
        Guid id,
        [FromBody] CreateImprovementActionRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Add action to ImprovementPlan {id}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.AddActionAsync(id, request, ct),
            cancellationToken);

        return OkResponse(result, "Đã thêm hành động cải tiến mới.");
    }

    [HttpPatch("actions/{actionId:guid}/progress")]
    [ProducesResponseType(typeof(ApiResponse<ImprovementActionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ImprovementActionDto>>> UpdateActionProgress(
        Guid actionId,
        [FromBody] UpdateActionProgressRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Update progress for action {actionId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.UpdateActionProgressAsync(actionId, request, ct),
            cancellationToken);

        return OkResponse(result, "Đã cập nhật tiến độ hành động cải tiến.");
    }

    [HttpPost("plans/{id:guid}/evidences")]
    [ProducesResponseType(typeof(ApiResponse<ImprovementEvidenceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ImprovementEvidenceDto>>> AttachEvidence(
        Guid id,
        [FromBody] AttachImprovementEvidenceRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Attach evidence to ImprovementPlan {id}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.AttachEvidenceAsync(id, request, ct),
            cancellationToken);

        return OkResponse(result, "Đã đính kèm minh chứng thực hiện.");
    }

    [HttpPost("evidences/{evidenceId:guid}/verify")]
    [ProducesResponseType(typeof(ApiResponse<ImprovementEvidenceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ImprovementEvidenceDto>>> VerifyEvidence(
        Guid evidenceId,
        [FromBody] VerifyEvidenceRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Verify evidence {evidenceId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.VerifyEvidenceAsync(evidenceId, request, ct),
            cancellationToken);

        return OkResponse(result, "Đã xác minh minh chứng thực hiện.");
    }

    [HttpPost("plans/{id:guid}/remeasurements")]
    [ProducesResponseType(typeof(ApiResponse<RemeasurementEvaluationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<RemeasurementEvaluationDto>>> CreateRemeasurement(
        Guid id,
        [FromBody] CreateRemeasurementEvaluationRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Create remeasurement for ImprovementPlan {id}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.CreateRemeasurementAsync(id, request, context.PrincipalId, ct),
            cancellationToken);

        return OkResponse(result, "Đã ghi nhận kết quả đo lại (remeasurement) và đánh giá tác động cải tiến.");
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ApiResponse<CqiDashboardSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CqiDashboardSummaryDto>>> GetCqiDashboard(
        [FromQuery] Guid? programVersionId,
        [FromQuery] Guid? orgUnitId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read CQI Dashboard");
        var dashboard = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetCqiDashboardAsync(programVersionId, orgUnitId, ct),
            cancellationToken);

        return OkResponse(dashboard, "Bảng tổng hợp kế hoạch cải tiến chất lượng (CQI Dashboard).");
    }
}
