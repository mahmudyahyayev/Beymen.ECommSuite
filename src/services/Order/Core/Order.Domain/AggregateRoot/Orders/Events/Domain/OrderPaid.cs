using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Order.Domain.AggregateRoot.Orders.Events.Domain;

public record OrderPaid(
    Guid OrderId,
    Guid CustomerId
      ) : DomainEvent(OrderId.ToString(), 300);

