using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Caching;
using BuildingBlocks.Caching.Options;
using EasyCaching.Core;
using MediatR;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Caching.Behaviors
{
    public class WriteOnlyCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
       where TResponse : notnull
    {
        private readonly IEasyCachingProvider _cacheProvider;
        private readonly IEnumerable<IWriteOnlyCacheRequest<TRequest, TResponse>> _cacheRequest;

        public WriteOnlyCachingBehavior(
            IEasyCachingProviderFactory cachingProviderFactory,
            IOptions<CacheOptions> cacheOptions,
            IEnumerable<IWriteOnlyCacheRequest<TRequest, TResponse>> cacheRequest)
        {
            Guard.Against.Null(cacheOptions.Value);

            _cacheProvider = Guard.Against
                .Null(cachingProviderFactory)
                .GetCachingProvider(cacheOptions.Value.RedisWriteOptions.RedisOptions.ProviderName);

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

            var response = await next();

            await _cacheProvider.SetAsync(
                cacheKey,
                response,
                cacheRequest.ExpirationTime,
                cancellationToken);

            return response;
        }
    }
}
