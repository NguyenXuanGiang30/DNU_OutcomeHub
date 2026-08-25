using System.Globalization;
using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.Common.Security;
using OutcomeHub.Application.Interfaces.Persistence;

namespace OutcomeHub.Infrastructure.Persistence.Rls;

public sealed class RlsTransactionExecutor(OutcomeHubDbContext dbContext) : IRlsTransactionExecutor
{
    private readonly OutcomeHubDbContext _dbContext = dbContext
        ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task ExecuteAsync(
        DatabaseRequestContext context,
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        await ExecuteAsync(
            context,
            async operationCancellationToken =>
            {
                await operation(operationCancellationToken).ConfigureAwait(false);
                return true;
            },
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        DatabaseRequestContext context,
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(operation);

        if (_dbContext.Database.CurrentTransaction is not null)
        {
            throw new InvalidOperationException(
                "An RLS transaction cannot start while another database transaction is active.");
        }

        await using var transaction = await _dbContext.Database
            .BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);

        await SetContextAsync(context, cancellationToken).ConfigureAwait(false);

        var result = await operation(cancellationToken).ConfigureAwait(false);

        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

        return result;
    }

    private async Task SetContextAsync(
        DatabaseRequestContext context,
        CancellationToken cancellationToken)
    {
        var principalId = context.PrincipalId.ToString("D", CultureInfo.InvariantCulture);
        var requestId = context.RequestId.ToString("D", CultureInfo.InvariantCulture);
        var jobId = context.JobId?.ToString("D", CultureInfo.InvariantCulture) ?? string.Empty;

        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
            SELECT
                set_config('app.principal_id', {principalId}, true),
                set_config('app.request_id', {requestId}, true),
                set_config('app.purpose', {context.Purpose}, true),
                set_config('app.job_id', {jobId}, true)
            """,
            cancellationToken).ConfigureAwait(false);
    }
}
