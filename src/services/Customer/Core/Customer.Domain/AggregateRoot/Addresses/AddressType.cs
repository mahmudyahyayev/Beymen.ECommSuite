using BuildingBlocks.Core.Domain;

namespace Customer.Domain.AggregateRoot.Addresses;

public class AddressType : Enumeration
{
    public static AddressType Shipping = new(1, nameof(Shipping).ToLowerInvariant());
    public static AddressType Billing = new(2, nameof(Billing).ToLowerInvariant());
    public AddressType(int id, string name) : base(id, name)
    {
    }

    public static IEnumerable<AddressType> List() => new[] { Shipping, Billing };

    public static AddressType FromName(string name)
    {
        var state = List()
            .SingleOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (state is null)
        {
            ArgumentNullException.ThrowIfNull(nameof(state));
        }

        return state;
    }

    public static AddressType From(int id)
    {
        var state = List().SingleOrDefault(x => x.Id == id);

        if (state is null)
            ArgumentNullException.ThrowIfNull(nameof(state));

        return state;
    }
}