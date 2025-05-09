using BuildingBlocks.Abstractions.Domain;

namespace Inventory.Domain.AggregateRoot.Products.BusinessRules;

public class StockQuantityCannotBeNegativeBusinessRule : IBusinessRule
{
    private readonly int _actualStock;
    private readonly int _stockChange;
    public StockQuantityCannotBeNegativeBusinessRule(int actualStock, int stockChange)
    {
        _actualStock = actualStock;
        _stockChange = stockChange;
    }

    public string Message => "Stock quantity cannot be negative.";
    public int StatusCode => 404;

    public string ExceptionId => "Stock quantity cannot be negative.";

    public bool IsBroken()
    {
        return _actualStock + _stockChange < 0;
    }
}