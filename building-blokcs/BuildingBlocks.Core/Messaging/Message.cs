using BuildingBlocks.Abstractions.Messaging;

namespace BuildingBlocks.Core.Messaging
{
    public record Message(
        string MessageKey,
        int Priority,
        MessageSendMode SendMode,
        string QueueName) : IMessage
    {
        public Guid MessageId => Guid.NewGuid();
        public DateTime Created { get; } = DateTime.Now;
    }
}
