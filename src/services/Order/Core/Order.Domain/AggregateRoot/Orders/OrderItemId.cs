using BuildingBlocks.Abstractions.Domain;

namespace Order.Domain.AggregateRoot.Orders;

public record OrderItemId : EntityId
{
    private OrderItemId(Guid value)
        : base(value) { }
    public static OrderItemId Of(Guid id) => new(id);

    public static implicit operator Guid(OrderItemId id) => id.Value;
}

