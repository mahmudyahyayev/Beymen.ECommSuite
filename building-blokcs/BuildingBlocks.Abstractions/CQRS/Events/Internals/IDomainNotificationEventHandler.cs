namespace BuildingBlocks.Abstractions.CQRS.Events.Internals
{
    public interface IDomainNotificationEventHandler<in TEvent> : IEventHandler<TEvent>
        where TEvent : IDomainNotificationEvent
    { }
}
