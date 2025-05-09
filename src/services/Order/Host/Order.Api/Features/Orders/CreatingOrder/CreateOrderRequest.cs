using BuildingBlocks.Abstractions.CQRS.Commands;
using Order.Application.Features.Orders.CreatingOrder.v1;

namespace Order.Api.Features.Orders.CreatingOrder;

public record CreateOrderRequest(
    Guid CustomerId,
    Guid ShippingAddressId,
    Guid BillingAddressId,
    List<OrderItemDto> Items);


