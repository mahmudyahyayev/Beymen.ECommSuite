using BuildingBlocks.Core.Domain;
using Order.Domain.AggregateRoot.Orders.Exceptions;

namespace Order.Domain.AggregateRoot.Orders;

public class OrderItem : Entity<OrderItemId>
{
    public OrderId  _orderId;
    public OrderId OrderId => _orderId;

    public Order _order;
    public Order Order => _order;

    public ProductSnapshot _product;
    public ProductSnapshot Product => _product;

    public int _quantity;
    public int Quantity => _quantity;

    public decimal _totalPrice;
    public decimal TotalPrice => _totalPrice;

    private OrderItem() { }

    public static OrderItem Create(ProductSnapshot product, int quantity)
    {
        if (quantity <= 0)
            throw new ItemQuantityMustBeGreaterThanZeroException();

        var item = new OrderItem()
        {
            Id = OrderItemId.Of(Guid.NewGuid()),
            _product = product,
            _quantity = quantity,
            _totalPrice = quantity * product.UnitPrice
        };

        return item;
    }
}
