using BuildingBlocks.Abstractions.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Core.Persistence.EfCore.Interceptors
{
    public class AuditInterceptor<TUserIdType, TAudit> : SaveChangesInterceptor
    {
        private readonly ICurrentUser<TUserIdType, TAudit> _currentUser;
        public AuditInterceptor(ICurrentUser<TUserIdType,TAudit> currentUser)
        {
            _currentUser = currentUser;
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken)

        {
            if (eventData.Context == null)
                return base.SavingChangesAsync(eventData, result, cancellationToken);

            var now = DateTime.Now;

            TAudit userId = default;

            if (_currentUser is not null)
                userId = _currentUser.GetCurrentUser();

            foreach (var entry in eventData.Context.ChangeTracker.Entries<IHaveAudit<TAudit>>())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.CurrentValues[nameof(IHaveAudit<TAudit>.LastModified)] = now;
                        entry.CurrentValues[nameof(IHaveAudit<TAudit>.LastModifiedBy)] = userId;
                        break;
                    case EntityState.Added:
                        entry.CurrentValues[nameof(IHaveAudit<TAudit>.Created)] = now;
                        entry.CurrentValues[nameof(IHaveAudit<TAudit>.CreatedBy)] = userId;
                        break;
                }
            }

            foreach (var entry in eventData.Context.ChangeTracker.Entries<IHaveCreator<TAudit>>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.CurrentValues[nameof(IHaveCreator<TAudit>.Created)] = now;
                    entry.CurrentValues[nameof(IHaveCreator<TAudit>.CreatedBy)] = userId;
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
