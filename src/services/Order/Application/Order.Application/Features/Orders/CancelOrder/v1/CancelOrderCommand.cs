using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Order.Application.Features.Orders.CancelOrder.v1;

public record CancelOrderCommand(
   Guid ReservationId,
    Guid OrderId,
    Guid CustomerId,
    string Message) : ICommand;

