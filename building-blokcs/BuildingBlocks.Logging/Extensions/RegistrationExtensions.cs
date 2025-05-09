using System.Collections.Specialized;
using Destructurama;
using Elastic.Channels;
using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Grafana.Loki;

namespace BuildingBlocks.Logging.Extensions
{
    public static class RegistrationExtensions
    {
        public static WebApplicationBuilder AddCustomSerilog(
            this WebApplicationBuilder builder,
            string sectionName = "SerilogOptions",
            Action<LoggerConfiguration>? extraConfigure = null)
        {
            IConfigurationSection sectionData = builder.Configuration.GetSection(sectionName);


            var serilogOptions = new SerilogOptions();
            sectionData.Bind(serilogOptions);

            builder.Host.UseSerilog(
                (context, serviceProvider, loggerConfiguration) =>
                {
                    extraConfigure?.Invoke(loggerConfiguration);

                    loggerConfiguration.Enrich
                        .WithProperty("Application", builder.Environment.ApplicationName)
                        // TODO: Externalize correlation-id key to options?
                        .Enrich.WithCorrelationIdHeader(headerKey: "CorrelationId")
                        .Enrich.FromLogContext()
                        .Enrich.WithEnvironmentName()
                        .Enrich.WithMachineName()
                        .Enrich.WithExceptionDetails(
                            new DestructuringOptionsBuilder()
                                .WithDefaultDestructurers()
                                .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() })
                        );

                    loggerConfiguration.ReadFrom.Configuration(context.Configuration, sectionName: sectionName);

                    if (serilogOptions.UseConsole)
                    {
                        if (serilogOptions.UseElasticsearchJsonFormatter)
                        {
                            loggerConfiguration.WriteTo.Async(
                                writeTo => writeTo.Console(new ExceptionAsObjectJsonFormatter(renderMessage: true))
                            );
                        }
                        else
                        {

                            loggerConfiguration.WriteTo.Async(
                                writeTo => writeTo.Console(outputTemplate: serilogOptions.LogTemplate)
                            );
                        }
                    }

                    if (!string.IsNullOrEmpty(serilogOptions.ElasticSearchUrl))
                    {
                        string? elasticSearchApiKey = $"ApiKey {serilogOptions.ElasticSearchPrefix}";
                        var indexFormat = $"{builder.Environment.ApplicationName}-{builder.Environment.EnvironmentName}-{DateTime.Now:yyyy-MM}";

                        loggerConfiguration.WriteTo.Elasticsearch(new[] { new Uri(serilogOptions.ElasticSearchUrl) },
                        options =>
                        {
                            options.DataStream = new DataStreamName(indexFormat);
                            options.TextFormatting = new EcsTextFormatterConfiguration();
                            options.BootstrapMethod = BootstrapMethod.Failure;
                            options.ConfigureChannel = channelOptions =>
                            {
                                channelOptions.BufferOptions = new BufferOptions();
                            };
                        },
                        configureTransport =>
                        {
                            var headers = new NameValueCollection
                            {
                                { "Authorization", elasticSearchApiKey }
                            };
                            configureTransport.GlobalHeaders(headers);
                            configureTransport.ServerCertificateValidationCallback((_, _, _, _) => true);
                        });


                        if (!string.IsNullOrEmpty(serilogOptions.GrafanaLokiUrl))
                        {
                            loggerConfiguration.WriteTo.GrafanaLoki(
                                serilogOptions.GrafanaLokiUrl,
                                new[]
                                {
                            new LokiLabel { Key = "service", Value = "ecommerce" }
                                },
                                new[] { "app" }
                            );
                        }

                        if (!string.IsNullOrEmpty(serilogOptions.SeqUrl))
                        {
                            loggerConfiguration.WriteTo.Seq(serilogOptions.SeqUrl);
                        }

                        if (serilogOptions.ExportLogsToOpenTelemetry)
                        {
                            loggerConfiguration.WriteTo.OpenTelemetry();
                        }

                        if (!string.IsNullOrEmpty(serilogOptions.LogPath))
                        {
                            loggerConfiguration.WriteTo.Async(
                                writeTo =>
                                    writeTo.File(
                                        serilogOptions.LogPath,
                                        outputTemplate: serilogOptions.LogTemplate,
                                        rollingInterval: RollingInterval.Day,
                                        rollOnFileSizeLimit: true
                                    )
                            );
                        }
                    }
                }
            );

            return builder;
        }
    }
}
