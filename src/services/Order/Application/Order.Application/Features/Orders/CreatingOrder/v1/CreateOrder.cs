using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Order.Application.Features.Orders.CreatingOrder.v1;

public record CreateOrder(
    Guid CustomerId,
    Guid ShippingAddressId,
    Guid BillingAddressId,
    List<OrderItemDto> Items) : ITxCreateCommand<CreateOrderResponse>;


