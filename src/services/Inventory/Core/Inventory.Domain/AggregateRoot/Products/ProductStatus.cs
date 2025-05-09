using BuildingBlocks.Core.Domain;

namespace Inventory.Domain.AggregateRoot.Products;

public class ProductStatus : Enumeration
{
    public static ProductStatus Available = new(1, nameof(Available).ToLowerInvariant());
    public static ProductStatus Unavailable = new(2, nameof(Unavailable).ToLowerInvariant());
    public static ProductStatus Discontinued = new(3, nameof(Discontinued).ToLowerInvariant());
    public ProductStatus(int id, string name) : base(id, name)
    {
    }

    public static IEnumerable<ProductStatus> List() => new[] { Available, Unavailable, Discontinued };

    public static ProductStatus FromName(string name)
    {
        var state = List()
            .SingleOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (state is null)
        {
            ArgumentNullException.ThrowIfNull(nameof(state));
        }

        return state;
    }

    public static ProductStatus From(int id)
    {
        var state = List().SingleOrDefault(x => x.Id == id);

        if (state is null)
            ArgumentNullException.ThrowIfNull(nameof(state));

        return state;
    }
}