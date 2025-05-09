using BuildingBlocks.Abstractions.Domain;

namespace Inventory.Domain.AggregateRoot.Products;
public record ProductId : AggregateId
{
    private ProductId(Guid value)
        : base(value) { }
    public static ProductId Of(Guid id) => new(id);

    public static implicit operator Guid(ProductId id) => id.Value;
}
