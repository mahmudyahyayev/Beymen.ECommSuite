namespace BuildingBlocks.Caching.Options
{
    public class CacheOptions
    {
        public RedisWriteOptions RedisWriteOptions { get; set; }
        public RedisReadOnlyOptions RedisReadOnlyOptions { get; set; }
        public InMemoryOptions InMemoryOptions { get; set; }
    }
}
