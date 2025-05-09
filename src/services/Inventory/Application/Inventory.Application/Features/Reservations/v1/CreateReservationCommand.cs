using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Inventory.Application.Features.Reservations.v1;

public record CreateReservationCommand(
    Guid OrderId,
    Guid CustomerId,
    List<ReservationCreatedCommandItem> Items) : ICommand;

public class ReservationCreatedCommandItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}