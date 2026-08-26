using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Iam;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/admin/sod")]
public sealed class SodController : ApiControllerBase
{
    private readonly IIamRepository _repository;
    private readonly IIamService _service;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public SodController(
        IIamRepository repository,
        IIamService service,
        IRlsTransactionExecutor rlsExecutor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpGet("rules")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SodRuleDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SodRuleDto>>>> GetSodRules(
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read SoD Rules List");
        var rules = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetSodRulesAsync(ct),
            cancellationToken);

        return OkResponse(rules, "Danh sách quy tắc tách bạch nhiệm vụ (Separation of Duties).");
    }

    [HttpPost("check")]
    [ProducesResponseType(typeof(ApiResponse<SodViolationCheckResultDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<SodViolationCheckResultDto>>> CheckSodViolation(
        [FromBody] CheckSodViolationRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext("Check SoD Violation");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.CheckSodViolationAsync(request, ct),
            cancellationToken);

        return OkResponse(result, result.HasViolation
            ? "Phát hiện vi phạm quy tắc tách bạch nhiệm vụ (SoD)."
            : "Không phát hiện vi phạm quy tắc tách bạch nhiệm vụ.");
    }
}
