using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/cohorts")]
public sealed class CohortsController : ApiControllerBase
{
    private readonly ICohortRepository _cohortRepository;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public CohortsController(
        ICohortRepository cohortRepository,
        IRlsTransactionExecutor rlsExecutor)
    {
        _cohortRepository = cohortRepository ?? throw new ArgumentNullException(nameof(cohortRepository));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CohortDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<CohortDto>>>> GetPaged(
        [FromQuery] PagedRequest request,
        [FromQuery] Guid? programId,
        [FromQuery] int? admissionYear,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read cohorts list");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _cohortRepository.GetPagedCohortsAsync(request, programId, admissionYear, ct),
            cancellationToken);

        return PagedResponse(result, "Danh sách khóa tuyển sinh (Cohort).");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CohortDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CohortDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Cohort {id}");
        var cohort = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _cohortRepository.GetCohortByIdAsync(id, ct),
            cancellationToken);

        if (cohort == null)
        {
            throw new NotFoundException("Cohort", id);
        }

        return OkResponse(cohort, "Thông tin khóa tuyển sinh.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CohortDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CohortDto>>> Create(
        [FromBody] CreateCohortRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var cohortId = Guid.NewGuid();
        var cohort = Cohort.Create(
            cohortId,
            request.ProgramId,
            request.Code,
            request.Name,
            request.AdmissionYear,
            request.StartDate,
            request.EndDate);

        var context = GetDatabaseRequestContext("Create Cohort");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _cohortRepository.CreateCohortAsync(cohort, ct),
            cancellationToken);

        var resultDto = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _cohortRepository.GetCohortByIdAsync(cohortId, ct),
            cancellationToken);

        return CreatedResponse(nameof(GetById), new { id = cohortId }, resultDto!, "Tạo khóa tuyển sinh thành công.");
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CohortDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CohortDto>>> Update(
        Guid id,
        [FromBody] UpdateCohortRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Update Cohort {id}");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _cohortRepository.UpdateCohortAsync(id, request.Name, request.EndDate, ct),
            cancellationToken);

        var updated = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _cohortRepository.GetCohortByIdAsync(id, ct),
            cancellationToken);

        return OkResponse(updated!, "Cập nhật khóa tuyển sinh thành công.");
    }
}
