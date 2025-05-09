using MediatR;

namespace BuildingBlocks.Abstractions.Caching
{
    public interface IWriteOnlyCacheRequest<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        string Prefix { get; }
        TimeSpan ExpirationTime { get; }
        string Key(TRequest request);
    }
}
