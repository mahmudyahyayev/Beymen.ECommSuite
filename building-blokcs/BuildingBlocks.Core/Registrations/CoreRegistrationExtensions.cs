using BuildingBlocks.Abstractions.Correlation;
using BuildingBlocks.Abstractions.CQRS.Events;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Abstractions.Messaging.PersistMessage.Partition;
using BuildingBlocks.Abstractions.Serialization;
using BuildingBlocks.Abstractions.Types;
using BuildingBlocks.Core.Correlation;
using BuildingBlocks.Core.CQRS.Events;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging.BackgroundServices;
using BuildingBlocks.Core.Messaging.MessagePersistence;
using BuildingBlocks.Core.Reflection;
using BuildingBlocks.Core.Serialization;
using BuildingBlocks.Core.Types;
using BuildingBlocks.Core.Web.Extensions.ServiceCollection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace BuildingBlocks.Core.Registrations
{
    public static class CoreRegistrationExtensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration, params Assembly[] scanAssemblies)
        {
            var systemInfo = MachineInstanceInfo.New();
            var assemblies = scanAssemblies.Any()
                ? scanAssemblies
                : ReflectionUtilities.GetReferencedAssemblies(Assembly.GetCallingAssembly()).Distinct().ToArray();

            services.AddSingleton<IMachineInstanceInfo>(systemInfo);
            services.AddSingleton(systemInfo);

            services.AddSingleton<ICorrelationService, CorrelationService>();

            services.AddTransient<IAggregatesDomainEventsRequestStore, AggregatesDomainEventsStore>();

            services.AddHttpContextAccessor();

            AddDefaultSerializer(services);

            AddMessagingCore(services, configuration, assemblies);

            RegisterEventMappers(services, assemblies);

            RegisterCommandMappers(services, assemblies);

            return services;
        }

        private static void RegisterEventMappers(IServiceCollection services, Assembly[] scanAssemblies)
        {
            services.Scan(
                scan =>
                    scan.FromAssemblies(scanAssemblies)
                        .AddClasses(classes => classes.AssignableTo(typeof(IEventMapper)), false)
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime()
                        .AddClasses(classes => classes.AssignableTo(typeof(IIntegrationEventMapper)), false)
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime()
                        .AddClasses(classes => classes.AssignableTo(typeof(IIDomainNotificationEventMapper)), false)
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime()
            );
        }

        private static void RegisterCommandMappers(IServiceCollection services, Assembly[] scanAssemblies)
        {
            services.Scan(
                scan =>
                    scan.FromAssemblies(scanAssemblies)
                        .AddClasses(classes => classes.AssignableTo(typeof(ICommandMapper)), false)
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime()
                        .AddClasses(classes => classes.AssignableTo(typeof(IMessageMapper)), false)
                        .AsImplementedInterfaces()
                        .WithSingletonLifetime()
            );
        }

        private static void AddMessagingCore(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly[] scanAssemblies,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            AddMessagingMediator(services, serviceLifetime, scanAssemblies);

            AddPersistenceMessage(services, configuration);
        }

        private static void AddPersistenceMessage(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMessagePersistenceService, MessagePersistenceService>();

            var messagePersistenceOptions = new MessagePersistenceOptions();
            configuration.GetSection("MessagePersistenceOptions").Bind(messagePersistenceOptions);

            services
                .AddOptions<MessagePersistenceOptions>()
                .Bind(configuration.GetSection(nameof(MessagePersistenceOptions)))
                .ValidateDataAnnotations();

            if (messagePersistenceOptions.EnableBackgroundService)
            {
                services.AddHostedService<MessagePersistenceBackgroundService>();
            }
        }

        private static void AddMessagingMediator(
            IServiceCollection services,
            ServiceLifetime serviceLifetime,
            Assembly[] scanAssemblies)
        {
            services.Scan(
                scan =>
                    scan.FromAssemblies(scanAssemblies)
                        .AddClasses(classes => classes.AssignableTo(typeof(IMessageHandler<>)))
                        .UsingRegistrationStrategy(RegistrationStrategy.Append)
                        .AsClosedTypeOf(typeof(IMessageHandler<>))
                        .AsSelf()
                        .WithLifetime(serviceLifetime)
            );
        }

        private static void AddDefaultSerializer(
            IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            services.Add<ISerializer, DefaultSerializer>(lifetime);
            services.Add<IMessageSerializer, DefaultMessageSerializer>(lifetime);
        }
    }
}
