using BuildingBlocks.Core.Domain;

namespace Inventory.Domain.AggregateRoot.Reservations;
public class OrderId : ValueObject
{
    private Guid _value;
    public Guid Value => _value;
    private OrderId() { }

    public static OrderId Of(Guid name)
    {
        return new OrderId
        {
            _value = name
        };
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }
}
