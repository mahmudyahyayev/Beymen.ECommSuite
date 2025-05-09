using BuildingBlocks.Core.Domain;

namespace Order.Domain.AggregateRoot.Orders;
public class AddressSnapshot : ValueObject
{
    private Guid _addressId;
    public Guid AddressId => _addressId;

    private string _address;
    public string Address => _address;
    private AddressSnapshot() { }

    public static AddressSnapshot Of(Guid addressId, string address)
    {
        return new AddressSnapshot
        {
            _addressId = addressId,
            _address = address
        };
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _addressId;
        yield return _address;
    }
}
