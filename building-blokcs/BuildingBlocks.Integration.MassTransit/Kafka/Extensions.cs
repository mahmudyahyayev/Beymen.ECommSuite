using System.Reflection;
using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Reflection;
using BuildingBlocks.Core.Validations;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IBus = BuildingBlocks.Abstractions.Messaging.IBus;

namespace BuildingBlocks.Integration.MassTransit.Kafka;

public static class Extensions
{
    public static KafkaBuilder AddKafkaBuilder(this IServiceCollection services, Action<KafkaOptions> kafkaOptions)
    {
        var options = new KafkaOptions();
        kafkaOptions.Invoke(options);

        Guard.Against.NullOrEmpty(options.Host);
        //Guard.Against.NullOrEmpty(options.Username);
        //Guard.Against.NullOrEmpty(options.Password);

        services.AddTransient<IBus, MassTransitBus>();
        
        return new KafkaBuilder(services, options);
    }
    
    private static IRetryConfigurator AddRetryConfiguration(IRetryConfigurator retryConfigurator)
    {
        retryConfigurator
            .Exponential(3, TimeSpan.FromMilliseconds(200), TimeSpan.FromMinutes(120), TimeSpan.FromMilliseconds(200))
            .Ignore<ValidationException>();

        return retryConfigurator;
    }
}
