using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Reflection;
using BuildingBlocks.Core.Validations;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using IBus = BuildingBlocks.Abstractions.Messaging.IBus;

namespace BuildingBlocks.Integration.MassTransit
{
    public static class Extensions
    {
        public static WebApplicationBuilder AddCustomMassTransit(
            this WebApplicationBuilder builder,
            Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? configureReceiveEndpoints = null,
            Action<IBusRegistrationConfigurator>? configureBusRegistration = null,
            bool autoConfigEndpoints = false,
            params Assembly[] scanAssemblies)
        {
            var assemblies = scanAssemblies.Any()
                ? scanAssemblies
                : ReflectionUtilities.GetReferencedAssemblies(Assembly.GetCallingAssembly()).ToArray();

             builder.Services.AddMassTransit(ConfiguratorAction);

            void ConfiguratorAction(IBusRegistrationConfigurator busRegistrationConfigurator)
            {
                configureBusRegistration?.Invoke(busRegistrationConfigurator);
                busRegistrationConfigurator.AddConsumers(assemblies);
                busRegistrationConfigurator.SetEndpointNameFormatter(new SnakeCaseEndpointNameFormatter(false));
                busRegistrationConfigurator.UsingRabbitMq(
                    (context, cfg) =>
                    {
                        cfg.PublishTopology.BrokerTopologyOptions = PublishBrokerTopologyOptions.FlattenHierarchy;

                        if (autoConfigEndpoints)
                        {
                            cfg.ConfigureEndpoints(context);
                        }


                        IConfigurationSection sectionData = builder.Configuration.GetSection("RabbitMqOptions");
                        var rabbitMqOptions = new RabbitMqOptions();
                        sectionData.Bind(rabbitMqOptions);

                        cfg.Host(
                            rabbitMqOptions.Host,
                            rabbitMqOptions.Port,
                            rabbitMqOptions.VirtualHost,
                            hostConfigurator =>
                            {
                                hostConfigurator.Username(rabbitMqOptions.UserName);
                                hostConfigurator.Password(rabbitMqOptions.Password);
                            }
                        );

                        cfg.UseMessageRetry(r => AddRetryConfiguration(r));
                        cfg.Publish<IIntegrationEvent>(p => p.Exclude = true);
                        cfg.Publish<IntegrationEvent>(p => p.Exclude = true);
                        cfg.Publish<IMessage>(p => p.Exclude = true);
                        cfg.Publish<Message>(p => p.Exclude = true);
                        cfg.Publish<ITxRequest>(p => p.Exclude = true);
                        cfg.MessageTopology.SetEntityNameFormatter(new CustomEntityNameFormatter());
                        configureReceiveEndpoints?.Invoke(context, cfg);
                    }
                );
            }

            builder.Services.AddTransient<IBus, MassTransitBus>();

            return builder;
        }

        private static IRetryConfigurator AddRetryConfiguration(IRetryConfigurator retryConfigurator)
        {
            retryConfigurator
                .Exponential(3, TimeSpan.FromMilliseconds(200), TimeSpan.FromMinutes(120), TimeSpan.FromMilliseconds(200))
                .Ignore<ValidationException>();

            return retryConfigurator;
        }
    }
}
