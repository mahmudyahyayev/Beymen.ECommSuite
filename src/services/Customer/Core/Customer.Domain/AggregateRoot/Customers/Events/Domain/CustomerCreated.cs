using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Customer.Domain.AggregateRoot.Customers.Events.Domain;

public record CustomerCreated(
    Guid CustomerId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email) : DomainEvent(CustomerId.ToString(), 200);