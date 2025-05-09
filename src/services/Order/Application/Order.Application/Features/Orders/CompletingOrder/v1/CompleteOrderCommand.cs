using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Order.Application.Features.Orders.CompletingOrder.v1;

public record CompleteOrderCommand(
   Guid ReservationId,
    Guid OrderId,
    Guid CustomerId) : ICommand;
