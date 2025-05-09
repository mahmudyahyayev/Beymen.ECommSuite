using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Caching;
using BuildingBlocks.Caching.Options;
using EasyCaching.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Caching.Behaviors
{
    public class InvalidatingCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        private readonly ILogger<InvalidatingCachingBehavior<TRequest, TResponse>> _logger;
        private readonly IEasyCachingProvider _cacheProvider;
        private readonly IEnumerable<IInvalidateCacheRequest<TRequest, TResponse>> _invalidateCacheRequest;

        public InvalidatingCachingBehavior(
            ILogger<InvalidatingCachingBehavior<TRequest, TResponse>> logger,
            IEasyCachingProviderFactory cachingProviderFactory,
            IOptions<CacheOptions> cacheOptions,
            IEnumerable<IInvalidateCacheRequest<TRequest, TResponse>> invalidateCacheRequest)
        {
            _logger = logger;

            Guard.Against.Null(cacheOptions.Value);

            _cacheProvider = Guard.Against
                .Null(cachingProviderFactory)
                .GetCachingProvider(cacheOptions.Value.RedisWriteOptions.RedisOptions.ProviderName);

            _invalidateCacheRequest = invalidateCacheRequest;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var cacheRequest = _invalidateCacheRequest.FirstOrDefault();

            if (cacheRequest == null)
            {
                return await next();
            }

            var cacheKeys = cacheRequest.CacheKeys(request);
            
            foreach (var cacheKey in cacheKeys)
            {
                await _cacheProvider.RemoveAsync(cacheKey, cancellationToken);
                _logger.LogDebug("Cache data with cache key: {CacheKey} invalidated", cacheKey);
            }

            return default;
        }
    }
}
