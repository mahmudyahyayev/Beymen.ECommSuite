namespace BuildingBlocks.Core.Messaging.MessagePersistence
{
    public class MessagePersistenceOptions
    {
        public int? Interval { get; set; } = 5;
        public string ConnectionString { get; set; } = default!;
        public bool Enabled { get; set; } = true;
        public string? MigrationAssembly { get; set; } = null!;
        public bool EnableBackgroundService { get; set; } = true;
        public int? ErrorInterval { get; set; } = 5;
        public int PartitionMaxCount { get; set; } = 50;
        public int ErrorRetryCount { get; set; } = 5;
        public int BatchCount { get; set; } = 500;
        public int StuckProcessingInterval { get; set; } = 900;
        public int StuckProcessingBatchCount { get; set; } = 5000;
    }
}
