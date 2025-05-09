using BuildingBlocks.Abstractions.CQRS.Events.Internals;

namespace BuildingBlocks.Abstractions.Domain
{
    public interface IDomainEventRaisable
    {
        public bool HasUncommittedDomainEvents();
        IReadOnlyList<CQRS.Events.Internals.IDomainEvent> GetUncommittedDomainEvents();
        void ClearDomainEvents();
        void MarkUncommittedDomainEventAsCommitted();
        void CheckRule(IBusinessRule rule);
    }
}
