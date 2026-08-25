using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OutcomeHub.Infrastructure.Persistence.Interceptors;

public sealed class RowVersionSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        IncrementRowVersions(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        IncrementRowVersions(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void IncrementRowVersions(DbContext? dbContext)
    {
        if (dbContext is null)
        {
            return;
        }

        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Modified)
            {
                continue;
            }

            var rowVersionProperty = FindRowVersionProperty(entry);

            if (rowVersionProperty is null)
            {
                continue;
            }

            var originalValue = (long)(rowVersionProperty.OriginalValue ?? 0L);
            rowVersionProperty.CurrentValue = checked(originalValue + 1L);
        }
    }

    private static PropertyEntry? FindRowVersionProperty(EntityEntry entry)
    {
        var rowVersionMetadata = entry.Metadata.FindProperty("RowVersion");

        if (rowVersionMetadata?.ClrType != typeof(long))
        {
            return null;
        }

        return entry.Property(rowVersionMetadata.Name);
    }
}
