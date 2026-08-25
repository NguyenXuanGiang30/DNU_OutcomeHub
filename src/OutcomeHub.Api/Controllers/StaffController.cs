using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/staff")]
public sealed class StaffController : ApiControllerBase
{
    private readonly IStaffRepository _staffRepository;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public StaffController(
        IStaffRepository staffRepository,
        IRlsTransactionExecutor rlsExecutor)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StaffDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<StaffDto>>>> GetPaged(
        [FromQuery] PagedRequest request,
        [FromQuery] Guid? homeOrgUnitId,
        [FromQuery] string? staffType,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read staff list");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _staffRepository.GetPagedStaffAsync(request, homeOrgUnitId, staffType, ct),
            cancellationToken);

        return PagedResponse(result, "Danh sách cán bộ giảng viên.");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<StaffDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StaffDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Staff {id}");
        var staff = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _staffRepository.GetStaffByIdAsync(id, ct),
            cancellationToken);

        if (staff == null)
        {
            throw new NotFoundException("Staff", id);
        }

        return OkResponse(staff, "Thông tin chi tiết cán bộ giảng viên.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StaffDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<StaffDto>>> Create(
        [FromBody] CreateStaffRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var personId = Guid.NewGuid();
        var person = Person.Create(
            personId,
            request.FullName,
            request.EffectiveFrom,
            null,
            request.CurrentStatus,
            request.SourceSystemId,
            request.SourcePersonId);

        var staff = Staff.Create(
            personId,
            request.StaffCode,
            request.HomeOrgUnitId,
            request.StaffType,
            request.CurrentStatus);

        var context = GetDatabaseRequestContext("Create Staff profile");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _staffRepository.CreateStaffAsync(person, staff, ct),
            cancellationToken);

        var resultDto = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _staffRepository.GetStaffByIdAsync(personId, ct),
            cancellationToken);

        return CreatedResponse(nameof(GetById), new { id = personId }, resultDto!, "Tạo hồ sơ giảng viên thành công.");
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<StaffDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<StaffDto>>> Update(
        Guid id,
        [FromBody] UpdateStaffRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Update Staff {id}");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _staffRepository.UpdateStaffAsync(
                id,
                request.FullName,
                request.HomeOrgUnitId,
                request.StaffType,
                request.CurrentStatus,
                request.EffectiveTo,
                ct),
            cancellationToken);

        var updated = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _staffRepository.GetStaffByIdAsync(id, ct),
            cancellationToken);

        return OkResponse(updated!, "Cập nhật thông tin giảng viên thành công.");
    }
}
