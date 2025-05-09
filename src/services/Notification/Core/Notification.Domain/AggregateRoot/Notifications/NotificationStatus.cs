using BuildingBlocks.Core.Domain;

namespace Notification.Domain.AggregateRoot.Notifications;

public class NotificationStatus : Enumeration
{
    public static NotificationStatus Pending = new(1, nameof(Pending).ToLowerInvariant());
    public static NotificationStatus Sent = new(2, nameof(Sent).ToLowerInvariant());
    public static NotificationStatus Failed = new(3, nameof(Failed).ToLowerInvariant());
    public NotificationStatus(int id, string name) : base(id, name)
    {
    }

    public static IEnumerable<NotificationStatus> List() => new[] { Pending, Sent, Failed };

    public static NotificationStatus FromName(string name)
    {
        var state = List()
            .SingleOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (state is null)
        {
            ArgumentNullException.ThrowIfNull(nameof(state));
        }

        return state;
    }

    public static NotificationStatus From(int id)
    {
        var state = List().SingleOrDefault(x => x.Id == id);

        if (state is null)
            ArgumentNullException.ThrowIfNull(nameof(state));

        return state;
    }
}