using BuildingBlocks.Abstractions.Domain;

namespace Order.Domain.AggregateRoot.Orders.BusinessRule;

public class OrderMustHaveAtLeastOneItemRule : IBusinessRule
{
    private readonly IReadOnlyList<OrderItem> _items;
    public OrderMustHaveAtLeastOneItemRule(IReadOnlyList<OrderItem> items)
    {
        _items = items;
    }

    public string Message => "An order must contain at least one item.";
    public int StatusCode => 404;

    public string ExceptionId => "An order must contain at least one item.";

    public bool IsBroken() => _items == null || !_items.Any();
}
