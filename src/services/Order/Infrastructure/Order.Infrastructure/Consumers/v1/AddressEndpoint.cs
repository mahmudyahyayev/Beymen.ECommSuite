using Beymen.ECommSuite.Shared.Events.Integration.v1;
using Humanizer;
using MassTransit;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Order.Infrastructure.Consumers.v1;
public static class AddressEndpoint
{
    public static void AddCreateAddressEndpointV1(
        this IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context)
    {
        cfg.ReceiveEndpoint(
            $"{nameof(AddressCreatedV1).Underscore()}",
            re =>
            {
                re.ConfigureConsumeTopology = true;
                //re.SetQuorumQueue();

                re.Bind(
                    $"{nameof(AddressCreatedV1).Underscore()}.input_exchange",
                    e =>
                    {
                        e.RoutingKey = $"{nameof(AddressCreatedV1).Underscore()}";
                        e.ExchangeType = ExchangeType.Fanout;
                    }
                );

                re.ConfigureConsumer<CreateAddressConsumerV1>(context);
                re.UseMessageRetry(r =>
                {
                    r.Ignore(typeof(ArgumentNullException), typeof(JsonSerializationException));
                    r.Interval(3, 5000);
                });
            }
        );
    }
}
