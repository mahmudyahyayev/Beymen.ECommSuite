namespace Order.Application.Features.Orders.CreatingOrder.v1;

public record OrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
