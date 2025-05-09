using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Customer.Domain.AggregateRoot.Customers.Events.Domain;
public record CustomerActivated(Guid CustomerId) : DomainEvent(CustomerId.ToString(), 100);
