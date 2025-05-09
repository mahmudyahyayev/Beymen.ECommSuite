using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Inventory.Domain.AggregateRoot.Reservations.Events.Domain;

public record ReservationCanceled(Guid ReservationId, Guid OrderId, Guid CustomerId, string Message) : DomainEvent(ReservationId.ToString(), 100);

