using Beymen.ECommSuite.Shared.Events.Integration.v1;
using Humanizer;
using MassTransit;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Notification.Infrastructure.Consumers.v1;

public static class NotificationEndpoint
{
    public static void AddSendNotificationEndpointV1(
        this IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context)
    {
        cfg.ReceiveEndpoint(
            $"{nameof(SendNotificationV1).Underscore()}",
            re =>
            {
                re.ConfigureConsumeTopology = true;
                //re.SetQuorumQueue();

                re.Bind(
                    $"{nameof(SendNotificationV1).Underscore()}.input_exchange",
                    e =>
                    {
                        e.RoutingKey = $"{nameof(SendNotificationV1).Underscore()}";
                        e.ExchangeType = ExchangeType.Fanout;
                    }
                );

                re.ConfigureConsumer<SendNotificationConsumerV1>(context);
                re.UseMessageRetry(r =>
                {
                    r.Ignore(typeof(ArgumentNullException), typeof(JsonSerializationException));
                    r.Interval(3, 5000);
                });
            }
        );
    }
}
