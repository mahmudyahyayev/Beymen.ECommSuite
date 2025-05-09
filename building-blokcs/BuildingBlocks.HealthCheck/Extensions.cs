using System.Net;
using System.Net.Http.Headers;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System.Net.Mime;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Elasticsearch.Net;
using RabbitMQ.Client;

namespace BuildingBlocks.HealthCheck
{
    public static class Extensions
    {
        public static IHealthChecksBuilder AddAliveHealthCheck(
            this IServiceCollection serviceCollection) =>
            serviceCollection
                .AddHealthChecks()
                .AddCheck(
                    name: "Alive",
                    check: () => HealthCheckResult.Healthy("Healthy."),
                    tags: new[] { "alive" }
                );

        public static IHealthChecksBuilder AddPostgreSqlReadyHealthCheck(
            this IHealthChecksBuilder healthChecksBuilder,
            string npgsqlConnectionString,
            string name) =>
            healthChecksBuilder
                .AddNpgSql(
                    npgsqlConnectionString,
                    healthQuery: "SELECT 1;",
                    name: name,
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready", "database", "postgresql" }
                );

        public static IHealthChecksBuilder AddElasticReadyHealthCheck(
            this IHealthChecksBuilder healthChecksBuilder,
            string elasticConnection,
            string elasticPrefix,
            string name) =>
            healthChecksBuilder
                .AddElasticsearch(
                    options =>
                    {
                        options.UseServer(elasticConnection);
                        options.UseApiKey(new ApiKeyAuthenticationCredentials(elasticPrefix));
                        options.UseCertificateValidationCallback(UseCertificateForElastic);
                    },
                    name: name,
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready", "kibana", "elastic" }
                );


        public static bool UseCertificateForElastic(object obj, X509Certificate? certificate, X509Chain? chain,
            SslPolicyErrors error)
        {
            return true;
        }

        public static IHealthChecksBuilder AddElasticReadyHealthCheck(
            this IHealthChecksBuilder healthChecksBuilder,
            string elasticConnection,
            string name) =>
            healthChecksBuilder
                .AddElasticsearch(
                    elasticConnection,
                    name: name,
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready", "kibana", "elastic" }
                );

        public static IHealthChecksBuilder AddApiReadyHealthCheck(
            this IHealthChecksBuilder healthChecksBuilder,
            string healthCheckUrl,
            string name) =>
            healthChecksBuilder
                .AddCheck(
                    name: name,
                    instance: new CustomHttpClientHealthCheck(
                        healthCheckUrl: healthCheckUrl,
                        apiName: name),
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready", "api", "services" }
                );

        public static IHealthChecksBuilder AddSqlServerReadyHealthCheck(
            this IHealthChecksBuilder healthChecksBuilder,
            string connectionString,
            string name) =>
            healthChecksBuilder
                .AddSqlServer(
                    connectionString: connectionString,
                    healthQuery: "SELECT 1;",
                    name: name,
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready", "database", "mssql" }
                );

        public static IHealthChecksBuilder AddMongoDbReadyHealthCheck(
            this IHealthChecksBuilder healthChecksBuilder,
            string mongodbConnectionString,
            string mongoDatabaseName,
            string name) =>
            healthChecksBuilder
                .AddMongoDb(
                    mongodbConnectionString: mongodbConnectionString,
                    mongoDatabaseName: mongoDatabaseName,
                    name: name,
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready", "database", "mongodb" }
                );

        public static IHealthChecksBuilder AddRabbitMqReadyHealthCheck(
            this IHealthChecksBuilder healthChecksBuilder,
            string rabbitConnectionString,
            string virtualHost,
            string name) =>
            healthChecksBuilder
                .AddRabbitMQ(
                    options =>
                    {
                        options.ConnectionFactory = new ConnectionFactory();
                        options.ConnectionFactory.VirtualHost = virtualHost;
                        options.ConnectionFactory.Uri = new Uri(rabbitConnectionString);
                    },

                    //rabbitConnectionString: rabbitConnectionString,
                    name: name,
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready", "bus", "messaging", "rabbitmq" }
                );

        public static IHealthChecksBuilder AddConfluentKafkaReadyHealthCheck(
            this IHealthChecksBuilder healthChecksBuilder,
            string bootstrapServer,
            string username,
            string password,
            string topic,
            string name) =>
            healthChecksBuilder
                .AddKafka(
                    config: new ProducerConfig
                    {
                        BootstrapServers = bootstrapServer,
                        SaslUsername = username,
                        SaslPassword = password,
                        SaslMechanism = SaslMechanism.Plain,
                        SecurityProtocol = SecurityProtocol.SaslSsl,
                    },
                    topic: topic,
                    name: name,
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready", "bus", "messaging", "kafka" }
                );

        public static IHealthChecksBuilder AddRedisReadyHealthCheck(
            this IHealthChecksBuilder healthChecksBuilder,
            EndPointCollection endPointCollection,
            string password,
            string name,
            int defaultDatabase) =>
            healthChecksBuilder
                .AddRedis(
                    connectionMultiplexer: ConnectionMultiplexer.Connect(
                        new ConfigurationOptions
                        {
                            EndPoints = endPointCollection, Password = password, DefaultDatabase = defaultDatabase
                        }),
                    name: name,
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready", "db", "cache", "redis" }
                );


        public static IHealthChecksBuilder AddVaultReadyHealthCheck(
            this IHealthChecksBuilder healthChecksBuilder,
            string healthCheckUrl,
            string name) =>
            healthChecksBuilder
                .AddCheck(
                    name: name,
                    instance: new CustomVaultHttpClientHealthCheck(
                        vaultUrl: healthCheckUrl,
                        apiName: name),
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "ready", "vault", "hashicorp" }
                );

        public static IApplicationBuilder UseCustomHealthCheck(
            this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseRouting();
            applicationBuilder.UseEndpoints(endpoints =>
            {
                //var version = Assembly.GetEntryAssembly().GetName().Version.ToString();
                const int peHeaderOffset = 60;
                const int linkerTimestampOffset = 8;
                byte[] bytes = new byte[2048];
                using (FileStream file = new FileStream(Assembly.GetEntryAssembly()?.Location ?? "", FileMode.Open,
                           FileAccess.Read, FileShare.ReadWrite))
                {
                    file.Read(bytes, 0, bytes.Length);
                }

                Int32 headerPos = BitConverter.ToInt32(bytes, peHeaderOffset);
                Int32 secondsSince1970 = BitConverter.ToInt32(bytes, headerPos + linkerTimestampOffset);
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                string appAssemblyLastBuildDate = dt.AddSeconds(secondsSince1970).ToString("dd-MMM-yyyy hh:mm:sstt");

                endpoints.MapHealthChecks("/health-alive", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("alive"),
                    ResponseWriter = async (context, report) =>
                    {
                        var result = JsonSerializer.Serialize(new
                        {
                            lastBuilt = appAssemblyLastBuildDate,
                            status = report.Status.ToString(),
                            checks = report.Entries.Select(entry => new
                            {
                                name = entry.Key, status = entry.Value.Status.ToString()
                            })
                        });
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                });

                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("ready"),
                    ResponseWriter = async (context, report) =>
                    {
                        var result = JsonSerializer.Serialize(new
                        {
                            status = report.Status.ToString(),
                            checks = report.Entries.Select(entry => new
                            {
                                name = entry.Key, status = entry.Value.Status.ToString()
                            })
                        });
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                });
            });

            return applicationBuilder;
        }
    }

    public sealed class CustomHttpClientHealthCheck : IHealthCheck
    {
        private readonly string _healthCheckUrl;
        private readonly string _apiName;

        public CustomHttpClientHealthCheck(
            string healthCheckUrl,
            string apiName)
        {
            _healthCheckUrl = healthCheckUrl;
            _apiName = apiName;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = new HttpClient();

                var response = await httpClient.GetAsync(_healthCheckUrl, cancellationToken);

                if (response.IsSuccessStatusCode)
                    return HealthCheckResult.Healthy();
                else
                    return HealthCheckResult.Unhealthy($"{_apiName} response status code: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"{_apiName} exception during health check: {ex.Message}");
            }
        }
    }

    public sealed class CustomVaultHttpClientHealthCheck : IHealthCheck
    {
        private readonly string _vaultUrl;
        private readonly string _apiName;

        public CustomVaultHttpClientHealthCheck(
            string vaultUrl,
            string apiName)
        {
            _vaultUrl = vaultUrl;
            _apiName = apiName;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = new HttpClient();

                var response =
                    await httpClient.GetAsync($"{_vaultUrl}/v1/sys/health?standbyok=true", cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy("Vault is healthy.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    return HealthCheckResult.Degraded("Vault is unsealed but not active.");
                }

                return HealthCheckResult.Unhealthy($"Vault health check failed with status: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"An error occurred: {ex.Message}");
            }
        }
    }
}
