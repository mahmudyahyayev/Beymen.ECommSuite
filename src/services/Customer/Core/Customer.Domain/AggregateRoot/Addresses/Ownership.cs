using BuildingBlocks.Core.Domain;

namespace Customer.Domain.AggregateRoot.Addresses;

public class Ownership : ValueObject
{
    private Guid _value;
    public Guid Value => _value;
    private Ownership() { }

    public static Ownership Of(Guid name)
    {
        return new Ownership
        {
            _value = name
        };
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }
}
