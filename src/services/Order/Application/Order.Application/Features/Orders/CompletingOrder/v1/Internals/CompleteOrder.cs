using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Order.Application.Features.Orders.CompletingOrder.v1.Internals;

public record CompleteOrder(
     Guid ReservationId,
    Guid OrderId,
    Guid CustomerId) : ITxUpdateCommand<bool>;

