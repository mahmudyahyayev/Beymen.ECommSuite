using MediatR;

namespace BuildingBlocks.Abstractions.CQRS.Events
{
    public interface IEvent : INotification
    {
        Guid EventId { get; }
        long EventVersion { get; }
        DateTime OccurredOn { get; }
        DateTimeOffset TimeStamp { get; }
        public string EventType { get; }
        string MessageKey { get; }
        int Priority { get; }
    }
}
