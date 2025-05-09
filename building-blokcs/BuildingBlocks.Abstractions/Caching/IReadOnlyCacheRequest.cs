using MediatR;

namespace BuildingBlocks.Abstractions.Caching
{
    public interface IReadOnlyCacheRequest<in TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        string Key(TRequest request);
    }
}
