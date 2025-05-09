using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Core.Domain;
using Order.Domain.AggregateRoot.Orders.BusinessRule;
using Order.Domain.AggregateRoot.Orders.Events.Domain;

namespace Order.Domain.AggregateRoot.Orders;

public class Order :Aggregate<OrderId, string>,
    IHaveAudit<string>,
    IHaveIdentity<OrderId>
{
    private CustomerId _customerId;
    public CustomerId CustomerId => _customerId;

    private int _status;
    public OrderStatus Status  { get; private set; }

    private AddressSnapshot _shippingAddress;
    public AddressSnapshot ShippingAddress => _shippingAddress;

    private AddressSnapshot _billingAddress;
    public AddressSnapshot BillingAddress => _billingAddress;

    private List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items;

    private decimal _totalPrice;
    public decimal TotalPrice => _totalPrice;


    /// <summary>
    /// Auditable
    /// </summary>
    private string _lastModifiedBy;
    public string LastModifiedBy => _lastModifiedBy;
    private DateTime? _lastModified;
    public DateTime? LastModified => _lastModified;

    private Order()
    {
        // for Efcore
    }

    public static Order Create(Guid customerId, AddressSnapshot shippingAddress, AddressSnapshot billingAddress, List<OrderItem> items)
    {
        var order = new Order
        {
            Id = OrderId.Of(Guid.NewGuid()),
            _customerId = CustomerId.Of(customerId),
            _status = OrderStatus.Created.Id,
            _billingAddress = billingAddress,
            _shippingAddress = shippingAddress
        };

        order.AddItems(items);

        order.WhenCreate();
        return order;
    }

    public void WhenCreate()
    {
        var domainObjectItems = _items
            .Select(i => new OrderCreatedDomainEventObject() { ProductId = i.Product.ProductId, Quantity = i.Quantity, TotalPrice = i.TotalPrice })
            .ToList();

        AddDomainEvents(new OrderCreated(Id.Value, _customerId.Value, domainObjectItems));
    }

    public void AddItems(List<OrderItem> items)
    {
        CheckRule(new OrderMustHaveAtLeastOneItemRule(items));

        _items = items;

        _totalPrice = items.Sum(i => i.TotalPrice);
    }


    //Aslinda burada payment servis gonderilmesi gerek donen sonuca gore paid alip notification cikmasi gerek ama Payment servisi istenmedi. :))

    //Biz burada inventory okeyse Paid gibi kabul edip musteriye notify cikicaz.

    //Asil senaryoda Shipping tarafi adimi (Fullfillment adimlari da olmasi gerekir) :)))))

    public void MarkAsPaid()
    {
        _status = OrderStatus.Paid.Id;

        AddDomainEvents(new OrderPaid(Id.Value, _customerId.Value));
    }

    public void Cancel(string reason)
    {
        _status = OrderStatus.Cancelled.Id;

        AddDomainEvents(new OrderCancelled(Id, _customerId.Value, reason));
    }
}
