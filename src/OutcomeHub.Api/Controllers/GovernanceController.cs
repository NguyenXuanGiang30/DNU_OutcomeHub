using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Iam;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/admin/governance")]
public sealed class GovernanceController : ApiControllerBase
{
    private readonly IIamRepository _repository;
    private readonly IIamService _service;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public GovernanceController(
        IIamRepository repository,
        IIamService service,
        IRlsTransactionExecutor rlsExecutor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpPost("legal-holds")]
    [ProducesResponseType(typeof(ApiResponse<LegalHoldDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<LegalHoldDto>>> CreateLegalHold(
        [FromBody] CreateLegalHoldRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext("Create Legal Hold");
        var hold = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.CreateLegalHoldAsync(request, context.PrincipalId, ct),
            cancellationToken);

        return OkResponse(hold, "Đã tạo đóng băng pháp lý (Legal Hold) thành công.");
    }

    [HttpPost("legal-holds/{id:guid}/release")]
    [ProducesResponseType(typeof(ApiResponse<LegalHoldDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<LegalHoldDto>>> ReleaseLegalHold(
        Guid id,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Release Legal Hold {id}");
        var hold = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.ReleaseLegalHoldAsync(id, ct),
            cancellationToken);

        return OkResponse(hold, "Đã giải phóng đóng băng pháp lý.");
    }

    [HttpGet("legal-holds/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LegalHoldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<LegalHoldDto>>> GetLegalHold(
        Guid id,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Legal Hold {id}");
        var hold = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetLegalHoldByIdAsync(id, ct),
            cancellationToken);

        if (hold == null)
        {
            throw new NotFoundException("LegalHold", id);
        }

        return OkResponse(hold, "Chi tiết đóng băng pháp lý.");
    }

    [HttpGet("legal-holds")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<LegalHoldDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<LegalHoldDto>>>> GetLegalHolds(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read Legal Holds List");
        var holds = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetLegalHoldsAsync(status, ct),
            cancellationToken);

        return OkResponse(holds, "Danh sách đóng băng pháp lý.");
    }
}
