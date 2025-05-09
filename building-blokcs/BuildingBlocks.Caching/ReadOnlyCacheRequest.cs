using BuildingBlocks.Abstractions.Caching;
using BuildingBlocks.Abstractions.CQRS.Queries;

namespace BuildingBlocks.Caching
{
    public abstract class ReadOnlyCacheRequest<TRequest, TResponse> : IReadOnlyCacheRequest<TRequest, TResponse>
        where TRequest : ICacheQuery<TResponse>
    {
        public virtual string Key(TRequest request) => string.Empty;
    }
}
