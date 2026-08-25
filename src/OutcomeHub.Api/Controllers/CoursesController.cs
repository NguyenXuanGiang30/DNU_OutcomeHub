using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/courses")]
public sealed class CoursesController : ApiControllerBase
{
    private readonly ICourseRepository _courseRepository;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public CoursesController(
        ICourseRepository courseRepository,
        IRlsTransactionExecutor rlsExecutor)
    {
        _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<CourseDto>>>> GetPaged(
        [FromQuery] PagedRequest request,
        [FromQuery] Guid? ownerOrgUnitId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read courses list");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _courseRepository.GetPagedCoursesAsync(request, ownerOrgUnitId, ct),
            cancellationToken);

        return PagedResponse(result, "Danh sách học phần.");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CourseDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Course {id}");
        var course = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _courseRepository.GetCourseByIdAsync(id, ct),
            cancellationToken);

        if (course == null)
        {
            throw new NotFoundException("Course", id);
        }

        var dto = new CourseDto(
            course.Id,
            course.Code,
            course.Name,
            course.OwnerOrgUnitId,
            course.OwnerOrgUnit?.Name,
            course.Status);

        return OkResponse(dto, "Thông tin chi tiết học phần.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CourseDto>>> Create(
        [FromBody] CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var course = Course.Create(
            Guid.NewGuid(),
            request.Code,
            request.Name,
            request.OwnerOrgUnitId,
            request.Status);

        var context = GetDatabaseRequestContext("Create Course");
        var created = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _courseRepository.CreateCourseAsync(course, ct),
            cancellationToken);

        var dto = new CourseDto(
            created.Id,
            created.Code,
            created.Name,
            created.OwnerOrgUnitId,
            null,
            created.Status);

        return CreatedResponse(nameof(GetById), new { id = created.Id }, dto, "Tạo học phần thành công.");
    }

    [HttpGet("{id:guid}/versions")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CourseVersionDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CourseVersionDto>>>> GetVersions(
        Guid id,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read versions of Course {id}");
        var versions = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _courseRepository.GetCourseVersionsAsync(id, ct),
            cancellationToken);

        return OkResponse(versions, "Danh sách phiên bản học phần.");
    }

    [HttpPost("{id:guid}/versions")]
    [ProducesResponseType(typeof(ApiResponse<CourseVersionDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CourseVersionDto>>> CreateVersion(
        Guid id,
        [FromBody] CreateCourseVersionRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var checksum = request.Checksum ?? Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        var version = CourseVersion.Create(
            Guid.NewGuid(),
            id,
            request.VersionNo,
            request.Name,
            request.CreditValue,
            request.CourseType,
            request.EffectiveFrom,
            request.EffectiveTo,
            request.SharedCoreFlag,
            "DRAFT",
            request.DecisionId,
            request.WorkflowInstanceId,
            request.SupersedesId,
            checksum);

        var context = GetDatabaseRequestContext($"Create CourseVersion for Course {id}");
        var created = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _courseRepository.CreateCourseVersionAsync(version, ct),
            cancellationToken);

        var dto = new CourseVersionDto(
            created.Id,
            created.CourseId,
            created.VersionNo,
            created.Name,
            created.CreditValue,
            created.CourseType,
            created.EffectiveFrom,
            created.EffectiveTo,
            created.SharedCoreFlag,
            created.Status,
            created.DecisionId,
            created.WorkflowInstanceId,
            created.SupersedesId,
            created.Checksum);

        return Created(string.Empty, ApiResponse.Ok(dto, "Tạo phiên bản học phần thành công."));
    }
}
