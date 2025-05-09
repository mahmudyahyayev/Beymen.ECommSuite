using BuildingBlocks.Abstractions.Messaging;

namespace BuildingBlocks.Core.Messaging
{
    public record IntegrationEvent(string MessageKey, int Priority, MessageSendMode MessageSendMode,  string QueueName = "") : Message(MessageKey, Priority, MessageSendMode, QueueName), IIntegrationEvent { }
}
