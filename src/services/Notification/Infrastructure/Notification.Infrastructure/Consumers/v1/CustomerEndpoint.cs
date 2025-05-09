using Beymen.ECommSuite.Shared.Events.Integration.v1;
using Humanizer;
using MassTransit;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Notification.Infrastructure.Consumers.v1;

public static class CustomerEndpoint
{
    public static void AddCreateCustomerEndpointV1(
        this IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context)
    {
        cfg.ReceiveEndpoint(
            $"{nameof(CustomerCreatedV1).Underscore()}",
            re =>
            {
                re.ConfigureConsumeTopology = true;
                //re.SetQuorumQueue();

                re.Bind(
                    $"{nameof(CustomerCreatedV1).Underscore()}.input_exchange",
                    e =>
                    {
                        e.RoutingKey = $"{nameof(CustomerCreatedV1).Underscore()}";
                        e.ExchangeType = ExchangeType.Fanout;
                    }
                );

                re.ConfigureConsumer<CreateCustomerConsumerV1>(context);
                re.UseMessageRetry(r =>
                {
                    r.Ignore(typeof(ArgumentNullException), typeof(JsonSerializationException));
                    r.Interval(3, 5000);
                });
            }
        );
    }
}