using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Notification.Application.Features.Notifications.CreatingNotification.v1;

public record CreateNotificationCommand(
    Guid CustomerId,
    string Message,
    int Type) : ICommand;

