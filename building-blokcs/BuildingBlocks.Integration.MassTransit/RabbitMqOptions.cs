namespace BuildingBlocks.Integration.MassTransit
{
    public class RabbitMqOptions
    {
        public string Host { get; set; } = "localhost";
        public ushort Port { get; set; } = 5672;
        public string UserName { get; set; }
        public string Password { get; set; }

        public string VirtualHost { get; set; } = "/";
        public string ConnectionString => $"amqp://{UserName}:{Password}@{Host}:{Port}/";
    }
}
