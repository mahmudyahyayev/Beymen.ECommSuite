using BuildingBlocks.Core.Domain;

namespace Notification.Domain.AggregateRoot.Notifications;

public class NotificationType : Enumeration
{
    public static NotificationType Sms = new(1, nameof(Sms).ToLowerInvariant());
    public static NotificationType Email = new(2, nameof(Email).ToLowerInvariant());
    public NotificationType(int id, string name) : base(id, name)
    {
    }

    public static IEnumerable<NotificationType> List() => new[] { Sms, Email };

    public static NotificationType FromName(string name)
    {
        var state = List()
            .SingleOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (state is null)
        {
            ArgumentNullException.ThrowIfNull(nameof(state));
        }

        return state;
    }

    public static NotificationType From(int id)
    {
        var state = List().SingleOrDefault(x => x.Id == id);

        if (state is null)
            ArgumentNullException.ThrowIfNull(nameof(state));

        return state;
    }
}
