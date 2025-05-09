using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Inventory.Domain.AggregateRoot.Products.Events.Domain;

public record ProductCreated(
    Guid ProductId, 
    string Name, 
    string Description,
    decimal Price,
    int Stock,  
    int Status) : DomainEvent(ProductId.ToString(), 100);
