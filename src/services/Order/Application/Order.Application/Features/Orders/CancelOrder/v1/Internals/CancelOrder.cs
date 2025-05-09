using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Order.Application.Features.Orders.CancelOrder.v1.Internals;

public record CancelOrder(
     Guid ReservationId,
    Guid OrderId,
    Guid CustomerId,
    string Message) : ITxUpdateCommand<bool>;
