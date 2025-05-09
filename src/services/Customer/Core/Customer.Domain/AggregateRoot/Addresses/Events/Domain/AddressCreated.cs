using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Customer.Domain.AggregateRoot.Addresses.Events.Domain;

public record AddressCreated(
    Guid AddressId,
    Guid CustomerId,
    int Type,
    string FullAddress) : DomainEvent(AddressId.ToString(), 200);