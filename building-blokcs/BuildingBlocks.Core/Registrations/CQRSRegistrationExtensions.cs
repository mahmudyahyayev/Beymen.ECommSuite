using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Events;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using BuildingBlocks.Abstractions.CQRS.Queries;
using BuildingBlocks.Abstractions.Scheduler;
using BuildingBlocks.Core.CQRS.Commands;
using BuildingBlocks.Core.CQRS.Events;
using BuildingBlocks.Core.CQRS.Queries;
using BuildingBlocks.Core.Scheduler;
using BuildingBlocks.Core.Web.Extensions.ServiceCollection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Core.Registrations
{
    public static class CQRSRegistrationExtensions
    {
        public static IServiceCollection AddCqrs(
            this IServiceCollection services,
            Assembly[]? assemblies = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient,
            params Type[] pipelines
        )
        {
            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssemblies(assemblies ?? new[] { Assembly.GetCallingAssembly() });
                x.Lifetime = serviceLifetime;
            });


            //services.AddMediatR(
            //    assemblies ?? new[] { Assembly.GetCallingAssembly() },
            //    x =>
            //    {
            //        switch (serviceLifetime)
            //        {
            //            case ServiceLifetime.Transient:
            //                x.AsTransient();
            //                break;
            //            case ServiceLifetime.Scoped:
            //                x.AsScoped();
            //                break;
            //            case ServiceLifetime.Singleton:
            //                x.AsSingleton();
            //                break;
            //        }
            //    }
            //);

            foreach (var pipeline in pipelines)
                services.AddScoped(typeof(IPipelineBehavior<,>), pipeline);

            services
                .Add<ICommandProcessor, CommandProcessor>(serviceLifetime)
                .Add<IQueryProcessor, QueryProcessor>(serviceLifetime)
                .Add<IEventProcessor, EventProcessor>(serviceLifetime)
                .Add<ICommandScheduler, NullCommandScheduler>(serviceLifetime)
                .Add<IDomainEventPublisher, DomainEventPublisher>(serviceLifetime)
                .Add<IDomainNotificationEventPublisher, DomainNotificationEventPublisher>(serviceLifetime);

            services.AddScoped<IDomainEventsAccessor, NullDomainEventsAccessor>();

            return services;
        }
    }
}
