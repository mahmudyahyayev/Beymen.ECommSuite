using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Order.Domain.AggregateRoot.Orders.Events.Domain;

public record OrderCancelled(
    Guid OrderId,
    Guid CustomerId,
    string Reason
      ) : DomainEvent(OrderId.ToString(), 300);
