using BuildingBlocks.Core.Domain;

namespace Order.Domain.AggregateRoot.Orders;

public class CustomerId : ValueObject
{
    private Guid _value;
    public Guid Value => _value;
    private CustomerId() { }

    public static CustomerId Of(Guid name)
    {
        return new CustomerId
        {
            _value = name
        };
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }
}
