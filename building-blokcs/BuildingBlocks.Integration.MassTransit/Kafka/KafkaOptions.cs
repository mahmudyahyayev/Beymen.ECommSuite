namespace BuildingBlocks.Integration.MassTransit.Kafka;

public sealed record KafkaOptions
{
    public string Host { get; set; }
    public bool SaslEnable { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ClientId { get; set; }
}
