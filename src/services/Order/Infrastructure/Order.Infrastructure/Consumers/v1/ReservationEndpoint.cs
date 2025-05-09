using Beymen.ECommSuite.Shared.Events.Integration.v1;
using Humanizer;
using MassTransit;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Order.Infrastructure.Consumers.v1;

public static class ReservationEndpoint
{
    public static void AddCancelOrderEndpointV1(
        this IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context)
    {
        cfg.ReceiveEndpoint(
            $"{nameof(ReservationCanceledV1).Underscore()}",
            re =>
            {
                re.ConfigureConsumeTopology = true;
                //re.SetQuorumQueue();

                re.Bind(
                    $"{nameof(ReservationCanceledV1).Underscore()}.input_exchange",
                    e =>
                    {
                        e.RoutingKey = $"{nameof(ReservationCanceledV1).Underscore()}";
                        e.ExchangeType = ExchangeType.Fanout;
                    }
                );

                re.ConfigureConsumer<CancelOrderConsumerV1>(context);
                re.UseMessageRetry(r =>
                {
                    r.Ignore(typeof(ArgumentNullException), typeof(JsonSerializationException));
                    r.Interval(3, 5000);
                });
            }
        );
    }

    public static void AddCompleteOrderEndpointV1(
       this IRabbitMqBusFactoryConfigurator cfg,
       IBusRegistrationContext context)
    {
        cfg.ReceiveEndpoint(
            $"{nameof(ReservationConfirmedV1).Underscore()}",
            re =>
            {
                re.ConfigureConsumeTopology = true;
                //re.SetQuorumQueue();

                re.Bind(
                    $"{nameof(ReservationConfirmedV1).Underscore()}.input_exchange",
                    e =>
                    {
                        e.RoutingKey = $"{nameof(ReservationConfirmedV1).Underscore()}";
                        e.ExchangeType = ExchangeType.Fanout;
                    }
                );

                re.ConfigureConsumer<CompleteOrderConsumerV1>(context);
                re.UseMessageRetry(r =>
                {
                    r.Ignore(typeof(ArgumentNullException), typeof(JsonSerializationException));
                    r.Interval(3, 5000);
                });
            }
        );
    }
}
