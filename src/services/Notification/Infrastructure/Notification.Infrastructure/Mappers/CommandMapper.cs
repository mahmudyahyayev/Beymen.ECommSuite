using Beymen.ECommSuite.Shared.Events.Integration.v1;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Messaging;
using Notification.Application.Features.Customers.CreatingCustomer.v1;
using Notification.Application.Features.Notifications.CreatingNotification.v1;

namespace Notification.Infrastructure.Mappers;

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
            CustomerCreatedV1 e
               => new CreateCustomerCommand(
                   e.CustomerId,
                   e.FirstName,
                   e.LastName,
                   e.PhoneNumber,
                   e.Email),

            SendNotificationV1 e
               => new CreateNotificationCommand(
                   e.CustomerId,
                   e.Message,
                   e.Type),

            _ => null
        };
    }
}