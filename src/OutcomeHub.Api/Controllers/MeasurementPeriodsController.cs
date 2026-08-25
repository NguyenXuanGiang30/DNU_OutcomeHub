using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Measurement;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/measurement-periods")]
public sealed class MeasurementPeriodsController : ApiControllerBase
{
    private readonly IMeasurementPeriodRepository _periodRepository;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public MeasurementPeriodsController(
        IMeasurementPeriodRepository periodRepository,
        IRlsTransactionExecutor rlsExecutor)
    {
        _periodRepository = periodRepository ?? throw new ArgumentNullException(nameof(periodRepository));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MeasurementPeriodDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<MeasurementPeriodDto>>>> GetPaged(
        [FromQuery] PagedRequest request,
        [FromQuery] Guid? orgUnitId,
        [FromQuery] Guid? programVersionId,
        [FromQuery] short? academicYearStart,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read measurement periods list");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _periodRepository.GetPagedPeriodsAsync(
                request,
                orgUnitId,
                programVersionId,
                academicYearStart,
                status,
                ct),
            cancellationToken);

        return PagedResponse(result, "Danh sách đợt đo lường chuẩn đầu ra.");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MeasurementPeriodDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MeasurementPeriodDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read MeasurementPeriod {id}");
        var period = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _periodRepository.GetPeriodByIdAsync(id, ct),
            cancellationToken);

        if (period == null)
        {
            throw new NotFoundException("MeasurementPeriod", id);
        }

        return OkResponse(period, "Thông tin chi tiết đợt đo, danh sách khóa và lớp học phần tham gia.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MeasurementPeriodDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<MeasurementPeriodDto>>> Create(
        [FromBody] CreateMeasurementPeriodRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var periodId = Guid.NewGuid();
        var period = MeasurementPeriod.Create(
            periodId,
            request.Code,
            request.Name,
            request.OrgUnitId,
            request.ProgramVersionId,
            request.AcademicYearStart,
            request.TermCode,
            request.ProgramPolicyBindingId,
            request.WorkflowInstanceId,
            request.Status,
            request.CollectionOpenAt,
            request.CollectionCloseAt,
            request.DataCutoffAt);

        var context = GetDatabaseRequestContext("Create MeasurementPeriod");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _periodRepository.CreatePeriodAsync(period, ct),
            cancellationToken);

        var resultDto = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _periodRepository.GetPeriodByIdAsync(periodId, ct),
            cancellationToken);

        return CreatedResponse(nameof(GetById), new { id = periodId }, resultDto!, "Tạo đợt đo lường thành công.");
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MeasurementPeriodDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<MeasurementPeriodDto>>> Update(
        Guid id,
        [FromBody] UpdateMeasurementPeriodRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Update MeasurementPeriod {id}");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _periodRepository.UpdatePeriodAsync(
                id,
                request.Name,
                request.Status,
                request.CollectionOpenAt,
                request.CollectionCloseAt,
                request.DataCutoffAt,
                ct),
            cancellationToken);

        var updated = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _periodRepository.GetPeriodByIdAsync(id, ct),
            cancellationToken);

        return OkResponse(updated!, "Cập nhật đợt đo lường thành công.");
    }

    [HttpPost("{id:guid}/cohorts")]
    [ProducesResponseType(typeof(ApiResponse<MeasurementPeriodCohortDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<MeasurementPeriodCohortDto>>> AttachCohort(
        Guid id,
        [FromBody] AttachCohortToPeriodRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var periodCohort = MeasurementPeriodCohort.Create(
            id,
            request.ProgramVersionId,
            request.CohortId);

        var context = GetDatabaseRequestContext($"Attach Cohort {request.CohortId} to Period {id}");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _periodRepository.AttachCohortAsync(periodCohort, ct),
            cancellationToken);

        var period = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _periodRepository.GetPeriodByIdAsync(id, ct),
            cancellationToken);

        var dto = period?.Cohorts?.FirstOrDefault(c => c.CohortId == request.CohortId);
        return Created(string.Empty, ApiResponse.Ok(dto!, "Gắn khóa sinh viên vào đợt đo thành công."));
    }

    [HttpPost("{id:guid}/offerings")]
    [ProducesResponseType(typeof(ApiResponse<MeasurementPeriodOfferingDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<MeasurementPeriodOfferingDto>>> AttachOffering(
        Guid id,
        [FromBody] AttachOfferingToPeriodRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var periodOffering = MeasurementPeriodOffering.Create(
            id,
            request.ProgramVersionId,
            request.AcademicYearStart,
            request.CourseOfferingId,
            request.PlannedSourceRole,
            request.CollectionStatus,
            request.DueAt);

        var context = GetDatabaseRequestContext($"Attach Offering {request.CourseOfferingId} to Period {id}");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _periodRepository.AttachOfferingAsync(periodOffering, ct),
            cancellationToken);

        var period = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _periodRepository.GetPeriodByIdAsync(id, ct),
            cancellationToken);

        var dto = period?.Offerings?.FirstOrDefault(o => o.CourseOfferingId == request.CourseOfferingId);
        return Created(string.Empty, ApiResponse.Ok(dto!, "Gắn lớp học phần vào đợt đo thành công."));
    }

    [HttpPost("{id:guid}/targets")]
    [ProducesResponseType(typeof(ApiResponse<MeasurementPeriodTargetDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<MeasurementPeriodTargetDto>>> CreateTarget(
        Guid id,
        [FromBody] CreatePeriodTargetRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var targetId = Guid.NewGuid();
        var target = MeasurementPeriodTarget.Create(
            targetId,
            id,
            request.ProgramVersionId,
            request.OutcomeLevel,
            request.TargetRole,
            request.CourseOfferingId,
            request.SyllabusVersionId,
            request.CloId,
            request.ProgramPiId,
            request.ProgramPloId);

        var context = GetDatabaseRequestContext($"Create Target for Period {id}");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _periodRepository.CreateTargetAsync(target, ct),
            cancellationToken);

        var period = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _periodRepository.GetPeriodByIdAsync(id, ct),
            cancellationToken);

        var dto = period?.Targets?.FirstOrDefault(t => t.Id == targetId);
        return Created(string.Empty, ApiResponse.Ok(dto!, "Thiết lập mục tiêu/ngưỡng đo thành công."));
    }
}
