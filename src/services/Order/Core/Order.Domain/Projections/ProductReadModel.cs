using BuildingBlocks.Core.Domain;

namespace Order.Domain.Projections;
public class ProductReadModel : Entity<Guid, string>
{
    /// <summary>
    /// Main Attributes
    /// </summary>
    public string _name;
    public string Name => _name;

    public decimal _unitPrice;
    public decimal UnitPrice => _unitPrice;

    public int _status;
    public int Status => _status;

    private ProductReadModel()
    {
        //for efcore
    }
    public static ProductReadModel Create(Guid id, string name, decimal unitPrice, int status)
    {
        ProductReadModel product = new()
        {
            Id = id,
            _name = name,
            _unitPrice = unitPrice,
            _status = status,
        };
        return product;
    }

    public void UpdateMainAttributes(string name, decimal unitPrice, int status)
    {
        _name = name;
        _unitPrice = unitPrice;
        _status = status;
    }
}
