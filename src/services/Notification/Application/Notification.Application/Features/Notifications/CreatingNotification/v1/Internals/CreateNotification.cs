using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Notification.Application.Features.Notifications.CreatingNotification.v1.Internals;

public record CreateNotification(
    Guid CustomerId,
    string Message,
    int Type) : ITxCreateCommand<bool>;

