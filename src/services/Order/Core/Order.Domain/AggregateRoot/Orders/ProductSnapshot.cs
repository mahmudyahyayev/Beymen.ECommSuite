using BuildingBlocks.Core.Domain;

namespace Order.Domain.AggregateRoot.Orders;
public class ProductSnapshot : ValueObject
{
    private Guid _productId;
    public Guid ProductId => _productId;


    private string _name;
    public string Name => _name;

    public decimal _unitPrice;
    public decimal UnitPrice => _unitPrice;

    private ProductSnapshot() { }

    public static ProductSnapshot Of(Guid productId, string name, decimal unitPrice)
    {
        return new ProductSnapshot
        {
            _productId = productId,
            _name = name,
            _unitPrice = unitPrice
        };
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _productId;
        yield return _name;
        yield return _unitPrice;
    }
}
