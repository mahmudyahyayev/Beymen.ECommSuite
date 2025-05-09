namespace BuildingBlocks.Core.CQRS.Events.Internals
{
    public record NotificationEvent(
        dynamic Data,
        string MessageKey,
        int Priority) : DomainNotificationEvent(MessageKey, Priority)
    {
    };
}
