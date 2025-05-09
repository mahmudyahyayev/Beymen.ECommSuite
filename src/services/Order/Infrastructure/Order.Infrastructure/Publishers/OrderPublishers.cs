using Beymen.ECommSuite.Shared.Events.Integration.v1;
using Humanizer;
using MassTransit;

namespace Order.Infrastructure.Publishers;

public static class OrderPublishers
{
    public static void AddCreateOrderPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<OrderCreatedV1>(
            e => e.SetEntityName($"{nameof(OrderCreatedV1).Underscore()}.input_exchange")
        );

        cfg.Publish<OrderCreatedV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<OrderCreatedV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }

    public static void AddPaidOrderPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<OrderPaidV1>(
            e => e.SetEntityName($"{nameof(OrderPaidV1).Underscore()}.input_exchange")
        );

        cfg.Publish<OrderPaidV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<OrderPaidV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }

    public static void AddCanceledOrderPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<OrderCancelledV1>(
            e => e.SetEntityName($"{nameof(OrderCancelledV1).Underscore()}.input_exchange")
        );

        cfg.Publish<OrderCancelledV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<OrderCancelledV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }
}

