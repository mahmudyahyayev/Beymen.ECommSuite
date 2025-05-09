using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Order.Domain.AggregateRoot.Orders.Events.Domain;

public record OrderCreated(
    Guid OrderId,
    Guid CustomerId,
    List<OrderCreatedDomainEventObject> Items
      ) : DomainEvent(OrderId.ToString(), 300);

public class OrderCreatedDomainEventObject
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}
