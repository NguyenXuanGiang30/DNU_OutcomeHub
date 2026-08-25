using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Exceptions;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Academic;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Api.Controllers;

public sealed class OutcomesController : ApiControllerBase
{
    private readonly IOutcomeRepository _outcomeRepository;
    private readonly IRlsTransactionExecutor _rlsTransactionExecutor;

    public OutcomesController(
        IOutcomeRepository outcomeRepository,
        IRlsTransactionExecutor rlsTransactionExecutor)
    {
        _outcomeRepository = outcomeRepository ?? throw new ArgumentNullException(nameof(outcomeRepository));
        _rlsTransactionExecutor = rlsTransactionExecutor ?? throw new ArgumentNullException(nameof(rlsTransactionExecutor));
    }

    [HttpGet("program-versions/{versionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProgramOutcomeTreeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProgramOutcomeTreeDto>>> GetOutcomeTree(
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var context = GetDatabaseRequestContext($"Read Outcome Tree of ProgramVersion {versionId}");
        var result = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _outcomeRepository.GetOutcomeTreeAsync(versionId, ct),
            cancellationToken);

        if (result == null)
        {
            throw new NotFoundException(nameof(ProgramVersion), versionId);
        }

        return OkResponse(result, "Cây Chuẩn đầu ra (PLO & PI) của chương trình đào tạo.");
    }

    [HttpPost("program-versions/{versionId:guid}/plos")]
    [ProducesResponseType(typeof(ApiResponse<ProgramPloDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ProgramPloDto>>> CreatePlo(
        Guid versionId,
        [FromBody] CreateProgramPloRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var plo = ProgramPlo.Create(
            id: Guid.NewGuid(),
            programVersionId: versionId,
            code: request.Code,
            description: request.Description,
            domain: request.Domain,
            bloomLevel: request.BloomLevel,
            sourceTemplatePloId: null,
            isLocked: false,
            sortOrder: request.SortOrder);

        var context = GetDatabaseRequestContext($"Create PLO for ProgramVersion {versionId}");
        var created = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _outcomeRepository.CreatePloAsync(plo, ct),
            cancellationToken);

        return CreatedResponse(
            nameof(GetOutcomeTree),
            new { versionId },
            created,
            "Tạo Chuẩn đầu ra (PLO) thành công.");
    }

    [HttpPost("plos/{ploId:guid}/pis")]
    [ProducesResponseType(typeof(ApiResponse<ProgramPiDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ProgramPiDto>>> CreatePi(
        Guid ploId,
        [FromBody] CreateProgramPiRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = GetDatabaseRequestContext($"Create PI for PLO {ploId}");
        var plo = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _outcomeRepository.GetPloByIdAsync(ploId, ct),
            cancellationToken);

        if (plo == null)
        {
            throw new NotFoundException(nameof(ProgramPlo), ploId);
        }

        var pi = ProgramPi.Create(
            id: Guid.NewGuid(),
            programVersionId: plo.ProgramVersionId,
            programPloId: ploId,
            code: request.Code,
            description: request.Description,
            sourceTemplatePiId: null,
            isLocked: false,
            isCore: request.IsCore,
            weightRatio: request.WeightRatio,
            sortOrder: request.SortOrder);

        var created = await _rlsTransactionExecutor.ExecuteAsync(
            context,
            ct => _outcomeRepository.CreatePiAsync(pi, ct),
            cancellationToken);

        return OkResponse(created, "Tạo Chỉ số đánh giá (PI) thành công.");
    }
}
