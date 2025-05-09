using MediatR;

namespace BuildingBlocks.Abstractions.Caching
{
    public interface ICacheRequest<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        TimeSpan ExpirationTime { get; }
        string Prefix { get; }
        string Key(TRequest request);
    }
}
