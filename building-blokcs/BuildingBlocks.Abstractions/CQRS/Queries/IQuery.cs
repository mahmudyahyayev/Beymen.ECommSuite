using MediatR;

namespace BuildingBlocks.Abstractions.CQRS.Queries
{
    public interface IQuery<out T> : IRequest<T>
        where T : notnull
    { }

    public interface IStreamQuery<out T> : IStreamRequest<T>
        where T : notnull
    { }
}
