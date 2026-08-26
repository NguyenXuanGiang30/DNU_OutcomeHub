using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Iam;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Application.Interfaces.Services;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/admin/roles")]
public sealed class RolesController : ApiControllerBase
{
    private readonly IIamRepository _repository;
    private readonly IIamService _service;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public RolesController(
        IIamRepository repository,
        IIamService service,
        IRlsTransactionExecutor rlsExecutor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole(
        [FromBody] CreateRoleRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext("Create Role");
        var role = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.CreateRoleAsync(request, context.PrincipalId, ct),
            cancellationToken);

        return OkResponse(role, "Đã tạo vai trò thành công.");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RoleDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<RoleDetailDto>>> GetRoleDetail(
        Guid id,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Role Detail {id}");
        var role = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetRoleDetailByIdAsync(id, ct),
            cancellationToken);

        if (role == null)
        {
            throw new NotFoundException("Role", id);
        }

        return OkResponse(role, "Chi tiết vai trò.");
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RoleDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RoleDto>>>> GetRoles(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read Roles List");
        var roles = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetRolesAsync(status, ct),
            cancellationToken);

        return OkResponse(roles, "Danh sách vai trò.");
    }

    [HttpGet("permissions")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PermissionDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PermissionDto>>>> GetAllPermissions(
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read Permissions List");
        var permissions = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetAllPermissionsAsync(ct),
            cancellationToken);

        return OkResponse(permissions, "Danh mục quyền hạn trong hệ thống.");
    }

    // Role Assignments
    [HttpPost("assignments")]
    [ProducesResponseType(typeof(ApiResponse<RoleAssignmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<RoleAssignmentDto>>> AssignRole(
        [FromBody] AssignRoleRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext("Assign Role to Principal");
        var assignment = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.AssignRoleAsync(request, context.PrincipalId, ct),
            cancellationToken);

        return OkResponse(assignment, "Đã gán vai trò cho người dùng thành công.");
    }

    [HttpDelete("assignments/{assignmentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RoleAssignmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<RoleAssignmentDto>>> RevokeRoleAssignment(
        Guid assignmentId,
        [FromBody] RevokeRoleAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Revoke Role Assignment {assignmentId}");
        var assignment = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _service.RevokeRoleAssignmentAsync(assignmentId, request, ct),
            cancellationToken);

        return OkResponse(assignment, "Đã thu hồi vai trò thành công.");
    }

    [HttpGet("assignments")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RoleAssignmentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RoleAssignmentDto>>>> GetRoleAssignments(
        [FromQuery] Guid? principalId,
        [FromQuery] Guid? roleId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read Role Assignments");
        var assignments = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.GetRoleAssignmentsAsync(principalId, roleId, status, ct),
            cancellationToken);

        return OkResponse(assignments, "Danh sách gán vai trò.");
    }
}
