namespace BuildingBlocks.Abstractions.Domain
{
    public interface IAggregate<out TId, TAudit> : IEntity<TId, TAudit>, IHaveAggregate { }

    public interface IAggregate<out TIdentity, TId, TAudit> : IAggregate<TIdentity, TAudit>
        where TIdentity : Identity<TId>
    { }

    public interface IAggregate<TAudit> : IAggregate<AggregateId, Guid, TAudit> { }
}
