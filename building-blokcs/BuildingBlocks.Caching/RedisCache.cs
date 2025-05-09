using Ardalis.GuardClauses;
using BuildingBlocks.Caching.Options;
using EasyCaching.Core;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Caching
{
    public class RedisCache
    {
        private readonly IEasyCachingProvider _cacheProvider;
        public IEasyCachingProvider CacheProvider => _cacheProvider;


        private readonly IEasyCachingProvider _readOnlyCacheProvider;
        public IEasyCachingProvider ReadOnlyCacheProvider => _readOnlyCacheProvider;

        public RedisCache(
            IEasyCachingProviderFactory easyCachingProviderFactory,
            IOptions<CacheOptions> cacheOptions)
        {
            Guard.Against.Null(cacheOptions.Value);

            if (cacheOptions.Value.RedisWriteOptions is not null)
                _cacheProvider = Guard.Against
                    .Null(easyCachingProviderFactory)
                    .GetCachingProvider(cacheOptions.Value.RedisWriteOptions.RedisOptions.ProviderName);


            if (cacheOptions.Value.RedisReadOnlyOptions is not null)
                _readOnlyCacheProvider = Guard.Against
                    .Null(easyCachingProviderFactory)
                    .GetCachingProvider(cacheOptions.Value.RedisReadOnlyOptions.RedisOptions.ProviderName);
        }
    }
}
