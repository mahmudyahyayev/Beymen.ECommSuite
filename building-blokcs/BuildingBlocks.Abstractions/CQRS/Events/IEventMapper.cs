using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using BuildingBlocks.Abstractions.Messaging;

namespace BuildingBlocks.Abstractions.CQRS.Events
{

    public interface IEventMapper : IIDomainNotificationEventMapper, IIntegrationEventMapper { }

    public interface IIDomainNotificationEventMapper
    {
        IReadOnlyList<List<IDomainNotificationEvent>?>? MapToDomainNotificationEvents(IReadOnlyList<IDomainEvent> domainEvents);
        List<IDomainNotificationEvent>? MapToDomainNotificationEvent(IDomainEvent domainEvent);
    }

    public interface IIntegrationEventMapper
    {
        IReadOnlyList<List<IIntegrationEvent>?>? MapToIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents);
        List<IIntegrationEvent>? MapToIntegrationEvent(IDomainEvent domainEvent);
    }
}
