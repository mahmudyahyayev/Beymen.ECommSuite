using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Customer.Domain.AggregateRoot.Addresses.Events.Domain;
public record AddressTypeChanged(
    Guid AddressId, 
    int Type) : DomainEvent(AddressId.ToString(), 100);
