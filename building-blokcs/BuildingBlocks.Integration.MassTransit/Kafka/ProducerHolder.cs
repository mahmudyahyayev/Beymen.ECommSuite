using Confluent.Kafka;

namespace BuildingBlocks.Integration.MassTransit.Kafka;

class ProducerHolder
{
    public required Type ProducerType { get; set; }

    public required string Topic { get; set; }

    public ProducerConfig? ProducerConfig { get; set; }
}
