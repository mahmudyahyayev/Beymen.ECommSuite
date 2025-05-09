using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Caching;
using BuildingBlocks.Caching.Options;
using BuildingBlocks.Core.Reflection;
using BuildingBlocks.Core.Web.Extenions;
using EasyCaching.Core;
using EasyCaching.Core.Configurations;
using EasyCaching.CSRedis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Caching.Extensions
{
    public static class RegistrationExtensions
    {
        private static string serializerName = "Json";
        public static WebApplicationBuilder AddCustomCaching(
            this WebApplicationBuilder builder,
            params Assembly[] scanAssemblies)
        {
            var cacheOptions = builder.Configuration.BindOptions<CacheOptions>();

            builder.Services
                  .AddOptions<CacheOptions>()
                  .Bind(builder.Configuration.GetSection(nameof(CacheOptions)))
                  .ValidateDataAnnotations();

            Guard.Against.Null(cacheOptions);

            AddCachingRequests(builder.Services, scanAssemblies);

            builder.Services.AddEasyCaching(option =>
            {
                option.UseRedis(
                          config =>
                          {
                              foreach (var endpoint in cacheOptions.RedisWriteOptions.RedisOptions.Endpoints)
                              {
                                  config.DBConfig.Endpoints.Add(new ServerEndPoint(endpoint.Host, endpoint.Port));
                              }

                              config.DBConfig.Password = cacheOptions.RedisWriteOptions.RedisOptions.Password;
                              config.SerializerName = serializerName;
                              config.DBConfig.SyncTimeout = 15000;
                          },
                          cacheOptions.RedisWriteOptions.RedisOptions.ProviderName
                      );

                if (cacheOptions.RedisReadOnlyOptions is not null)
                {
                    option.UseRedis(
                             config =>
                             {
                                 foreach (var endpoint in cacheOptions.RedisReadOnlyOptions.RedisOptions.Endpoints)
                                 {
                                     config.DBConfig.Endpoints.Add(new ServerEndPoint(endpoint.Host, endpoint.Port));
                                 }

                                 config.DBConfig.Password = cacheOptions.RedisReadOnlyOptions.RedisOptions.Password;
                                 config.SerializerName = serializerName;
                                 config.DBConfig.SyncTimeout = 15000;
                             },
                             cacheOptions.RedisReadOnlyOptions.RedisOptions.ProviderName
                         );

                    option.UseCSRedis(
                             config =>
                             {
                                 var connectionStrings = new List<string>();

                                 foreach (var endpoint in cacheOptions.RedisReadOnlyOptions.RedisOptions.Endpoints)
                                 {
                                     connectionStrings.Add($"{endpoint.Host}:{endpoint.Port},password={cacheOptions.RedisReadOnlyOptions.RedisOptions.Password}");
                                 }

                                 config.DBConfig = new CSRedisDBOptions
                                 {
                                     ConnectionStrings = connectionStrings,
                                     ReadOnly = true
                                 };
                                 config.SerializerName = serializerName;

                             },
                             cacheOptions.RedisReadOnlyOptions.RedisOptions.ProviderName
                         );
                }

                option.WithJson(
                         jsonSerializerSettingsConfigure: x =>
                         {
                             x.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;
                         },
                         serializerName
                     );
            });

            builder.Services.AddSingleton(typeof(RedisCache));
            return builder;
        }

        private static IServiceCollection AddCachingRequests(
            this IServiceCollection services,
            params Assembly[] scanAssemblies)
        {
            var assemblies = scanAssemblies.Any()
                ? scanAssemblies
                : ReflectionUtilities.GetReferencedAssemblies(Assembly.GetCallingAssembly()).ToArray();

            services.Scan(
                scan =>
                    scan.FromAssemblies(assemblies)
                        .AddClasses(classes => classes.AssignableTo(typeof(ICacheRequest<,>)), false)
                        .AsImplementedInterfaces()
                        .WithTransientLifetime());

            services.Scan(
                scan =>
                    scan.FromAssemblies(assemblies)
                        .AddClasses(classes => classes.AssignableTo(typeof(IInvalidateCacheRequest<,>)), false)
                        .AsImplementedInterfaces()
                        .WithTransientLifetime());

            services.Scan(
                scan =>
                    scan.FromAssemblies(assemblies)
                        .AddClasses(classes => classes.AssignableTo(typeof(IReadOnlyHashCacheRequest<,>)), false)
                        .AsImplementedInterfaces()
                        .WithTransientLifetime());

            services.Scan(
                scan =>
                    scan.FromAssemblies(assemblies)
                        .AddClasses(classes => classes.AssignableTo(typeof(IReadOnlyCacheRequest<,>)), false)
                        .AsImplementedInterfaces()
                        .WithTransientLifetime());

            services.Scan(
                scan =>
                    scan.FromAssemblies(assemblies)
                        .AddClasses(classes => classes.AssignableTo(typeof(IWriteOnlyCacheRequest<,>)), false)
                        .AsImplementedInterfaces()
                        .WithTransientLifetime());

            return services;
        }
    }
}
