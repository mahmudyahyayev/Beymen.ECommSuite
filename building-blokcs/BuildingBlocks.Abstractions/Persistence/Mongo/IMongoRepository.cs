using BuildingBlocks.Abstractions.Domain;

namespace BuildingBlocks.Abstractions.Persistence.Mongo;

public interface IMongoRepository<TEntity, in TId, TAudit> : IRepository<TEntity, TId, TAudit>
    where TEntity : class, IHaveIdentity<TId> { }
