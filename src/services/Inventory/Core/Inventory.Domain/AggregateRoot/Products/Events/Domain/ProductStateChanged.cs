using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Inventory.Domain.AggregateRoot.Products.Events.Domain;

public record ProductStateChanged(Guid ProductId, int status) : DomainEvent(ProductId.ToString(), 100);

