namespace BuildingBlocks.Logging
{
    public sealed class SerilogOptions
    {
        public string? SeqUrl { get; set; }
        public bool UseConsole { get; set; } = true;
        public bool ExportLogsToOpenTelemetry { get; set; } = false;
        public string? ElasticSearchUrl { get; set; }
        public string? ElasticSearchPrefix { get; set; }
        public string? GrafanaLokiUrl { get; set; }
        public bool UseElasticsearchJsonFormatter { get; set; }
        public string LogTemplate { get; set; } =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] [{CorrelationId}] - {Message:lj}{NewLine}{Exception}";
        public string? LogPath { get; set; }
    }
}
