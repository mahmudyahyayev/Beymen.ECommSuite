using BuildingBlocks.Abstractions.Persistence;

namespace BuildingBlocks.Abstractions.CQRS.Commands
{
    public interface ICacheCommand<out TResponse> : ICommand<TResponse>, ITxRequest
    where TResponse : notnull
    { }
}
