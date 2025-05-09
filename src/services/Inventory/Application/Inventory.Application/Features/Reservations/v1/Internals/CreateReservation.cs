using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Inventory.Application.Features.Reservations.v1.Internals;

public record CreateReservation(
     Guid OrderId,
     Guid CustomerId,
     List<ReservationItem> Items) : ITxCreateCommand<bool>;

public class ReservationItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}