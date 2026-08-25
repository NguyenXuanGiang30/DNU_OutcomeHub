using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Portfolio;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Portfolio;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class ClosController : ApiControllerBase
{
    private readonly ICloRepository _cloRepository;
    private readonly IRlsTransactionExecutor _rlsTransactionExecutor;

    public ClosController(
        ICloRepository cloRepository,
        IRlsTransactionExecutor rlsTransactionExecutor)
    {
        _cloRepository = cloRepository ?? throw new ArgumentNullException(nameof(cloRepository));
        _rlsTransactionExecutor = rlsTransactionExecutor ?? throw new ArgumentNullException(nameof(rlsTransactionExecutor));
    }

    [HttpGet("syllabuses/versions/{versionId:guid}/clos")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CloDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CloDto>>>> GetClosBySyllabusVersion(
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read CLOs for SyllabusVersion {versionId}");
        var clos = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _cloRepository.GetClosBySyllabusVersionIdAsync(versionId, ct),
            cancellationToken);

        return OkResponse(clos, "Danh sách chuẩn đầu ra học phần (CLO).");
    }

    [HttpPost("syllabuses/versions/{versionId:guid}/clos")]
    [ProducesResponseType(typeof(ApiResponse<CloDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CloDto>>> CreateClo(
        Guid versionId,
        [FromBody] CreateCloRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var clo = Clo.Create(
            Guid.NewGuid(),
            versionId,
            request.Code,
            request.Description,
            request.Domain,
            request.BloomLevel,
            request.IsCore,
            request.SortOrder);

        var context = GetDatabaseRequestContext($"Create CLO for SyllabusVersion {versionId}");
        var created = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _cloRepository.CreateCloAsync(clo, ct),
            cancellationToken);

        var dto = new CloDto(
            created.Id,
            created.SyllabusVersionId,
            created.Code,
            created.Description,
            created.Domain,
            created.BloomLevel,
            created.IsCore,
            created.SortOrder);

        return Created(string.Empty, ApiResponse.Ok(dto, "Tạo CLO thành công."));
    }

    [HttpPut("syllabuses/versions/{versionId:guid}/clos/{cloId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CloDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CloDto>>> UpdateClo(
        Guid versionId,
        Guid cloId,
        [FromBody] UpdateCloRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Update CLO {cloId}");
        var updated = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _cloRepository.UpdateCloAsync(
                cloId,
                request.Description,
                request.Domain,
                request.BloomLevel,
                request.IsCore,
                request.SortOrder,
                ct),
            cancellationToken);

        var dto = new CloDto(
            updated.Id,
            updated.SyllabusVersionId,
            updated.Code,
            updated.Description,
            updated.Domain,
            updated.BloomLevel,
            updated.IsCore,
            updated.SortOrder);

        return OkResponse(dto, "Cập nhật CLO thành công.");
    }

    [HttpDelete("syllabuses/versions/{versionId:guid}/clos/{cloId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteClo(
        Guid versionId,
        Guid cloId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Delete CLO {cloId}");
        var deleted = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _cloRepository.DeleteCloAsync(cloId, ct),
            cancellationToken);

        return OkResponse(deleted, "Xóa CLO thành công.");
    }

    [HttpGet("programs/versions/{programVersionId:guid}/course-pi-mappings")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CoursePiMappingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CoursePiMappingDto>>>> GetCoursePiMappings(
        Guid programVersionId,
        [FromQuery] Guid? programCourseId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Course-PI mappings for ProgramVersion {programVersionId}");
        var mappings = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _cloRepository.GetCoursePiMappingsAsync(programVersionId, programCourseId, ct),
            cancellationToken);

        return OkResponse(mappings, "Ma trận ánh xạ Học phần - Chuẩn đầu ra PI.");
    }

    [HttpPost("programs/versions/{programVersionId:guid}/course-pi-mappings")]
    [ProducesResponseType(typeof(ApiResponse<CoursePiMappingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CoursePiMappingDto>>> SetCoursePiMapping(
        Guid programVersionId,
        [FromBody] SetCoursePiMappingRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var mapping = CoursePiMapping.Create(
            Guid.NewGuid(),
            programVersionId,
            request.ProgramCourseId,
            request.ProgramPiId,
            request.ContributionLevel,
            request.IsDirectAssessment,
            request.Rationale,
            request.SourceType);

        var context = GetDatabaseRequestContext($"Set Course-PI mapping for ProgramVersion {programVersionId}");
        var saved = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _cloRepository.SetCoursePiMappingAsync(mapping, ct),
            cancellationToken);

        var dto = new CoursePiMappingDto(
            saved.Id,
            saved.ProgramVersionId,
            saved.ProgramCourseId,
            string.Empty,
            string.Empty,
            saved.ProgramPiId,
            string.Empty,
            saved.ContributionLevel,
            saved.IsDirectAssessment,
            saved.Rationale,
            saved.SourceType,
            saved.IsLocked);

        return OkResponse(dto, "Thiết lập ma trận ánh xạ Học phần - PI thành công.");
    }
}
