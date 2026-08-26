using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.DTOs.Iam;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Domain.Entities.Audit;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Audit;

public sealed class AuditRepository : IAuditRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public AuditRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<AuditLogEntryDto> SaveAuditEventAsync(
        AuditEvent auditEvent,
        CancellationToken cancellationToken)
    {
        _dbContext.AuditEvents.Add(auditEvent);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuditLogEntryDto(
            auditEvent.Id,
            auditEvent.OccurredAt,
            auditEvent.ActorPrincipalId,
            null,
            auditEvent.Action,
            auditEvent.Category,
            auditEvent.Outcome,
            auditEvent.ResourceType,
            auditEvent.ResourceId,
            auditEvent.Purpose,
            auditEvent.Reason,
            auditEvent.EventHash);
    }

    public async Task<IReadOnlyList<AuditLogEntryDto>> QueryAuditLogsAsync(
        QueryAuditLogsRequest request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.AuditEvents
            .AsNoTracking()
            .Include(a => a.ActorPrincipal)
            .AsQueryable();

        if (request.ActorPrincipalId.HasValue)
            query = query.Where(a => a.ActorPrincipalId == request.ActorPrincipalId.Value);

        if (!string.IsNullOrWhiteSpace(request.Action))
            query = query.Where(a => a.Action == request.Action);

        if (!string.IsNullOrWhiteSpace(request.Category))
            query = query.Where(a => a.Category == request.Category);

        if (!string.IsNullOrWhiteSpace(request.ResourceType))
            query = query.Where(a => a.ResourceType == request.ResourceType);

        if (request.FromDate.HasValue)
            query = query.Where(a => a.OccurredAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(a => a.OccurredAt <= request.ToDate.Value);

        int pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        int pageSize = request.PageSize <= 0 ? 50 : Math.Min(request.PageSize, 100);

        return await query
            .OrderByDescending(a => a.OccurredAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AuditLogEntryDto(
                a.Id,
                a.OccurredAt,
                a.ActorPrincipalId,
                a.ActorPrincipal != null ? a.ActorPrincipal.DisplayName : null,
                a.Action,
                a.Category,
                a.Outcome,
                a.ResourceType,
                a.ResourceId,
                a.Purpose,
                a.Reason,
                a.EventHash))
            .ToListAsync(cancellationToken);
    }

    public async Task<long> GetNextChainSequenceAsync(
        Guid chainId,
        CancellationToken cancellationToken)
    {
        var maxSeq = await _dbContext.AuditEvents
            .AsNoTracking()
            .Where(a => a.ChainId == chainId)
            .Select(a => (long?)a.ChainSequence)
            .MaxAsync(cancellationToken);

        return (maxSeq ?? 0) + 1;
    }

    public async Task<string?> GetLastEventHashAsync(
        Guid chainId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.AuditEvents
            .AsNoTracking()
            .Where(a => a.ChainId == chainId)
            .OrderByDescending(a => a.ChainSequence)
            .Select(a => a.EventHash)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
