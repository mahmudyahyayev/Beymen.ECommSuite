using BuildingBlocks.Abstractions.Caching;
using MediatR;

namespace BuildingBlocks.Caching
{
    public abstract class InvalidateCacheRequest<TRequest, TResponse> : IInvalidateCacheRequest<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public virtual string Prefix => "App:";
        public virtual string RequestName => null;
        public abstract IEnumerable<string> CacheKeys(TRequest request);
    }

    public abstract class InvalidateCacheRequest<TRequest> : IInvalidateCacheRequest<TRequest>
        where TRequest : IRequest<Unit>
    {
        public virtual string Prefix => "App:";
        public virtual string RequestName => null;
        public abstract IEnumerable<string> CacheKeys(TRequest request);
    }
}
