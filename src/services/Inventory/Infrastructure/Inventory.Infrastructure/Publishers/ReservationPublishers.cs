using Beymen.ECommSuite.Shared.Events.Integration.v1;
using Humanizer;
using MassTransit;

namespace Inventory.Infrastructure.Publishers;
public static class ReservationPublishers
{
    public static void AddReservationCanceledPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<ReservationCanceledV1>(
            e => e.SetEntityName($"{nameof(ReservationCanceledV1).Underscore()}.input_exchange")
        );

        cfg.Publish<ReservationCanceledV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<ReservationCanceledV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }

    public static void AddReservationConfirmedPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<ReservationConfirmedV1>(
            e => e.SetEntityName($"{nameof(ReservationConfirmedV1).Underscore()}.input_exchange")
        );

        cfg.Publish<ReservationConfirmedV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<ReservationConfirmedV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }
}
