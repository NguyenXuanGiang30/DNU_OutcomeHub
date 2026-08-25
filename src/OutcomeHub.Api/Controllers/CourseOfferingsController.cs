using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/course-offerings")]
public sealed class CourseOfferingsController : ApiControllerBase
{
    private readonly ICourseOfferingRepository _offeringRepository;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public CourseOfferingsController(
        ICourseOfferingRepository offeringRepository,
        IRlsTransactionExecutor rlsExecutor)
    {
        _offeringRepository = offeringRepository ?? throw new ArgumentNullException(nameof(offeringRepository));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CourseOfferingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<CourseOfferingDto>>>> GetPaged(
        [FromQuery] PagedRequest request,
        [FromQuery] Guid? programVersionId,
        [FromQuery] short? academicYearStart,
        [FromQuery] string? termCode,
        [FromQuery] Guid? orgUnitId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read course offerings list");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _offeringRepository.GetPagedOfferingsAsync(
                request,
                programVersionId,
                academicYearStart,
                termCode,
                orgUnitId,
                ct),
            cancellationToken);

        return PagedResponse(result, "Danh sách lớp học phần.");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CourseOfferingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CourseOfferingDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read CourseOffering {id}");
        var offering = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _offeringRepository.GetOfferingByIdAsync(id, ct),
            cancellationToken);

        if (offering == null)
        {
            throw new NotFoundException("CourseOffering", id);
        }

        return OkResponse(offering, "Thông tin chi tiết lớp học phần và danh sách giảng viên phụ trách.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CourseOfferingDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CourseOfferingDto>>> Create(
        [FromBody] CreateCourseOfferingRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var offeringId = Guid.NewGuid();
        var offering = CourseOffering.Create(
            offeringId,
            request.Code,
            request.ProgramCourseId,
            request.CourseVersionId,
            request.ProgramVersionId,
            request.SyllabusVersionId,
            request.AcademicYearStart,
            request.TermCode,
            request.OrgUnitId,
            request.StartDate,
            request.EndDate,
            request.Status,
            request.SourceSystemId,
            request.SourceRecordId);

        var context = GetDatabaseRequestContext("Create CourseOffering");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _offeringRepository.CreateOfferingAsync(offering, ct),
            cancellationToken);

        var resultDto = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _offeringRepository.GetOfferingByIdAsync(offeringId, ct),
            cancellationToken);

        return CreatedResponse(nameof(GetById), new { id = offeringId }, resultDto!, "Tạo lớp học phần thành công.");
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CourseOfferingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CourseOfferingDto>>> Update(
        Guid id,
        [FromBody] UpdateCourseOfferingRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Update CourseOffering {id}");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _offeringRepository.UpdateOfferingAsync(id, request.Status, request.StartDate, request.EndDate, ct),
            cancellationToken);

        var updated = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _offeringRepository.GetOfferingByIdAsync(id, ct),
            cancellationToken);

        return OkResponse(updated!, "Cập nhật lớp học phần thành công.");
    }

    [HttpGet("{id:guid}/instructors")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CourseOfferingInstructorDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CourseOfferingInstructorDto>>>> GetInstructors(
        Guid id,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read instructors of CourseOffering {id}");
        var instructors = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _offeringRepository.GetOfferingInstructorsAsync(id, ct),
            cancellationToken);

        return OkResponse(instructors, "Danh sách giảng viên phân công giảng dạy / chấm thi.");
    }

    [HttpPost("{id:guid}/instructors")]
    [ProducesResponseType(typeof(ApiResponse<CourseOfferingInstructorDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CourseOfferingInstructorDto>>> AssignInstructor(
        Guid id,
        [FromBody] AssignInstructorRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var assignmentId = Guid.NewGuid();
        var assignment = CourseOfferingInstructor.Create(
            assignmentId,
            id,
            request.StaffId,
            request.AssignmentRole,
            request.EffectiveFrom,
            request.EffectiveTo,
            request.IsPrimary);

        var context = GetDatabaseRequestContext($"Assign instructor to CourseOffering {id}");
        var created = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _offeringRepository.AssignInstructorAsync(assignment, ct),
            cancellationToken);

        var instructors = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _offeringRepository.GetOfferingInstructorsAsync(id, ct),
            cancellationToken);

        var dto = instructors.FirstOrDefault(i => i.Id == created.Id);
        return Created(string.Empty, ApiResponse.Ok(dto!, "Phân công giảng viên thành công."));
    }

    [HttpDelete("{id:guid}/instructors/{instructorAssignmentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveInstructor(
        Guid id,
        Guid instructorAssignmentId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Remove instructor assignment {instructorAssignmentId}");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _offeringRepository.RemoveInstructorAsync(instructorAssignmentId, ct),
            cancellationToken);

        return OkResponse(result, "Hủy phân công giảng viên thành công.");
    }
}
