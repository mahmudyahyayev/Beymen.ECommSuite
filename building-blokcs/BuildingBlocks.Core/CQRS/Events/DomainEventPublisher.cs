using System.Collections.Immutable;
using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Correlation;
using BuildingBlocks.Abstractions.CQRS.Events;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Messaging.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.CQRS.Events
{
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IEventProcessor _eventProcessor;
        private readonly IMessagePersistenceService _messagePersistenceService;
        private readonly IDomainEventsAccessor _domainEventsAccessor;
        private readonly IDomainNotificationEventPublisher _domainNotificationEventPublisher;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICorrelationService _correlationService;

        public DomainEventPublisher(
            IEventProcessor eventProcessor,
            IMessagePersistenceService messagePersistenceService,
            IDomainNotificationEventPublisher domainNotificationEventPublisher,
            IDomainEventsAccessor domainEventsAccessor,
            IServiceProvider serviceProvider,
            ICorrelationService correlationService
        )
        {
            _messagePersistenceService = messagePersistenceService;
            _domainEventsAccessor = domainEventsAccessor;
            _domainNotificationEventPublisher = Guard.Against.Null(
                domainNotificationEventPublisher,
                nameof(domainNotificationEventPublisher)
            );
            _eventProcessor = Guard.Against.Null(eventProcessor, nameof(eventProcessor));
            _serviceProvider = Guard.Against.Null(serviceProvider, nameof(serviceProvider));
            _correlationService = correlationService;
        }

        public Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            return PublishAsync(new[] { domainEvent }, cancellationToken);
        }

        public async Task PublishAsync(
            IDomainEvent[] domainEvents,
            CancellationToken cancellationToken)
        {
            Guard.Against.Null(domainEvents, nameof(domainEvents));

            if (!domainEvents.Any())
                return;

            var eventsToDispatch = domainEvents.ToList();

            if (!eventsToDispatch.Any())
            {
                eventsToDispatch = new List<IDomainEvent>(_domainEventsAccessor.UnCommittedDomainEvents);
            }

            //Important Note From Mahmud Yahyayev: Domain events are directly connected to internal MediatR commands and used in this way. Essentially, they function as internal events and are utilized for communication between aggregates and asynchronous processes within the domain.
            await _eventProcessor.DispatchAsync(eventsToDispatch.ToArray(), cancellationToken);


            //Important Note From Mahmud Yahyayev: Domain events can be used without writing a mapper if they are marked with IHaveNotificationEvent. Essentially, they function as internal events and are used for communication between aggregates.
            var wrappedNotificationEvents = eventsToDispatch.GetWrappedDomainNotificationEvents().ToArray();
            await _domainNotificationEventPublisher.PublishAsync(wrappedNotificationEvents.ToArray(),
                cancellationToken);

            // Important Note From Mahmud Yahyayev: Domain events can be used without writing a mapper if they are marked with IHaveExternalEvent. They function as integration events and are used for communication between domains.
            var wrappedIntegrationEvents = eventsToDispatch.GetWrappedIntegrationEvents().ToArray();
            foreach (var wrappedIntegrationEvent in wrappedIntegrationEvents)
            {
                var headers = new Dictionary<string, object?>();

                headers.AddCorrelationId(_correlationService.CorrelationId);

                await _messagePersistenceService.AddPublishMessageAsync(
                    new MessageEnvelope(
                    wrappedIntegrationEvent,
                    headers),
                    cancellationToken
                );
            }

            //Important Note From Mahmud Yahyayev: Domain events are used by writing an IntegrationEventMapper (IIntegrationEventMapper), and integration events are used for communication between domains.
            var eventMappers = _serviceProvider.GetServices<IEventMapper>().ToImmutableList();

            var integrationEvents = GetIntegrationEvents(_serviceProvider, eventMappers, eventsToDispatch);
            if (integrationEvents.Any())
            {
                foreach (var integrationEvent in integrationEvents)
                {
                    var headers = new Dictionary<string, object?>();

                    headers.AddCorrelationId(_correlationService.CorrelationId);

                    await _messagePersistenceService.AddPublishMessageAsync(
                        new MessageEnvelope(
                            integrationEvent,
                            headers),
                        cancellationToken
                    );
                }
            }

            //Important Note From Mahmud Yahyayev: Domain events are used by writing IIDomainNotificationEventMapper, and they are internal events used for communication between domains.
            var notificationEvents = GetNotificationEvents(_serviceProvider, eventMappers, eventsToDispatch);

            if (notificationEvents.Any())
            {
                foreach (var notification in notificationEvents)
                {
                    await _messagePersistenceService.AddNotificationAsync(
                        notification: notification,
                        cancellationToken);
                }
            }
        }

        private IReadOnlyList<IDomainNotificationEvent> GetNotificationEvents(
            IServiceProvider serviceProvider,
            IReadOnlyList<IEventMapper> eventMappers,
            IReadOnlyList<IDomainEvent> eventsToDispatch
        )
        {
            var notificationEventMappers =
                serviceProvider.GetServices<IIDomainNotificationEventMapper>().ToImmutableList();

            List<IDomainNotificationEvent> notificationEvents = new List<IDomainNotificationEvent>();

            if (eventMappers.Any())
            {
                foreach (var eventMapper in eventMappers)
                {
                    var itemsSet = eventMapper.MapToDomainNotificationEvents(eventsToDispatch)?.ToList();
                    if (itemsSet is not null && itemsSet.Any())
                    {
                        foreach (var items in itemsSet)
                        {
                            if (items is not null && items.Any())
                            {
                                notificationEvents.AddRange(items.Where(x => x is not null)!);
                            }
                        }   
                    }
                }
            }
            else if (notificationEventMappers.Any())
            {
                foreach (var notificationEventMapper in notificationEventMappers)
                {
                    var itemsSet = notificationEventMapper.MapToDomainNotificationEvents(eventsToDispatch)?.ToList();
                    if (itemsSet is not null && itemsSet.Any())
                    {
                        foreach (var items in itemsSet)
                        {
                            if (items is not null && items.Any())
                            {
                                notificationEvents.AddRange(items.Where(x => x is not null)!);
                            }
                        }
                    }
                }
            }
            return notificationEvents.ToImmutableList();
        }

        private static IReadOnlyList<IIntegrationEvent> GetIntegrationEvents(
            IServiceProvider serviceProvider,
            IReadOnlyList<IEventMapper> eventMappers,
            IReadOnlyList<IDomainEvent> eventsToDispatch
        )
        {
            var integrationEventMappers = serviceProvider.GetServices<IIntegrationEventMapper>().ToImmutableList();

            List<IIntegrationEvent> integrationEvents = new List<IIntegrationEvent>();

            if (eventMappers.Any())
            {
                foreach (var eventMapper in eventMappers)
                {
                    var itemsSet = eventMapper.MapToIntegrationEvents(eventsToDispatch)?.ToList();
                    if (itemsSet is not null && itemsSet.Any())
                    {
                        foreach (var items in itemsSet)
                        {
                            if (items is not null && items.Any())
                            {
                                integrationEvents.AddRange(items.Where(x => x is not null)!);
                            }
                        }
                    }
                }
            }
            else if (integrationEventMappers.Any())
            {
                foreach (var integrationEventMapper in integrationEventMappers)
                {
                    var itemsSet = integrationEventMapper.MapToIntegrationEvents(eventsToDispatch)?.ToList();
                    if (itemsSet is not null && itemsSet.Any())
                    {
                        foreach (var items in itemsSet)
                        {
                            if (items is not null && items.Any())
                            {
                                integrationEvents.AddRange(items.Where(x => x is not null)!);
                            }
                        }
                    }
                }
            }

            return integrationEvents.ToImmutableList();
        }
    }
}
