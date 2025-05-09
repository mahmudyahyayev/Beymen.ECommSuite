using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Events;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Persistence.Efcore;
using BuildingBlocks.Core.Persistence.Efcore;
using BuildingBlocks.Core.Persistence.EfCore;
using BuildingBlocks.Core.Persistence.EfCore.Interceptors;
using BuildingBlocks.Core.Web.Extensions.ServiceCollection;
using Core.Persistence.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Persistence.EfCore.Postgres
{

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPostgresDbContext<TDbContext, TUserTypeId, TAudit>(
            this IServiceCollection services,
            Assembly? migrationAssembly = null,
            Action<DbContextOptionsBuilder>? builder = null,
            string migrationDefaultSchema = null,
            Type stronglyTypedId = null)
            where TDbContext : DbContext, IDbFacadeResolver, IDomainEventContext
        {
            stronglyTypedId ??= typeof(Guid);

            if (string.IsNullOrWhiteSpace(migrationDefaultSchema))
            {
                migrationDefaultSchema = "public";
            }

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddValidatedOptions<PostgresOptions>(nameof(PostgresOptions));

            services.AddScoped<IConnectionFactory>(sp =>
            {
                var postgresOptions = sp.GetService<PostgresOptions>();
                Guard.Against.NullOrEmpty(postgresOptions?.ConnectionString);
                return new NpgsqlConnectionFactory(postgresOptions.ConnectionString);
            });

            services.AddDbContext<TDbContext>(
                (sp, options) =>
                {
                    var postgresOptions = sp.GetRequiredService<PostgresOptions>();

                    options
                        .UseNpgsql(
                            postgresOptions.ConnectionString,
                            sqlOptions =>
                            {
                                var name =
                                    migrationAssembly?.GetName().Name
                                    ?? postgresOptions.MigrationAssembly
                                    ?? typeof(TDbContext).Assembly.GetName().Name;

                                sqlOptions.MigrationsAssembly(name);
                                sqlOptions.MigrationsHistoryTable(
                                    $"efcore_{migrationDefaultSchema}_migration_history", migrationDefaultSchema);
                                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                            }
                        )
                        .UseSnakeCaseNamingConvention();

                    if (stronglyTypedId == typeof(Guid))
                    {
                        options.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector<Guid>>();
                    }
                    if (stronglyTypedId == typeof(long))
                    {
                        options.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector<long>>();
                    }
                    if (stronglyTypedId == typeof(string))
                    {
                        options.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector<string>>();
                    }
                    if (stronglyTypedId == typeof(int))
                    {
                        options.ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector<int>>();
                    }
                    options.AddInterceptors(
                        new AuditInterceptor<TUserTypeId, TAudit>(sp.GetService<ICurrentUser<TUserTypeId, TAudit>>()),
                        new SoftDeleteInterceptor<TUserTypeId, TAudit>(sp.GetService<ICurrentUser<TUserTypeId, TAudit>>()),
                        new ConcurrencyInterceptor()
                    );

                    builder?.Invoke(options);
                }
            );

            services.AddScoped<IDbFacadeResolver>(provider => provider.GetService<TDbContext>()!);
            services.AddScoped<IDomainEventContext>(provider => provider.GetService<TDbContext>()!);
            services.AddScoped<IDomainEventsAccessor, EfDomainEventAccessor>();

            return services;
        }

        public static IServiceCollection AddUnitOfWork<TContext>(
            this IServiceCollection services,
            ServiceLifetime lifeTime = ServiceLifetime.Scoped,
            bool registerGeneric = false)
            where TContext : EfDbContextBase
        {
            if (registerGeneric)
            {
                services.RegisterService<IEfUnitOfWork, EfUnitOfWork<TContext>>(lifeTime);
            }

            return services.RegisterService<IEfUnitOfWork<TContext>, EfUnitOfWork<TContext>>(lifeTime);
        }

        public static IServiceCollection AddPostgresCustomRepository<TEntity, TKey, TAudit, TIRepository, TRepository>(
            this IServiceCollection services,
            ServiceLifetime lifeTime = ServiceLifetime.Scoped)
            where TEntity : class, IAggregate<TKey, TAudit>
            where TIRepository : class, IRepository<TEntity, TKey, TAudit>
            where TRepository : class, TIRepository
                => services.RegisterService<TIRepository, TRepository>(lifeTime);


        private static IServiceCollection RegisterService<TService, TImplementation>(
            this IServiceCollection services,
            ServiceLifetime lifeTime = ServiceLifetime.Scoped)
            where TService : class
            where TImplementation : class, TService
        {
            ServiceDescriptor serviceDescriptor = lifeTime switch
            {
                ServiceLifetime.Singleton => ServiceDescriptor.Singleton<TService, TImplementation>(),
                ServiceLifetime.Scoped => ServiceDescriptor.Scoped<TService, TImplementation>(),
                ServiceLifetime.Transient => ServiceDescriptor.Transient<TService, TImplementation>(),
                _ => throw new ArgumentOutOfRangeException(nameof(lifeTime), lifeTime, null)
            };
            services.Add(serviceDescriptor);
            return services;
        }
    }
}
