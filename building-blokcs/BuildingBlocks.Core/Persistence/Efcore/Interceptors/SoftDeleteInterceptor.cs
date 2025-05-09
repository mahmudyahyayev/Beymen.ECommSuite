using BuildingBlocks.Abstractions.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Core.Persistence.EfCore.Interceptors;

public class SoftDeleteInterceptor<TUserTypeId, TAudit> : SaveChangesInterceptor
{
    private readonly ICurrentUser<TUserTypeId, TAudit> _currentUser;
    public SoftDeleteInterceptor(ICurrentUser<TUserTypeId, TAudit> currentUser)
    {
        _currentUser = currentUser;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken
    )
    {
        if (eventData.Context == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (entry.Entity is ISoftDeletable)
                        entry.CurrentValues["IsDeleted"] = false;
                    break;
                case EntityState.Deleted:
                    if (entry.Entity is ISoftDeletable)
                    {
                        entry.State = EntityState.Modified;
                        eventData.Context.Entry(entry.Entity).CurrentValues["IsDeleted"] = true;

                        if (entry.Entity is IHaveAudit<TUserTypeId>)
                        {
                            eventData.Context.Entry(entry.Entity).CurrentValues["LastModified"] = DateTime.Now;
                        }
                    }
                    //if (entry.Entity is ValueObject)
                    //    entry.State = EntityState.Modified;

                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
