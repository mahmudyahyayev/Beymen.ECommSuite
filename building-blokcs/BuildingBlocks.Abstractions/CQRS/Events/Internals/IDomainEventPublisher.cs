namespace BuildingBlocks.Abstractions.CQRS.Events.Internals;

public interface IDomainEventPublisher
{
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);
    Task PublishAsync(IDomainEvent[] domainEvents, CancellationToken cancellationToken);
}
