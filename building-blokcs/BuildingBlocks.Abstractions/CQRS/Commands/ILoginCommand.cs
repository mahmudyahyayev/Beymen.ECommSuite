using BuildingBlocks.Abstractions.Persistence;

namespace BuildingBlocks.Abstractions.CQRS.Commands
{
    public interface ILoginCommand<out TResponse> : ICommand<TResponse>, ITxRequest
    where TResponse : notnull
    { }
}
