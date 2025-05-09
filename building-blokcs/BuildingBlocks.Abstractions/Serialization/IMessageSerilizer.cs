using BuildingBlocks.Abstractions.Messaging;

namespace BuildingBlocks.Abstractions.Serialization
{
    public interface IMessageSerializer : ISerializer
    {
        string ContentType { get; }
        string Serialize(MessageEnvelope messageEnvelope);

        string Serialize<TMessage>(TMessage message)
            where TMessage : IMessage;
        MessageEnvelope? Deserialize(string json);
        IMessage? Deserialize(ReadOnlySpan<byte> data, string payloadType);
        TMessage? Deserialize<TMessage>(string message)
            where TMessage : IMessage;
        object? Deserialize(string payload, string payloadType);
    }
}
