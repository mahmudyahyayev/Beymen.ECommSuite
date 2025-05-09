using Beymen.ECommSuite.Shared.Events.Integration.v1;
using BuildingBlocks.Abstractions.CQRS.Events;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using BuildingBlocks.Abstractions.Messaging;
using Order.Domain.AggregateRoot.Orders.Events.Domain;

namespace Order.Infrastructure.Mappers
{
    public class IntegrationEventMapper : IIntegrationEventMapper
    {
        public IReadOnlyList<List<IIntegrationEvent>?> MapToIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents)
        {
            return domainEvents.Select(MapToIntegrationEvent).ToList();
        }

        public List<IIntegrationEvent>? MapToIntegrationEvent(IDomainEvent domainEvent)
        {
            return domainEvent switch
            {
                OrderCreated e
                   => [
                       new OrderCreatedV1(
                      e.OrderId,
                      e.CustomerId,
                      e.Items.Select(u=>new OrderCreatedItem{  ProductId = u.ProductId, Quantity = u.Quantity, TotalPrice = u.TotalPrice}).ToList(),
                      e.MessageKey,
                      e.Priority),
                       
                       new SendNotificationV1(
                            e.CustomerId,
                            "Your order is being prepared.",
                            1,
                            e.MessageKey,
                            e.Priority)
                        ],

                OrderPaid e
                    => [
                        new OrderPaidV1(
                        e.OrderId,
                        e.CustomerId,
                        e.MessageKey,
                        e.Priority), 

                        new SendNotificationV1(
                            e.CustomerId, 
                            "Your order has been completed.",
                            1,
                            e.MessageKey,
                            e.Priority)
                        ],

                OrderCancelled e
                    => [
                        new OrderCancelledV1(
                      e.OrderId,
                      e.CustomerId,
                      e.Reason,
                      e.MessageKey,
                      e.Priority),

                      new SendNotificationV1(
                            e.CustomerId,
                            $@"Your order has been cancelled due to the following reason: {e.Reason}.",
                            1,
                            e.MessageKey,
                            e.Priority)
                        ],

                _ => null
            };
        }
    }
}
