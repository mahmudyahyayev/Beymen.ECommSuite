using Confluent.Kafka;
using MassTransit;

namespace BuildingBlocks.Integration.MassTransit.Kafka;

class ConsumerHolder
{
    public required Type ConsumerType { get; set; }

    public required string Topic { get; set; }

    public string? GroupId { get; set; }

    public ConsumerConfig? ConsumerConfig { get; set; }

    public Action<IKafkaTopicReceiveEndpointConfigurator>? Configure { get; set; } 
}
