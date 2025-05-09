using System.Linq.Expressions;
using BuildingBlocks.Abstractions.Domain;
using Microsoft.EntityFrameworkCore.Query;

namespace BuildingBlocks.Abstractions.Persistence.EfCore
{
    public interface IEfRepository<TEntity, in TId, TAudit> : IRepository<TEntity, TId, TAudit>
        where TEntity : class, IHaveIdentity<TId>
    {
        IEnumerable<TEntity> GetInclude(
            CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null);

        IEnumerable<TEntity> GetInclude(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null,
            bool withTracking = true
        );

        Task<IEnumerable<TEntity>> GetIncludeAsync(
            CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null
        );

        Task<IEnumerable<TEntity>> GetIncludeAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null,
            bool withTracking = true
        );
    }
}
