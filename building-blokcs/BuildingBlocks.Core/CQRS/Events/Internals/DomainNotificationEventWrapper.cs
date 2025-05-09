using BuildingBlocks.Abstractions.CQRS.Events.Internals;

namespace BuildingBlocks.Core.CQRS.Events.Internals
{
    public record DomainNotificationEventWrapper<TDomainEventType>(
        TDomainEventType DomainEvent,
        string MessageKey,
        int Priority)
        : DomainNotificationEvent(MessageKey, Priority) where TDomainEventType : IDomainEvent;
}
