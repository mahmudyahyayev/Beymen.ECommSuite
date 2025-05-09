namespace Order.Domain.AggregateRoot.Orders.Exceptions;

[Serializable]
public class ItemQuantityMustBeGreaterThanZeroException : BuildingBlocks.Core.Domain.Exceptions.DomainException
{

    public ItemQuantityMustBeGreaterThanZeroException()
        : base($"Quantity must be greater than zero.", "Quantity must be greater than zero.", 404)
    {
    }
}