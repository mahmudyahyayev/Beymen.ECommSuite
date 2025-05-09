using Beymen.ECommSuite.Shared.Events.Integration.v1;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Messaging;
using Inventory.Application.Features.Reservations.v1;

namespace Inventory.Infrastructure.Mappers;

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
            OrderCreatedV1 e
               => new CreateReservationCommand(
                   e.OrderId,
                   e.CustomerId,
                   e.Items.Select(u=> new ReservationCreatedCommandItem { ProductId = u.ProductId, Quantity = u.Quantity}).ToList()),

            _ => null
        };
    }
}
