using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Abstractions.CQRS.Events;
using BuildingBlocks.Abstractions.Domain;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Core.Persistence.EfCore
{
    public class EfRepository<TDbContext, TEntity, TKey, TAudit> : EfRepositoryBase<TDbContext, TEntity, TKey, TAudit>
        where TEntity : class, IHaveIdentity<TKey>
        where TDbContext : DbContext
    {
        public EfRepository(TDbContext dbContext, IAggregatesDomainEventsRequestStore aggregatesDomainEventsStore)
            : base(dbContext, aggregatesDomainEventsStore) { }
    }

    public class EfRepository<TDbContext, TEntity, TAudit> : EfRepository<TDbContext, TEntity, Guid, TAudit>
        where TEntity : class, IHaveIdentity<Guid>
        where TDbContext : DbContext
    {
        public EfRepository(TDbContext dbContext, [NotNull] IAggregatesDomainEventsRequestStore aggregatesDomainEventsStore)
            : base(dbContext, aggregatesDomainEventsStore) { }
    }
}
