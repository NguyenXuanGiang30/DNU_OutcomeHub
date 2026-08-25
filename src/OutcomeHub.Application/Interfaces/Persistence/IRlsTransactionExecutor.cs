using OutcomeHub.Application.Common.Security;

namespace OutcomeHub.Application.Interfaces.Persistence;

public interface IRlsTransactionExecutor
{
    Task ExecuteAsync(
        DatabaseRequestContext context,
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);

    Task<TResult> ExecuteAsync<TResult>(
        DatabaseRequestContext context,
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default);
}
