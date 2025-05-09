using BuildingBlocks.Abstractions.Caching;
using BuildingBlocks.Abstractions.CQRS.Commands;

namespace BuildingBlocks.Caching
{
    public abstract class WriteOnlyCacheRequest<TRequest, TResponse> : IWriteOnlyCacheRequest<TRequest, TResponse>
      where TRequest : ICacheCommand<TResponse>
    {
        public virtual TimeSpan ExpirationTime => TimeSpan.FromMinutes(5);
        public virtual string Prefix => "App:";
        public virtual string Key(TRequest request) => string.Empty;
    }
}
