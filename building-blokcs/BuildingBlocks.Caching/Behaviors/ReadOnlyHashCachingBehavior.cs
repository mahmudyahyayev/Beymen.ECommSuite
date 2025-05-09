using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Caching;
using BuildingBlocks.Caching.Options;
using EasyCaching.Core;
using MediatR;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Caching.Behaviors
{
    [Obsolete("Kullanilmamasi tavsiye edilir")]
    public class ReadOnlyHashCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
      where TRequest : IRequest<TResponse>
      where TResponse : notnull
    {
        private readonly IRedisCachingProvider _cacheProvider;
        private readonly IEnumerable<IReadOnlyHashCacheRequest<TRequest, TResponse>> _cacheRequest;

        public ReadOnlyHashCachingBehavior(
            IRedisCachingProvider cachingProviderFactory,
            IOptions<CacheOptions> cacheOptions,
            IEnumerable<IReadOnlyHashCacheRequest<TRequest, TResponse>> cacheRequest)
        {
            Guard.Against.Null(cacheOptions.Value);

            _cacheProvider = cachingProviderFactory;

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

            try
            {
                string jsonValue = _cacheProvider.HGet(cacheKey, cacheKey);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(jsonValue);
            }
            catch (Exception ex)
            {
                return default;
            }
        }
    }
}
