using Serilog;
using Serilog.Configuration;

namespace BuildingBlocks.Logging
{
    public static class LoggerEnrichmentConfigurationExtensions
    {
        public static LoggerConfiguration WithBaggage(this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration)
        {
            ArgumentNullException.ThrowIfNull(loggerEnrichmentConfiguration);
            return loggerEnrichmentConfiguration.With(new BaggageEnricher());
        }
    }
}
