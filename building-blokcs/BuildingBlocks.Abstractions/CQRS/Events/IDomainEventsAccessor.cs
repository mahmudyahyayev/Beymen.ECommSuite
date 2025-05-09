using BuildingBlocks.Abstractions.CQRS.Events.Internals;

namespace BuildingBlocks.Abstractions.CQRS.Events
{
    public interface IDomainEventsAccessor
    {
        IReadOnlyList<IDomainEvent> UnCommittedDomainEvents { get; }
    }
}
