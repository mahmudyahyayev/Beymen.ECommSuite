using BuildingBlocks.Abstractions.Domain;

namespace BuildingBlocks.Core.Domain
{
    public abstract class Entity<TId, TAudit> : IEntity<TId, TAudit>
    {
        public TId Id { get; protected init; } = default!;
        public DateTime Created { get; private set; } = default!;
        public TAudit? CreatedBy { get; private set; } = default!;
    }

    public abstract class Entity<TIdentity, TId, TAudit> : Entity<TIdentity, TAudit>
        where TIdentity : Identity<TId>
    { }

     public abstract class Entity<TAudit> : Entity<EntityId, Guid, TAudit>, IEntity<TAudit> { }
}
