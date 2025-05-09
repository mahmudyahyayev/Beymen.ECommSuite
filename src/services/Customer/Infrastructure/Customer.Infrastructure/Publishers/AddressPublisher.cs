using Beymen.ECommSuite.Shared.Events.Integration.v1;
using Humanizer;
using MassTransit;
namespace Customer.Infrastructure.Publishers;

public static class AddressPublisher
{
    public static void AddCreateAddressPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<AddressCreatedV1>(
            e => e.SetEntityName($"{nameof(AddressCreatedV1).Underscore()}.input_exchange")
        );

        cfg.Publish<AddressCreatedV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<AddressCreatedV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }

    public static void AddChangeAddressMainAttributesPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<AddressMainAttributesChangedV1>(
            e => e.SetEntityName($"{nameof(AddressMainAttributesChangedV1).Underscore()}.input_exchange")
        );

        cfg.Publish<AddressMainAttributesChangedV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<AddressMainAttributesChangedV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }

    public static void AddActivatedAddressPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<AddressActivatedV1>(
            e => e.SetEntityName($"{nameof(AddressActivatedV1).Underscore()}.input_exchange")
        );

        cfg.Publish<AddressActivatedV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<AddressActivatedV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }

    public static void AddDeactivatedAddressPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<AddressDeactivatedV1>(
            e => e.SetEntityName($"{nameof(AddressDeactivatedV1).Underscore()}.input_exchange")
        );

        cfg.Publish<AddressDeactivatedV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<AddressDeactivatedV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }

    public static void AddAddressTypeChangedPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<AddressTypeChangedV1>(
            e => e.SetEntityName($"{nameof(AddressTypeChangedV1).Underscore()}.input_exchange")
        );

        cfg.Publish<AddressTypeChangedV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<AddressTypeChangedV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }
}
