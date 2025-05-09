using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Customer.Domain.AggregateRoot.Addresses.Events.Domain;

public record AddressMainAttributesChanged(
    Guid AddressId,
    string FullAddress) : DomainEvent(AddressId.ToString(), 100);

