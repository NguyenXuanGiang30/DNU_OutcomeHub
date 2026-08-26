using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Iam;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/admin/users")]
public sealed class UsersController : ApiControllerBase
{
    private readonly IIamRepository _repository;
    private readonly IIamService _service;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public UsersController(
        IIamRepository repository,
        IIamService service,
        IRlsTransactionExecutor rlsExecutor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserAccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UserAccountDto>>> CreateUser(
        [FromBody] CreateUserAccountRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext("Create User Account");
        var user = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.CreateUserAsync(request, ct),
            cancellationToken);

        return OkResponse(user, "Đã tạo tài khoản người dùng thành công.");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserDetailDto>>> GetUserDetail(
        Guid id,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read User Detail {id}");
        var user = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetUserDetailByIdAsync(id, ct),
            cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("UserAccount", id);
        }

        return OkResponse(user, "Chi tiết tài khoản người dùng.");
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UserAccountDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UserAccountDto>>>> GetUsers(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read Users List");
        var users = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetUsersAsync(status, ct),
            cancellationToken);

        return OkResponse(users, "Danh sách tài khoản người dùng.");
    }
}
