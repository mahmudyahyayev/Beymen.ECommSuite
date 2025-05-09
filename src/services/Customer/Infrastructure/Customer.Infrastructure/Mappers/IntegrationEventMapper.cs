using Beymen.ECommSuite.Shared.Events.Integration.v1;
using BuildingBlocks.Abstractions.CQRS.Events;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using BuildingBlocks.Abstractions.Messaging;
using Customer.Domain.AggregateRoot.Addresses.Events.Domain;
using Customer.Domain.AggregateRoot.Customers.Events.Domain;

namespace Customer.Infrastructure.Mappers;

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
            CustomerCreated e
               => [
                   new CustomerCreatedV1(
                      e.CustomerId,
                      e.FirstName,
                      e.LastName,
                      e.PhoneNumber,
                      e.Email,
                      e.MessageKey,
                      e.Priority)],

            CustomerActivated e
                => [
                    new CustomerActivatedV1(
                        e.CustomerId,
                        e.MessageKey,
                        e.Priority)],

            CustomerDeactivated e
                => [
                    new CustomerDeactivatedV1(
                      e.CustomerId,
                      e.MessageKey,
                      e.Priority)],

            CustomerMainAttributesChanged e
                    => [
                    new CustomerMainAttributesChangedV1(
                      e.CustomerId,
                      e.FirstName,
                      e.LastName,
                      e.PhoneNumber,
                      e.Email,
                      e.MessageKey,
                      e.Priority)],


            AddressCreated e
             => [
                 new AddressCreatedV1(
                      e.AddressId,
                      e.CustomerId,
                      e.Type,
                      e.FullAddress,
                      e.MessageKey,
                      e.Priority)],

            AddressActivated e
              => [
                  new AddressActivatedV1(
                        e.AddressId,
                        e.MessageKey,
                        e.Priority)],

            AddressDeactivated e
                 => [
                     new AddressDeactivatedV1(
                        e.AddressId,
                        e.MessageKey,
                        e.Priority)],

            AddressMainAttributesChanged e
               => [
               new AddressMainAttributesChangedV1(
                            e.AddressId,
                            e.FullAddress,
                            e.MessageKey,
                            e.Priority)],

            AddressTypeChanged e
                    => [
                    new AddressTypeChangedV1(
                                        e.AddressId,
                                        e.Type,
                                        e.MessageKey,
                                        e.Priority)],

            _ => null
        };
    }
}

    