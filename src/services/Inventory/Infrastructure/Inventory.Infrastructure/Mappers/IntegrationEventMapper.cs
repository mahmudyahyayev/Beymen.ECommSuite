using Beymen.ECommSuite.Shared.Events.Integration.v1;
using BuildingBlocks.Abstractions.CQRS.Events;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using BuildingBlocks.Abstractions.Messaging;
using Inventory.Domain.AggregateRoot.Products.Events.Domain;
using Inventory.Domain.AggregateRoot.Reservations.Events.Domain;

namespace Inventory.Infrastructure.Mappers;
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
            ReservationCanceled e
               => [
                   new ReservationCanceledV1(
                      e.ReservationId,
                      e.OrderId,
                      e.CustomerId,
                      e.Message,
                      e.MessageKey,
                      e.Priority)],

            ProductCreated e
    => [
        new ProductCreatedV1(
                      e.ProductId,
                      e.Name,
                      e.Description,
                      e.Price,
                      e.Stock,
                      e.Status,
                      e.MessageKey,
                      e.Priority)],

            ReservationConfirmed e
    => [
        new ReservationConfirmedV1(
                      e.ReservationId,
                      e.OrderId,
                      e.CustomerId,
                      e.MessageKey,
                      e.Priority)],


            _ => null
        };
    }
}
