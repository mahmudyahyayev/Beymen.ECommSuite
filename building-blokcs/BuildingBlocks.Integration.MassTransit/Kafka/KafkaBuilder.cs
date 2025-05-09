using System.Reflection;
using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Messaging;
using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Integration.MassTransit.Kafka;

public class KafkaBuilder
{
    List<ProducerHolder> _producers;
    List<ConsumerHolder> _consumers;
    IServiceCollection _services;
    KafkaOptions _kafkaOptions;

    public KafkaBuilder(IServiceCollection services, KafkaOptions kafkaOptions)
    {
        _producers = new List<ProducerHolder>();
        _consumers = new List<ConsumerHolder>();
        _services = services;
        _kafkaOptions = kafkaOptions;
    }

    public KafkaBuilder AddProducer<T>(string topicName, ProducerConfig? producerConfig = null) where T : class
    {
        _producers.Add(new ProducerHolder()
        {
            ProducerType = typeof(T), Topic = topicName, ProducerConfig = producerConfig
        });
        return this;
    }

    public KafkaBuilder AddConsumer<T>(string topicName, string groupId,
        Action<IKafkaTopicReceiveEndpointConfigurator>? configure = null) where T : IConsumer
    {
        _consumers.Add(new ConsumerHolder()
        {
            ConsumerType = typeof(T), Topic = topicName, GroupId = groupId, Configure = configure
        });
        return this;
    }

    public KafkaBuilder AddConsumer<T>(string topicName, ConsumerConfig? consumerConfig = null,
        Action<IKafkaTopicReceiveEndpointConfigurator>? configure = null) where T : IConsumer
    {
        _consumers.Add(new ConsumerHolder()
        {
            ConsumerType = typeof(T), Topic = topicName, ConsumerConfig = consumerConfig, Configure = configure
        });
        return this;
    }

    public IServiceCollection BuildKafkaServiceBus(LogLevel logLevel = LogLevel.Trace)
    {
        _services.AddMassTransit<IKafkaBus>(c =>
        {
            c.AddLogging(a => a.SetMinimumLevel(logLevel));
            c.UsingInMemory();

            c.AddRider(rider =>
            {
                AddProducers(rider);
                foreach (var consumer in _consumers)
                {
                    var entryAssembly = Assembly.GetEntryAssembly();
                    var referencedAssemblies = entryAssembly.GetReferencedAssemblies().Select(Assembly.Load);
                    var assemblies = new List<Assembly> { entryAssembly }.Concat(referencedAssemblies);
                    _services.Scan(a =>
                    {
                        a.FromAssemblies(assemblies)
                            .AddClasses(a => a.AssignableTo(typeof(IFaultConsumer<>)))
                            .AsImplementedInterfaces()
                            .WithScopedLifetime();
                    });

                    var consumerMessageType = consumer.ConsumerType.GetInterfaces().Where(p => p.IsGenericType == true)
                        .Select(s => s.GetGenericArguments().FirstOrDefault()).FirstOrDefault();

                    Guard.Against.Null(consumerMessageType);

                    _services.AddScoped(typeof(GenericFaultConsumer<>).MakeGenericType(consumerMessageType));

                    rider.AddConsumer(consumer.ConsumerType);
                }

                rider.UsingKafka((context, kfc) =>
                {
                    kfc.Host(_kafkaOptions.Host, a =>
                    {
                        if (_kafkaOptions.SaslEnable)
                        {
                            a.UseSasl(s =>
                            {
                                s.Username = _kafkaOptions.Username;
                                s.Password = _kafkaOptions.Password;
                                s.Mechanism = SaslMechanism.Plain;
                                s.SecurityProtocol = SecurityProtocol.SaslSsl;
                            });
                        }
                    });

                    kfc.ClientId = _kafkaOptions.ClientId;

                    AddConsumers(kfc, context);
                });
            });
        });

        return _services;
    }

    private void AddProducers(IRiderRegistrationConfigurator rider)
    {
        var producerMethod = typeof(KafkaProducerRegistrationExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .FirstOrDefault(mi => mi.Name == "AddProducer" && mi.IsGenericMethod == true &&
                                  mi.GetParameters().Count() == 3 &&
                                  mi.GetParameters()[0].Name == "configurator" &&
                                  mi.GetParameters()[1].Name == "topicName"
                                  && mi.GetParameters()[2].Name == "configure" &&
                                  mi.GetGenericArguments().Count() == 2);

        var producerMethodWithConfiguration = typeof(KafkaProducerRegistrationExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .FirstOrDefault(mi => mi.Name == "AddProducer" && mi.IsGenericMethod == true &&
                                  mi.GetParameters().Count() == 4 &&
                                  mi.GetParameters()[0].Name == "configurator" &&
                                  mi.GetParameters()[1].Name == "topicName"
                                  && mi.GetParameters()[2].Name == "producerConfig" &&
                                  mi.GetParameters()[3].Name == "configure" && mi.GetGenericArguments().Count() == 2);

        foreach (var producer in _producers)
        {
            MethodInfo generic = producerMethod.MakeGenericMethod(typeof(string), producer.ProducerType);
            MethodInfo genericConfig =
                producerMethodWithConfiguration.MakeGenericMethod(typeof(string), producer.ProducerType);
            if (producer.ProducerConfig != null)
                genericConfig.Invoke(rider, new object[] { rider, producer.Topic, producer.ProducerConfig, null });
            else
                generic.Invoke(rider, new object[] { rider, producer.Topic, null });
        }
    }

    private void AddConsumers(IKafkaFactoryConfigurator kafkaFactoryConfigurator, IRiderRegistrationContext context)
    {
        var topicEndpointMethod = typeof(KafkaConfiguratorExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .FirstOrDefault(mi => mi.Name == "TopicEndpoint" && mi.IsGenericMethod == true &&
                                  mi.GetParameters().Count() == 4 &&
                                  mi.GetParameters()[0].Name == "configurator" &&
                                  mi.GetParameters()[1].Name == "topicName"
                                  && mi.GetParameters()[2].Name == "groupId" &&
                                  mi.GetParameters()[3].Name == "configure" &&
                                  mi.GetGenericArguments().Count() == 1);

        var topicEndpointConsumerConfigMethod = typeof(KafkaConfiguratorExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .FirstOrDefault(mi => mi.Name == "TopicEndpoint" && mi.IsGenericMethod == true &&
                                  mi.GetParameters().Count() == 4 &&
                                  mi.GetParameters()[0].Name == "configurator" &&
                                  mi.GetParameters()[1].Name == "topicName"
                                  && mi.GetParameters()[2].Name == "consumerConfig" &&
                                  mi.GetParameters()[3].Name == "configure" && mi.GetGenericArguments().Count() == 1);

        Guard.Against.Null(topicEndpointMethod);
        Guard.Against.Null(topicEndpointConsumerConfigMethod);


        foreach (var consumer in _consumers)
        {
            var consumerMessageType = consumer.ConsumerType.GetInterfaces().Where(p => p.IsGenericType == true)
                .Select(s => s.GetGenericArguments().FirstOrDefault()).FirstOrDefault();

            Guard.Against.Null(consumerMessageType);

            consumer.Configure += a => a.ConfigureConsumer(context, consumer.ConsumerType);

            consumer.Configure += a => a.ConfigureError(x =>
                x.UseFilter(
                    (IFilter<ExceptionReceiveContext>)context.GetService(
                        typeof(GenericFaultConsumer<>).MakeGenericType(consumerMessageType))));

            if (consumer.ConsumerConfig != null)
            {
                MethodInfo generic = topicEndpointConsumerConfigMethod.MakeGenericMethod(consumerMessageType);

                generic.Invoke(kafkaFactoryConfigurator,
                    new object[]
                    {
                        kafkaFactoryConfigurator, consumer.Topic, consumer.ConsumerConfig, consumer.Configure
                    });
            }
            else
            {
                MethodInfo generic = topicEndpointMethod.MakeGenericMethod(consumerMessageType);

                generic.Invoke(kafkaFactoryConfigurator,
                    new object[] { kafkaFactoryConfigurator, consumer.Topic, consumer.GroupId, consumer.Configure });
            }
        }
    }
}
