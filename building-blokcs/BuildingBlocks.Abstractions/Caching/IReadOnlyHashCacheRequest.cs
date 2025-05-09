using MediatR;

namespace BuildingBlocks.Abstractions.Caching
{
    public interface IReadOnlyHashCacheRequest<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        string Key(TRequest request);
    }
}
