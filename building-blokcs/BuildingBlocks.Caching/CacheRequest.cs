using BuildingBlocks.Abstractions.Caching;
using MediatR;

namespace BuildingBlocks.Caching
{
    public abstract class CacheRequest<TRequest, TResponse> : ICacheRequest<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public virtual TimeSpan ExpirationTime => TimeSpan.FromMinutes(5);
        public virtual string Prefix => "App:";
        public virtual string RequestName => null;

        public virtual string Key(TRequest request)
        {
            if (!string.IsNullOrEmpty(RequestName))
            {
                return $"{Prefix}{RequestName}";
            }

            return $"{Prefix}{typeof(TRequest).Name}";
        }
    }
}