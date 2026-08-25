using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using AcademicProgram = OutcomeHub.Domain.Entities.Academic.Program;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/programs")]
public sealed class ProgramsController : ApiControllerBase
{
    private readonly IProgramRepository _programRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IRlsTransactionExecutor _rlsTransactionExecutor;

    public ProgramsController(
        IProgramRepository programRepository,
        ICourseRepository courseRepository,
        IRlsTransactionExecutor rlsTransactionExecutor)
    {
        _programRepository = programRepository ?? throw new ArgumentNullException(nameof(programRepository));
        _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        _rlsTransactionExecutor = rlsTransactionExecutor ?? throw new ArgumentNullException(nameof(rlsTransactionExecutor));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProgramDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<ProgramDto>>>> GetPaged(
        [FromQuery] PagedRequest request,
        [FromQuery] Guid? ownerOrgUnitId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read programs");
        var result = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _programRepository.GetPagedAsync(request, ownerOrgUnitId, ct),
            cancellationToken);

        return PagedResponse(result, "Danh sách chương trình đào tạo.");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProgramDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProgramDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Program {id}");
        var result = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _programRepository.GetByIdAsync(id, ct),
            cancellationToken);

        if (result == null)
        {
            throw new NotFoundException("Program", id);
        }

        return OkResponse(result, "Thông tin chi tiết chương trình đào tạo.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProgramDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ProgramDto>>> Create(
        [FromBody] CreateProgramRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var program = AcademicProgram.Create(
            id: Guid.NewGuid(),
            code: request.Code,
            name: request.Name,
            degreeLevel: request.DegreeLevel,
            educationMode: request.EducationMode,
            ownerOrgUnitId: request.OwnerOrgUnitId,
            status: request.Status,
            createdBy: CurrentUser.PrincipalId);

        var context = GetDatabaseRequestContext("Create Program");
        var created = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _programRepository.CreateAsync(program, ct),
            cancellationToken);

        return CreatedResponse(nameof(GetById), new { id = created.Id }, created, "Tạo chương trình đào tạo thành công.");
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProgramDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProgramDto>>> Update(
        Guid id,
        [FromBody] UpdateProgramRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var program = AcademicProgram.Create(
            id: id,
            code: "DUMMY",
            name: request.Name,
            degreeLevel: request.DegreeLevel,
            educationMode: request.EducationMode,
            ownerOrgUnitId: Guid.Empty,
            status: request.Status,
            createdBy: CurrentUser.PrincipalId);

        var context = GetDatabaseRequestContext($"Update Program {id}");
        var updated = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _programRepository.UpdateAsync(program, ct),
            cancellationToken);

        return OkResponse(updated, "Cập nhật chương trình đào tạo thành công.");
    }

    [HttpGet("{id:guid}/versions")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProgramVersionDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProgramVersionDto>>>> GetVersions(
        Guid id,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read versions of program {id}");
        var result = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _programRepository.GetVersionsByProgramIdAsync(id, ct),
            cancellationToken);

        return OkResponse(result, "Danh sách phiên bản CTĐT.");
    }

    [HttpGet("{id:guid}/versions/{versionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProgramVersionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProgramVersionDto>>> GetVersionById(
        Guid id,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read ProgramVersion {versionId}");
        var result = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _programRepository.GetVersionByIdAsync(versionId, ct),
            cancellationToken);

        if (result == null)
        {
            throw new NotFoundException(nameof(ProgramVersion), versionId);
        }

        return OkResponse(result, "Thông tin chi tiết phiên bản CTĐT.");
    }

    [HttpPost("{id:guid}/versions")]
    [ProducesResponseType(typeof(ApiResponse<ProgramVersionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ProgramVersionDto>>> CreateVersion(
        Guid id,
        [FromBody] CreateProgramVersionRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var checksum = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        var version = ProgramVersion.Create(
            id: Guid.NewGuid(),
            programId: id,
            institutionTemplateVersionId: request.InstitutionTemplateVersionId,
            versionNo: request.VersionNo,
            code: request.Code,
            decisionId: request.DecisionId,
            effectiveFrom: request.EffectiveFrom,
            effectiveTo: request.EffectiveTo,
            status: "DRAFT",
            totalCredits: request.TotalCredits,
            workflowInstanceId: request.WorkflowInstanceId,
            supersedesId: request.SupersedesId,
            checksum: checksum);

        var context = GetDatabaseRequestContext($"Create ProgramVersion for program {id}");
        var created = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _programRepository.CreateVersionAsync(version, ct),
            cancellationToken);

        return CreatedResponse(
            nameof(GetVersionById),
            new { id, versionId = created.Id },
            created,
            "Tạo phiên bản CTĐT thành công.");
    }

    [HttpPut("{id:guid}/versions/{versionId:guid}/publish")]
    [ProducesResponseType(typeof(ApiResponse<ProgramVersionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProgramVersionDto>>> PublishVersion(
        Guid id,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Publish ProgramVersion {versionId}");
        var published = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _programRepository.PublishVersionAsync(versionId, ct),
            cancellationToken);

        return OkResponse(published, "Ban hành phiên bản CTĐT thành công.");
    }

    [HttpGet("versions/{versionId:guid}/courses")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProgramCourseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProgramCourseDto>>>> GetCoursesByVersion(
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read courses in ProgramVersion {versionId}");
        var result = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _courseRepository.GetProgramCoursesAsync(versionId, ct),
            cancellationToken);

        return OkResponse(result, "Danh sách học phần thuộc phiên bản CTĐT.");
    }

    [HttpPost("versions/{versionId:guid}/courses")]
    [ProducesResponseType(typeof(ApiResponse<ProgramCourseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<ProgramCourseDto>>> AddCourseToProgramVersion(
        Guid versionId,
        [FromBody] AddCourseToProgramRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var programCourse = ProgramCourse.Create(
            Guid.NewGuid(),
            versionId,
            request.CourseVersionId,
            request.CurriculumBlockId,
            request.CatalogRole,
            request.CreditOverride,
            isLocked: false,
            status: "DRAFT");

        var context = GetDatabaseRequestContext($"Add course to ProgramVersion {versionId}");
        var created = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _courseRepository.AddCourseToProgramAsync(programCourse, ct),
            cancellationToken);

        var dto = new ProgramCourseDto(
            created.Id,
            created.ProgramVersionId,
            created.CourseVersionId,
            string.Empty,
            string.Empty,
            created.CurriculumBlockId,
            created.CatalogRole,
            0m,
            created.CreditOverride,
            created.IsLocked,
            created.Status);

        return Created(string.Empty, ApiResponse.Ok(dto, "Gắn học phần vào CTĐT thành công."));
    }
}
