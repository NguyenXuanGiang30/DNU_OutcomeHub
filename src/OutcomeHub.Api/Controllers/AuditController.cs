using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.DTOs.Iam;
using OutcomeHub.Application.Interfaces.Persistence;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/admin/audit")]
public sealed class AuditController : ApiControllerBase
{
    private readonly IAuditRepository _repository;
    private readonly IRlsTransactionExecutor _rlsExecutor;

    public AuditController(
        IAuditRepository repository,
        IRlsTransactionExecutor rlsExecutor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _rlsExecutor = rlsExecutor ?? throw new ArgumentNullException(nameof(rlsExecutor));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AuditLogEntryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AuditLogEntryDto>>>> QueryAuditLogs(
        [FromQuery] Guid? actorPrincipalId,
        [FromQuery] string? action,
        [FromQuery] string? category,
        [FromQuery] string? resourceType,
        [FromQuery] DateTimeOffset? fromDate,
        [FromQuery] DateTimeOffset? toDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var context = GetDatabaseRequestContext("Query Audit Logs");
        var request = new QueryAuditLogsRequest(
            actorPrincipalId, action, category, resourceType,
            fromDate, toDate, pageNumber, pageSize);

        var logs = await _rlsExecutor.ExecuteAsync(
            context,
            ct => _repository.QueryAuditLogsAsync(request, ct),
            cancellationToken);

        return OkResponse(logs, "Nhật ký kiểm toán hệ thống (Audit Trail).");
    }
}
