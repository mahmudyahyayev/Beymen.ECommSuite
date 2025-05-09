using BuildingBlocks.Abstractions.Domain;

namespace Order.Domain.AggregateRoot.Orders;
public record OrderId : AggregateId
{
    private OrderId(Guid value)
        : base(value) { }
    public static OrderId Of(Guid id) => new(id);

    public static implicit operator Guid(OrderId id) => id.Value;
}
