using Beymen.ECommSuite.Shared.Events.Integration.v1;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Messaging;
using Order.Application.Features.Addresses.CreatingAddress.v1;
using Order.Application.Features.Orders.CancelOrder.v1;
using Order.Application.Features.Orders.CompletingOrder.v1;
using Order.Application.Features.Products.CreatingProduct.v1;

namespace Order.Infrastructure.Mappers;

public class CommandMapper : ICommandMapper
{
    public IReadOnlyList<ICommand?>? MapToCommand(IReadOnlyList<IMessage> messages)
    {
        return messages.Select(MapToCommand).ToList();
    }

    public ICommand? MapToCommand(IMessage message)
    {
        return message switch
        {
            AddressCreatedV1 e
               => new CreateAddressCommand(
                   e.AddressId,
                   e.CustomerId,
                   e.Type,
                   e.FullAddress),

            ProductCreatedV1 e
              => new CreateProductCommand(
                  e.ProductId,
                  e.Name,
                  e.Price,
                  e.Status),

            ReservationCanceledV1 e
            => new CancelOrderCommand(
                e.ReservationId,
                e.OrderId,
                e.CustomerId,
                e.Message),

            ReservationConfirmedV1 e
             => new CompleteOrderCommand(
                 e.ReservationId,
                 e.OrderId,
                 e.CustomerId),

            _ => null
        };
    }
}