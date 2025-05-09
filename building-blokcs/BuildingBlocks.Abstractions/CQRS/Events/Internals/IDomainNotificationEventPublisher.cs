namespace BuildingBlocks.Abstractions.CQRS.Events.Internals
{
    public interface IDomainNotificationEventPublisher
    {
        Task PublishAsync(
            IDomainNotificationEvent domainNotificationEvent,
            CancellationToken cancellationToken);

        Task PublishAsync(
            IDomainNotificationEvent[] domainNotificationEvents,
            CancellationToken cancellationToken);
    }
}
