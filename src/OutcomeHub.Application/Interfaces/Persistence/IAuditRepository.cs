using OutcomeHub.Application.DTOs.Iam;
using OutcomeHub.Domain.Entities.Audit;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IAuditRepository
{
    Task<AuditLogEntryDto> SaveAuditEventAsync(
        AuditEvent auditEvent,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AuditLogEntryDto>> QueryAuditLogsAsync(
        QueryAuditLogsRequest request,
        CancellationToken cancellationToken);

    Task<long> GetNextChainSequenceAsync(
        Guid chainId,
        CancellationToken cancellationToken);

    Task<string?> GetLastEventHashAsync(
        Guid chainId,
        CancellationToken cancellationToken);
}
