using BuildingBlocks.Core.Domain;

namespace Order.Domain.AggregateRoot.Orders;

public class OrderStatus : Enumeration
{
    public static OrderStatus Created = new(1, nameof(Created).ToLowerInvariant());
    public static OrderStatus Paid = new(2, nameof(Paid).ToLowerInvariant());
    public static OrderStatus Cancelled = new(3, nameof(Cancelled).ToLowerInvariant());
    public OrderStatus(int id, string name) : base(id, name)
    {
    }

    public static IEnumerable<OrderStatus> List() => new[] { Created, Paid, Cancelled };

    public static OrderStatus FromName(string name)
    {
        var state = List()
            .SingleOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (state is null)
        {
            ArgumentNullException.ThrowIfNull(nameof(state));
        }

        return state;
    }

    public static OrderStatus From(int id)
    {
        var state = List().SingleOrDefault(x => x.Id == id);

        if (state is null)
            ArgumentNullException.ThrowIfNull(nameof(state));

        return state;
    }
}
