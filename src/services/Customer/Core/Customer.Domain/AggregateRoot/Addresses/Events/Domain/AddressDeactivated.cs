using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Customer.Domain.AggregateRoot.Addresses.Events.Domain;
public record AddressDeactivated(Guid AddressId) : DomainEvent(AddressId.ToString(), 100);