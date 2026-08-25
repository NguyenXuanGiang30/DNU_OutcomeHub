using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Api.Controllers;

public sealed class OrgUnitsController : ApiControllerBase
{
    private readonly IOrgUnitRepository _orgUnitRepository;
    private readonly IRlsTransactionExecutor _rlsTransactionExecutor;

    public OrgUnitsController(
        IOrgUnitRepository orgUnitRepository,
        IRlsTransactionExecutor rlsTransactionExecutor)
    {
        _orgUnitRepository = orgUnitRepository ?? throw new ArgumentNullException(nameof(orgUnitRepository));
        _rlsTransactionExecutor = rlsTransactionExecutor ?? throw new ArgumentNullException(nameof(rlsTransactionExecutor));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OrgUnitDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OrgUnitDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read all OrgUnits");
        var result = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _orgUnitRepository.GetAllAsync(ct),
            cancellationToken);

        return OkResponse(result, "Danh sách đơn vị tổ chức.");
    }

    [HttpGet("tree")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OrgUnitTreeDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OrgUnitTreeDto>>>> GetTree(CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read OrgUnits tree");
        var result = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _orgUnitRepository.GetTreeAsync(ct),
            cancellationToken);

        return OkResponse(result, "Cây phân cấp cơ cấu tổ chức.");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrgUnitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrgUnitDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read OrgUnit {id}");
        var result = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _orgUnitRepository.GetByIdAsync(id, ct),
            cancellationToken);

        if (result == null)
        {
            throw new NotFoundException(nameof(OrgUnit), id);
        }

        return OkResponse(result, "Thông tin chi tiết đơn vị tổ chức.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrgUnitDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<OrgUnitDto>>> Create(
        [FromBody] CreateOrgUnitRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var orgUnit = OrgUnit.Create(
            id: Guid.NewGuid(),
            parentId: request.ParentId,
            code: request.Code,
            name: request.Name,
            unitType: request.UnitType,
            effectiveFrom: request.EffectiveFrom,
            effectiveTo: request.EffectiveTo,
            status: request.Status,
            createdBy: CurrentUser.PrincipalId);

        var context = GetDatabaseRequestContext("Create OrgUnit");
        var created = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _orgUnitRepository.CreateAsync(orgUnit, ct),
            cancellationToken);

        return CreatedResponse(nameof(GetById), new { id = created.Id }, created, "Tạo đơn vị tổ chức thành công.");
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrgUnitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrgUnitDto>>> Update(
        Guid id,
        [FromBody] UpdateOrgUnitRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var orgUnit = OrgUnit.Create(
            id: id,
            parentId: request.ParentId,
            code: "DUMMY",
            name: request.Name,
            unitType: request.UnitType,
            effectiveFrom: request.EffectiveFrom,
            effectiveTo: request.EffectiveTo,
            status: request.Status,
            createdBy: CurrentUser.PrincipalId);

        var context = GetDatabaseRequestContext($"Update OrgUnit {id}");
        var updated = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _orgUnitRepository.UpdateAsync(orgUnit, ct),
            cancellationToken);

        return OkResponse(updated, "Cập nhật đơn vị tổ chức thành công.");
    }
}
