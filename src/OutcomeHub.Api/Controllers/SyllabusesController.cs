using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Portfolio;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/syllabuses")]
public sealed class SyllabusesController : ApiControllerBase
{
    private readonly ISyllabusRepository _syllabusRepository;
    private readonly IRlsTransactionExecutor _rlsTransactionExecutor;

    public SyllabusesController(
        ISyllabusRepository syllabusRepository,
        IRlsTransactionExecutor rlsTransactionExecutor)
    {
        _syllabusRepository = syllabusRepository ?? throw new ArgumentNullException(nameof(syllabusRepository));
        _rlsTransactionExecutor = rlsTransactionExecutor ?? throw new ArgumentNullException(nameof(rlsTransactionExecutor));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<SyllabusDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<SyllabusDto>>>> GetPaged(
        [FromQuery] PagedRequest request,
        [FromQuery] Guid? programCourseId,
        [FromQuery] Guid? ownerOrgUnitId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext("Read syllabuses");
        var result = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _syllabusRepository.GetPagedSyllabusesAsync(request, programCourseId, ownerOrgUnitId, ct),
            cancellationToken);

        return PagedResponse(result, "Danh sách đề cương chi tiết học phần.");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SyllabusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SyllabusDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Syllabus {id}");
        var syllabus = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _syllabusRepository.GetSyllabusByIdAsync(id, ct),
            cancellationToken);

        if (syllabus == null)
        {
            throw new NotFoundException("Syllabus", id);
        }

        var dto = new SyllabusDto(
            syllabus.Id,
            syllabus.ProgramCourseId,
            syllabus.Code,
            syllabus.OwnerOrgUnitId,
            syllabus.CreatedAt,
            syllabus.Versions.Count);

        return OkResponse(dto, "Thông tin đề cương học phần.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SyllabusDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<SyllabusDto>>> Create(
        [FromBody] CreateSyllabusRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var syllabus = Syllabus.Create(
            Guid.NewGuid(),
            request.ProgramCourseId,
            request.Code,
            request.OwnerOrgUnitId);

        var context = GetDatabaseRequestContext("Create Syllabus");
        var created = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _syllabusRepository.CreateSyllabusAsync(syllabus, ct),
            cancellationToken);

        var dto = new SyllabusDto(
            created.Id,
            created.ProgramCourseId,
            created.Code,
            created.OwnerOrgUnitId,
            created.CreatedAt,
            0);

        return CreatedResponse(nameof(GetById), new { id = created.Id }, dto, "Tạo đề cương học phần thành công.");
    }

    [HttpGet("{id:guid}/versions")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SyllabusVersionDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SyllabusVersionDto>>>> GetVersions(
        Guid id,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read versions of Syllabus {id}");
        var versions = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _syllabusRepository.GetSyllabusVersionsAsync(id, ct),
            cancellationToken);

        return OkResponse(versions, "Danh sách phiên bản đề cương.");
    }

    [HttpGet("versions/{versionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SyllabusVersionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SyllabusVersionDto>>> GetVersionById(
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read SyllabusVersion {versionId}");
        var version = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _syllabusRepository.GetSyllabusVersionByIdAsync(versionId, ct),
            cancellationToken);

        if (version == null)
        {
            throw new NotFoundException("SyllabusVersion", versionId);
        }

        var dto = new SyllabusVersionDto(
            version.Id,
            version.SyllabusId,
            version.ProgramCourseId,
            version.ProgramVersionId,
            version.InstitutionTemplateVersionId,
            version.CourseVersionId,
            version.SyllabusTemplateVersionId,
            version.VersionNo,
            version.ApplicableFrom,
            version.ApplicableTo,
            version.Status,
            version.ContentChecksum);

        return OkResponse(dto, "Chi tiết phiên bản đề cương.");
    }

    [HttpPost("{id:guid}/versions")]
    [ProducesResponseType(typeof(ApiResponse<SyllabusVersionDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<SyllabusVersionDto>>> CreateVersion(
        Guid id,
        [FromQuery] Guid programVersionId,
        [FromBody] CreateSyllabusVersionRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var checksum = request.ContentChecksum ?? Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        var version = SyllabusVersion.Create(
            Guid.NewGuid(),
            id,
            Guid.Empty, // resolved in domain/repo
            programVersionId,
            request.InstitutionTemplateVersionId,
            request.CourseVersionId,
            request.SyllabusTemplateVersionId,
            request.VersionNo,
            request.ApplicableFrom,
            request.ApplicableTo,
            "DRAFT",
            request.SharedSyllabusCoreVersionId,
            request.WorkflowInstanceId,
            request.SupersedesId,
            checksum);

        var context = GetDatabaseRequestContext($"Create SyllabusVersion for Syllabus {id}");
        var created = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _syllabusRepository.CreateSyllabusVersionAsync(version, ct),
            cancellationToken);

        var dto = new SyllabusVersionDto(
            created.Id,
            created.SyllabusId,
            created.ProgramCourseId,
            created.ProgramVersionId,
            created.InstitutionTemplateVersionId,
            created.CourseVersionId,
            created.SyllabusTemplateVersionId,
            created.VersionNo,
            created.ApplicableFrom,
            created.ApplicableTo,
            created.Status,
            created.ContentChecksum);

        return CreatedResponse(nameof(GetVersionById), new { versionId = created.Id }, dto, "Tạo phiên bản đề cương thành công.");
    }
}
