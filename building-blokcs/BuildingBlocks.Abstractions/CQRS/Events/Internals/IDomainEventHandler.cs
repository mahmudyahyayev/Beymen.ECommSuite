namespace BuildingBlocks.Abstractions.CQRS.Events.Internals
{

    public interface IDomainEventHandler<in TEvent> : IEventHandler<TEvent>
        where TEvent : IDomainEvent
    { }
}
