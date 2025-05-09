using System.Linq.Expressions;
using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Events;
using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Abstractions.Persistence.Efcore;
using BuildingBlocks.Abstractions.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BuildingBlocks.Core.Persistence.EfCore
{
    public abstract class EfRepositoryBase<TDbContext, TEntity, TKey, TAudit>
        : IEfRepository<TEntity, TKey, TAudit>,
            IPageRepository<TEntity, TKey>
        where TEntity : class, IHaveIdentity<TKey>
        where TDbContext : DbContext
    {
        protected readonly TDbContext DbContext;
        private readonly IAggregatesDomainEventsRequestStore _aggregatesDomainEventsStore;
        protected readonly DbSet<TEntity> DbSet;

        protected EfRepositoryBase(TDbContext dbContext,
            IAggregatesDomainEventsRequestStore aggregatesDomainEventsStore)
        {
            DbContext = dbContext;
            _aggregatesDomainEventsStore = aggregatesDomainEventsStore;
            DbSet = dbContext.Set<TEntity>();
        }

        public Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken)
        {
            return DbSet.SingleOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        }

        public Task<TEntity?> FindOneAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken
        )
        {
            Guard.Against.Null(predicate, nameof(predicate));

            return DbSet.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken
        )
        {
            return await DbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await DbSet.ToListAsync(cancellationToken);
        }

        public virtual IEnumerable<TEntity> GetInclude(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null,
            bool withTracking = true
        )
        {
            IQueryable<TEntity> query = DbSet;

            if (includes != null)
            {
                query = includes(query);
            }

            query = query.Where(predicate);

            if (withTracking == false)
            {
                query = query.Where(predicate).AsNoTracking();
            }

            return query.AsEnumerable();
        }

        public virtual IEnumerable<TEntity> GetInclude(
            CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null
        )
        {
            IQueryable<TEntity> query = DbSet;

            if (includes != null)
            {
                query = includes(query);
            }

            return query.AsEnumerable();
        }

        public virtual async Task<IEnumerable<TEntity>> GetIncludeAsync(
            CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null
        )
        {
            IQueryable<TEntity> query = DbSet;

            if (includes != null)
            {
                query = includes(query);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> GetIncludeAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null,
            bool withTracking = true
        )
        {
            IQueryable<TEntity> query = DbSet;

            if (includes != null)
            {
                query = includes(query);
            }

            query = query.Where(predicate);

            if (withTracking == false)
            {
                query = query.Where(predicate).AsNoTracking();
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            Guard.Against.Null(entity, nameof(entity));

            await DbSet.AddAsync(entity, cancellationToken);

            return entity;
        }

        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            Guard.Against.Null(entity, nameof(entity));

            var entry = DbContext.Entry(entity);
            entry.State = EntityState.Modified;

            return Task.FromResult(entry.Entity);
        }

        public async Task DeleteRangeAsync(
            IReadOnlyList<TEntity> entities,
            CancellationToken cancellationToken)
        {
            Guard.Against.NullOrEmpty(entities, nameof(entities));

            foreach (var entity in entities)
            {
                await DeleteAsync(entity, cancellationToken);
            }
        }

        public Task DeleteAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            var items = DbSet.Where(predicate).ToList();

            return DeleteRangeAsync(items, cancellationToken);
        }

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            Guard.Against.Null(entity, nameof(entity));

            DbSet.Remove(entity);

            return Task.CompletedTask;
        }

        public async Task DeleteByIdAsync(TKey id, CancellationToken cancellationToken)
        {
            var item = await DbSet.SingleOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
            Guard.Against.NotFound(id.ToString(), id.ToString(), nameof(id));

            DbSet.Remove(item);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
