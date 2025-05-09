using BuildingBlocks.Abstractions.Caching;
using BuildingBlocks.Abstractions.CQRS.Queries;

namespace BuildingBlocks.Caching
{
    public abstract class ReadOnlyHashCacheRequest<TRequest, TResponse> : IReadOnlyHashCacheRequest<TRequest, TResponse>
        where TRequest : ICacheHashQuery<TResponse>
    {
        public virtual string Key(TRequest request) => string.Empty;
    }
}
