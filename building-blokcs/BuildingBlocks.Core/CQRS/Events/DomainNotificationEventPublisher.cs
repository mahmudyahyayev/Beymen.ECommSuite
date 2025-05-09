using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;

namespace BuildingBlocks.Core.CQRS.Events
{
    public class DomainNotificationEventPublisher : IDomainNotificationEventPublisher
    {
        private readonly IMessagePersistenceService _messagePersistenceService;

        public DomainNotificationEventPublisher(IMessagePersistenceService messagePersistenceService)
        {
            _messagePersistenceService = messagePersistenceService;
        }

        public Task PublishAsync(
            IDomainNotificationEvent domainNotificationEvent,
            CancellationToken cancellationToken
        )
        {
            Guard.Against.Null(domainNotificationEvent, nameof(domainNotificationEvent));

            return _messagePersistenceService.AddNotificationAsync(
                notification: domainNotificationEvent,
                cancellationToken);
        }

        public async Task PublishAsync(
            IDomainNotificationEvent[] domainNotificationEvents,
            CancellationToken cancellationToken
        )
        {
            Guard.Against.Null(domainNotificationEvents, nameof(domainNotificationEvents));

            foreach (var domainNotificationEvent in domainNotificationEvents)
            {
                await _messagePersistenceService.AddNotificationAsync(domainNotificationEvent, cancellationToken);
            }
        }
    }
}
