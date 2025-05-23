using BuildingBlocks.Abstractions.CQRS.Events;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;

namespace BuildingBlocks.Core.CQRS.Events;

public class NullDomainEventsAccessor : IDomainEventsAccessor
{
    public IReadOnlyList<IDomainEvent> UnCommittedDomainEvents { get; }
}
