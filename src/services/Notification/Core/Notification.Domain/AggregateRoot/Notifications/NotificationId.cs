using BuildingBlocks.Abstractions.Domain;

namespace Notification.Domain.AggregateRoot.Notifications;
public record NotificationId : AggregateId
{
    private NotificationId(Guid value)
        : base(value) { }
    public static NotificationId Of(Guid id) => new(id);

    public static implicit operator Guid(NotificationId id) => id.Value;
}
