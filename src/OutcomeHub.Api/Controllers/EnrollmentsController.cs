using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Measurement;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/enrollments")]
public sealed class EnrollmentsController : ApiControllerBase
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public EnrollmentsController(
        IEnrollmentRepository enrollmentRepository,
        IRlsTransactionExecutor rlsExecutor)
    {
        _enrollmentRepository = enrollmentRepository ?? throw new ArgumentNullException(nameof(enrollmentRepository));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EnrollmentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<EnrollmentDto>>>> GetPaged(
        [FromQuery] PagedRequest request,
        [FromQuery] Guid? courseOfferingId,
        [FromQuery] Guid? studentId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read enrollments list");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _enrollmentRepository.GetPagedEnrollmentsAsync(request, courseOfferingId, studentId, ct),
            cancellationToken);

        return PagedResponse(result, "Danh sách đăng ký lớp học phần (Enrollments).");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnrollmentDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Enrollment {id}");
        var enrollment = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _enrollmentRepository.GetEnrollmentByIdAsync(id, ct),
            cancellationToken);

        if (enrollment == null)
        {
            throw new NotFoundException("Enrollment", id);
        }

        return OkResponse(enrollment, "Thông tin đăng ký học phần của sinh viên.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<EnrollmentDto>>> Create(
        [FromBody] CreateEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var enrollmentId = Guid.NewGuid();
        var enrollment = Enrollment.Create(
            enrollmentId,
            request.CourseOfferingId,
            request.StudentId,
            request.AttemptNo,
            request.SourceSystemId,
            request.SourceRecordId);

        var revisionId = Guid.NewGuid();
        var initialRevision = EnrollmentRevision.Create(
            revisionId,
            enrollmentId,
            revisionNo: 1,
            enrollmentStatus: request.EnrollmentStatus,
            effectiveFrom: request.EffectiveFrom ?? DateTimeOffset.UtcNow,
            ingestionBatchId: Guid.NewGuid(),
            checksum: Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
            recordedAt: DateTimeOffset.UtcNow,
            repeatFlag: request.RepeatFlag,
            improvementFlag: request.ImprovementFlag);

        var context = GetDatabaseRequestContext("Create Enrollment");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _enrollmentRepository.CreateEnrollmentAsync(enrollment, initialRevision, ct),
            cancellationToken);

        var resultDto = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _enrollmentRepository.GetEnrollmentByIdAsync(enrollmentId, ct),
            cancellationToken);

        return CreatedResponse(nameof(GetById), new { id = enrollmentId }, resultDto!, "Đăng ký sinh viên vào lớp học phần thành công.");
    }
}
