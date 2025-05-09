using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Caching.Options
{
    public class RedisOptions
    {
        public string ProviderName { get; set; }
        public List<Endpoint> Endpoints { get; set; }
        public int DefaultDatabase { get; set; }
        public string Password { get; set; }
    }

    public class Endpoint
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
