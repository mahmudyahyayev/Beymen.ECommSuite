using MediatR;

namespace BuildingBlocks.Abstractions.Messaging
{
    public interface IMessage : INotification
    {
        Guid MessageId { get; }
        DateTime Created { get; }
        string MessageKey { get; }
        int Priority { get; }
        MessageSendMode SendMode { get; }
        string QueueName { get; }
    }
}
