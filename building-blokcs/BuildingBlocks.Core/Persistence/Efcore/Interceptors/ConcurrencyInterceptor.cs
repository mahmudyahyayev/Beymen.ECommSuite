using BuildingBlocks.Abstractions.Domain;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Core.Persistence.EfCore.Interceptors;

public class ConcurrencyInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken
    )
    {
        if (eventData.Context == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (var entry in eventData.Context.ChangeTracker.Entries<IDomainEventRaisable>())
        {
            var events = entry.Entity.GetUncommittedDomainEvents();
            if (events.Any())
            {
                if (entry.Entity is IHaveAggregateVersion av)
                {
                    entry.CurrentValues[nameof(IHaveAggregateVersion.OriginalVersion)] = av.OriginalVersion + 1;
                }
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
