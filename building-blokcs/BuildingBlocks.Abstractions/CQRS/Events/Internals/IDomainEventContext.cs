namespace BuildingBlocks.Abstractions.CQRS.Events.Internals
{
    public interface IDomainEventContext
    {
        IReadOnlyList<IDomainEvent> GetAllUncommittedEvents();
        void MarkUncommittedDomainEventAsCommitted();
    }
}
