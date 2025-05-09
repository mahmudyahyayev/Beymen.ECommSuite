namespace BuildingBlocks.Abstractions.CQRS.Events.Internals
{
    public interface IDomainEvent : IEvent
    {
        dynamic AggregateId { get; }
        long AggregateSequenceNumber { get; }
        public IDomainEvent WithAggregate(dynamic aggregateId, long version);
    }
}
