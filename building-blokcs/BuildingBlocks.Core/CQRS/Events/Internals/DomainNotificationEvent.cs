using BuildingBlocks.Abstractions.CQRS.Events.Internals;

namespace BuildingBlocks.Core.CQRS.Events.Internals
{
    public abstract record DomainNotificationEvent(
        string MessageKey,
        int Priority)
        : Event(MessageKey, Priority), IDomainNotificationEvent;
}
