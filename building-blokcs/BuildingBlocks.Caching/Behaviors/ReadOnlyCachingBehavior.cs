using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Caching;
using BuildingBlocks.Caching.Options;
using EasyCaching.Core;
using MediatR;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Caching.Behaviors
{
    public class ReadOnlyCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
      where TRequest : IRequest<TResponse>
      where TResponse : notnull
    {
        private readonly IEasyCachingProvider _cacheProvider;
        private readonly IEnumerable<IReadOnlyCacheRequest<TRequest, TResponse>> _cacheRequest;

        public ReadOnlyCachingBehavior(
            IEasyCachingProviderFactory cachingProviderFactory,
            IOptions<CacheOptions> cacheOptions,
            IEnumerable<IReadOnlyCacheRequest<TRequest, TResponse>> cacheRequest)
        {
            Guard.Against.Null(cacheOptions.Value);

            _cacheProvider = Guard.Against
                .Null(cachingProviderFactory)
                .GetCachingProvider(cacheOptions.Value.RedisReadOnlyOptions.RedisOptions.ProviderName);

            _cacheRequest = cacheRequest;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var cacheRequest = _cacheRequest.FirstOrDefault();

            if (cacheRequest == null)
            {
                return await next();
            }

            var cacheKey = cacheRequest.Key(request);

            var cachedResponse = await _cacheProvider
                .GetAsync<TResponse>(cacheKey, cancellationToken);

            if (cachedResponse.Value != null)
            {
                return cachedResponse.Value;
            }

            return default;
        }
    }
}
