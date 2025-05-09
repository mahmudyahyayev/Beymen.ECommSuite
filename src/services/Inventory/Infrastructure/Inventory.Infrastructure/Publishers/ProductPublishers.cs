using Beymen.ECommSuite.Shared.Events.Integration.v1;
using Humanizer;
using MassTransit;

namespace Inventory.Infrastructure.Publishers;

public static class ProductPublishers
{
    public static void AddCreateProductPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<ProductCreatedV1>(
            e => e.SetEntityName($"{nameof(ProductCreatedV1).Underscore()}.input_exchange")
        );

        cfg.Publish<ProductCreatedV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<ProductCreatedV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }
}

