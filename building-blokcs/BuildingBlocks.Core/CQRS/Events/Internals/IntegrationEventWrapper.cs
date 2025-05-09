using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Messaging;

namespace BuildingBlocks.Core.CQRS.Events.Internals
{
    public record IntegrationEventWrapper<TDomainEventType>(string Key, int Priority, MessageSendMode SendMode,  string QueueName) : IntegrationEvent(Key, Priority, SendMode, QueueName)
        where TDomainEventType : IDomainEvent;
}
