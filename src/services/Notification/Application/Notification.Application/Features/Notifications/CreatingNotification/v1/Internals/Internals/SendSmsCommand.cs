using BuildingBlocks.Core.CQRS.Commands;

namespace Notification.Application.Features.Notifications.CreatingNotification.v1.Internals.Internals;
public record SendSmsCommand(
    Guid NotificationId,
    Guid CustomerId,
    string Message,
    string MessageKey,
    int Priority) : InternalCommand(MessageKey, Priority);
