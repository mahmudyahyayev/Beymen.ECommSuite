using Beymen.ECommSuite.Shared.Events.Integration.v1;
using Humanizer;
using MassTransit;

namespace Customer.Infrastructure.Publishers;

public static class CustomerPublisher
{
    public static void AddCreateCustomerPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<CustomerCreatedV1>(
            e => e.SetEntityName($"{nameof(CustomerCreatedV1).Underscore()}.input_exchange")
        );

        cfg.Publish<CustomerCreatedV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<CustomerCreatedV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }

    public static void AddChangeCustomerMainAttributesPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<CustomerMainAttributesChangedV1>(
            e => e.SetEntityName($"{nameof(CustomerMainAttributesChangedV1).Underscore()}.input_exchange")
        );

        cfg.Publish<CustomerMainAttributesChangedV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<CustomerMainAttributesChangedV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }

    public static void AddActivatedCustomerPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<CustomerActivatedV1>(
            e => e.SetEntityName($"{nameof(CustomerActivatedV1).Underscore()}.input_exchange")
        );

        cfg.Publish<CustomerActivatedV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<CustomerActivatedV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }

    public static void AddDeactivatedCustomerPublishV1(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Message<CustomerDeactivatedV1>(
            e => e.SetEntityName($"{nameof(CustomerDeactivatedV1).Underscore()}.input_exchange")
        );

        cfg.Publish<CustomerDeactivatedV1>(e => e.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout);

        cfg.Send<CustomerDeactivatedV1>(e =>
        {
            e.UseRoutingKeyFormatter(context => context.Message.GetType().Name.Underscore());
        });
    }
}

