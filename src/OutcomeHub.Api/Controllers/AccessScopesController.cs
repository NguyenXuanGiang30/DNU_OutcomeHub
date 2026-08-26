using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Iam;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/admin/access-scopes")]
public sealed class AccessScopesController : ApiControllerBase
{
    private readonly IIamRepository _repository;
    private readonly IIamService _service;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public AccessScopesController(
        IIamRepository repository,
        IIamService service,
        IRlsTransactionExecutor rlsExecutor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AccessScopeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AccessScopeDto>>> CreateAccessScope(
        [FromBody] CreateAccessScopeRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext("Create Access Scope");
        var scope = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.CreateAccessScopeAsync(request, ct),
            cancellationToken);

        return OkResponse(scope, "Đã tạo phạm vi truy cập (Access Scope) thành công.");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AccessScopeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AccessScopeDto>>> GetAccessScope(
        Guid id,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Access Scope {id}");
        var scope = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetAccessScopeByIdAsync(id, ct),
            cancellationToken);

        if (scope == null)
        {
            throw new NotFoundException("AccessScope", id);
        }

        return OkResponse(scope, "Chi tiết phạm vi truy cập.");
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AccessScopeDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AccessScopeDto>>>> GetAccessScopes(
        [FromQuery] string? scopeType,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read Access Scopes List");
        var scopes = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetAccessScopesAsync(scopeType, ct),
            cancellationToken);

        return OkResponse(scopes, "Danh sách phạm vi truy cập.");
    }
}
