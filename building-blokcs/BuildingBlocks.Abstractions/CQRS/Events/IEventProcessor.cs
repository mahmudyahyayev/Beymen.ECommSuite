namespace BuildingBlocks.Abstractions.CQRS.Events
{
    public interface IEventProcessor
    {
        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IEvent;

        public Task PublishAsync<TEvent>(TEvent[] events, CancellationToken cancellationToken )
            where TEvent : IEvent;

        public Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken )
            where TEvent : IEvent;

        public Task DispatchAsync<TEvent>(TEvent[] events, CancellationToken cancellationToken)
            where TEvent : IEvent;
    }
}
