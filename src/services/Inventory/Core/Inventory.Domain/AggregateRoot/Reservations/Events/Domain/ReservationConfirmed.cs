using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Inventory.Domain.AggregateRoot.Reservations.Events.Domain;
public record ReservationConfirmed(Guid ReservationId,Guid OrderId, Guid CustomerId) : DomainEvent(ReservationId.ToString(), 100);

