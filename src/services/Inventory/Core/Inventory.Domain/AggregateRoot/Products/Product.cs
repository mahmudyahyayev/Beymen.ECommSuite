using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Core.Domain;
using Inventory.Domain.AggregateRoot.Products.BusinessRules;
using Inventory.Domain.AggregateRoot.Products.Events.Domain;

namespace Inventory.Domain.AggregateRoot.Products;

public class Product : Aggregate<ProductId, string>,
    IHaveAudit<string>,
    IHaveIdentity<ProductId>
{
    private string _name;
    public string Name => _name;

    private string _description;
    public string Description => _description;

    private decimal _price;
    public decimal Price => _price;

    private int _stock;
    public int Stock => _stock;

    private int _status;
    public ProductStatus Status { get; private set; }

    /// <summary>
    /// Auditable
    /// </summary>
    private string _lastModifiedBy;
    public string LastModifiedBy => _lastModifiedBy;
    private DateTime? _lastModified;
    public DateTime? LastModified => _lastModified;

    private Product()
    {
        // EF Core için
    }

    public static Product Create(string name, string description, decimal price, int stock)
    {
        var product = new Product
        {
            Id = ProductId.Of(Guid.NewGuid()),
            _name = name,
            _description = description,
            _price = price,
            _stock = stock,
            _status = ProductStatus.Available.Id
        };

        product.WhenCreate();
        return product;
    }

    public void WhenCreate()
    {
        //genel olarak Listing servisleri icin amaclanmistir. Ayni zamanda order domaininda snapshot tutmak icin.
        AddDomainEvents(new ProductCreated(Id, _name, _description, _price, _stock, _status));
    }

    public bool IsStockAvailable(int quantity)
    {
        return _stock >= quantity && _status == ProductStatus.Available.Id;
    }

    public void UpdateStock(int stockChange)
    {
        CheckRule(new StockQuantityCannotBeNegativeBusinessRule(_stock, stockChange));

        _stock += stockChange;

        //ozellikle listing servise bildirilmelidir.
        AddDomainEvents(new ProductStockChanged(Id.Value, _stock));
    }

    public void MarkAsUnavailable()
    {
        _status = ProductStatus.Unavailable.Id;


        //genel olarak Listing servisleri icin amaclanmistir
        AddDomainEvents(new ProductStateChanged(Id, _status));
    }

    public void MarkAsAvailable()
    {
        //genel olarak Listing servisleri icin amaclanmistir
        _status = ProductStatus.Available.Id;
        AddDomainEvents(new ProductStateChanged(Id, _status));
    }

    public void MarkAsDiscontinued()
    {
        //genel olarak Listing servisleri icin amaclanmistir
        _status = ProductStatus.Discontinued.Id;
        AddDomainEvents(new ProductStateChanged(Id, _status));
    }
}
