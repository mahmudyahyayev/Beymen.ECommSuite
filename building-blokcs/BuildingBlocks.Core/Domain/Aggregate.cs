using System.Collections.Concurrent;
using System.Collections.Immutable;
using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Core.Domain.Exceptions;

namespace BuildingBlocks.Core.Domain
{
    public abstract class Aggregate<TId, TAudit> : Entity<TId, TAudit>, IAggregate<TId, TAudit>
    {
        [NonSerialized]
        private readonly ConcurrentQueue<Abstractions.CQRS.Events.Internals.IDomainEvent> _uncommittedDomainEvents = new();

        private const long NewAggregateVersion = 0;

        public long OriginalVersion { get; private set; } = NewAggregateVersion;

        protected void AddDomainEvents(Abstractions.CQRS.Events.Internals.IDomainEvent domainEvent)
        {
            if (!_uncommittedDomainEvents.Any(x => Equals(x.EventId, domainEvent.EventId)))
            {
                _uncommittedDomainEvents.Enqueue(domainEvent);
            }
        }

        public bool HasUncommittedDomainEvents()
        {
            return _uncommittedDomainEvents.Any();
        }

        public IReadOnlyList<Abstractions.CQRS.Events.Internals.IDomainEvent> GetUncommittedDomainEvents()
        {
            return _uncommittedDomainEvents.ToImmutableList();
        }

        public void ClearDomainEvents()
        {
            _uncommittedDomainEvents.Clear();
        }

        public IReadOnlyList<Abstractions.CQRS.Events.Internals.IDomainEvent> DequeueUncommittedDomainEvents()
        {
            var events = _uncommittedDomainEvents.ToImmutableList();
            MarkUncommittedDomainEventAsCommitted();
            return events;
        }

        public void MarkUncommittedDomainEventAsCommitted()
        {
            _uncommittedDomainEvents.Clear();
        }

        public void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken())
            {
                throw new BusinessRuleValidationException(rule);
            }
        }
    }

    public abstract class Aggregate<TIdentity, TId, TAudit> : Aggregate<TIdentity, TAudit>
        where TIdentity : Identity<TId>
    { }

    public abstract class Aggregate<TAudit> : Aggregate<AggregateId, Guid, TAudit>, IAggregate<TAudit> { }
}
