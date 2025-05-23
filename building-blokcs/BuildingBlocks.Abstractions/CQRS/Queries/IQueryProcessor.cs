namespace BuildingBlocks.Abstractions.CQRS.Queries;

public interface IQueryProcessor
{
    Task<TResponse> SendAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken
    )
        where TResponse : notnull;

    IAsyncEnumerable<TResponse> SendAsync<TResponse>(
        IStreamQuery<TResponse> query,
        CancellationToken cancellationToken
    )
        where TResponse : notnull;
}
