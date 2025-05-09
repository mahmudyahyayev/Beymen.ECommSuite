using BuildingBlocks.Abstractions.CQRS.Events.Internals;

namespace BuildingBlocks.Core.CQRS.Events.Internals
{
    public abstract record DomainEvent(string MessageKey, int Priority) : Event(MessageKey, Priority), IDomainEvent
    {
        public dynamic AggregateId { get; protected set; } = null!;
        public long AggregateSequenceNumber { get; protected set; }

        public virtual IDomainEvent WithAggregate(dynamic aggregateId, long version)
        {
            AggregateId = aggregateId;
            AggregateSequenceNumber = version;

            return this;
        }
    }
}
