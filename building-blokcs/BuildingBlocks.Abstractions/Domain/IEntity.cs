namespace BuildingBlocks.Abstractions.Domain
{
    public interface IEntity<out TId, TAudit> : IHaveIdentity<TId>, IHaveCreator<TAudit> { }

    public interface IEntity<out TIdentity, in TId, TAudit> : IEntity<TIdentity, TAudit>
        where TIdentity : IIdentity<TId>
    { }

    public interface IEntity<TAudit> : IEntity<EntityId, TAudit> { }
}
