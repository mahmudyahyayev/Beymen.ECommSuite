using BuildingBlocks.Core.CQRS.Events.Internals;

namespace Notification.Domain.AggregateRoot.Notifications.Events.Domain;

public record EmailNotificationCreated(
    Guid NotificationId,
    Guid CustomerId,
    string Message) : DomainEvent(NotificationId.ToString(), 300);

