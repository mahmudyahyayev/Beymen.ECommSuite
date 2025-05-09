using BuildingBlocks.Abstractions.Domain;
using System.Linq.Expressions;

namespace BuildingBlocks.Abstractions.Persistence
{
    public interface IRepository
    {
    }

    public interface IReadRepository<TEntity, in TId> : IRepository
        where TEntity : class, IHaveIdentity<TId>
    {
        Task<TEntity?> FindByIdAsync(
            TId id,
            CancellationToken cancellationToken);

        Task<TEntity?> FindOneAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken
        );

        Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken
        );

        Task<IReadOnlyList<TEntity>> GetAllAsync(
            CancellationToken cancellationToken);
    }

    public interface IWriteRepository<TEntity, in TId, TAudit> : IRepository
        where TEntity : class, IHaveIdentity<TId>
    {
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        Task DeleteRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
        Task DeleteByIdAsync(TId id, CancellationToken cancellationToken);
    }


    public interface IRepository<TEntity, in TId, TAudit>
        : IReadRepository<TEntity, TId>,
            IWriteRepository<TEntity, TId, TAudit>,
            IDisposable
        where TEntity : class, IHaveIdentity<TId>
    {
    }


    public interface IRepository<TEntity, TAudit> : IRepository<TEntity, Guid, TAudit>
        where TEntity : class, IHaveIdentity<Guid>
    {
    }
}
