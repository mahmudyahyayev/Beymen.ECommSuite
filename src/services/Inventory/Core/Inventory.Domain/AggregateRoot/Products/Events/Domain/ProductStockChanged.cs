using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Inventory.Domain.AggregateRoot.Products.Events.Domain;

public record ProductStockChanged(
    Guid ProductId,
    int Stock) : DomainEvent(ProductId.ToString(), 100);

