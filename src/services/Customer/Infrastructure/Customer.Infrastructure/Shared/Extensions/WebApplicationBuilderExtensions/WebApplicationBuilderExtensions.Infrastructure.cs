using System.Reflection;
using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Abstractions.Messaging.PersistMessage.Partition;
using BuildingBlocks.Abstractions.Template.Render;
using BuildingBlocks.Core.Messaging.MessagePersistence;
using BuildingBlocks.Core.Persistence.EfCore;
using BuildingBlocks.Core.Registrations;
using BuildingBlocks.Core.Template.Render;
using BuildingBlocks.Core.Validations;
using BuildingBlocks.Core.Web.Extensions;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.Integration.MassTransit;
using BuildingBlocks.Logging.Extensions;
using BuildingBlocks.Messaging.Persistence.Postgres.Extensions;
using BuildingBlocks.Persistence.EfCore.Postgres;
using BuildingBlocks.Swagger;
using BuildingBlocks.Web.Extensions;
using Customer.Application;
using Customer.Infrastructure.BackgroundServices;
using Customer.Infrastructure.Publishers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Customer.Infrastructure.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        Assembly[] assemblies =
        {
            typeof(InfrastructureAssemblyInfo).Assembly,
            typeof(ApplicationAssemblyInfo).Assembly
        };

        builder.Services.AddCore(builder.Configuration);

        DotNetEnv.Env.TraversePath().Load();

        if (builder.Environment.IsTest() == false)
        {
            var postgresOptions = new PostgresOptions();
            builder.Configuration.GetSection("PostgresOptions")
                .Bind(postgresOptions);

            var messagePostgresOptions = new MessagePersistenceOptions();
            builder.Configuration.GetSection("MessagePersistenceOptions")
                .Bind(messagePostgresOptions);

            var rabbitMqOptions = new RabbitMqOptions();
            builder.Configuration.GetSection("RabbitMqOptions")
                .Bind(rabbitMqOptions);

            var healthCheckBuilder = builder.Services
                .AddAliveHealthCheck()
                .AddPostgreSqlReadyHealthCheck(postgresOptions.ConnectionString, "Customer-Postgres-Check")
                .AddPostgreSqlReadyHealthCheck(messagePostgresOptions.ConnectionString,
                    "Customer-Messaging-Postgres-Check")
                .AddRabbitMqReadyHealthCheck(rabbitMqOptions.ConnectionString, rabbitMqOptions.VirtualHost,
                    "Customer-RabbitMQ-Check");
        }

        builder.Services.AddScoped<ICurrentUser<long, string>, CurrentApplicationUser>();

        builder.AddCustomProblemDetails();

        builder.AddCustomVersioning();

        builder.AddCustomSwagger(typeof(InfrastructureAssemblyInfo).Assembly);

        builder.AddCustomSerilog(extraConfigure: u =>
        {
        });

        builder.Services.TryAddSingleton<IRenderService, RenderService>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddCqrs(
            assemblies: assemblies,
            serviceLifetime: ServiceLifetime.Transient,
            pipelines: new[]
            {
                typeof(RequestValidationBehavior<,>),
                typeof(EfTxBehavior<,>)
            }
        );
        builder.Services.AddPostgresMessagePersistence(builder.Configuration);

        builder.AddCustomMassTransit((context, cfg) =>
        {
            cfg.AddCreateAddressPublishV1();
            cfg.AddChangeAddressMainAttributesPublishV1();
            cfg.AddActivatedAddressPublishV1();
            cfg.AddDeactivatedAddressPublishV1();
            cfg.AddAddressTypeChangedPublishV1();
            cfg.AddCreateCustomerPublishV1();
            cfg.AddChangeCustomerMainAttributesPublishV1();
            cfg.AddActivatedCustomerPublishV1();
            cfg.AddDeactivatedCustomerPublishV1();
        });

        builder.Services.AddCustomValidators(assemblies);
        builder.Services.AddAutoMapper(assemblies);
        builder.AddRateLimiter();

        builder.Services.AddHostedService<RunningPodAndPartitionAssignmentBackgroundService<PodPartition, RunningPod>>();
        builder.Services.AddHostedService<RunningPodAndPartitionCheckBackgroundService<PodPartition, RunningPod>>();

        return builder;
    }
}
