using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/students")]
public sealed class StudentsController : ApiControllerBase
{
    private readonly IStudentRepository _studentRepository;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public StudentsController(
        IStudentRepository studentRepository,
        IRlsTransactionExecutor rlsExecutor)
    {
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StudentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<StudentDto>>>> GetPaged(
        [FromQuery] PagedRequest request,
        [FromQuery] Guid? admissionCohortId,
        [FromQuery] Guid? programId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read students list");
        var result = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _studentRepository.GetPagedStudentsAsync(request, admissionCohortId, programId, ct),
            cancellationToken);

        return PagedResponse(result, "Danh sách hồ sơ sinh viên.");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StudentDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Student {id}");
        var student = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _studentRepository.GetStudentByIdAsync(id, ct),
            cancellationToken);

        if (student == null)
        {
            throw new NotFoundException("Student", id);
        }

        return OkResponse(student, "Thông tin chi tiết sinh viên và lộ trình học.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<StudentDto>>> Create(
        [FromBody] CreateStudentRequest request,
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

        var student = Student.Create(
            personId,
            request.StudentCode,
            request.AdmissionCohortId,
            request.CurrentStatus);

        var context = GetDatabaseRequestContext("Create Student profile");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _studentRepository.CreateStudentAsync(person, student, ct),
            cancellationToken);

        var resultDto = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _studentRepository.GetStudentByIdAsync(personId, ct),
            cancellationToken);

        return CreatedResponse(nameof(GetById), new { id = personId }, resultDto!, "Tạo hồ sơ sinh viên thành công.");
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<StudentDto>>> Update(
        Guid id,
        [FromBody] UpdateStudentRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Update Student {id}");
        await _rlsExecutor.ExecuteAsync(
            context,
            ct => _studentRepository.UpdateStudentAsync(id, request.FullName, request.CurrentStatus, request.EffectiveTo, ct),
            cancellationToken);

        var updated = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _studentRepository.GetStudentByIdAsync(id, ct),
            cancellationToken);

        return OkResponse(updated!, "Cập nhật thông tin sinh viên thành công.");
    }

    [HttpGet("{id:guid}/paths")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<StudentPathDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<StudentPathDto>>>> GetPaths(
        Guid id,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read paths for Student {id}");
        var paths = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _studentRepository.GetStudentPathsAsync(id, ct),
            cancellationToken);

        return OkResponse(paths, "Danh sách lộ trình học tập của sinh viên.");
    }

    [HttpPost("{id:guid}/paths")]
    [ProducesResponseType(typeof(ApiResponse<StudentPathDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<StudentPathDto>>> AssignPath(
        Guid id,
        [FromBody] AssignStudentPathRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var pathId = Guid.NewGuid();
        var studentPath = StudentPath.Create(
            pathId,
            id,
            request.ProgramId,
            request.ProgramVersionId,
            request.CurriculumPathId,
            request.EffectiveFrom,
            request.EffectiveTo,
            request.PathStatus,
            request.DecisionId,
            request.IsPrimary);

        var context = GetDatabaseRequestContext($"Assign path to Student {id}");
        var created = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _studentRepository.AssignStudentPathAsync(studentPath, ct),
            cancellationToken);

        var paths = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _studentRepository.GetStudentPathsAsync(id, ct),
            cancellationToken);

        var dto = paths.FirstOrDefault(p => p.Id == created.Id);
        return Created(string.Empty, ApiResponse.Ok(dto!, "Gán lộ trình CTĐT cho sinh viên thành công."));
    }
}
