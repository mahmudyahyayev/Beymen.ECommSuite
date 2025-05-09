using BuildingBlocks.Abstractions.CQRS.Events;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;

namespace BuildingBlocks.Core.Persistence.Efcore
{
    public class EfDomainEventAccessor : IDomainEventsAccessor
    {
        private readonly IDomainEventContext _domainEventContext;
        private readonly IAggregatesDomainEventsRequestStore _aggregatesDomainEventsStore;

        public EfDomainEventAccessor(
            IDomainEventContext domainEventContext,
            IAggregatesDomainEventsRequestStore aggregatesDomainEventsStore
        )
        {
            _domainEventContext = domainEventContext;
            _aggregatesDomainEventsStore = aggregatesDomainEventsStore;
        }

        public IReadOnlyList<IDomainEvent> UnCommittedDomainEvents
        {
            get
            {
                _ = _aggregatesDomainEventsStore.GetAllUncommittedEvents();

                return _domainEventContext.GetAllUncommittedEvents();
            }
        }
    }
}
